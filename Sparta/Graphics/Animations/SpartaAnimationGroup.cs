using Microsoft.Xna.Framework;

namespace Sparta.Graphics.Animations
{
    public class SpartaAnimationGroup : SpartaAbstractAnimation
    {
        private SpartaList<SpartaAbstractAnimation> animationsList;
        private int activeAnimationsCount;

        public int Count { get { return animationsList.Count; } }

        public SpartaAbstractAnimation this[int index] { get { return animationsList[index]; } }

        public SpartaAnimationGroup()
            : this(null)
        {
        }

        public SpartaAnimationGroup(FinishedDelegate callback)
        {
            animationsList = new SpartaList<SpartaAbstractAnimation>();
            activeAnimationsCount = 0;
            Finished = callback;
        }

        public override void Begin()
        {
            base.Begin();

            int count = animationsList.Count;
            SpartaAbstractAnimation[] animations = animationsList.Array;

            activeAnimationsCount = 0;
            for (int i = 0; i < count; i++)
            {
                activeAnimationsCount++;
                animations[i].InternalFinished = completedAnimation;
                animations[i].Direction = Direction;
                animations[i].Begin();
            }
        }

        public void Add(SpartaAbstractAnimation animation)
        {
            animationsList.Add(animation);
        }

        public override void Update(GameTime gameTime)
        {
            if (State != SpartaAnimationState.Running || animationsList.Count == 0)
            {
                return;
            }

            int count = animationsList.Count;
            SpartaAbstractAnimation[] animations = animationsList.Array;
            for (int i = 0; i < count; i++)
            {
                animations[i].Update(gameTime);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            SpartaAbstractAnimation[] animations = animationsList.Array;
            int count = animationsList.Count;
            for (int i = 0; i < count; i++)
            {
                animations[i].Dispose();
                animations[i] = null;
            }
            animationsList.Dispose();
            animationsList = null;
            base.Dispose();
        }

        private void completedAnimation()
        {
            activeAnimationsCount--;
            if (activeAnimationsCount == 0)
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
