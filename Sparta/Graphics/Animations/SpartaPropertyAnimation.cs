using System;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Sparta.Graphics.Animations
{
    public class SpartaPropertyAnimation : SpartaAbstractAnimation
    {
        public object From { get; set; }
        public object To { get; set; }

        private object property;
        private EasingCurve easingCurve;
        private readonly object instance;
        private readonly PropertyInfo propertyInstance;
        private readonly string propertyName;

        public SpartaPropertyAnimation(object instance, string propertyName, object from, object to, TimeSpan time, EasingCurve.EasingCurveType easingCurve, FinishedDelegate callback)
        {
            this.instance = instance;
            this.propertyName = propertyName;
            this.propertyInstance = instance.GetType().GetProperty(propertyName);
            this.property = propertyInstance.GetValue(instance, null);

            this.From = from;
            this.To = to;

            this.Duration = time;
            this.easingCurve = new EasingCurve(easingCurve);

            Finished = callback;

            State = SpartaAnimationState.Stopped;
        }

        public override void Begin()
        {
            property = Direction == SpartaAnimationDirection.Forward ? From : To;
            propertyInstance.SetValue(instance, property, null);
            State = SpartaAnimationState.Running;

            if (Duration.TotalSeconds == 0d)
            {
                State = SpartaAnimationState.Stopped;

                if (Finished != null)
                {
                    Finished();
                }
                if (InternalFinished != null)
                {
                    InternalFinished();
                }

                property = Direction == SpartaAnimationDirection.Forward ? To : From;
                propertyInstance.SetValue(instance, property, null);
                return;
            }

            propertyInstance.SetValue(instance, property, null);
            CurrentTime = TimeSpan.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (State == SpartaAnimationState.Running)
            {
                object from = Direction == SpartaAnimationDirection.Forward ? From : To;
                object to = Direction == SpartaAnimationDirection.Forward ? To : From;
                float progress = easingCurve.ValueForProgress((float)CurrentTime.TotalSeconds / (float)Duration.TotalSeconds);

                if (property is int)
                {
                    int intProperty = (int)property;
                    int intFrom = (int)from;
                    int intTo = (int)to;

                    intProperty = (int)Math.Floor(intFrom + (intTo - intFrom) * progress);
                    propertyInstance.SetValue(instance, intProperty, null);
                    property = intProperty;

                    if (CurrentTime > Duration)
                    {
                        endAnimation<int>();
                    }
                }
                else if (property is float)
                {
                    float floatProperty = (float)property;
                    float floatFrom = (float)from;
                    float floatTo = (float)to;

                    floatProperty = floatFrom + (floatTo - floatFrom) * progress;
                    propertyInstance.SetValue(instance, floatProperty, null);
                    property = floatProperty;

                    if (CurrentTime > Duration)
                    {
                        endAnimation<float>();
                    }
                }
                else if (property is Vector2)
                {
                    Vector2 vectorProperty = (Vector2)property;
                    Vector2 vectorFrom = (Vector2)from;
                    Vector2 vectorTo = (Vector2)to;

                    vectorProperty = vectorFrom + (vectorTo - vectorFrom) * progress;
                    propertyInstance.SetValue(instance, vectorProperty, null);
                    property = vectorProperty;

                    if (CurrentTime > Duration)
                    {
                        endAnimation<Vector2>();
                    }
                }
                CurrentTime += gameTime.ElapsedGameTime;
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            GC.SuppressFinalize(easingCurve);
            easingCurve = null;

            base.Dispose();
        }

        private void endAnimation<T>()
        {
            property = Direction == SpartaAnimationDirection.Forward ? (T)To : (T)From;
            propertyInstance.SetValue(instance, property, null);

            State = SpartaAnimationState.Stopped;

            if (Finished != null)
            {
                Finished();
            }
            if (InternalFinished != null)
            {
                InternalFinished();
            }
        }
    }
}
