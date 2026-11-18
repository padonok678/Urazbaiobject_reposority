using EnterpriseLinq.Data;

namespace EnterpriseLinq;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            DatabaseInitializer.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new FormMain());
    }
}
