using App.Common.AssetSystem.Runtime;
using App.Common.Input.Runtime;
using App.Common.Logger.Runtime;
using App.Common.ModuleItem.Runtime;
using App.Common.Utilities.Utility.Runtime;
using App.Game.Player.External.Context;
using App.Game.Player.External.View;

namespace App.Game.Player.External
{
    public class PlayerController : IInitSystem, IUpdateSystem
    {
        private readonly IAssetManager _assetManager;
        private readonly IModuleItemsManager _moduleItemsManager;
        private readonly IInputService _inputService;
        
        private EntityView _view;
        private IModuleItem _player;
        private PlayerMoveController _playerMoveController;
        private PlayerContext _context;

        public EntityView PlayerView => _view;
        
        public PlayerController(
            IAssetManager assetManager,
            IModuleItemsManager moduleItemsManager, 
            IInputService inputService)
        {
            _assetManager = assetManager;
            _moduleItemsManager = moduleItemsManager;
            _inputService = inputService;
        }

        public void Init()
        {
            var playerCreator = new PlayerCreator(_moduleItemsManager);
            _player = playerCreator.Create();
            
            CreateView();
            InitContext();
            InitMove();
        }

        private void InitContext()
        {
            _context = new PlayerContext
            {
                ModuleItem = _player,
                View = _view,
            };
        }

        private void CreateView()
        {
            var viewCreator = new PlayerViewCreator(_assetManager);
            var viewResult = viewCreator.Create(_player);
            if (!viewResult.HasValue)
            {
                HLogger.LogError("Cant create view.");
                return;
            }

            _view = viewResult.Value;
        }

        private void InitMove()
        {
            _playerMoveController = new PlayerMoveController(
                _inputService, 
                _context);
            _playerMoveController.Init();
        }

        public void OnUpdate()
        {
            _playerMoveController.OnUpdate();
        }
    }
}