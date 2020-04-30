using System;
using System.Linq;
using System.Runtime.Serialization;
using Sparta.Camera;
using Sparta.Graphics;
using Sparta.UI;
using Microsoft.Xna.Framework;

namespace Sparta
{
    [DataContract]
    [KnownType(typeof(SpartaObject))]
    [KnownType(typeof(SpartaCamera2D))]
    [KnownType(typeof(SpartaFlickable))]
    [KnownType(typeof(SpartaDrawable))]
    [KnownType(typeof(SpartaTexture))]
    [KnownType(typeof(SpartaSprite))]
    [KnownType(typeof(SpartaTiled))]
    [KnownType(typeof(SpartaButton))]
    [KnownType(typeof(SpartaText))]
    [KnownType(typeof(SpartaTextButton))]
    public class SpartaList<T> : IDisposable
    {
        private T[] objectsArray;
        [DataMember]
        public T[] Array 
        { 
            get { return objectsArray; }
            set
            {
                if (value != null)
                {
                    if (objectsArray != null)
                    {
                        GC.SuppressFinalize(objectsArray);
                    }
                    objectsArray = value.Where(element => element != null).ToArray();
                    objectsCount = objectsArray.Length;
                }
                else
                {
                    if (objectsArray == null)
                    {
                        objectsCount = 0;
                        objectsArray = createDefaultArray();
                    }
                    else
                    {
                        Clear();
                    }
                }
            }
        }

        private int objectsCount;
        public int Count { get { return objectsCount; } }

        public bool IsDisposed { get; protected set; }

        public SpartaList()
        {
            objectsCount = 0;
            objectsArray = createDefaultArray();
        }

        public void Add(T item)
        {
            if (objectsCount >= objectsArray.Length)
            {
                objectsCount++;
                resize();
                objectsArray[objectsCount - 1] = item;
            }
            else
            {
                objectsArray[objectsCount++] = item;
            }
        }

        public void AddBefore(T item, T before)
        {
            int index = System.Array.IndexOf<T>(objectsArray, before);
            if (index != -1)
            {
                Add(item);
                if (index == objectsCount - 1)
                {
                    objectsArray[index] = item;
                    objectsArray[index + 1] = before;
                }
                else
                {
                    unshift(index);
                    objectsArray[index] = item;
                }
            }
        }

        public bool Remove(T item)
        {
            if (objectsCount > 0)
            {
                int index = System.Array.IndexOf<T>(objectsArray, item);
                if (index != -1)
                {
                    shift(index);
                    objectsArray[objectsCount - 1] = default(T);
                    objectsCount--;
                    return true;
                }
            }
            return false;
        }

        public bool Contains(T item)
        {
            return System.Array.IndexOf<T>(objectsArray, item) != -1;
        }

        public void Clear()
        {
            objectsCount = 0;
            System.Array.Clear(objectsArray, 0, objectsArray.Length);
        }

        public T this[int i]
        {
            get { return objectsArray[i]; }
            set { objectsArray[i] = value; }
        }

        public override string ToString()
        {
            string toString = string.Empty;
            string comma = string.Empty;
            for (int i = 0; i < objectsCount; i++)
            {
                toString += comma + objectsArray[i];
                if (string.IsNullOrEmpty(comma))
                {
                    comma = ", ";
                }
            }
            return "[" + toString + "]";
        }

        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            if (objectsArray != null)
            {
                GC.SuppressFinalize(objectsArray);
            }
            objectsArray = null;
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        private T[] createDefaultArray()
        {
            return new T[(int)MathHelper.Max(20, objectsCount * 1.2f)];
        }

        private void resize()
        {
            System.Array.Resize<T>(ref objectsArray, (int)MathHelper.Max(20, objectsCount * 1.2f));
        }

        private void shift(int startIndex)
        {
            int i,
                length = objectsCount - 1;
            for (i = startIndex; i < length; i++)
            {
                objectsArray[i] = objectsArray[i + 1];
            }
        }

        private void unshift(int startIndex)
        {
            int i,
                length = objectsCount - 1;
            for (i = length; i > startIndex ; i--)
            {
                objectsArray[i] = objectsArray[i - 1];
            }
        }
    }
}
