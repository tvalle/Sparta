using Microsoft.Xna.Framework;

namespace Sparta.Graphics.Animations
{
    public class SpartaSequentialAnimation : SpartaAbstractAnimation
    {
        private SpartaList<SpartaAbstractAnimation> animations;
        private int currentAnimationIndex;

        public int Count { get { return animations.Count; } }

        public SpartaAbstractAnimation this[int index] { get { return animations[index]; } }

        public SpartaSequentialAnimation()
            : this(null)
        {
        }

        public SpartaSequentialAnimation(FinishedDelegate onFinishedCallback)
        {
            State = SpartaAnimationState.Stopped;
            animations = new SpartaList<SpartaAbstractAnimation>();
            currentAnimationIndex = -1;
            Finished = onFinishedCallback;
        }

        public void Add(SpartaAbstractAnimation nextAnimation)
        {
            if (nextAnimation == null || State == SpartaAnimationState.Running || State == SpartaAnimationState.Paused)
            {
                return;
            }

            SpartaAbstractAnimation lastAnimation = this.LastAnimation();
            if (lastAnimation != null)
            {
                lastAnimation.InternalFinished = () => { AnimationCompleted(nextAnimation); };
            }

            animations.Add(nextAnimation);
        }

        public void Remove(SpartaAbstractAnimation animation)
        {
            if (animation == null || State == SpartaAnimationState.Running || State == SpartaAnimationState.Paused)
            {
                return;
            }
            animations.Remove(animation);
        }

        public void Clear()
        {
            Stop();
            animations.Clear();
        }

        public override void Begin()
        {
            if (animations.Count == 0)
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
                return;
            }

            this.LastAnimation().InternalFinished = () => { AnimationCompleted(null); };

            currentAnimationIndex = 0;
            base.Begin();
            animations.Array[0].Direction = this.Direction;
            animations.Array[0].Begin();
        }

        public override void Update(GameTime gameTime)
        {
            if (State != SpartaAnimationState.Running || animations.Count == 0)
            {
                return;
            }

            SpartaAbstractAnimation currentAnimation = animations.Array[currentAnimationIndex];
            if (currentAnimation == null)
            {
                return;
            }
            currentAnimation.Update(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            
            SpartaAbstractAnimation[] animationsArray = animations.Array;
            int count = animations.Count;

            for (int i = 0; i < count; i++)
            {
                animationsArray[i].Dispose();
                animationsArray[i] = null;
            }
            animations.Dispose();
            animations = null;
            base.Dispose();
        }

        protected SpartaAbstractAnimation LastAnimation()
        {
            if (animations.Count == 0)
            {
                return null;
            }

            return animations.Array[animations.Count - 1];
        }

        protected void AnimationCompleted(SpartaAbstractAnimation nextAnimation)
        {
            int count = animations.Count;

            if (currentAnimationIndex + 1 == count)
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
            else
            {
                currentAnimationIndex++;
                nextAnimation.Direction = this.Direction;
                nextAnimation.Begin();
            }
        }
    }
}
