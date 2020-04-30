using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.Graphics
{
    [DataContract]
    public class SpartaTexture : SpartaDrawable
    {
        public virtual Texture2D Texture { get; set; }

        public override int Width
        {
            get { return SourceRect.Width > 0 ? SourceRect.Width : (Texture == null ? 0 : Texture.Width); }
        }

        public override int Height
        {
            get { return SourceRect.Height > 0 ? SourceRect.Height : (Texture == null ? 0 : Texture.Height); }
        }

        [DataMember]
        public virtual string TextureAssetName { get; set; }

        [DataMember]
        public virtual Rectangle SourceRect { get; set; }

        public SpartaTexture()
            : this(string.Empty)
        {
        }

        public SpartaTexture(string imageName)
        {
            TextureAssetName = imageName;
        }

        public override void LoadContent()
        {
            if (string.IsNullOrEmpty(TextureAssetName))
            {
                return;                
            }

            Texture = SpartaGame.Instance.LoadTexture(TextureAssetName);            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //if (Texture == null || !Visible)
            //{
            //    DrawDebug(gameTime, spriteBatch);
            //    return;
            //}
            DrawTexture(gameTime, spriteBatch);
            DrawDebug(gameTime, spriteBatch);
        }

        public virtual void DrawTexture(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Texture != null && Visible)
            {
                if (SourceRect.IsEmpty)
                {
                    spriteBatch.Draw(Texture, Position, null, Color, Angle, Origin, Scale, SpriteEffect, LayerDepth);
                }
                else
                {
                    spriteBatch.Draw(Texture, Position, SourceRect, Color, Angle, Origin, Scale, SpriteEffect, LayerDepth);
                }
            }
        }

        public override void Dispose()
        {
            Texture = null;
            base.Dispose();
        }
    }
}
