using App.Common.Input.Runtime;
using App.Common.Logger.Runtime;
using App.Game.Modules.Move.Runtime;
using App.Game.Player.External.Context;
using Game.Project.Gameplay.Move.External.View;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App.Game.Player.External
{
    public class PlayerMoveController
    {
        private readonly IInputService _inputService;
        private readonly PlayerContext _context;

        private MoveModule _moveModule;
        private InputAction _moveInput;
        private Rigidbody2D _rigidbody;
        private Camera _camera;
        private MoveAnimationPresenter _animationPresenter;

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
            
            _moveInput = _inputService.Input.Movement.Move;

            _camera = Camera.main;

            _rigidbody = _context.View.GetComponent<Rigidbody2D>();

            _animationPresenter = new MoveAnimationPresenter(_rigidbody.transform);
            _animationPresenter.Initialize();
        }

        public void OnUpdate()
        {
            var direction = _moveInput.ReadValue<Vector2>().normalized;
            _moveModule.SetDirection(new Common.Algorithms.Runtime.Vector2(direction.x, direction.y));

            var velocity2 = _moveModule.GetVelocity();
            var velocity = new Vector3(velocity2.X, velocity2.Y, 0);
            
            _animationPresenter.OnUpdate(velocity, Time.deltaTime);
            
            _rigidbody.linearVelocity = velocity;
        }
    }
}