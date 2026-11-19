public class Enemy
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; private set; }
    public long LastMovedAt { get; set; } = 0L;
    public long LastAttackedAt { get; set; } = 0L;
    public int PatrolStepX { get; set; } = 1;
    // public int PatrolStepY { get; set; } = 1;
    public EnemyState State { get; set; } = EnemyState.Patrolling;

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