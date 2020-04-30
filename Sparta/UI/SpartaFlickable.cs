using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sparta.Camera;
using Sparta.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sparta.UI
{
    [DataContract]
    public class SpartaFlickable : SpartaCamera2D
    {
        protected RenderTarget2D ContentsLayer;
        protected Matrix ContentsTransform;

        public enum FlickableState
        {
            Steady
            , Pressed
            , ManualScroll
            , AutoScroll
            , Stop
        }

        public enum FlickableScrollMode
        {
            Horizontally
            , Vertically
        }

        public override int Width
        {
            get { return base.Width; }
            set
            {
                if (value != base.Width)
                {
                    base.Width = value;
                    rebuildContentsLayer();
                }
            }
        }

        public override int Height
        {
            get { return base.Height; }
            set
            {
                if (value != base.Height)
                {
                    base.Height = value;
                    rebuildContentsLayer();
                }
            }
        }

        [DataMember]
        public int ContentsWidth { get; set; }

        [DataMember]
        public int ContentsHeight { get; set; }

        private Vector2 contentsOffset;
        [DataMember]
        public Vector2 ContentsOffset 
        {
            get { return contentsOffset; }
            set { contentsOffset = value; }
        }

        public float ContentsOffsetX
        {
            get { return contentsOffset.X; }
            set { contentsOffset.X = value; }
        }

        public float ContentsOffsetY
        {
            get { return contentsOffset.Y; }
            set { contentsOffset.Y = value; }
        }

        [DataMember]
        public float ScrollVelocity { get; set; }

        [DataMember]
        public bool Slowing { get; set; }

        [DataMember]
        public FlickableState State { get; set; }

        [DataMember]
        public FlickableScrollMode ScrollMode { get; set; }

        [DataMember]
        public bool LoopingMode { get; set; }

        [DataMember]
        public float Threshold { get; set; }

        [DataMember]
        public float DefaultScrollVelocity { get; set; }

        [DataMember]
        public int MaximumScrollVelocity { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public bool UserInputEnabled { get; set; }

        private Vector2 previousPoint;
        private static readonly List<float> deltas = new List<float>();

        public SpartaFlickable()
            : this(0, 0)
        {
        }

        public SpartaFlickable(int width, int height)
            : base(width, height)
        {
            ContentsWidth = width;
            ContentsHeight = height;
            ContentsOffset = Vector2.Zero;

            previousPoint = Vector2.Zero;
            LoopingMode = true;
            State = FlickableState.Steady;
            ScrollMode = FlickableScrollMode.Horizontally;
            Threshold = 10f;
            DefaultScrollVelocity = 0f;
            MaximumScrollVelocity = 100;
            BackgroundColor = Color.Transparent;
            UserInputEnabled = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            Slowing = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            if (ContentsLayer == null)
            {
                ContentsLayer = CreateContentsLayer();
            }
        }

        public override void Update(GameTime gameTime, Matrix? parentTransform)
        {
            base.Update(gameTime, parentTransform);
            Matrix.CreateTranslation(ContentsOffset.X, ContentsOffset.Y, 0f, out ContentsTransform);

            if (UserInputEnabled)
            {
                TouchCollection touches = SpartaTouch.TouchCollection;
                if (touches.Count > 0)
                {
                    TouchLocation touch = touches[0];
                    TouchLocationState touchState = touch.State;
                    Vector2 touchPosition = touch.Position;

                    if (parentTransform.HasValue)
                    {
                        Matrix inverse = Matrix.Invert(parentTransform.Value);
                        Vector2.Transform(ref touchPosition, ref inverse, out touchPosition);
                    }

                    bool boundingBoxContainsPoint = BoundingBox.Contains((int)touchPosition.X, (int)touchPosition.Y);

                    switch (touchState)
                    {
                        case TouchLocationState.Pressed:
                            if (boundingBoxContainsPoint)
                            {
                                deltas.Clear();
                                if (State == FlickableState.Steady)
                                {
                                    State = FlickableState.Pressed;
                                    previousPoint = touchPosition;
                                }
                                else if (State == FlickableState.AutoScroll)
                                {
                                    State = FlickableState.Stop;
                                    ScrollVelocity = DefaultScrollVelocity;
                                    previousPoint = touchPosition;
                                    Slowing = false;
                                }
                                SpartaTouch.Accept();
                            }
                            break;
                        case TouchLocationState.Moved:
                            if (boundingBoxContainsPoint && (State == FlickableState.Pressed || State == FlickableState.Stop))
                            {
                                ScrollVelocity = DefaultScrollVelocity;
                                Vector2 delta = touchPosition - previousPoint;
                                if (delta.X > Threshold || delta.X < -Threshold
                                    || delta.Y > Threshold || delta.Y < -Threshold)
                                {
                                    State = FlickableState.ManualScroll;
                                    previousPoint = touchPosition;
                                }
                            }
                            else if (State == FlickableState.ManualScroll)
                            {
                                Vector2 delta = touchPosition - previousPoint;
                                float value = ScrollMode == FlickableScrollMode.Horizontally ? delta.X : delta.Y;
                                deltas.Add(value);
                                ScrollVelocity = value;
                                previousPoint = touchPosition;
                            }
                            break;
                        case TouchLocationState.Released:
                        case TouchLocationState.Invalid:
                            if (State == FlickableState.Pressed || State == FlickableState.Stop)
                            {
                                State = FlickableState.Steady;
                                ScrollVelocity = DefaultScrollVelocity;
                            }
                            else if (State == FlickableState.ManualScroll)
                            {
                                ScrollVelocity = deltas.Sum() / deltas.Count;
                                previousPoint = touchPosition;
                                if (ScrollVelocity == 0f)
                                {
                                    State = FlickableState.Steady;
                                    ScrollVelocity = DefaultScrollVelocity;
                                }
                                else
                                {
                                    State = FlickableState.AutoScroll;
                                    Slowing = true;
                                }
                            }
                            break;
                    }
                }
            }

            if (Slowing)
            {
                ScrollVelocity = Deaccelerate(ScrollVelocity, 1, MaximumScrollVelocity);
                if (ScrollVelocity == 0f)
                {
                    State = FlickableState.Steady;
                    ScrollVelocity = DefaultScrollVelocity;
                    Slowing = false;
                }
            }

            if (!float.IsNaN(ScrollVelocity))
            {
                if (ScrollMode == FlickableScrollMode.Horizontally)
                {
                    contentsOffset.X += ScrollVelocity;
                }
                else
                {
                    contentsOffset.Y += ScrollVelocity;
                }
            }
            else
            {
                ScrollVelocity = DefaultScrollVelocity;
            }

            if (LoopingMode)
            {
                if (contentsOffset.X < -ContentsWidth)
                {
                    contentsOffset.X = Width + (contentsOffset.X + ContentsWidth);
                }
                else if (contentsOffset.X > Width)
                {
                    contentsOffset.X = -ContentsWidth + (contentsOffset.X - Width);
                }
                if (contentsOffset.Y < -ContentsHeight)
                {
                    contentsOffset.Y = Height + (contentsOffset.Y + ContentsHeight);
                }
                else if (contentsOffset.Y > Height)
                {
                    contentsOffset.Y = -ContentsHeight + (contentsOffset.Y - Height);
                }
            }
            else
            {
                if (contentsOffset.X < -ContentsWidth + Width)
                {
                    contentsOffset.X = -ContentsWidth + Width;
                }
                else if (contentsOffset.X > 0f)
                {
                    contentsOffset.X = 0f;
                }
                if (contentsOffset.Y < -ContentsHeight + Height)
                {
                    contentsOffset.Y = -ContentsHeight + Height;
                }
                else if (contentsOffset.Y > 0f)
                {
                    contentsOffset.Y = 0f;
                }
            }
        }

        public void OnBeforeDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (ContentsLayer == null)
            {
                return;
            }

            GraphicsDevice graphicsDevice = SpartaGame.Instance.GraphicsDevice;
            graphicsDevice.SetRenderTarget(ContentsLayer);
            graphicsDevice.Clear(BackgroundColor);

            spriteBatch.Begin(SortMode, BlendState, null, null, null, null, ContentsTransform);
            DrawContents(gameTime, spriteBatch, SortMode, BlendState);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
        }

        protected override void DrawChildren(GameTime gameTime, SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState)
        {
            spriteBatch.Draw(ContentsLayer, Vector2.Zero, Color);
        }

        protected virtual void DrawContents(GameTime gameTime, SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState)
        {
            base.DrawChildren(gameTime, spriteBatch, sortMode, blendState);
        }

        public override void Dispose()
        {
            if (ContentsLayer != null)
            {
                ContentsLayer.Dispose();
                ContentsLayer = null;
            }
            base.Dispose();
        }

        protected virtual RenderTarget2D CreateContentsLayer()
        {
            return new RenderTarget2D(SpartaGame.Instance.GraphicsDevice, Width, Height);
        }

        protected float Deaccelerate(float speed, float a, float max)
        {
            float value = MathHelper.Clamp(speed, -max, max);
            //int y = (int)MathHelper.Clamp(speed.Y, -max, max);
            value = (value == 0) ? value : (value > 0) ? Math.Max(0, value - a) : Math.Min(0, value + a);
            //y = (y == 0) ? y : (y > 0) ? Math.Max(0, y - a) : Math.Min(0, y + a);
            return value;
        }

        private void rebuildContentsLayer()
        {
            if (ContentsLayer != null)
            {
                ContentsLayer.Dispose();
                ContentsLayer = null;
                if (SpartaGame.Instance.contentLoaded && Width > 0 && Height > 0)
                {
                    ContentsLayer = CreateContentsLayer();
                }
            }
        }
    }
}
