// doggo crawler requirements
// player navigates a dungeon using the commands u,d,l,r (up, down, left, right)
// each time the player performs an action the doggo moves to a new room
// if the cute fluffy dog catches the player the dog steals all the money and the player loses the game
// if the player gathers all the items without being caught by the dog the player wins the game
// each room has at least one door
// dungeon is anywhere from 3x3 to 10x10
// there are two items
// * money (dollars and cents)
// * weapon with damage up to 100
// the player can pick up or drop items, but there is no good reason to drop an item idiot
// 

using System.Diagnostics;
using System.Windows.Input;using CommandLine;

var state = new GameState();

while (!state.Quit)
{
    var input = Console.ReadKey().KeyChar;
    Command.Process(Command.Parse(input), state);
}

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

interface IItem{}
record MoneyItem(decimal Value) : IItem;
record WeaponItem(int Damage) : IItem;


interface IHistoryEntry{}

record Move(Room PreviousRoom) : IHistoryEntry;

record Pickup : IHistoryEntry;

record Drop : IHistoryEntry;

class GameState
{
    /// <summary>
    /// Room the player is currently in
    /// </summary>
    internal Room PlayerRoom { get; set; }
    internal bool Quit { get; set; }
    internal Stack<IHistoryEntry> History { get; set; }
    public Stack<IItem> PlayerItems { get; set; }
}

static class Command
{
    internal static PlayerCommand Parse(char c) => c switch
    {
        'n' => PlayerCommand.MoveNorth,
        's' => PlayerCommand.MoveSouth,
        'e' => PlayerCommand.MoveEast,
        'w' => PlayerCommand.MoveWest,
        'p' => PlayerCommand.PickUp,
        'd' => PlayerCommand.Drop,
        'u' => PlayerCommand.Undo,
        'q' => PlayerCommand.Quit,
        _ => PlayerCommand.Unknown
    };

    internal static GameState Process(PlayerCommand command, GameState state) => command switch
    {
        PlayerCommand.Quit => Quit(state),
        PlayerCommand.Unknown => state,
        PlayerCommand.MoveNorth => AttemptMove(state, "north", room => room.NorthRoom),
        PlayerCommand.MoveSouth => AttemptMove(state, "south", room => room.SouthRoom),
        PlayerCommand.MoveEast => AttemptMove(state, "east", room => room.EastRoom),
        PlayerCommand.MoveWest => AttemptMove(state, "west", room => room.WestRoom),
        PlayerCommand.PickUp => PickUp(state),
        PlayerCommand.Drop => Drop(state),
        PlayerCommand.Undo => Undo(state),
        _ => throw new ArgumentOutOfRangeException(nameof(command), command, "This code should never be reached!")
    };

    static GameState Quit(GameState state)
    {
        state.Quit = true;
        return state;
    }

    static GameState AttemptMove(GameState state, string direction, Func<Room, Room?> roomSelector)
    {
        var newRoom = roomSelector(state.PlayerRoom);
        if (newRoom == null)
        {
            Console.WriteLine($"Can't move {direction}, no door!");
        }
        else
        {
            var action = new Move(newRoom);
            state.History.Push(action);
            state.PlayerRoom = newRoom;
        }
           
        return state;
    }

    static GameState PickUp(GameState state)
    {
        if (state.PlayerRoom.Items.TryPop(out var item))
        {
            state.PlayerRoom.Items.Push(item);
            var pickUpHistory = new Pickup();
            state.History.Push(pickUpHistory);
        }
        else
        {
            Console.WriteLine("Nothing to pick up!");
        }

        return state;
    }

    static GameState Drop(GameState state)
    {
        if(state.PlayerItems.TryPop(out var item))
        {
            state.PlayerRoom.Items.Push(item);
            var drop = new Drop();
            state.History.Push(drop);
        }
        else
        {
            Console.WriteLine("You have no items to drop!");
        }

        return state;
    }

    static GameState Undo(GameState state)
    {
        if (state.History.TryPop(out var historyEntry))
        {
            return historyEntry switch
            {
                Move move => UndoMove(move, state),
                Drop _ => UndoDrop(state),
                Pickup _ => UndoPickup(state),
                null => throw new ArgumentNullException("HistoryEntry"),
                _ => throw new ArgumentOutOfRangeException("HistoryEntry"),
            };
        }

        return state;
    }

    static GameState UndoPickup(GameState state)
    {
        var pickedUpItem = state.PlayerItems.Pop();
        state.PlayerRoom.Items.Push(pickedUpItem);
        return state;
    }

    static GameState UndoDrop(GameState state)
    {
        var droppedItem = state.PlayerRoom.Items.Pop();
        state.PlayerItems.Push(droppedItem);
        return state;
    }

    static GameState UndoMove(Move move, GameState state)
    {
        var previousRoom = move.PreviousRoom;
        state.PlayerRoom = previousRoom;
        return state;
    }
}