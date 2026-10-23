using App.Common.FSM.External;
using App.Core.Startups.External;
using App.Core.Startups.External.Attributes;
using App.Core.Startups.External.Constants;
using App.Game.Update.External;

namespace App.Game.Player.External
{
    [Configurator(DIContext.CoreContext)]
    public class PlayerConfigurator : Configurator
    {
        public override void Configuration()
        {
            BindSingle<PlayerController>();

            RegisterFSM<PlayerController>(FSMStage.CoreInitStage, StageOrders.PlayerController);

            RegisterUpdate<PlayerController>(UpdateStage.PlayerController);
        }
    }
}