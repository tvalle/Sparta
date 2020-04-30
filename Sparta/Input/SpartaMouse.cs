using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Sparta.Input
{
    public static class SpartaMouse
    {
        private static MouseState mouseState;
        public static MouseState MouseState
        {
            get { return IsAccepted ? emptyMouseState : mouseState; }
            set { mouseState = value; }
        }

        public static bool IsAccepted { get; private set; }
        private static readonly MouseState emptyMouseState;

        public static void Update()
        {
            MouseState = Mouse.GetState();
            IsAccepted = false;
        }

        public static void Accept()
        {
            IsAccepted = true;
        }
    }
}
