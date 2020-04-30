using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Sparta.Graphics
{
    [DataContract]
    public class SpartaSprite : SpartaBaseSprite<int>
    {
        private Rectangle sourceRect;
        private int currentFrame;

        [DataMember]
        public override Rectangle SourceRect
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }

        [DataMember]
        public override int CurrentFrame
        {
            get { return currentFrame; }
            set
            {
                currentFrame = value;

                if (this.Texture != null)
                {
                    if (this.SourceRect.Width > 0)
                    {
                        var frameCountX = this.Texture.Width / this.SourceRect.Width;
                        var frameY = (int)Math.Floor((double)(this.currentFrame / frameCountX));
                        sourceRect.X = this.SourceRect.Width * (this.currentFrame - (frameY * frameCountX));
                        sourceRect.Y = this.SourceRect.Height * frameY;
                    }
                    else
                    {
                        sourceRect.X = currentFrame * SourceRect.Width;
                    }
                }
            }
        }

        public SpartaSprite()
            : base(0, 0, string.Empty)
        {
        }

        public SpartaSprite(int width, int height, string imageName)
            : base(width, height, imageName)
        {
        }
    }
}
