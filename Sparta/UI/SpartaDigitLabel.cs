using System.Runtime.Serialization;
using Sparta.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.UI
{
    [DataContract]
    public abstract class SpartaDigitLabel : SpartaTexture
    {
        [DataMember]
        public string Value { get; set; }
        public abstract Rectangle[] Sources { get; }

        public SpartaDigitLabel()
            : base()
        {
        }

        public SpartaDigitLabel(string imageName)
            : base(imageName)
        {
        }

        public virtual int GetCharIndex(char item)
        {
            return (int)char.GetNumericValue(item);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            string value = Value;
            if (!Visible || string.IsNullOrEmpty(value))
            {
                return;
            }

            int length = value.Length;
            Rectangle[] sources = Sources;
            Texture2D texture = Texture;
            Vector2 offset = Position;
            Color color = Color;
            Vector2 origin = Origin;
            Vector2 scale = Scale;
            SpriteEffects spriteEffect = SpriteEffect;
            float layerDepth = LayerDepth;
            Rectangle source;
            for (int i = 0; i < length; i++)
            {
                source = sources[GetCharIndex(value[i])];
                DrawChar(spriteBatch, texture, offset, source, color, 0f, origin, scale, spriteEffect, layerDepth);
                offset.X += source.Width * scale.X;
            }
        }

        public virtual void DrawChar(SpriteBatch spriteBatch,
                                     Texture2D texture,
                                     Vector2 position,
                                     Rectangle source,
                                     Color color,
                                     float angle,
                                     Vector2 origin,
                                     Vector2 scale,
                                     SpriteEffects spriteEffect,
                                     float layerDepth)
        {
            spriteBatch.Draw(texture, position, source, color, angle, origin, scale, spriteEffect, layerDepth);
        }

        public override int Width
        {
            get
            {
                string value = Value;
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }

                int length = value.Length;
                int width = 0;
                Rectangle[] sources = Sources;
                for (int i = 0; i < length; i++)
                {
                    width += sources[GetCharIndex(value[i])].Width;
                }
                return width;
            }
        }

        public override int Height { get { return Sources[0].Height; } }
    }
}
