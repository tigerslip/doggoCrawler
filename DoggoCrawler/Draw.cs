namespace DoggoCrawler;

internal static class Draw
{
    internal static void Update(GameState state)
    {
        switch (state.LatestUpdate)
        {
            case InvalidMove move:
                Console.WriteLine($"Can't move {move.Direction}, no door!");
                break;
            case ValidMove(var direction):
                Console.WriteLine($"You enter the room to the {direction}. The room has {state.PlayerRoom.Items.Count} items. No cute fluffy dog in site, whew....");
                break;
            case InvalidPickUp:
                Console.WriteLine("Nothing to pick up!");
                break;
            case ValidPickUp:
                var latestItem = state.PlayerItems.Peek();
                switch (latestItem)
                {
                    case MoneyItem money:
                        Console.WriteLine($"You pick up some MONEY, wow it's {money.Value:C}!");
                        break;
                    case WeaponItem weapon:
                        Console.WriteLine($"You pick up the sword, it does {weapon.Damage} damage. WOW! Wait til you see that cute fluffy pup!");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(latestItem));
                }
                break;
            case InvalidDrop:
                Console.WriteLine("No items in your inventory to drop!");
                break;
            case ValidDrop drop:
                switch (drop.Item)
                {
                    case MoneyItem moneyItem:
                        Console.WriteLine($"You dropped {moneyItem.Value:C}. What is wrong with you...?");
                        break;
                    case WeaponItem weaponItem:
                        Console.WriteLine($"You dropped a sword that does {weaponItem.Damage} damage. What if the cute pup gets you??");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(drop.Item));
                }
                break;
            case InvalidUndo:
                Console.WriteLine("Nothing left to undo!");
                break;
            case ValidUndo:
                Console.WriteLine("You undid your last move... ok, what now??");
                break;
            case PlayerQuit:
                Console.WriteLine("Oh running away are we??? You yellow bastard, I'll bite your legs off!");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state.LatestUpdate));
        }
    }
}