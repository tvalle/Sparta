using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.Graphics
{
    [DataContract]
    public class SpartaSpriteMap : SpartaBaseSprite<Rectangle>
    {
        private Rectangle currentFrame;
        [DataMember]
        public override Rectangle CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        [DataMember]
        public override Rectangle SourceRect
        {
            get { return CurrentFrame; }
            set { CurrentFrame = value; }
        }

        [DataMember]
        public int FrameWidth { get; set; }

        [DataMember]
        public int FrameHeight { get; set; }

        public override Rectangle BoundingBox
        {
            get
            {
                Vector2 position = Position;
                Vector2 origin = Origin;
                Vector2 scale = Scale;
                Rectangle boundingBox = Rectangle.Empty;
                Rectangle currentFrame = CurrentFrame;
                if (FrameWidth != 0)
                {
                    origin.X = currentFrame.Width * (origin.X / FrameWidth);
                }
                if (FrameHeight != 0)
                {
                    origin.Y = currentFrame.Height * (origin.Y / FrameHeight);
                }
                boundingBox.Width = (int)Math.Ceiling(currentFrame.Width * scale.X);
                boundingBox.Height = (int)Math.Ceiling(currentFrame.Height * scale.Y);
                boundingBox.X = (int)Math.Floor(position.X);
                boundingBox.Y = (int)Math.Floor(position.Y);
                boundingBox.Offset((int)Math.Floor(-origin.X * scale.X), (int)Math.Floor(-origin.Y * scale.Y));
                return boundingBox;
            }
        }

        public SpartaSpriteMap()
        {
        }

        public SpartaSpriteMap(string imageName)
            : base(imageName)
        {
        }

        public SpartaSpriteMap(int width, int height, string imageName)
            : base(width, height, imageName)
        {
            FrameWidth = width;
            FrameHeight = height;
        }

        public void Play(string animationName, bool looping, int width, int height, Vector2 origin)
        {
            base.Play(animationName, looping);
            FrameWidth = width;
            FrameHeight = height;
            Origin = origin;
        }

        public void Play(string animationName, bool looping, SpriteEffects spriteEffect, int frameTime, int width, int height, Vector2 origin)
        {
            base.Play(animationName, looping, spriteEffect, frameTime);
            FrameWidth = width;
            FrameHeight = height;
            Origin = Origin;
        }

        public override void DrawTexture(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Texture == null || !Visible)
            {
                return;
            }
            Rectangle currentFrame = CurrentFrame;
            if (currentFrame.IsEmpty)
            {
                spriteBatch.Draw(Texture, Position, null, Color, Angle, Origin, Scale, SpriteEffect, LayerDepth);
            }
            else
            {
                Vector2 origin = Origin;
                if (FrameWidth != 0)
                {
                    origin.X = currentFrame.Width * (origin.X / FrameWidth);
                }
                if (FrameHeight != 0)
                {
                    origin.Y = currentFrame.Height * (origin.Y / FrameHeight);
                }
                spriteBatch.Draw(Texture, Position, currentFrame, Color, Angle, origin, Scale, SpriteEffect, LayerDepth);
            }
        }
    }
}
