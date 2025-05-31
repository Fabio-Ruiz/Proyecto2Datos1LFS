public static class GameConstants
{
    // Physics
    public const float GRAVITY_SCALE = 1f;
    public const float DRAG = 0.5f;          // Aumentado para dar más fricción
    public const float ANGULAR_DRAG = 0.05f;
    
    // Combat
    public const float SHOOT_COOLDOWN = 0.25f;
    
    // Movement
    public const float BASE_SPEED = 3f;      // Reducido de 5f a 3f
    public const float BASE_JUMP_FORCE = 6f; // Reducido de 7f a 6f
    public const float COLLISION_FORCE = 5f;  // Nueva constante para colisiones

    // Respawn
    public const float FALL_THRESHOLD = -5f;
    public const float RESPAWN_HEIGHT = 2f;
}