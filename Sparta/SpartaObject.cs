using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Sparta
{
    [DataContract]
    public abstract class SpartaObject : IDisposable
    {
        [DataMember]
        public object Tag { get; set; }

        public bool IsDisposed { get; protected set; }

        public virtual void Initialize()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            Tag = null;
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
