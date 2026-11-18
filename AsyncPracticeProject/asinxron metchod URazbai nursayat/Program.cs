using AsyncPractice;

Console.OutputEncoding = System.Text.Encoding.UTF8;

await Task1_BasicAsync.RunAsync();
await Task2_SequentialVsParallel.RunAsync();
await Task3_TaskResultAndLambda.RunAsync();

Console.WriteLine("Готово. Нажмите любую клавишу...");
Console.ReadKey();
