namespace DoggoCrawler;

internal static class DungeonBuilder
{
    internal static GameState BuildDungeon()
    {
        // sure you could build a dungeon generator but it's getting late
        var room1 = new Room();
        var room2 = new Room();
        var room3 = new Room();
        var room4 = new Room();

        // connect rooms
        room1.EastRoom = room2;
        room2.WestRoom = room1;

        room2.SouthRoom = room3;
        room3.NorthRoom = room2;

        room3.WestRoom = room4;
        room4.EastRoom = room3;

        room4.NorthRoom = room1;
        room1.SouthRoom = room4;

        room4.Items.Push(new MoneyItem(25));
        room2.Items.Push(new WeaponItem(100));
        
        return new GameState(room1);
    }
}