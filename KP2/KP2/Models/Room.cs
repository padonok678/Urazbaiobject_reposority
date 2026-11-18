namespace KP2.Models;

public class Room
{
    public int Id { get; }
    public string Title { get; }
    public int Floor { get; }

    public Room(int id, string title, int floor)
    {
        Id = id;
        Title = title;
        Floor = floor;
    }

    public override string ToString() => Title;
}
