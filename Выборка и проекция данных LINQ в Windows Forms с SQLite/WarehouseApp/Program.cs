using WarehouseApp.Data;

namespace WarehouseApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        DbInitializer.Initialize();      
        Application.Run(new FormMain());
    }
}
