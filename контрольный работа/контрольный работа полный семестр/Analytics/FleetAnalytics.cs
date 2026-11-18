using CorpFleet.Data;

namespace CorpFleet.Analytics;

public static class FleetAnalytics
{
    /// <summary>Join + GroupJoin + Select → DTO.</summary>
    public static List<FleetReportDto> BuildReport(
        IEnumerable<VehicleRow> vehicles,
        IEnumerable<CategoryRow> categories,
        IEnumerable<MaintenanceLogRow> logs) =>
        vehicles
            .Join(categories, v => v.CategoryId, c => c.Id, (v, c) => new { v, c })
            .GroupJoin(logs, x => x.v.Id, l => l.VehicleId, (x, ls) => new FleetReportDto
            {
                VehicleId = x.v.Id,
                Model = x.v.Model,
                CategoryName = x.c.CategoryName,
                Power = x.v.Power,
                BasePrice = x.v.BasePrice,
                TaxRate = x.c.TaxRate,
                TaxAmount = x.v.BasePrice * x.c.TaxRate,
                LogsCount = ls.Count(),
                MaintenanceCost = ls.Sum(l => l.Cost)
            })
            .ToList();

    /// <summary>Where (диапазон цен + поиск) и составная сортировка OrderBy + ThenByDescending.</summary>
    public static IEnumerable<FleetReportDto> Filter(IEnumerable<FleetReportDto> source, double? minPrice, double? maxPrice, string text)
    {
        var query = source;
        if (minPrice.HasValue) query = query.Where(d => d.BasePrice >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(d => d.BasePrice <= maxPrice.Value);
        if (!string.IsNullOrWhiteSpace(text))
            query = query.Where(d => d.Model.Contains(text, StringComparison.OrdinalIgnoreCase)
                                  || d.CategoryName.Contains(text, StringComparison.OrdinalIgnoreCase));

        return query.OrderBy(d => d.CategoryName).ThenByDescending(d => d.BasePrice);
    }

    /// <summary>Пагинация Skip/Take.</summary>
    public static List<T> Page<T>(IEnumerable<T> source, int page, int pageSize) =>
        source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    /// <summary>GroupBy + Count/Sum/Average/Min/Max.</summary>
    public static List<CategoryStatDto> GroupStats(IEnumerable<FleetReportDto> report) =>
        report
            .GroupBy(d => d.CategoryName)
            .Select(g => new CategoryStatDto
            {
                Category = g.Key,
                Count = g.Count(),
                TotalPrice = g.Sum(d => d.BasePrice),
                AvgPrice = g.Average(d => d.BasePrice),
                MinPrice = g.Min(d => d.BasePrice),
                MaxPrice = g.Max(d => d.BasePrice),
                MaintenanceTotal = g.Sum(d => d.MaintenanceCost)
            })
            .OrderBy(s => s.Category)
            .ToList();

    /// <summary>Итоговая сводка через Aggregate + проверки Any/All/FirstOrDefault.</summary>
    public static string BuildSummary(IEnumerable<FleetReportDto> report)
    {
        var list = report.ToList();
        if (!list.Any()) return "Нет данных.";

        string byCategory = GroupStats(list).Aggregate(
            "СВОДКА CorpFleet Analytics",
            (acc, s) => acc + $"\r\n • {s.Category}: {s.Count} ТС, сумма {s.TotalPrice:N0}, средняя {s.AvgPrice:N0}, мин {s.MinPrice:N0}, макс {s.MaxPrice:N0}");

        double totalPrice = list.Sum(d => d.BasePrice);
        double avgPrice = list.Average(d => d.BasePrice);
        double totalMaintenance = list.Sum(d => d.MaintenanceCost);
        bool anyExpensiveService = list.Any(d => d.MaintenanceCost > 300000);
        bool allPowerPositive = list.All(d => d.Power > 0);
        var priciest = list.OrderByDescending(d => d.BasePrice).FirstOrDefault();
        var neverServiced = list.FirstOrDefault(d => d.LogsCount == 0);
        string neverServicedName = neverServiced == null ? "нет" : neverServiced.Model;

        return byCategory +
               $"\r\n\r\nВсего ТС: {list.Count}" +
               $"\r\nСумма стоимости: {totalPrice:N0}" +
               $"\r\nСредняя стоимость: {avgPrice:N0}" +
               $"\r\nСумма затрат на обслуживание: {totalMaintenance:N0}" +
               $"\r\nAny: есть ТС с затратами на обслуживание больше 300 000: {anyExpensiveService}" +
               $"\r\nAll: у всех ТС мощность больше 0: {allPowerPositive}" +
               $"\r\nFirstOrDefault: самое дорогое ТС: {priciest.Model}" +
               $"\r\nFirstOrDefault: первое ТС без обслуживания: {neverServicedName}";
    }

    /// <summary>Записи обслуживания одного канала (Online/Offline) → ключи для операций над множествами.</summary>
    public static List<ServiceLogKey> ToKeys(IEnumerable<MaintenanceLogRow> logs, IEnumerable<VehicleRow> vehicles, string channel) =>
        logs.Where(l => l.Channel == channel)
            .Join(vehicles, l => l.VehicleId, v => v.Id, (l, v) => new ServiceLogKey(l.VehicleId, v.Model, l.ServiceName))
            .ToList();
}
