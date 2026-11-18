using System.Diagnostics;

// Game window boundaries - 24x14
const int Width = 23; //20
const int Height = 13; //12

// Timed Events
const long DrawIntervalMs = 100L; //Screen redraw rate
const long AttackDurationMs = 200L; //Sword Attack
const long SwordFrameDurationMs = AttackDurationMs / 2; //Duration of first half of sword attack animation
const long AttackCooldownMs = 400L; //Cooldown between sword attacks
const long EnemyMoveIntervalMs = 500L; //Time between enemy patrolling moves

const int HorizScale = 2; //This controls horizontal movement scaling (corrects horiz if 2)
// 2 gives each tile two terminal columns.
// 1 gives each tile one terminal column.


// Player starting position - better to write as class aprams (player.x, player.y) i think
int playerX = 11;
int playerY = 6;

// Clears screen then draws boundaries and draws player
void DrawGame(int x, int y, Direction playerFaces, bool isAttacking, long attackElapsedMs, List<Enemy> enemies)
{
    Console.Clear();
    DrawWallBoundaries();

    foreach (Enemy enemy in enemies)
    {
        if (enemy.IsAlive)
        {
            Console.SetCursorPosition(enemy.X * HorizScale, enemy.Y);
            Console.Write("&");
        }
    }

    Console.SetCursorPosition(x * HorizScale, y);
    Console.Write('@');
    if (isAttacking)
    {
        DrawSword(playerFaces, x, y, attackElapsedMs);
    }
    Console.SetCursorPosition(4, Height + 1);
    Console.Write($"{playerFaces}");

    for (int i = 0; i < enemies.Count; i++)
    {
        Console.SetCursorPosition(15, Height + (i + 1));
        Console.Write($"Enemy {i + 1} Health: {enemies[i].Health}");
    }
}

// Sword Animations
void DrawSword(Direction playerFaces, int x, int y, long attackElapsedMs)
{
    if (playerFaces == Direction.North && (BoundaryCheck(x, y - 1)))
    {
        if (attackElapsedMs < SwordFrameDurationMs)
        {
            Console.SetCursorPosition(x * HorizScale, y - 1);
            Console.Write("╱");
        }
        else if (attackElapsedMs >= SwordFrameDurationMs && attackElapsedMs < AttackDurationMs)
        {
            Console.SetCursorPosition(x * HorizScale, y - 1);
            Console.Write("┃");
        }
    }
    else if (playerFaces == Direction.South && (BoundaryCheck(x, y + 1)))
    {
        if (attackElapsedMs < SwordFrameDurationMs)
        {
            Console.SetCursorPosition(x * HorizScale, y + 1);
            Console.Write("╲");
        }
        else if (attackElapsedMs >= SwordFrameDurationMs && attackElapsedMs < AttackDurationMs)
        {
            Console.SetCursorPosition(x * HorizScale, y + 1);
            Console.Write("┃");
        }
    }
    else if (playerFaces == Direction.East && (BoundaryCheck(x + 1, y)))
    {
        if (attackElapsedMs < SwordFrameDurationMs && (BoundaryCheck(x, y - 1)))
        {
            Console.SetCursorPosition((x * HorizScale) + 1, y - 1);
            Console.Write("╱");
        }
        else if (attackElapsedMs >= SwordFrameDurationMs && attackElapsedMs < AttackDurationMs)
        {
            Console.SetCursorPosition((x * HorizScale) + 1, y);
            Console.Write("━━");
        }
    }
    else if (playerFaces == Direction.West && (BoundaryCheck(x - 1, y)))
    {
        if (attackElapsedMs < SwordFrameDurationMs && (BoundaryCheck(x, y - 1)))
        {
            Console.SetCursorPosition((x * HorizScale) - 1, y - 1);
            Console.Write("╲");
        }
        else if (attackElapsedMs >= SwordFrameDurationMs && attackElapsedMs < AttackDurationMs)
        {
            Console.SetCursorPosition((x * HorizScale) - 2, y);
            Console.Write("━━");
        }
    }
}

// Draw walls for room bounday
//  - eventually will need to create doors for player to go through
void DrawWallBoundaries()
{
    for (int i = 0; i <= Width * HorizScale; i++)
    {
        Console.SetCursorPosition(i, Height);
        Console.Write('#');
        Console.SetCursorPosition(i, 0);
        Console.Write('#');
    }
    for (int i = 1; i < Height; i++)
    {
        Console.SetCursorPosition(0, i);
        Console.Write('#');
        Console.SetCursorPosition(Width * HorizScale, i);
        Console.Write('#');
    }
    return;
}

// Restricts movements to within the boundaries of the game 
// - DOWN THE LINE CHECK OBSTABLES IN GAME
static bool BoundaryCheck(int playerX, int playerY)
{
    if (playerY >= Height || playerY <= 0 || playerX >= Width || playerX <= 0)
    {
        return false;
    }
    else
    {
        return true;
    }
}

bool IsOccupiedByEnemy(int x, int y, List<Enemy> enemies)
{
    foreach (Enemy enemy in enemies)
    {
        if (enemy.IsAlive)
        {
            if (x == enemy.X && y == enemy.Y)
            {
                return true;
            }
        }
    }
    return false;
}

// Controls player movement within the game console
(int x, int y, Direction playerFaces) UpdatePlayerPosition(int x, int y, ConsoleKey input, Direction playerFaces, List<Enemy> enemies)
{
    if (input == ConsoleKey.RightArrow)
    {
        bool blockedByEnemy = IsOccupiedByEnemy(x + 1, y, enemies);
        if (!blockedByEnemy) //Checks if player X,Y position is blocked by an enemy
        {
            //Checks to see if movement is impeded by wall boundary
            x = BoundaryCheck((x + 1), y) ? (x + 1) : x;
        }
        playerFaces = Direction.East;
    }
    if (input == ConsoleKey.LeftArrow)
    {
        bool blockedByEnemy = IsOccupiedByEnemy(x - 1, y, enemies);
        if (!blockedByEnemy)
        {
            x = BoundaryCheck((x - 1), y) ? (x - 1) : x;
        }
        playerFaces = Direction.West;
    }
    if (input == ConsoleKey.DownArrow)
    {
        bool blockedByEnemy = IsOccupiedByEnemy(x, y + 1, enemies);
        if (!blockedByEnemy)
        {
            y = BoundaryCheck(x, (y + 1)) ? (y + 1) : y;
        }
        playerFaces = Direction.South;
    }
    if (input == ConsoleKey.UpArrow)
    {
        bool blockedByEnemy = IsOccupiedByEnemy(x, y - 1, enemies);
        if (!blockedByEnemy)
        {
            y = BoundaryCheck(x, (y - 1)) ? (y - 1) : y;
        }
        playerFaces = Direction.North;
    }
    return (x, y, playerFaces);
}

void UpdateEnemyPatrols(List<Enemy> enemies, int playerX, int playerY, long currentTimeMs)
{
    foreach (Enemy enemy in enemies)
    {
        if (enemy.IsAlive &&
            (currentTimeMs - enemy.LastMovedAt >= EnemyMoveIntervalMs))
        {
            int nextX = enemy.X + enemy.PatrolStepX;
            if (BoundaryCheck(nextX, enemy.Y) && !(nextX == playerX && enemy.Y == playerY) && !IsOccupiedByEnemy(nextX, enemy.Y, enemies))
            {
                enemy.X = nextX;
            }
            else
            {
                enemy.PatrolStepX *= -1;
            }
            enemy.LastMovedAt = currentTimeMs;
        }
    }
}

// Main Game Loop
void Main(int playerX, int playerY)
{
    // Hides flashing cursor during gameplay
    Console.CursorVisible = false;

    // Intialize variable to keep track which direction player faces
    Direction playerFaces = Direction.South;

    // Is player currently attacking - for sword effect and damage
    bool isAttacking = false;

    // Enemy
    List<Enemy> enemies = new List<Enemy> { new Enemy(18, 3, 3), new Enemy(4, 3, 4) };

    //Game loop internal clock
    Stopwatch timeElapsed = Stopwatch.StartNew();
    //Event timekeepers
    long timeLastUpdated = 0L; //Stores the last updated time value for comparison
    long attackStartedAt = -AttackCooldownMs; //Start of player attack
    long attackElapsedMs = 0L; //Elapsed time since start of player attack

    DrawGame(playerX, playerY, playerFaces, isAttacking, attackElapsedMs, enemies);

    while (true)
    {
        if (isAttacking == true &&
            (timeElapsed.ElapsedMilliseconds - attackStartedAt >= AttackDurationMs))
        {
            isAttacking = false;
        }

        // Each enemy patrols back and forth across the room
        UpdateEnemyPatrols(enemies, playerX, playerY, timeElapsed.ElapsedMilliseconds);

        if (Console.KeyAvailable)
        {
            ConsoleKey input = Console.ReadKey(true).Key;

            // Being attack after 400ms cooldown for the attack (& not already mid-attack)
            if (input == ConsoleKey.Spacebar && !isAttacking
                && timeElapsed.ElapsedMilliseconds - attackStartedAt >= AttackCooldownMs)
            {
                // Set attack to true and begin counting start of attack
                isAttacking = true;
                attackStartedAt = timeElapsed.ElapsedMilliseconds;

                int targetX = playerX;
                int targetY = playerY;
                if (playerFaces == Direction.North)
                {
                    targetY--;
                }
                else if (playerFaces == Direction.South)
                {
                    targetY++;
                }
                else if (playerFaces == Direction.East)
                {
                    targetX++;
                }
                else if (playerFaces == Direction.West)
                {
                    targetX--;
                }

                foreach (Enemy enemy in enemies)
                {
                    if (targetX == enemy.X && targetY == enemy.Y && enemy.IsAlive)
                    {
                        enemy.TakeDamage(1);
                    }
                }
            }

            // This is for breaking out of program when debugging
            if (input == ConsoleKey.Enter)
            {
                Console.CursorVisible = true;
                Console.Clear();
                break;
            }

            (playerX, playerY, playerFaces) = UpdatePlayerPosition(playerX, playerY, input, playerFaces, enemies);
        }

        if (timeElapsed.ElapsedMilliseconds - timeLastUpdated >= DrawIntervalMs)
        {
            attackElapsedMs = timeElapsed.ElapsedMilliseconds - attackStartedAt;
            DrawGame(playerX, playerY, playerFaces, isAttacking, attackElapsedMs, enemies);
            timeLastUpdated = timeElapsed.ElapsedMilliseconds;
        }
        Thread.Sleep(1);
    }
}

Main(playerX, playerY);

enum Direction
{
    North,
    South,
    East,
    West
}