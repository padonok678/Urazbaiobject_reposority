namespace Practicum.Operators;

public class MegaByte
{
    public const long BytesInMegaByte = 1_048_576;

    public double Value { get; set; }

    
    public static explicit operator StorageSize(MegaByte megaByte)
    {
        ArgumentNullException.ThrowIfNull(megaByte);
        double bytes = Math.Round(megaByte.Value * BytesInMegaByte);
        return new StorageSize { Bytes = checked((long)bytes) };
    }

    public override string ToString() => $"{Value:0.######} МБ";
}
