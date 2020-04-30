using System;
using Microsoft.Xna.Framework;

namespace Sparta.Graphics.Animations
{
    public abstract class SpartaAbstractAnimation : SpartaObject
    {
        public enum SpartaAnimationState
        {
            Stopped
            , Paused
            , Running
        }

        public enum SpartaAnimationDirection
        {
            Forward
            , Backward
        }

        public TimeSpan Duration { get; protected set; }

        public TimeSpan CurrentTime { get; internal set; }

        public SpartaAnimationState State { get; protected set; }

        protected SpartaAnimationDirection direction;
        public SpartaAnimationDirection Direction
        {
            get { return direction; }
            set
            {
                if (State == SpartaAnimationState.Stopped)
                {
                    direction = value;
                }
            }
        }

        public delegate void FinishedDelegate();

        public FinishedDelegate Finished { get; set; }

        internal FinishedDelegate InternalFinished;

        protected SpartaAbstractAnimation()
        {
            State = SpartaAnimationState.Stopped;
            Duration = TimeSpan.Zero;
            InternalFinished = OnFinished;
        }

        public virtual void Begin()
        {
            State = SpartaAnimationState.Running;
            CurrentTime = TimeSpan.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (State == SpartaAnimationState.Running)
            {
                CurrentTime += gameTime.ElapsedGameTime;
            }
        }

        public override void Dispose()
        {
            Finished = null;
            InternalFinished = null;
            base.Dispose();
        }

        public void Pause()
        {
            if (State == SpartaAnimationState.Running)
            {
                State = SpartaAnimationState.Paused;
            }
        }

        public void Resume()
        {
            if (State == SpartaAnimationState.Paused)
            {
                State = SpartaAnimationState.Running;
            }
        }

        public void Stop()
        {
            CurrentTime = TimeSpan.Zero;
            State = SpartaAnimationState.Stopped;
        }

        protected virtual void OnFinished()
        {
        }
    }
}
