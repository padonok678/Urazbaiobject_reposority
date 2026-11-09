namespace PZ7;

/// <summary>Задание 1 и 2: денежная сумма в рублях и копейках.</summary>
public class Money
{
    public int Rubles { get; }
    public int Kopeks { get; }   // всегда от 0 до 99

    public Money(int rubles, int kopeks)
    {
        // Нормализация: 100 копеек = 1 рубль, отрицательные копейки тоже учитываются
        long total = (long)rubles * 100 + kopeks;
        long r = total / 100;
        long k = total % 100;
        if (k < 0) { k += 100; r--; }
        Rubles = (int)r;
        Kopeks = (int)k;
    }

    // Конструктор сам нормализует значения, в том числе отрицательные
    private static Money FromKopeks(long total) =>
        new Money((int)(total / 100), (int)(total % 100));

    public long TotalKopeks => Rubles * 100L + Kopeks;

    // ---------- Арифметика (каждый раз возвращается НОВЫЙ объект) ----------
    public static Money operator +(Money a, Money b) => FromKopeks(a.TotalKopeks + b.TotalKopeks);
    public static Money operator -(Money a, Money b) => FromKopeks(a.TotalKopeks - b.TotalKopeks);
    public static Money operator +(Money a, int rubles) => FromKopeks(a.TotalKopeks + rubles * 100L);

    // ---------- Сравнение (только парами) ----------
    public static bool operator <(Money a, Money b) => a.TotalKopeks < b.TotalKopeks;
    public static bool operator >(Money a, Money b) => a.TotalKopeks > b.TotalKopeks;
    public static bool operator <=(Money a, Money b) => a.TotalKopeks <= b.TotalKopeks;
    public static bool operator >=(Money a, Money b) => a.TotalKopeks >= b.TotalKopeks;

    public static bool operator ==(Money? a, Money? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.TotalKopeks == b.TotalKopeks;
    }
    public static bool operator !=(Money? a, Money? b) => !(a == b);

    // ---------- Операторы истинности: объект "истинен", если баланс > 0 ----------
    public static bool operator true(Money m) => m.TotalKopeks > 0;
    public static bool operator false(Money m) => m.TotalKopeks <= 0;
    public static bool operator !(Money m) => m.TotalKopeks <= 0;

    // ---------- Приведение типов (Задание 2) ----------
    // Копейки -> Money: потери данных нет, поэтому implicit
    public static implicit operator Money(long totalKopeks) => FromKopeks(totalKopeks);

    // Money -> копейки: явное приведение
    public static explicit operator long(Money m) => m.TotalKopeks;

    // Money -> CurrencyRecord: явное (теряется смысл "рубли и копейки")
    public static explicit operator CurrencyRecord(Money m) =>
        new CurrencyRecord { Code = "RUB", Amount = m.Rubles + m.Kopeks / 100m };

    // CurrencyRecord -> Money: явное, с округлением до копеек
    public static explicit operator Money(CurrencyRecord cr) =>
        FromKopeks((long)Math.Round(cr.Amount * 100m, MidpointRounding.AwayFromZero));

    public override bool Equals(object? obj) => obj is Money m && this == m;
    public override int GetHashCode() => TotalKopeks.GetHashCode();
    public override string ToString() => $"{Rubles} руб. {Kopeks:D2} коп.";
}
