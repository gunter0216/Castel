using System;

namespace App.Game.Update.External
{
    /// <summary>
    /// Устанавливает порядок обновления контроллеров/сервисов
    /// </summary>
    public enum UpdateStage
    {
        TimeManager = 1000,
        PlayerController = 0,
        CameraFollow = 100,
    }
}