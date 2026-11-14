using System.Diagnostics;

// Game window boundaries - 24x14
const int width = 23; //20
const int height = 13; //12

// Timed Events
const long drawIntervalMs = 100L; //Screen redraw rate
const long attackDurationMs = 200L; //Sword Attack
const long attackCooldownMs = 400L; //Cooldown between sword attacks

const int horizScale = 2; //This controls horizontal movement scaling (corrects horiz if 2)
// 2 gives each tile two terminal columns.
// 1 gives each tile one terminal column.


// Player starting position - better to write as class aprams (player.x, player.y) i think
int player_x = 11;
int player_y = 6;


// Clears screen then draws boundaries and draws player
void drawGame(int x, int y, Direction playerFaces, bool isAttacking, long attackElapsedMs)
{
    Console.Clear();
    drawWallBoundaries(width, height);
    Console.SetCursorPosition(x * horizScale, y);
    Console.Write('@');
    if (isAttacking)
    {
        drawSword(playerFaces, x, y, attackElapsedMs);
    }
    Console.SetCursorPosition(4, height + 1);
    Console.Write($"{playerFaces}");
}

// Sword Animations
void drawSword(Direction playerFaces, int x, int y, long attackElapsedMs)
{
    if (playerFaces == Direction.North && (boundaryCheck(width, height, x, y - 1)))
    {
        if (attackElapsedMs < 100L)
        {
            Console.SetCursorPosition(x * horizScale, y - 1);
            Console.Write("╱");
        }
        else if (attackElapsedMs >= 100L && attackElapsedMs < 200L)
        {
            Console.SetCursorPosition(x * horizScale, y - 1);
            Console.Write("┃");
        }
    }
    else if (playerFaces == Direction.South && (boundaryCheck(width, height, x, y + 1)))
    {
        if (attackElapsedMs < 100L)
        {
            Console.SetCursorPosition(x * horizScale, y + 1);
            Console.Write("╲");
        }
        else if (attackElapsedMs >= 100L && attackElapsedMs < 200L)
        {
            Console.SetCursorPosition(x * horizScale, y + 1);
            Console.Write("┃");
        }
    }
    else if (playerFaces == Direction.East && (boundaryCheck(width, height, x + 1, y)))
    {
        if (attackElapsedMs < 100L && (boundaryCheck(width, height, x, y - 1)))
        {
            Console.SetCursorPosition((x * horizScale) + 1, y - 1);
            Console.Write("╱");
        }
        else if (attackElapsedMs >= 100L && attackElapsedMs < 200L)
        {
            Console.SetCursorPosition((x * horizScale) + 1, y);
            Console.Write("━━");
        }
    }
    else if (playerFaces == Direction.West && (boundaryCheck(width, height, x - 1, y)))
    {
        if (attackElapsedMs < 100L && (boundaryCheck(width, height, x, y - 1)))
        {
            Console.SetCursorPosition((x * horizScale) - 1, y - 1);
            Console.Write("╲");
        }
        else if (attackElapsedMs >= 100L && attackElapsedMs < 200L)
        {
            Console.SetCursorPosition((x * horizScale) - 2, y);
            Console.Write("━━");
        }
    }
}

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
(int x, int y, Direction playerFaces) updatePlayerPosition(int x, int y, ConsoleKey input, Direction playerFaces)
{
    if (input == ConsoleKey.RightArrow)
    {
        x = boundaryCheck(width, height, (x + 1), y) ? (x + 1) : x;
        playerFaces = Direction.East;
    }
    if (input == ConsoleKey.LeftArrow)
    {
        x = boundaryCheck(width, height, (x - 1), y) ? (x - 1) : x;
        playerFaces = Direction.West;
    }
    if (input == ConsoleKey.DownArrow)
    {
        y = boundaryCheck(width, height, x, (y + 1)) ? (y + 1) : y;
        playerFaces = Direction.South;
    }
    if (input == ConsoleKey.UpArrow)
    {
        y = boundaryCheck(width, height, x, (y - 1)) ? (y - 1) : y;
        playerFaces = Direction.North;
    }
    return (x, y, playerFaces);
}

// Main Game Loop
void main(int player_x, int player_y, int width, int height)
{
    // Hides flashing cursor during gameplay
    Console.CursorVisible = false;

    // Intialize variable to keep track which direction player faces
    Direction playerFaces = Direction.South;

    // Is player currently attacking - for sword effect and damage
    bool isAttacking = false;


    //Game loop internal clock
    Stopwatch timeElapsed = Stopwatch.StartNew();
    //Event timekeepers
    long timeLastUpdated = 0L; //Stores the last updated time value for comparison
    long attackStartedAt = -attackCooldownMs; //Start of player attack
    long attackElapsedMs = 0L; //Elapsed time since start of player attack

    drawGame(player_x, player_y, playerFaces, isAttacking, attackElapsedMs);

    while (true)
    {
        if (isAttacking == true &&
            (timeElapsed.ElapsedMilliseconds - attackStartedAt >= attackDurationMs))
        {
            isAttacking = false;
        }

        if (Console.KeyAvailable)
        {
            ConsoleKey input = Console.ReadKey(true).Key;

            // Being attack after 400ms cooldown for the attack (& not already mid-attack)
            if (input == ConsoleKey.Spacebar && !isAttacking
                && timeElapsed.ElapsedMilliseconds - attackStartedAt >= attackCooldownMs)
            {
                // Set attack to true and begin counting start of attack
                isAttacking = true;
                attackStartedAt = timeElapsed.ElapsedMilliseconds;
            }

            // This is for breaking out of program when debugging
            if (input == ConsoleKey.Enter)
            {
                Console.CursorVisible = true;
                Console.Clear();
                break;
            }

            (player_x, player_y, playerFaces) = updatePlayerPosition(player_x, player_y, input, playerFaces);
        }

        if (timeElapsed.ElapsedMilliseconds - timeLastUpdated >= drawIntervalMs)
        {
            attackElapsedMs = timeElapsed.ElapsedMilliseconds - attackStartedAt;
            drawGame(player_x, player_y, playerFaces, isAttacking, attackElapsedMs);
            timeLastUpdated = timeElapsed.ElapsedMilliseconds;
        }
        Thread.Sleep(1);
    }
}

main(player_x, player_y, width, height);

enum Direction
{
    North,
    South,
    East,
    West
}