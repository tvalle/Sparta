using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sparta.Graphics
{
    [DataContract]
    public abstract class SpartaBaseSprite<T> : SpartaTexture
    {
        [DataMember]
        public int AnimationIndex { get; set; }

        [DataMember]
        public virtual T CurrentFrame { get; set; }

        [DataMember]
        public Dictionary<string, T[]> Animations { get; set; }

        [DataMember]
        public string CurrentAnimation { get; set; }

        [DataMember]
        public bool Looping { get; set; }

        [DataMember]
        public int FrameTime { get; set; }

        [DataMember]
        public int ElapsedTime { get; set; }

        [DataMember]
        public bool IsPlaying { get; set; }

        public Action<SpartaBaseSprite<T>> OnAnimationFinished;

        public SpartaBaseSprite()
            : this(0, 0, string.Empty)
        {
        }

        public SpartaBaseSprite(string imageName)
            : this(0, 0, imageName)
        {
        }

        public SpartaBaseSprite(int width, int height, string imageName)
        {
            AnimationIndex = 0;
            CurrentFrame = default(T);
            Looping = true;
            FrameTime = 50;
            ElapsedTime = 0;
            Animations = new Dictionary<string, T[]>();

            SourceRect = new Rectangle(0, 0, width, height);
            TextureAssetName = imageName;
        }

        public void AddAnimation(string animationName, T[] animationArray)
        {
            if (!Animations.ContainsKey(animationName))
            {
                Animations.Add(animationName, animationArray);
            }
        }

        public void Play(string animationName, bool looping)
        {
            Play(animationName, looping, SpriteEffects.None, 50);
        }

        public void Play(string animationName, bool looping, SpriteEffects spriteEffect)
        {
            Play(animationName, looping, spriteEffect, 50);
        }

        public void Play(string animationName, bool looping, SpriteEffects spriteEffect, int frameTime)
        {
            if (Animations.ContainsKey(animationName))
            {
                IsPlaying = true;
                CurrentAnimation = animationName;
                AnimationIndex = 0;
                Looping = looping;
                FrameTime = frameTime;
                SpriteEffect = spriteEffect;
                ElapsedTime = 0;
                CurrentFrame = Animations[CurrentAnimation][0];
            }
        }

        public void Stop()
        {
            IsPlaying = false;
            AnimationIndex = 0;
            ElapsedTime = 0;
        }

        public virtual void Reset()
        {
            AnimationIndex = 0;
            ElapsedTime = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsPlaying)
            {
                ElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ElapsedTime >= FrameTime && Animations.Count > 0)
                {
                    T[] animation = Animations[CurrentAnimation];
                    int length = animation.Length;
                    if (AnimationIndex == length - 1)
                    {
                        AnimationFinished();
                    }
                    if (Looping)
                    {
                        AnimationIndex = (AnimationIndex + 1) % length;
                    }
                    else
                    {
                        AnimationIndex++;
                        if (AnimationIndex >= length)
                        {
                            AnimationIndex = length - 1;
                            IsPlaying = false;
                        }
                    }
                    CurrentFrame = animation[AnimationIndex];
                    ElapsedTime = 0;
                }
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            Animations.Clear();
            GC.SuppressFinalize(Animations);
            Animations = null;
            OnAnimationFinished = null;
            base.Dispose();
        }

        protected virtual void AnimationFinished()
        {
            if (OnAnimationFinished != null)
            {
                OnAnimationFinished(this);
            }
        }
    }
}
