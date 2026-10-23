using System.Collections.Generic;
using App.Common.DataContainer.Runtime;
using App.Common.ModuleItem.Runtime;
using App.Common.Utilities.Utility.Runtime;
using App.Game.Modules.Move.Runtime.Config;
using App.Game.Modules.Move.Runtime.Data;

namespace App.Game.Modules.Move.Runtime
{
    public class MoveModuleSystem : IModuleSystem
    {
        public MoveModuleSystem()
        {
        }

        public Optional<IModuleItem> OnItemCreated(IModuleItem moduleItem)
        {
            if (!moduleItem.TryGetConfigModule<MoveModuleConfig>(out var config))
            {
                return Optional<IModuleItem>.Success(moduleItem);
            }

            if (!moduleItem.TryGetDataModule<MoveModuleData>(out var data))
            {
                data = new MoveModuleData()
                {
                };
                
                moduleItem.AddDataModule(data);
            }

            var moveModule = new MoveModule(moduleItem, data, config);
            moduleItem.AddModule(moveModule);
            
            return Optional<IModuleItem>.Success(moduleItem);
        }

        public void OnItemDestroyed(IModuleItem moduleItem)
        {
        }

        public ModuleIndex SortIndex()
        {
            return ModuleIndex.Move;
        }
    }
}