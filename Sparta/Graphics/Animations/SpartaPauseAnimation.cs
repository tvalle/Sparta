using System;
using Microsoft.Xna.Framework;

namespace Sparta.Graphics.Animations
{
    public class SpartaPauseAnimation : SpartaAbstractAnimation
    {
        public SpartaPauseAnimation(TimeSpan duration)
            : this(duration, null)
        {
        }

        public SpartaPauseAnimation(TimeSpan duration, FinishedDelegate callback)
        {
            this.Duration = duration;
            this.Finished = callback;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (State == SpartaAnimationState.Running)
            {
                if (CurrentTime > Duration)
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
                }
            }
        }
    }
}
