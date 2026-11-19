public enum EnemyState
{
    Idle, //Stays still, waiting for player
    Patrolling, //Follows a route
    Chasing, //Moves toward the player
    Attacking //Performs an attack
}