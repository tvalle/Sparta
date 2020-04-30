using System;
using Sparta.Graphics.Animations;

namespace Sparta.Graphics.Transitions
{
    public struct SpartaTransition
    {
        public TimeSpan transitionTime;
        public SpartaTransitionType transitionType;
        public EasingCurve.EasingCurveType transitionEasing;

        public static readonly SpartaTransition Default = new SpartaTransition() { transitionTime = TimeSpan.Zero, transitionType = SpartaTransitionType.None, transitionEasing = EasingCurve.EasingCurveType.Linear };

        public static SpartaTransition Create(TimeSpan transitionTime, SpartaTransitionType transitionType, EasingCurve.EasingCurveType transitionEasing)
        {
            return new SpartaTransition() { transitionTime = transitionTime, transitionType = transitionType, transitionEasing = transitionEasing };
        }
    }
}
