using AsyncResultsPractice;

Console.OutputEncoding = System.Text.Encoding.UTF8;

await Task1_ReturnTypes.RunAsync();
await Task2_WhenAll.RunAsync();
await Task3_WhenAny.RunAsync();

Console.WriteLine("Готово. Нажмите любую клавишу...");
Console.ReadKey();
