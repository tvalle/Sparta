using System.Runtime.Serialization;
using Sparta.Graphics;
using Sparta.Graphics.Animations;
using Sparta.Graphics.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta
{
    [DataContract]
    public abstract class SpartaState : SpartaGroup
    {
        public enum SpartaStateMode
        {
            Deactivated, 
            TransitionOff, 
            TransitionIn, 
            Activated, 
            Loading
        }

        private SpartaStateMode mode;
        [DataMember]
        public SpartaStateMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    SpartaStateMode oldMode = mode;
                    mode = value;
                    OnModeChanged(oldMode, mode);
                }
            }
        }

        [DataMember]
        public SpriteSortMode SpriteSortMode { get; set; }

        [IgnoreDataMember]
        public BlendState BlendState { get; set; }

        internal SpartaTransition TransitionIn = SpartaTransition.Default;
        internal SpartaTransition TransitionOut = SpartaTransition.Default;
        internal SpartaAbstractAnimation TransitionAnimation;
        internal SpartaDrawable BackBuffer;
        internal bool DrawPriority;
        protected Matrix Transform = Matrix.Identity;

        public SpartaState()
        {
            SpriteSortMode = SpartaGame.Instance.SpriteSortMode;
            BlendState = SpartaGame.Instance.BlendState;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime, Transform);

            if (Mode == SpartaStateMode.TransitionIn)
            {
                if (TransitionIn.transitionType != SpartaTransitionType.None)
                {
                    TransitionAnimation.Update(gameTime);
                }
                UpdateTransform();
            }
            else if (Mode == SpartaStateMode.TransitionOff)
            {
                if (TransitionOut.transitionType != SpartaTransitionType.None)
                {
                    TransitionAnimation.Update(gameTime);
                }
                UpdateTransform();
            }
        }

        public virtual void OnBeforeDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode, BlendState, null, null, null, null, Transform);
            Draw(gameTime, spriteBatch, SpriteSortMode, BlendState);
            spriteBatch.End();
        }

        public virtual void OnPostDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public virtual void OnBackButtonPressed()
        {
        }

        public virtual void PrepareForTombstone()
        {
        }

        public override void Dispose()
        {
            if (TransitionAnimation != null)
            {
                TransitionAnimation.Dispose();
                TransitionAnimation = null;
            }
            if (BackBuffer != null)
            {
                BackBuffer.Dispose();
                BackBuffer = null;
            }
            base.Dispose();
        }

        protected virtual void OnModeChanged(SpartaStateMode oldMode, SpartaStateMode newMode)
        {
            if (newMode == SpartaStateMode.TransitionIn)
            {
                if (TransitionIn.transitionType != SpartaTransitionType.None)
                {
                    if (BackBuffer == null)
                    {
                        BackBuffer = CreateBackBuffer();
                    }
                }
                SpartaTransitionAnimation.HandleTransition(this, animationEnded, TransitionIn);
            }
            else if (newMode == SpartaStateMode.TransitionOff)
            {
                if (TransitionOut.transitionType != SpartaTransitionType.None)
                {
                    if (BackBuffer == null)
                    {
                        BackBuffer = CreateBackBuffer();
                    }
                }
                SpartaTransitionAnimation.HandleTransition(this, animationEnded, TransitionOut);
            }
            else
            {
                if (newMode == SpartaStateMode.Activated)
                {
                    TransitionIn.transitionType = SpartaTransitionType.None;
                    TransitionOut.transitionType = SpartaTransitionType.None;
                    DrawPriority = false;
                }
                Transform = Matrix.Identity;
            }
        }

        protected virtual SpartaDrawable CreateBackBuffer()
        {
            return new SpartaDrawable();
        }

        protected virtual void UpdateTransform()
        {
            Vector2 position = BackBuffer.Position;
            int width = SpartaGame.Instance.ScreenWidth;
            int height = SpartaGame.Instance.ScreenHeight;
            Vector2 scale = BackBuffer.Scale;
            float angle = BackBuffer.Angle;

            Transform = Matrix.CreateTranslation(-width / 2f, -height / 2f, 0f)
                        * Matrix.CreateRotationZ(angle)
                        * Matrix.CreateScale(scale.X, scale.Y, 1f)
                        * Matrix.CreateTranslation(width / 2f, height / 2f, 0f)
                        * Matrix.CreateTranslation(position.X, position.Y, 0f);
        }

        private void animationEnded()
        {
            if (Mode == SpartaStateMode.TransitionIn)
            {
                Mode = SpartaStateMode.Activated;
            }
            else if (Mode == SpartaStateMode.TransitionOff)
            {
                Mode = SpartaStateMode.Deactivated;
            }
        }
    }
}
