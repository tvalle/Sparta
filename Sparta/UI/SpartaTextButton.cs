using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.UI
{
    [DataContract]
    public class SpartaTextButton : SpartaButton
    {
        [DataMember]
        public SpartaText ButtonText { get; set; }

        public override Vector2 Position
        {
            get { return ButtonText.Position; }
            set { if (ButtonText != null) ButtonText.Position = value; }
        }
        public override float X
        {
            get { return ButtonText.X; }
            set { if (ButtonText != null) ButtonText.X = value; }
        }
        public override float Y
        {
            get { return ButtonText.Y; }
            set { if (ButtonText != null) ButtonText.Y = value; }
        }
        public override float Angle
        {
            get { return ButtonText.Angle; }
            set { if (ButtonText != null) ButtonText.Angle = value; }
        }
        public override Color Color
        {
            get { return ButtonText.Color; }
            set { if (ButtonText != null) ButtonText.Color = value; }
        }
        public override float LayerDepth
        {
            get { return ButtonText.LayerDepth; }
            set { if (ButtonText != null) ButtonText.LayerDepth = value; }
        }
        public override float Opacity
        {
            get { return ButtonText.Opacity; }
            set { if (ButtonText != null) ButtonText.Opacity = value; }
        }
        public override Vector2 Origin
        {
            get { return ButtonText.Origin; }
            set { if (ButtonText != null) ButtonText.Origin = value; }
        }
        public override float OriginX
        {
            get { return ButtonText.OriginX; }
            set { if (ButtonText != null) ButtonText.OriginX = value; }
        }
        public override float OriginY
        {
            get { return ButtonText.OriginY; }
            set { if (ButtonText != null) ButtonText.OriginY = value; }
        }
        public override Vector2 Scale
        {
            get { return ButtonText.Scale; }
            set { if (ButtonText != null) ButtonText.Scale = value; }
        }
        public override float ScaleX
        {
            get { return ButtonText.ScaleX; }
            set { if (ButtonText != null) ButtonText.ScaleX = value; }
        }
        public override float ScaleY
        {
            get { return ButtonText.ScaleY; }
            set { if (ButtonText != null) ButtonText.ScaleY = value; }
        }
        public override bool Visible
        {
            get { return ButtonText.Visible; }
            set { if (ButtonText != null) ButtonText.Visible = value; }
        }
        public override SpriteEffects SpriteEffect
        {
            get { return ButtonText.SpriteEffect; }
            set { if (ButtonText != null) ButtonText.SpriteEffect = value; }
        }
        public override int Width { get { return ButtonText.Width; } }
        public override int Height { get { return ButtonText.Height; } }

        public SpartaTextButton()
            : this(string.Empty, string.Empty)
        {
        }

        public SpartaTextButton(string text, string fontName)
        {
            ButtonText = new SpartaText(text, fontName);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            ButtonText.LoadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            ButtonText.Draw(gameTime, spriteBatch);
        }

        public override void Dispose()
        {
            if (ButtonText != null)
            {
                ButtonText.Dispose();
                ButtonText = null;
            }
            base.Dispose();
        }
    }
}
