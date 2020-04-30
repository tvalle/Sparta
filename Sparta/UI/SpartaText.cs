using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sparta;
using Sparta.Graphics;

namespace Sparta.UI
{
    [DataContract]
    public class SpartaText : SpartaDrawable
    {
        [DataMember]
        public virtual string Text { get; set; }

        [DataMember]
        public virtual string FontName { get; set; }

        public override int Width 
        { 
            get { return Font != null ? (int)Math.Ceiling(Size.X) : 0; }
        }

        public override int Height
        {
            get { return Font != null ? (int)Math.Ceiling(Size.Y) : 0; }
        }

        public virtual Vector2 Size
        {
            get 
            { 
                if (Font != null)
                {
                    int lineSpacing = Font.LineSpacing;
                    float spacing = Font.Spacing;

                    Font.LineSpacing = this.LineSpacing;
                    Font.Spacing = this.Spacing;

                    Vector2 size = Font.MeasureString(Text);

                    Font.LineSpacing = lineSpacing;
                    Font.Spacing = spacing;

                    return size;
                }
                return Vector2.Zero; 
            }
        }

        public virtual SpriteFont Font { get; set; }

        [DataMember]
        public virtual int LineSpacing { get; set; }

        [DataMember]
        public virtual float Spacing { get; set; }

        public SpartaText()
            : this(string.Empty)
        {
        }

        public SpartaText(string text)
            : this(text, string.Empty)
        {
        }

        public SpartaText(string text, string fontName)
        {
            Text = text;
            FontName = fontName;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            if (!string.IsNullOrEmpty(FontName))
            {
                Font = SpartaGame.Instance.LoadSpriteFont(FontName);
                LineSpacing = Font.LineSpacing;
                Spacing = Font.Spacing;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Font != null)
            {
                int lineSpacing = Font.LineSpacing;
                float spacing = Font.Spacing;

                Font.LineSpacing = this.LineSpacing;
                Font.Spacing = this.Spacing;

                spriteBatch.DrawString(Font, Text, Position, Color, Angle, Origin, Scale, SpriteEffect, LayerDepth);

                Font.LineSpacing = lineSpacing;
                Font.Spacing = spacing;
            }
            base.Draw(gameTime, spriteBatch);
        }
    }
}
