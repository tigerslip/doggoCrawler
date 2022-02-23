namespace DoggoCrawler;

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
        state.LatestUpdate = new PlayerQuit();
        return state;
    }

    static GameState AttemptMove(GameState state, string direction, Func<Room, Room?> roomSelector)
    {
        var newRoom = roomSelector(state.PlayerRoom);
        if (newRoom == null)
        {
            state.LatestUpdate = new InvalidMove(direction);
        }
        else
        {
            var action = new Move(newRoom);
            state.History.Push(action);
            state.PlayerRoom = newRoom;
            state.LatestUpdate = new ValidMove(direction);
        }
           
        return state;
    }

    static GameState PickUp(GameState state)
    {
        if (state.PlayerRoom.Items.TryPop(out var item))
        {
            state.PlayerItems.Push(item);
            var pickUpHistory = new Pickup();
            state.History.Push(pickUpHistory);
            state.LatestUpdate = new ValidPickUp();
        }
        else
        {
            state.LatestUpdate = new InvalidPickUp();
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
            state.LatestUpdate = new ValidDrop(item);
        }
        else
        {
            state.LatestUpdate = new InvalidDrop();
        }

        return state;
    }

    static GameState Undo(GameState state)
    {
        if (state.History.TryPop(out var historyEntry))
        {
            var _ = historyEntry switch
            {
                Move move => UndoMove(move, state),
                Drop _ => UndoDrop(state),
                Pickup _ => UndoPickup(state),
                null => throw new ArgumentNullException(nameof(historyEntry)),
                _ => throw new ArgumentOutOfRangeException(nameof(historyEntry)),
            };

            state.LatestUpdate = new ValidUndo();
        }
        else
        {
            state.LatestUpdate = new InvalidUndo();
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