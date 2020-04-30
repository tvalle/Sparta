using System.Runtime.Serialization;
using Sparta.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.Camera
{
    [DataContract]
    public class SpartaCamera2D : SpartaDrawable
    {
        [DataMember]
        public SpartaGroup Group { get; set; }
        public SpartaObject[] Array { get { return Group.Array; } }
        public int Count { get { return Group.Count; } }

        public override float Opacity
        {
            get { return base.Opacity; }
            set
            {
                base.Opacity = value;
                UpdateOpacity(value, Array, Count);
            }
        }

        [DataMember]
        public SpriteSortMode SortMode { get; set; }

        [IgnoreDataMember]
        public BlendState BlendState { get; set; }

        [DataMember]
        public Matrix Transform { get; set; }

        protected Matrix? ParentTransform;

        public SpartaCamera2D()
            : this(0, 0)
        {
        }

        public SpartaCamera2D(int width, int height)
        {
            Group = new SpartaGroup();
            this.Width = width;
            this.Height = height;
            Transform = GetTransformation();
            ParentTransform = null;
            SortMode = SpartaGame.Instance.SpriteSortMode;
            BlendState = SpartaGame.Instance.BlendState;
        }

        public void Add(SpartaObject newObject)
        {
            Group.Add(newObject);
        }

        public void AddBefore(SpartaObject newObject, SpartaObject beforeObject)
        {
            Group.AddBefore(newObject, beforeObject);
        }

        public bool Remove(SpartaObject toBeRemoved)
        {
            return Group.Remove(toBeRemoved);
        }

        public bool Contains(SpartaObject child)
        {
            return Group.Contains(child);
        }

        public virtual void Clear()
        {
            Group.Clear();
        }

        public void Follow(SpartaDrawable item)
        {
        }

        public override void Initialize()
        {
            Group.Initialize();
        }

        public override void LoadContent()
        {
            //_renderTarget2D = new RenderTarget2D(SpartaGame.instance.graphicsDevice, renderTargetWidth, renderTargetHeight);
            Group.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Update(gameTime, null);
        }

        public virtual void Update(GameTime gameTime, Matrix? parentTransform)
        {
            Transform = GetTransformation();
            this.ParentTransform = parentTransform;
            if (parentTransform.HasValue)
            {
                Transform *= this.ParentTransform.Value;
            }
            UpdateChildren(gameTime, Transform);
        }

        protected virtual void UpdateChildren(GameTime gameTime, Matrix transform)
        {
            Group.Update(gameTime, transform);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Draw(gameTime, spriteBatch, SpartaGame.Instance.SpriteSortMode, SpartaGame.Instance.BlendState);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteSortMode parentSortMode, BlendState parentBlendState)
        {
            if (!Visible)
            {
                return;
            }

            spriteBatch.End();
            spriteBatch.Begin(SortMode, BlendState, null, null, null, null, Transform);

            DrawChildren(gameTime, spriteBatch, SortMode, BlendState);

            if (DebugDraw)
            {
                spriteBatch.Draw(SpartaGame.Instance.PixelTexture, new Rectangle(0, 0, Width, Height), null, debugColor);
                spriteBatch.Draw(SpartaGame.Instance.PixelTexture, Vector2.Zero, new Rectangle(0, 0, 3, 3), Color.Black, Angle, new Vector2(1.5f), Vector2.One, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            if (ParentTransform.HasValue)
            {
                spriteBatch.Begin(parentSortMode, parentBlendState, null, null, null, null, ParentTransform.Value);
            }
            else
            {
                spriteBatch.Begin(parentSortMode, parentBlendState);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Group.Dispose();
            Group = null;
            base.Dispose();
        }

        public virtual Matrix GetTransformation()
        {
            return Matrix.CreateTranslation(-Width / 2f, -Height / 2f, 0f)
                        * Matrix.CreateRotationZ(Angle)
                        * Matrix.CreateScale(ScaleX, ScaleY, 1f)
                        * Matrix.CreateTranslation(Width / 2f, Height / 2f, 0f)
                        * Matrix.CreateTranslation(X, Y, 0f);
        }

        protected virtual void UpdateOpacity(float opacity, SpartaObject[] children, int count)
        {
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                SpartaDrawable drawable = children[i] as SpartaDrawable;
                if (drawable == null)
                {
                    return;
                }
                drawable.Opacity = opacity;
            }
        }

        protected virtual void DrawChildren(GameTime gameTime, SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState)
        {
            Group.Draw(gameTime, spriteBatch, sortMode, blendState);
        }
    }
}
