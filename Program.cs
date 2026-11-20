using System.Diagnostics;
using System.Runtime.InteropServices;

// Game window boundaries - 24x14
const int Width = 23; //20
const int Height = 13; //12

const int HorizScale = 2; //This controls horizontal movement scaling (corrects horiz if 2)
// 2 gives each tile two terminal columns.
// 1 gives each tile one terminal column.


// Timed Events
const long DrawIntervalMs = 33L; //Screen redraw rate
const long AttackDurationMs = 200L; //Sword Attack
const long SwordFrameDurationMs = AttackDurationMs / 2; //Duration of first half of sword attack animation
const long AttackCooldownMs = 400L; //Cooldown between sword attacks
const long EnemyMoveIntervalMs = 500L; //Time between enemy patrolling moves
const long EnemyAttackCooldownMs = 1000L; //Time between enemy attacks
const long EnemyWindUpDurationMs = 500L; //Time from start of attack to actual effect (allows player time to dodge)
const long EnemyFlashIntervalMs = 100L; //Flashes enemy (hides) while they are preparing attack (wind-up)

// Enemy Behavior
const int EnemyDetectionRange = 5;

// Clears screen then draws boundaries and draws player
void DrawGame(int x, int y, Direction playerFaces, bool isAttacking, long currentTimeMs, long attackElapsedMs, List<Enemy> enemies, Player player)
{
    Console.Clear();
    Console.ResetColor();
    DrawWallBoundaries();

    foreach (Enemy enemy in enemies)
    {
        long windUpElapsedMs = currentTimeMs - enemy.AttackStartedAt;

        //Makes enemy flash twice, 0-99Ms and 200-299Ms after attack starts
        bool hiddenDuringWindUp = enemy.IsWindingUp &&
            (windUpElapsedMs < EnemyFlashIntervalMs ||
            (windUpElapsedMs >= EnemyFlashIntervalMs * 2 &&
            windUpElapsedMs < EnemyFlashIntervalMs * 3));

        if (enemy.IsAlive && !hiddenDuringWindUp)
        {
            Console.SetCursorPosition(enemy.X * HorizScale, enemy.Y);
            if (enemy.IsWindingUp)
            {
                //Attack Warning Color
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write("&");
            Console.ResetColor();
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

    //Write player health for testing
    Console.SetCursorPosition(15, Height + 3);
    Console.WriteLine($"Player Health: {player.Health}");
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
        if (enemy.IsAlive && enemy.State == EnemyState.Patrolling && !enemy.IsWindingUp &&
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

void UpdateEnemyChasing(List<Enemy> enemies, int playerX, int playerY, long currentTimeMs)
{
    foreach (Enemy enemy in enemies)
    {
        if (enemy.IsAlive && enemy.State == EnemyState.Chasing && !enemy.IsWindingUp &&
            (currentTimeMs - enemy.LastMovedAt >= EnemyMoveIntervalMs))
        {
            (int, int) nextStep = FindNextStep(enemy.X, enemy.Y, playerX, playerY, enemies);

            if (nextStep != (playerX, playerY) && !IsOccupiedByEnemy(nextStep.Item1, nextStep.Item2, enemies))
            {
                (enemy.X, enemy.Y) = nextStep; //Update enemy coord;
            }
            enemy.LastMovedAt = currentTimeMs;
        }
    }
}

void UpdateEnemyAttacking(List<Enemy> enemies, Player player, long currentTimeMs)
{
    foreach (Enemy enemy in enemies)
    {
        //Enemy Attack (Wind-Up Begins)
        if (enemy.IsAlive && enemy.State == EnemyState.Attacking && !(enemy.IsWindingUp) &&
            (currentTimeMs - enemy.LastAttackedAt >= EnemyAttackCooldownMs))
        {
            enemy.IsWindingUp = true;
            enemy.AttackTargetX = player.X;
            enemy.AttackTargetY = player.Y;
            enemy.AttackStartedAt = currentTimeMs;
            enemy.LastAttackedAt = currentTimeMs;
        }
        //Wind-Up Finishes & Attack Starts
        else if (enemy.IsAlive && (enemy.IsWindingUp) && (currentTimeMs - enemy.AttackStartedAt >= EnemyWindUpDurationMs))
        {
            if (ManhattanDistance(enemy.AttackTargetX, enemy.AttackTargetY, player.X, player.Y) == 0)
            {
                player.TakeDamage(1);
            }
            enemy.IsWindingUp = false;
        }
    }
}

int ManhattanDistance(int x1, int y1, int x2, int y2)
{
    return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
}

(int x, int y) FindNextStep(int startX, int startY, int targetX, int targetY, List<Enemy> enemies)
{
    // Return start coord if they match target coord
    if ((startX, startY) == (targetX, targetY))
    {
        return (startX, startY);
    }

    Queue<(int x, int y)> q = new Queue<(int x, int y)>();
    q.Enqueue((startX, startY));

    HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
    visited.Add((startX, startY));

    // Contains all linked visited nodes as k,v
    Dictionary<(int x, int y), (int x, int y)> cameFrom = new();

    while (q.Count > 0)
    {
        (int, int) curr = q.Dequeue();

        if (curr == (targetX, targetY))
        {
            //Trace back path
            (int x, int y) step = curr;
            while (cameFrom[step] != (startX, startY))
            {
                step = cameFrom[step];
            }
            return step; //Return first step in path (after start coord)
        }

        //List neighbor coordinates
        (int, int)[] neighbors =
        {
            (curr.Item1, curr.Item2 - 1), //North
            (curr.Item1, curr.Item2 + 1), //South
            (curr.Item1 + 1, curr.Item2), //East
            (curr.Item1 - 1, curr.Item2), //West
        };

        foreach ((int, int) neighbor in neighbors)
        {
            if (!visited.Contains(neighbor) && !IsOccupiedByEnemy(neighbor.Item1, neighbor.Item2, enemies) && BoundaryCheck(neighbor.Item1, neighbor.Item2))
            {
                visited.Add(neighbor); //Add neighbor to hashset
                cameFrom[neighbor] = curr; //Original coord (curr) is V, for 'Neighbor' Key (tells how we got here)
                q.Enqueue(neighbor); //Add to queue for loop
            }
        }
    }
    return (startX, startY); //Return start coord if target isn't found
}

void UpdateEnemyStates(int playerX, int playerY, List<Enemy> enemies)
{
    foreach (Enemy enemy in enemies)
    {
        if (enemy.IsAlive)
        {
            int distance = ManhattanDistance(playerX, playerY, enemy.X, enemy.Y);
            if (distance == 1)
            {
                enemy.State = EnemyState.Attacking;
            }
            else if (distance <= EnemyDetectionRange)
            {
                enemy.State = EnemyState.Chasing;
            }
            else
            {
                enemy.State = EnemyState.Patrolling;
            }
        }
    }
}

// Main Game Loop
void Main()
{
    // Hides flashing cursor during gameplay
    Console.CursorVisible = false;

    // Intialize variable to keep track which direction player faces
    Direction playerFaces = Direction.South;

    // Is player currently attacking - for sword effect and damage
    bool isAttacking = false;

    //Player
    Player player = new Player(11, 6, 5);

    // Enemy
    List<Enemy> enemies = new List<Enemy> { new Enemy(18, 3, 3), new Enemy(4, 3, 4) };

    //Game loop internal clock
    Stopwatch timeElapsed = Stopwatch.StartNew();
    //Event timekeepers
    long timeLastUpdated = 0L; //Stores the last updated time value for comparison
    long attackStartedAt = -AttackCooldownMs; //Start of player attack
    long attackElapsedMs = 0L; //Elapsed time since start of player attack

    DrawGame(player.X, player.Y, playerFaces, isAttacking, timeElapsed.ElapsedMilliseconds, attackElapsedMs, enemies, player);

    while (player.IsAlive)
    {
        if (isAttacking == true &&
            (timeElapsed.ElapsedMilliseconds - attackStartedAt >= AttackDurationMs))
        {
            isAttacking = false;
        }

        // Each enemy patrols back and forth across the room
        UpdateEnemyStates(player.X, player.Y, enemies);
        UpdateEnemyPatrols(enemies, player.X, player.Y, timeElapsed.ElapsedMilliseconds);
        UpdateEnemyChasing(enemies, player.X, player.Y, timeElapsed.ElapsedMilliseconds);
        UpdateEnemyAttacking(enemies, player, timeElapsed.ElapsedMilliseconds);
        if (!player.IsAlive)
        {
            break;
        }

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

                int targetX = player.X;
                int targetY = player.Y;
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

            (player.X, player.Y, playerFaces) = UpdatePlayerPosition(player.X, player.Y, input, playerFaces, enemies);
        }

        if (timeElapsed.ElapsedMilliseconds - timeLastUpdated >= DrawIntervalMs)
        {
            attackElapsedMs = timeElapsed.ElapsedMilliseconds - attackStartedAt;
            DrawGame(player.X, player.Y, playerFaces, isAttacking, timeElapsed.ElapsedMilliseconds, attackElapsedMs, enemies, player);
            timeLastUpdated = timeElapsed.ElapsedMilliseconds;
        }
        Thread.Sleep(1);
    }
    // GAME OVER!
    if (!player.IsAlive)
    {
        Console.Clear();
        Console.SetCursorPosition(15, 6);
        Console.WriteLine("Game Over!");
        Thread.Sleep(1500);
        while (Console.KeyAvailable) //While loop discards pending key presses
        {
            Console.ReadKey(true);
        }
        Console.ReadKey(true);
    }
    Console.Clear();
    Console.CursorVisible = true;
}

Main();

enum Direction
{
    North,
    South,
    East,
    West
}