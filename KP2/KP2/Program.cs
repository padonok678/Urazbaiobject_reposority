using KP2.Data;

namespace KP2;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!File.Exists(Db.DbPath))
        {
            MessageBox.Show($"Не найден файл базы данных:\n{Db.DbPath}\n\nПоложите smart_home.db рядом с exe.",
                "SmartHome Master Console", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new FormMain());
    }
}
