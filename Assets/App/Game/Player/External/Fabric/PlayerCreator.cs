using App.Common.Logger.Runtime;
using App.Common.ModuleItem.Runtime;
using App.Game.Modules.Name.Runtime.Config;
using App.Game.Modules.Name.Runtime.Data;

namespace App.Game.Player.External
{
    public class PlayerCreator
    {
        private readonly IModuleItemsManager _moduleItemsManager;

        public PlayerCreator(IModuleItemsManager moduleItemsManager)
        {
            _moduleItemsManager = moduleItemsManager;
        }

        public IModuleItem Create()
        {
            var moduleItem = _moduleItemsManager.Create("player");
            if (!moduleItem.HasValue)
            {
                HLogger.LogError("Cant create player.");
                return null;
            }
            
            InitName(moduleItem.Value);

            return moduleItem.Value;
        }

        private void InitName(IModuleItem player)
        {
            if (!player.TryGetDataModule<NameModuleData>(out var data))
            {
                if (player.TryGetConfigModule<NameModuleConfig>(out var config))
                {
                    data = new NameModuleData()
                    {
                        Name = config.Name,
                    };
                    
                    player.AddDataModule(data);
                }
                else
                {
                    HLogger.LogError("StatsModuleConfig not found.");   
                }
            }
        }
    }
}