namespace Practicum.Operators;

public class StorageSize
{
    public long Bytes { get; set; }

    public static implicit operator StorageSize(long bytes) => new() { Bytes = bytes };

    public static explicit operator long(StorageSize size)
    {
        ArgumentNullException.ThrowIfNull(size);
        return size.Bytes;
    }

    
    public static implicit operator MegaByte(StorageSize size)
    {
        ArgumentNullException.ThrowIfNull(size);
        return new MegaByte { Value = (double)size.Bytes / MegaByte.BytesInMegaByte };
    }

    public override string ToString() => $"{Bytes:N0} Б";
}
