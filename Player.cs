public class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; private set; }
    public Player(int x, int y, int health)
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