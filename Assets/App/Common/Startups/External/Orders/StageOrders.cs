namespace App.Core.Startups.External
{
    /// <summary>
    /// Порядок инициализации контроллеров/сервисов
    /// </summary>
    public enum StageOrders
    {
        // global
        Data = -100,
        DataContainers = 10,
        // start
        StartSceneManager = 100_000,
        // core
        ModuleItemsManager = -100_000,
        UpdateManager = 100_100,
        // end player systems
        TimeManager = 100,
        WindowManager = 100,
        Container = 0,
        PlayerController = 200,
        CameraFollow = 0,
        Camera = 0,
        ModuleItemsConfigLoader = -10_000,
        CanvasController = -10_000,
    }
}