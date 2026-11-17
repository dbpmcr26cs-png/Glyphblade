public class Enemy
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; }

    public Enemy(int x, int y, int health)
    {
        X = x;
        Y = y;
        Health = health;
    }
    public void TakeDamage(int amount)
    {
        
    }
}