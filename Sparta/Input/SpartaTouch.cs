using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sparta.Input
{
    public static class SpartaTouch
    {
        private static TouchCollection touchCollection;
        public static TouchCollection TouchCollection 
        {
            get { return IsAccepted ? emptyTouchLocation : touchCollection; }
            set { touchCollection = value; }
        }

        public static bool IsAccepted { get; private set; }
        private static readonly TouchCollection emptyTouchLocation;

        public static void Update()
        {
            TouchCollection = TouchPanel.GetState();
            IsAccepted = false;
        }

        public static void Accept()
        {
            IsAccepted = true;
        }
    }
}
