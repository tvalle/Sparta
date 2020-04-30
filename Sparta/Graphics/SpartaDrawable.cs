using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.Graphics
{
    [DataContract]
    public class SpartaDrawable : SpartaObject
    {
        #region Vars
        private Vector2 position = Vector2.Zero;
        private Vector2 origin = Vector2.Zero;
        private Vector2 scale = Vector2.One;
        private Color color = Color.White;
        private bool visible = true;

        protected Color debugColor = Color.White;
        private bool debugDraw = false;
        #endregion

        #region Properties
        public virtual float X
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public virtual float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        [DataMember]
        public virtual Vector2 Position
        {
            get { return position; }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        [DataMember]
        public virtual int Width { get; set; }
        [DataMember]
        public virtual int Height { get; set; }

        public virtual float OriginX
        {
            get { return origin.X; }
            set { origin.X = value; }
        }
        public virtual float OriginY
        {
            get { return origin.Y; }
            set { origin.Y = value; }
        }
        [DataMember]
        public virtual Vector2 Origin
        {
            get { return origin; }
            set
            {
                OriginX = value.X;
                OriginY = value.Y;
            }
        }

        public virtual float ScaleX
        {
            get { return scale.X; }
            set { scale.X = value; }
        }
        public virtual float ScaleY
        {
            get { return scale.Y; }
            set { scale.Y = value; }
        }
        [DataMember]
        public virtual Vector2 Scale
        {
            get { return scale; }
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
            }
        }

        [DataMember]
        public virtual float Angle { get; set; }

        [DataMember]
        public virtual Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public virtual float Opacity
        {
            get
            {
                return (float)Convert.ToDouble(Color.A, CultureInfo.InvariantCulture) / 255f;
            }
            set
            {
                Color newColor = this.Color;
                newColor.A = Convert.ToByte(255f * MathHelper.Clamp(value, 0f, 1f), CultureInfo.InvariantCulture);
                this.Color = newColor;
            }
        }

        [DataMember]
        public virtual bool Visible 
        {
            get { return visible; }
            set { visible = value; }
        }

        [DataMember]
        public virtual SpriteEffects SpriteEffect { get; set; }

        [DataMember]
        public virtual float LayerDepth { get; set; }

        public virtual Rectangle BoundingBox
        {
            get
            {
                Rectangle boundingBox = Rectangle.Empty;
                boundingBox.X = (int)X;
                boundingBox.Y = (int)Y;
                boundingBox.Width = (int)Math.Ceiling(Width * ScaleX);
                boundingBox.Height = (int)Math.Ceiling(Height * ScaleY);
                boundingBox.Offset((int)Math.Ceiling(-OriginX * ScaleX), (int)Math.Ceiling(-OriginY * ScaleY));
                return boundingBox;
            }
        }
        
        public bool DebugDraw 
        {
            get { return debugDraw; }
            set
            {
                debugDraw = value;
                if (debugDraw)
                {
                    Random random = SpartaGame.Instance.Random;
                    debugColor = new Color(random.Next(0, 127), random.Next(0, 127), random.Next(0, 127), 127);
                }
            }
        }
        #endregion

        #region Methods
        public SpartaDrawable()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawDebug(gameTime, spriteBatch);
        }

        public virtual void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (DebugDraw)
            {
                Texture2D debugPixel = SpartaGame.Instance.PixelTexture;
                if (debugPixel != null)
                {
                    Rectangle boundingBox = BoundingBox;
                    Vector2 position = Position;
                    float angle = Angle;
                    float layerDepth = LayerDepth;
                    Vector2 elementOrigin = position;
                    Vector2 initialBBPosition = new Vector2(boundingBox.X, boundingBox.Y);
                    Matrix myRotationMatrix = Matrix.CreateRotationZ(angle);

                    // Rotate relative to origin.
                    Vector2 rotatedVector = Vector2.Transform(initialBBPosition - elementOrigin, myRotationMatrix);

                    // Add origin to get final location.
                    initialBBPosition = rotatedVector + elementOrigin;

                    spriteBatch.Draw(debugPixel, initialBBPosition, new Rectangle(0, 0, boundingBox.Width, boundingBox.Height), debugColor, angle, Vector2.Zero, 1f, SpriteEffects.None, layerDepth + 0.1f);

                    // Origin point
                    spriteBatch.Draw(debugPixel, position, new Rectangle(0, 0, 3, 3), Color.Black, angle, new Vector2(1.5f), 1f, SpriteEffects.None, layerDepth + 0.1f);
                }
            }
        }

        public virtual bool CollidesWithDrawable(SpartaDrawable drawable)
        {
            if (drawable == null || this == drawable)
            {
                return false;
            }
            return drawable.BoundingBox.Intersects(this.BoundingBox);
        }

        /// <summary>
        /// Rotate a point from a given location and adjust using the Origin we
        /// are rotating around
        /// </summary>
        /// <param name="thePoint"></param>
        /// <param name="theOrigin"></param>
        /// <param name="theRotation"></param>
        /// <returns></returns>
        public static Vector2 RotatePoint(Vector2 thePoint, Vector2 theOrigin, float theRotation)
        {
            Vector2 aTranslatedPoint = Vector2.Zero;
            aTranslatedPoint.X = (float)(Math.Cos(theRotation) * (thePoint.X - theOrigin.X) - Math.Sin(theRotation) * (thePoint.Y - theOrigin.Y) + theOrigin.X);
            aTranslatedPoint.Y = (float)(Math.Sin(theRotation) * (thePoint.X - theOrigin.X) + Math.Cos(theRotation) * (thePoint.Y - theOrigin.Y) + theOrigin.Y);
            return aTranslatedPoint;
        }

        public Vector2 UpperLeftCorner()
        {
            return UpperLeftCorner(BoundingBox, Position, Angle);
        }

        public static Vector2 UpperLeftCorner(Rectangle boundingBox, Vector2 position, float angle)
        {
            Vector2 aUpperLeft = Vector2.Zero;
            aUpperLeft.X = boundingBox.Left;
            aUpperLeft.Y = boundingBox.Top;
            aUpperLeft = RotatePoint(aUpperLeft, position, -angle);
            return aUpperLeft;
        }

        public Vector2 UpperRightCorner()
        {
            return UpperRightCorner(BoundingBox, Position, Angle);
        }

        public static Vector2 UpperRightCorner(Rectangle boundingBox, Vector2 position, float angle)
        {
            Vector2 aUpperRight = Vector2.Zero;
            aUpperRight.X = boundingBox.Right;
            aUpperRight.Y = boundingBox.Top;
            aUpperRight = RotatePoint(aUpperRight, position, -angle);
            return aUpperRight;
        }

        public Vector2 LowerLeftCorner()
        {
            return LowerLeftCorner(BoundingBox, Position, Angle);
        }

        public static Vector2 LowerLeftCorner(Rectangle boundingBox, Vector2 position, float angle)
        {
            Vector2 aLowerLeft = Vector2.Zero;
            aLowerLeft.X = boundingBox.Left;
            aLowerLeft.Y = boundingBox.Bottom;
            aLowerLeft = RotatePoint(aLowerLeft, position, -angle);
            return aLowerLeft;
        }

        public Vector2 LowerRightCorner()
        {
            return LowerRightCorner(BoundingBox, Position, Angle);
        }

        public static Vector2 LowerRightCorner(Rectangle boundingBox, Vector2 position, float angle)
        {
            Vector2 aLowerRight = Vector2.Zero;
            aLowerRight.X = boundingBox.Right;
            aLowerRight.Y = boundingBox.Bottom;
            aLowerRight = RotatePoint(aLowerRight, position, -angle);
            return aLowerRight;
        }
        #endregion
    }
}
