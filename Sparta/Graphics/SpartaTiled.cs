using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.Graphics
{
    [DataContract]
    public class SpartaTiled : SpartaTexture
    {
        [DataMember]
        public int horizontalCount { get; set; }
        [DataMember]
        public int verticalCount { get; set; }

        private int _width;
        [DataMember]
        public override int Width 
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    updateCounters();
                }
            }
        }

        private int _height;
        [DataMember]
        public override int Height 
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    updateCounters();
                }
            }
        }

        public override Texture2D Texture
        {
            get
            {
                return base.Texture;
            }
            set
            {
                base.Texture = value;
                updateCounters();
            }
        }

        public SpartaTiled()
            : base()
        {
        }

        public SpartaTiled(string imageName, int width, int height)
            : base(imageName)
        {
            this.Width = width;
            this.Height = height;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Visible)
            {
                return;
            }

            int x = (int)Math.Ceiling(this.X);
            int y = (int)Math.Ceiling(this.Y);
            Rectangle destination = new Rectangle(x, y, Texture.Width, Texture.Height);
            int totalWidth = Width;
            int totalHeight = Height;

            for (int j = 0; j < verticalCount; j++)
            {
                totalWidth = Width;
                destination.Height = totalHeight >= Texture.Height ? Texture.Height : totalHeight;
                for (int i = 0; i < horizontalCount; i++)
                {
                    destination.Width = totalWidth >= Texture.Width ? Texture.Width : totalWidth;
                    destination.X = x + (i * Texture.Width);
                    destination.Y = y + (j * Texture.Height);
                    spriteBatch.Draw(Texture, destination, null, Color, Angle, Origin, SpriteEffect, LayerDepth);
                    totalWidth -= Texture.Width;
                }
                totalHeight -= Texture.Height;
            }
        }

        private void updateCounters()
        {
            if (Texture == null)
            {
                return;
            }
            horizontalCount = (int)Math.Ceiling((float)_width / (float)Texture.Width);
            verticalCount = (int)Math.Ceiling(_height / (float)Texture.Height);
        }
    }
}
