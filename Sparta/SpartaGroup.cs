using System.Runtime.Serialization;
using Sparta.Camera;
using Sparta.Graphics;
using Sparta.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta
{
    [DataContract]
    public class SpartaGroup : SpartaList<SpartaObject>
    {
        public SpartaObject GetObjectByTag(object tag)
        {
            if (Count == 0 || tag == null)
            {
                return null;
            }

            int statesObjectsCount = Count;
            SpartaObject[] stateObjects = Array;
            for (int i = 0; i < statesObjectsCount; i++)
            {
                if (tag.Equals(stateObjects[i].Tag))
                {
                    return stateObjects[i];
                }
            }
            return null;
        }

        public virtual void Initialize()
        {
            if (Count == 0)
            {
                return;
            }

            int statesObjectsCount = Count;
            SpartaObject[] stateObjects = Array;
            for (int i = 0; i < statesObjectsCount; i++)
            {
                stateObjects[i].Initialize();
            }
        }

        public virtual void LoadContent()
        {
            if (Count == 0)
            {
                return;
            }

            int statesObjectsCount = Count;
            SpartaObject[] stateObjects = Array;
            for (int i = 0; i < statesObjectsCount; i++)
            {
                SpartaDrawable stateObject = stateObjects[i] as SpartaDrawable;
                if (stateObject == null)
                {
                    continue;
                }
                stateObject.LoadContent();
            }
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            Update(gameTime, null);
        }

        public virtual void Update(GameTime gameTime, Matrix? transform)
        {
            if (Count == 0)
            {
                return;
            }

            int statesObjectsCount = Count;
            SpartaObject[] stateObjects = Array;
            for (int i = 0; i < statesObjectsCount; i++)
            {
                if (stateObjects[i] == null)
                {
                    return;
                }

                if (transform.HasValue)
                {
                    if (stateObjects[i] is SpartaCamera2D)
                    {
                        ((SpartaCamera2D)stateObjects[i]).Update(gameTime, transform);
                    }
                    else if (stateObjects[i] is SpartaButton)
                    {
                        ((SpartaButton)stateObjects[i]).Update(gameTime, transform);
                    }
                    else
                    {
                        stateObjects[i].Update(gameTime);
                    }
                }
                else
                {
                    stateObjects[i].Update(gameTime);
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Draw(gameTime, spriteBatch, SpartaGame.Instance.SpriteSortMode, SpartaGame.Instance.BlendState);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteSortMode parentSortMode, BlendState parentBlendState)
        {
            if (Count == 0)
            {
                return;
            }

            int statesObjectsCount = Count;
            SpartaObject[] stateObjects = Array;
            for (int i = 0; i < statesObjectsCount; i++)
            {
                SpartaDrawable stateObject = stateObjects[i] as SpartaDrawable;
                if (stateObject == null)
                {
                    continue;
                }
                else if (stateObject is SpartaCamera2D)
                {
                    ((SpartaCamera2D)stateObject).Draw(gameTime, spriteBatch, parentSortMode, parentBlendState);
                }
                else
                {
                    stateObject.Draw(gameTime, spriteBatch);
                }
            }
        }

        public void DisposeAll()
        {
            int statesObjectsCount = Count;
            SpartaObject[] stateObjects = Array;
            for (int i = 0; i < statesObjectsCount; i++)
            {
                stateObjects[i].Dispose();
                stateObjects[i] = null;
            }
            Clear();
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            DisposeAll();
            base.Dispose();
        }
    }
}
