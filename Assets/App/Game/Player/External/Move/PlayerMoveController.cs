using System;
using System.Collections;
using App.Common.Input.Runtime;
using App.Common.Logger.Runtime;
using App.Game.Modules.Move.Runtime;
using App.Game.Player.External.Context;
using Game.Project.Gameplay.Move.External.View;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App.Game.Player.External
{
    public class PlayerMoveController : IDisposable
    {
        private readonly IInputService _inputService;
        private readonly PlayerContext _context;
        private readonly float _maxGravity = -28f;
        private readonly float _maxJumpHeight = 7f;
        private readonly float _maxJumpTime = 0.75f;
        private readonly float _groundedGravity = -0.07f;
        
        private MoveModule _moveModule;
        private InputAction _moveInput;
        private InputAction _jumpInput;
        private MoveAnimationPresenter _animationPresenter;
        private CharacterController2D _characterController;
        private LayerMask _platformLayer;
        private Vector3 _movementVector;
        private float _yVelocity;

        private float _jumpForce = 20;
        private float _initialGravity;
        private float _currentGravity;

        private Coroutine _coroutine;

        public PlayerMoveController(
            IInputService inputService,
            PlayerContext context)
        {
            _inputService = inputService;
            _context = context;
        }

        public void Init()
        {
            if (!_context.ModuleItem.TryGetModule<MoveModule>(out var moveModule))
            {
                HLogger.LogError("Move not found.");
                return;
            }

            _moveModule = moveModule;
            _moveInput = _inputService.Input.Player.Move;
            _jumpInput = _inputService.Input.Player.Jump;
            
            _characterController = _context.View.GetComponent<CharacterController2D>();

            _animationPresenter = new MoveAnimationPresenter(_characterController.transform);
            _animationPresenter.Initialize();

            _characterController.HeadCollisionObject += OnHead;
            SetupJumpVariables();
        }

        private void SetupJumpVariables()
        {
            float timeToApex = _maxJumpTime / 2;
            _initialGravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            _jumpForce = (2 * _maxJumpHeight) / timeToApex;
        }

        public void OnUpdate()
        {
            _currentGravity = (_characterController.IsGrounded) ? _groundedGravity : _initialGravity;

            if (_characterController.IsGrounded == false)
            {
                _yVelocity += _currentGravity * Time.deltaTime;

                _yVelocity = Mathf.Max(_yVelocity, _maxGravity);
            }
            else
            {
                _yVelocity = _groundedGravity;
            }
            
            float yInput = _moveInput.ReadValue<Vector2>().normalized.y;
            
            if (yInput < 0 && _jumpInput.triggered)
            {
                if (IsOneWayPlatform(_characterController.CurrentGroundObject))
                {
                    IgnorePlatform(_characterController.CurrentGroundObject, 
                        () => _characterController.CurrentGroundObject.transform.position.y
                                         - _characterController.Center.y
                                         > _characterController.Height / 2);
                    return;
                }
            }

            if (_jumpInput.triggered && _characterController.IsGrounded)
            {
                _yVelocity = _jumpForce;
            }
            
            var direction = _moveInput.ReadValue<Vector2>().normalized;
            _moveModule.SetDirection(new Common.Algorithms.Runtime.Vector2(direction.x, direction.y));

            var velocity2 = _moveModule.GetVelocity();
            var velocity = new Vector3(velocity2.X, velocity2.Y, 0);
            
            _movementVector = new Vector2(velocity.x, _yVelocity);
            _characterController.Move(_movementVector);
        }

         private void OnHead(GameObject collisionObject)
         {
             if (IsOneWayPlatform(collisionObject))
             {
                 IgnorePlatform(collisionObject, () => _yVelocity <= 0);
                 HLogger.Log("IgnorePlatform");
                 return;
             }
        
             if (_yVelocity > 0)
             {
                 HLogger.Log("Stuck");
                 _yVelocity = 0;
             }
         }
        
         private void IgnorePlatform(GameObject platform, Func<bool> predicate)
         {
             if (_coroutine != null)
             {
                 _characterController.StopCoroutine(_coroutine);HLogger.LogError("StopCoroutine");
             }
        
             _coroutine = _characterController.StartCoroutine(IgnorePlatformRoutine(platform, predicate));
         }
        
         private IEnumerator IgnorePlatformRoutine(GameObject platform, Func<bool> predicate)
         {
             _characterController.ToggleIgnoreCollider(platform, true);
             yield return new WaitUntil(predicate);
             _characterController.ToggleIgnoreCollider(platform, false);
         }
         
         private bool IsOneWayPlatform(GameObject collisionObject)
         {
             return collisionObject.TryGetComponent<OneWayPlatform>(out var platform);
         }
         
        public void Dispose()
        {
            _characterController.HeadCollisionObject -= OnHead;
        }
    }
}