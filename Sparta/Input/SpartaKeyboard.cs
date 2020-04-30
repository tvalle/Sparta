using Microsoft.Xna.Framework.Input;

namespace Sparta.Input
{
    public static class SpartaKeyboard
    {
        public static KeyboardState CurrentKeyboardState { get; private set; }
        public static KeyboardState LastKeyboardState { get; private set; }

        public static void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }

        public static bool IsNewKeyPress(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key) && LastKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key);
        }
    }
}
