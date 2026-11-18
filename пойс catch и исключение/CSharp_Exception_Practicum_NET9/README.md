# Практикум C# — поиск catch и классы исключений (.NET 9)

## Требования
- Установленный .NET 9 SDK
- Visual Studio 2022 с поддержкой .NET 9 или Visual Studio Code

## Запуск
Откройте терминал в папке проекта и выполните:

```bash
dotnet restore
dotnet run
```

## Реализовано
- **Задание 1:** раскрутка стека вызовов. В `ServiceLevel` перехватывается `IndexOutOfRangeException`, но возникает `DivideByZeroException`, поэтому исключение проходит выше. Блоки `finally` выполняются в порядке: `DataLevel` → `ServiceLevel` → `Main`.
- **Задание 2:** классы `InsufficientFundsException` и `InvalidTransactionException`, пользовательские свойства и обработка исключений в `Main`.
- **Задание 3:** `UserValidationException`, проверка имени и возраста, вывод `FieldName` и `AttemptedValue`.

## Ожидаемый вывод задания 1
```text
Main: начало
[Finally в DataLevel]
[Finally в ServiceLevel]
[Catch в Main]: Attempted to divide by zero.
[Finally в Main]
```

Текст системного сообщения может отличаться в зависимости от версии .NET и языка системы.
