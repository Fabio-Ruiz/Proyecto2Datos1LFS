using UnityEngine;

public static class GameInput 
{
    public static class Player1
    {
        // Movement
        public const KeyCode LEFT = KeyCode.A;      // Changed from MOVE_LEFT
        public const KeyCode RIGHT = KeyCode.D;     // Changed from MOVE_RIGHT
        public const KeyCode JUMP = KeyCode.W;
        public const KeyCode SHOOT = KeyCode.LeftControl;

        // Powers
        public const KeyCode FORCE_PUSH = KeyCode.Alpha1;
        public const KeyCode SHIELD = KeyCode.Alpha2;
        public const KeyCode AIR_JUMP = KeyCode.Alpha3;
    }

    public static class Player2 // Huargen
    {
        // Movement
        public const KeyCode LEFT = KeyCode.J;      // Changed from MOVE_LEFT
        public const KeyCode RIGHT = KeyCode.L;     // Changed from MOVE_RIGHT
        public const KeyCode JUMP = KeyCode.I;
        public const KeyCode SHOOT = KeyCode.Space;

        // Powers
        public const KeyCode FORCE_PUSH = KeyCode.Alpha7;
        public const KeyCode SHIELD = KeyCode.Alpha8;
        public const KeyCode AIR_JUMP = KeyCode.Alpha9;
    }

    public static class Player3
    {
        // Keyboard controls (fallback)
        public const KeyCode LEFT = KeyCode.K;
        public const KeyCode RIGHT = KeyCode.Semicolon;
        public const KeyCode JUMP = KeyCode.O;
        
        // Controller mappings
        public const string HORIZONTAL_AXIS = "Horizontal";    // Left stick X-axis
        public const KeyCode JUMP_BUTTON = KeyCode.JoystickButton0;      // A button
        public const KeyCode FORCE_PUSH = KeyCode.JoystickButton2;       // X button
        public const KeyCode AIR_JUMP = KeyCode.JoystickButton3;         // Y button
        public const KeyCode SHIELD = KeyCode.JoystickButton1;           // B button
    }
}