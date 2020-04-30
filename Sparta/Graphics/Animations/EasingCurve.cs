using System;
using Microsoft.Xna.Framework;

namespace Sparta.Graphics.Animations
{
    internal abstract class EasingCurveFunction
    {
        public enum Type
        {
            In, Out, InOut, OutIn
        }

        protected EasingCurveFunction.Type type;
        protected float period;
        protected float amplitude;
        protected float overshoot;

        public EasingCurveFunction()
            : this(EasingCurveFunction.Type.In, 0.3f, 1f, 1.70158f)
        {
        }

        public EasingCurveFunction(EasingCurveFunction.Type type, float period, float amplitude, float overshoot)
        {
            this.type = type;
            this.period = period;
            this.amplitude = amplitude;
            this.overshoot = overshoot;
        }

        abstract public float Value(float t);
    }

    internal class BackEase : EasingCurveFunction
    {
        public BackEase(EasingCurveFunction.Type type)
            : base (type, 0.3f, 1f, 1.70158f)
        {
        }

        public override float Value(float t)
        {
            float o = (overshoot < 0) ? 1.70158f : overshoot;
            switch (type)
            {
                case EasingCurveFunction.Type.In:
                    return EasingCurve.EaseInBack(t, o);
                case EasingCurveFunction.Type.Out:
                    return EasingCurve.EaseOutBack(t, o);
                case EasingCurveFunction.Type.InOut:
                    return EasingCurve.EaseInOutBack(t, o);
                case EasingCurveFunction.Type.OutIn:
                    return EasingCurve.EaseOutInBack(t, o);
                default:
                    return t;
            }
        }
    }

    public class EasingCurve
    {
        public enum EasingCurveType
        {
            Linear, 
            InCubic, 
            OutCubic, 
            InQuart,
            OutQuart, 
            InOutQuart, 
            OutSine, 
            InOutSine,
            OutExpo,
            InBack,
            OutBack,
            InOutBack,
            OutInBack
        }

        private delegate float EasingFunction(float progress);
        private EasingFunction function;
        private EasingCurveType type;
        private EasingCurveFunction config;

        public EasingCurveType Type 
        {
            get { return type; }
            set
            {
                type = value;
                switch (type)
                {
                    case EasingCurveType.InCubic:
                        function = EaseInCubic;
                        break;
                    case EasingCurveType.OutCubic:
                        function = EaseOutCubic;
                        break;
                    case EasingCurveType.InQuart:
                        function = EaseInQuart;
                        break;
                    case EasingCurveType.OutQuart:
                        function = EaseOutQuart;
                        break;
                    case EasingCurveType.InOutQuart:
                        function = EaseInOutQuart;
                        break;
                    case EasingCurveType.OutSine:
                        function = EaseOutSine;
                        break;
                    case EasingCurveType.InOutSine:
                        function = EaseInOutSine;
                        break;
                    case EasingCurveType.OutExpo:
                        function = EaseOutExpo;
                        break;
                    case EasingCurveType.InBack:
                        function = null;
                        config = new BackEase(EasingCurveFunction.Type.In);
                        break;
                    case EasingCurveType.OutBack:
                        function = null;
                        config = new BackEase(EasingCurveFunction.Type.Out);
                        break;
                    case EasingCurveType.InOutBack:
                        function = null;
                        config = new BackEase(EasingCurveFunction.Type.InOut);
                        break;
                    case EasingCurveType.OutInBack:
                        function = null;
                        config = new BackEase(EasingCurveFunction.Type.OutIn);
                        break;
                    default:
                        function = EaseNone;
                        break;
                }
            }
        }

        public EasingCurve()
            : this(EasingCurveType.Linear)
        {
        }

        public EasingCurve(EasingCurveType type)
        {
            this.Type = type;
        }

        public float ValueForProgress(float progress)
        {
            progress = MathHelper.Clamp(progress, 0f, 1f);
            if (function != null)
            {
                return function(progress);
            }
            else if (config != null)
            {
                return config.Value(progress);
            }
            return progress;
        }

        internal static float EaseNone(float t)
        {
            return t;
        }

        internal static float EaseInCubic(float t)
        {
            return t*t*t;
        }

        internal static float EaseOutCubic(float t)
        {
            t -= 1f;
            return t * t * t + 1f;
        }

        internal static float EaseInQuart(float t)
        {
            return t * t * t * t;
        }

        internal static float EaseOutQuart(float t)
        {
            t -= 1f;
            return - (t*t*t*t - 1);
        }

        internal static float EaseInOutQuart(float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * t * t * t * t;
            }
            else
            {
                t -= 2.0f;
                return -0.5f * (t * t * t * t - 2);
            }
        }

        internal static float EaseOutSine(float t)
        {
            return (float)Math.Sin(t * (Math.PI / 2f));
        }

        internal static float EaseInOutSine(float t)
        {
            return -0.5f * ((float)Math.Cos(Math.PI * t) - 1);
        }

        internal static float EaseOutExpo(float t)
        {
            return (t.Equals(1.0f)) 
                ? 1.0f 
                : 1.001f * (-(float)Math.Pow(2.0f, -10 * t) + 1);
        }

        internal static float EaseInBack(float t, float s)
        {
            return t * t * ((s + 1) * t - s);
        }

        internal static float EaseOutBack(float t, float s)
        {
            t -= 1f;
            return t * t * ((s + 1) * t + s) + 1;
        }

        internal static float EaseInOutBack(float t, float s)
        {
            t *= 2.0f;
            if (t < 1)
            {
                s *= 1.525f;
                return 0.5f * (t * t * ((s + 1) * t - s));
            }
            else
            {
                t -= 2;
                s *= 1.525f;
                return 0.5f * (t * t * ((s + 1) * t + s) + 2);
            }
        }

        internal static float EaseOutInBack(float t, float s)
        {
            if (t < 0.5f) return EaseOutBack(2 * t, s) / 2;
            return EaseInBack(2 * t - 1, s) / 2 + 0.5f;
        }
    }
}
