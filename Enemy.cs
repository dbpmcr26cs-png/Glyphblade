using System.Dynamic;

public class Enemy
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; private set; }
    public long LastMovedAt { get; set; } = 0L;
    public long LastAttackedAt { get; set; } = 0L;
    public long AttackStartedAt { get; set; } = 0L;
    public int PatrolStepX { get; set; } = 1;
    public int PatrolStepY { get; set; } = 1; //Not used yet
    public int AttackTargetX { get; set;}
    public int AttackTargetY { get; set; }
    public bool IsWindingUp { get; set; } = false;

    //defunct - used when flash happens during attack instead of wind-up
    // public long StrikeFlashUntil { get; set; } = 0L;

    public EnemyState State { get; set; } = EnemyState.Patrolling;
    public Direction Facing { get; set; } = Direction.South;

    public Enemy(int x, int y, int health)
    {
        X = x;
        Y = y;
        Health = health;
    }
    public void TakeDamage(int amount)
    {
        //Reduce health by amount, make 0 lower limit
        Health = ((Health - amount) > 0) ? (Health - amount) : 0;
    }
    public bool IsAlive
    {
        get { return Health > 0; }
    }
}

/*
0–99 ms     Hidden
100–199 ms  Warning colour
200–299 ms  Hidden
500 ms      Strike
300–499 ms  Warning colour
*/