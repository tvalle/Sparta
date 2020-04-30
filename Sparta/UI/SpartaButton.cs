using System;
using System.Runtime.Serialization;
using Sparta.Graphics;
using Sparta.Input;
using Microsoft.Xna.Framework;
#if WINDOWS
using Microsoft.Xna.Framework.Input;
#endif
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace Sparta.UI
{
    [DataContract]
    public class SpartaButton : SpartaTexture
    {
        public Action<SpartaButton> OnClicked;
        public Action<SpartaButton> OnPressed;
        public Action<SpartaButton> OnMoved;
        public Action<SpartaButton> OnReleased;
        public Action<SpartaButton> OnTapped;

        [DataMember]
        public int DefaultFrame { get; set; }
        [DataMember]
        public int DisabledFrame { get; set; }
        [DataMember]
        public int PressedFrame { get; set; }

        [DataMember]
        public Rectangle DefaultFrameRect { get; set; }
        [DataMember]
        public Rectangle DisabledFrameRect { get; set; }
        [DataMember]
        public Rectangle PressedFrameRect { get; set; }

        private Rectangle sourceRect;
        [DataMember]
        public override Rectangle SourceRect
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }

        private SpartaButtonState state;
        [DataMember]
        public SpartaButtonState State
        {
            get { return state; }
            set
            {
                int currentFrame = 0;
                Rectangle frameRect = Rectangle.Empty;
                state = value;

                if (state == SpartaButtonState.Pressed)
                {
                    currentFrame = PressedFrame;
                    frameRect = PressedFrameRect;
                }
                else if (state == SpartaButtonState.Disabled)
                {
                    currentFrame = DisabledFrame;
                    frameRect = DisabledFrameRect;
                }
                else
                {
                    currentFrame = DefaultFrame;
                    frameRect = DefaultFrameRect;
                }

                if (frameRect.IsEmpty)
                {
                    if (Texture != null && currentFrame >= 0 && sourceRect.Width > 0)
                    {
                        int frameCountX = Texture.Width / sourceRect.Width;
                        int frameY = (int)Math.Floor((double)(currentFrame / frameCountX));
                        sourceRect.X = sourceRect.Width * (currentFrame - (frameY * frameCountX));
                        sourceRect.Y = sourceRect.Height * frameY;
                    }
                }
                else
                {
                    sourceRect = frameRect;
                }
            }
        }

        private TimeSpan pressedTime;
        private static readonly TimeSpan tapTime = TimeSpan.FromSeconds(0.2d);

        public SpartaButton()
        {
            DefaultFrame = -1;
            PressedFrame = -1;
            DisabledFrame = -1;
        }

        public SpartaButton(string imageName)
            : base(imageName)
        {
            DefaultFrame = -1;
            PressedFrame = -1;
            DisabledFrame = -1;
        }

        public SpartaButton(string imageName, Action<SpartaButton> onClicked)
            : base(imageName)
        {
            this.OnClicked = onClicked;
            DefaultFrame = -1;
            PressedFrame = -1;
            DisabledFrame = -1;
        }

        public SpartaButton(string imageName, int width, int height, Action<SpartaButton> onClicked)
            :base(imageName)
        {
            this.OnClicked = onClicked;
            
            DefaultFrame = 0;
            PressedFrame = -1;
            DisabledFrame = -1;

            SourceRect = new Rectangle(0, 0, width, height);

            state = SpartaButtonState.Normal;
        }

        public SpartaButton(string imageName, int width, int height, int defaultFrame, int pressedFrame, Action<SpartaButton> onClicked)
            : base(imageName)
        {
            this.OnClicked = onClicked;

            this.DefaultFrame = defaultFrame;
            this.PressedFrame = pressedFrame;
            this.DisabledFrame = -1;

            SourceRect = new Rectangle(0, 0, width, height);

            state = SpartaButtonState.Normal;
        }

        public SpartaButton(string imageName, int width, int height, int defaultFrame, int pressedFrame, int disabledFrame, Action<SpartaButton> onClicked)
            : base(imageName)
        {
            this.OnClicked = onClicked;

            this.DefaultFrame = defaultFrame;
            this.DisabledFrame = disabledFrame;
            this.PressedFrame = pressedFrame;

            SourceRect = new Rectangle(0, 0, width, height);

            state = SpartaButtonState.Normal;
        }

        public SpartaButton(string imageName, Rectangle defaultFrameRect, Action<SpartaButton> onClicked)
            : base(imageName)
        {
            this.OnClicked = onClicked;

            this.DefaultFrameRect = defaultFrameRect;
            this.PressedFrameRect = Rectangle.Empty;
            this.DisabledFrameRect = Rectangle.Empty;

            SourceRect = defaultFrameRect;

            state = SpartaButtonState.Normal;
        }

        public SpartaButton(string imageName, Rectangle defaultFrameRect, Rectangle pressedFrameRect, Action<SpartaButton> onClicked)
            : base(imageName)
        {
            this.OnClicked = onClicked;

            this.DefaultFrameRect = defaultFrameRect;
            this.PressedFrameRect = pressedFrameRect;
            this.DisabledFrameRect = Rectangle.Empty;

            SourceRect = defaultFrameRect;

            state = SpartaButtonState.Normal;
        }

        public SpartaButton(string imageName, Rectangle defaultFrameRect, Rectangle pressedFrameRect, Rectangle disabledFrameRect, Action<SpartaButton> onClicked)
            : base(imageName)
        {
            this.OnClicked = onClicked;

            this.DefaultFrameRect = defaultFrameRect;
            this.PressedFrameRect = pressedFrameRect;
            this.DisabledFrameRect = disabledFrameRect;

            SourceRect = defaultFrameRect;

            state = SpartaButtonState.Normal;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            State = state;
        }

        public override void Update(GameTime gameTime)
        {
            Update(gameTime, null);
        }

        public virtual void Update(GameTime gameTime, Matrix? parentTransform)
        {
            base.Update(gameTime);

            if (!Visible || State == SpartaButtonState.Disabled)
            {
                return;
            }

#if WINDOWS_PHONE
            TouchCollection touches = SpartaTouch.TouchCollection;
            if (touches.Count == 1)
            {
                TouchLocation touch = touches[0];
                Vector2 point = touch.Position;
                if (parentTransform.HasValue)
                {
                    Matrix inverse = Matrix.Invert(parentTransform.Value);
                    Vector2.Transform(ref point, ref inverse, out point);
                }
                switch (touch.State)
                {
                    case TouchLocationState.Pressed:
                        onPressed(gameTime, (int)Math.Ceiling(point.X), (int)Math.Ceiling(point.Y));
                        break;
                    case TouchLocationState.Moved:
                        onMoved((int)Math.Ceiling(point.X), (int)Math.Ceiling(point.Y));
                        break;
                    case TouchLocationState.Released:
                    case TouchLocationState.Invalid:
                        onReleased(gameTime, (int)Math.Ceiling(point.X), (int)Math.Ceiling(point.Y));
                        break;
                }
            }
#endif
#if WINDOWS
            MouseState mouseState = SpartaMouse.MouseState;
            ButtonState leftButtonState = mouseState.LeftButton;
            switch (leftButtonState)
            {
                case ButtonState.Pressed:
                    if (State == SpartaButtonState.Pressed)
                    {
                        onMoved(mouseState.X, mouseState.Y);
                    }
                    else
                    {
                        onPressed(gameTime, mouseState.X, mouseState.Y);
                    }
                    break;
                case ButtonState.Released:
                    onReleased(gameTime, mouseState.X, mouseState.Y);
                    break;
                default:
                    break;
            }
#endif
        }

        public override void Dispose()
        {
            OnClicked = null;
            OnPressed = null;
            OnMoved = null;
            OnReleased = null;
            OnTapped = null;
            base.Dispose();
        }

        private void onPressed(GameTime gameTime, int x, int y)
        {
            if (BoundingBox.Contains(x, y))
            {
                if (OnPressed != null)
                {
                    OnPressed(this);
                }
                State = SpartaButtonState.Pressed;
                pressedTime = gameTime.TotalGameTime;
            }
        }

        private void onMoved(int x, int y)
        {
            if (State == SpartaButtonState.Pressed)
            {
                if (!BoundingBox.Contains(x, y))
                {
                    if (OnReleased != null)
                    {
                        OnReleased(this);
                    }
                    State = SpartaButtonState.Normal;
                }
                else
                {
                    if (OnMoved != null)
                    {
                        OnMoved(this);
                    }
                }
            }
        }

        private void onInvalidated(GameTime gameTime, int x, int y)
        {
            onReleased(gameTime, x, y);
        }

        private void onReleased(GameTime gameTime, int x, int y)
        {
            if (State == SpartaButtonState.Pressed && BoundingBox.Contains(x, y))
            {
                if (OnReleased != null)
                {
                    OnReleased(this);
                }
                if (OnClicked != null)
                {
                    OnClicked(this);
                }
                if (OnTapped != null && gameTime.TotalGameTime - pressedTime <= tapTime)
                {
                    OnTapped(this);
                }
                State = SpartaButtonState.Normal;
            }
        }
    }
}
