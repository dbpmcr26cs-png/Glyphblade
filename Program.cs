// Game window boundaries - 24x14
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

const int width = 23; //20
const int height = 13; //12

const int horizScale = 2; //This controls horizontal movement scaling (corrects horiz if 2)
// 2 gives each tile two terminal columns.
// 1 gives each tile one terminal column.


// Player starting position - better to write as class aprams (player.x, player.y) i think
int player_x = 11;
int player_y = 6;


// Clears screen then draws boundaries and draws player
void drawGame(int x, int y)
{
    Console.Clear();
    drawWallBoundaries(width, height);
    Console.SetCursorPosition(x * horizScale, y);
    Console.Write('@');
}
;

// Draw walls for room bounday
//  - eventually will need to create doors for player to go through
void drawWallBoundaries(int width, int height)
{
    for (int i = 0; i <= width * horizScale; i++)
    {
        Console.SetCursorPosition(i, height);
        Console.Write('#');
        Console.SetCursorPosition(i, 0);
        Console.Write('#');
    }
    for (int i = 1; i < height; i++)
    {
        Console.SetCursorPosition(0, i);
        Console.Write('#');
        Console.SetCursorPosition(width * horizScale, i);
        Console.Write('#');
    }
    return;
}

// Restricts movements to within the boundaries of the game 
// - DOWN THE LINE CHECK OBSTABLES IN GAME
static bool boundaryCheck(int width, int height, int player_x, int player_y)
{
    if (player_y >= height || player_y <= 0 || player_x >= width || player_x <= 0)
    {
        return false;
    }
    else
    {
        return true;
    }
}

// Controls player movement within the game console
(int x, int y) updatePlayerPosition(int x, int y, ConsoleKey input)
{
    if (input == ConsoleKey.RightArrow)
    {
        x = boundaryCheck(width, height, (x + 1), y) ? (x + 1) : x;
    }
    if (input == ConsoleKey.LeftArrow)
    {
        x = boundaryCheck(width, height, (x - 1), y) ? (x - 1) : x;
    }
    if (input == ConsoleKey.DownArrow)
    {
        y = boundaryCheck(width, height, x, (y + 1)) ? (y + 1) : y;
    }
    if (input == ConsoleKey.UpArrow)
    {
        y = boundaryCheck(width, height, x, (y - 1)) ? (y - 1) : y;
    }
    return (x, y);
}

// Main Game Loop
void main(int player_x, int player_y, int width, int height)
{
    // Hides flashing cursor during gameplay
    Console.CursorVisible = false;

    drawGame(player_x, player_y);

    Stopwatch timeElapsed = Stopwatch.StartNew();
    long timeLastUpdated = 0L; //Stores the last updated time value for comparison

    while (true)
    {
        if (Console.KeyAvailable)
        {
            ConsoleKey input = Console.ReadKey(true).Key;

            // This is for breaking out of program when debugging
            if (input == ConsoleKey.Enter)
            {
                Console.CursorVisible = true;
                Console.Clear();
                break;
            }

            (player_x, player_y) = updatePlayerPosition(player_x, player_y, input);
        }

        if (timeElapsed.ElapsedMilliseconds - timeLastUpdated >= 100L)
        {
            drawGame(player_x, player_y);
            timeLastUpdated = timeElapsed.ElapsedMilliseconds;
        }
        Thread.Sleep(1);
    }
}

main(player_x, player_y, width, height);