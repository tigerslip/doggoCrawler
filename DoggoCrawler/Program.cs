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
using DoggoCrawler;

Draw.PrintHelp();
var state = DungeonBuilder.BuildDungeon();

while (!state.Quit)
{
    var input = Console.ReadKey(true).KeyChar;
    Command.Process(Command.Parse(input), state);
    Draw.Update(state);
}