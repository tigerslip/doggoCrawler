namespace DoggoCrawler;

enum PlayerCommand
{
    Unknown,
    MoveNorth,
    MoveSouth,
    MoveEast,
    MoveWest,
    PickUp,
    Drop,
    Undo,
    Quit
}

class Room
{
    internal Room NorthRoom { get; set; }
    internal Room SouthRoom { get; set; }
    internal Room EastRoom { get; set; }
    internal Room WestRoom { get; set; }
    internal Stack<IItem> Items = new();
}

interface IItem { }
record MoneyItem(decimal Value) : IItem;
record WeaponItem(int Damage) : IItem;


interface IHistoryEntry { }

record Move(Room PreviousRoom) : IHistoryEntry;

record Pickup : IHistoryEntry;

record Drop : IHistoryEntry;

class GameState
{
    public GameState(Room startingRoom)
    {
        PlayerRoom = startingRoom;
    }

    internal Room PlayerRoom { get; set; }
    internal bool Quit { get; set; }
    internal Stack<IHistoryEntry> History { get; set; } = new();
    public Stack<IItem> PlayerItems { get; set; } = new();
    public IGameUpdate LatestUpdate { get; set; }
}

interface IGameUpdate { };
record InvalidMove(string Direction) : IGameUpdate;
record ValidMove(string Direction) : IGameUpdate;
record InvalidPickUp : IGameUpdate;
record ValidPickUp : IGameUpdate;
record InvalidDrop : IGameUpdate;
record ValidDrop(IItem Item) : IGameUpdate;
record InvalidUndo : IGameUpdate;
record ValidUndo : IGameUpdate;
record PlayerQuit : IGameUpdate;
