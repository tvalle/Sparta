using Sparta.Camera;
using Sparta.Graphics.Animations;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sparta.Graphics.Transitions
{
    internal static class SpartaTransitionAnimation
    {
        internal static void HandleTransition(SpartaState state, SpartaAbstractAnimation.FinishedDelegate callback, SpartaTransition transition)
        {
            if (transition.transitionType == SpartaTransitionType.None)
            {
                callback();
                return;
            }

            if (state.TransitionAnimation != null)
            {
                state.TransitionAnimation.Dispose();
                state.TransitionAnimation = null;
            }

            SpartaGame spartaGame = SpartaGame.Instance;

            //TODO: Implementar as outras transicoes
            switch (transition.transitionType)
            {
                case SpartaTransitionType.Waiting:
                    state.TransitionAnimation = new SpartaPauseAnimation(transition.transitionTime, callback);
                    break;
                case SpartaTransitionType.SlideLeftIn:
                    state.TransitionAnimation = new SpartaPropertyAnimation(state.BackBuffer, "X", (float)-spartaGame.ScreenWidth, 0f, transition.transitionTime, transition.transitionEasing, callback);
                    break;
                case SpartaTransitionType.SlideLeftOut:
                    state.TransitionAnimation = new SpartaPropertyAnimation(state.BackBuffer, "X", 0f, (float)-spartaGame.ScreenWidth, transition.transitionTime, transition.transitionEasing, callback);
                    break;

                case SpartaTransitionType.SlideRightIn:
                    state.TransitionAnimation = new SpartaPropertyAnimation(state.BackBuffer, "X", (float)spartaGame.ScreenWidth, 0f, transition.transitionTime, transition.transitionEasing, callback);
                    break;

                case SpartaTransitionType.SlideRightOut:
                    state.TransitionAnimation = new SpartaPropertyAnimation(state.BackBuffer, "X", 0f, (float)spartaGame.ScreenWidth, transition.transitionTime, transition.transitionEasing, callback);
                    break;

                case SpartaTransitionType.SlideUpIn:
                    state.TransitionAnimation = new SpartaPropertyAnimation(state.BackBuffer, "Y", -(float)spartaGame.ScreenHeight, 0f, transition.transitionTime, transition.transitionEasing, callback);
                    break;

                case SpartaTransitionType.SlideUpOut:
                    state.TransitionAnimation = new SpartaPropertyAnimation(state.BackBuffer, "Y", 0f, -(float)spartaGame.ScreenHeight, transition.transitionTime, transition.transitionEasing, callback);
                    state.DrawPriority = true;
                    break;

                //case SpartaTransitionType.AlphaIn:
                    //for (int i = 0; i < count; i++)
                    //{
                    //    if (state.Array[i] is SpartaCamera2D)
                    //    {
                    //        SpartaCamera2D auxCamera = state.Array[i] as SpartaCamera2D;
                    //        state.TransitionAnimations.Add(new SpartaPropertyAnimation(auxCamera, "Opacity", 0.0f, 1.0f, transition.transitionTime, transition.transitionEasing, null));
                    //    }
                    //    else if (state.Array[i] is SpartaDrawable)
                    //    {
                    //        SpartaDrawable auxDrawable = state.Array[i] as SpartaDrawable;
                    //        state.TransitionAnimations.Add(new SpartaPropertyAnimation(auxDrawable, "Opacity", 0.0f, 1.0f, transition.transitionTime, transition.transitionEasing, null));
                    //    }
                    //}
                    //break;

                //case SpartaTransitionType.AlphaOut:
                    //for (int i = 0; i < count; i++)
                    //{
                    //    if (state.Array[i] is SpartaCamera2D)
                    //    {
                    //        SpartaCamera2D auxCamera = state.Array[i] as SpartaCamera2D;
                    //        state.TransitionAnimations.Add(new SpartaPropertyAnimation(auxCamera, "Opacity", 1.0f, 0.0f, transition.transitionTime, transition.transitionEasing, null));
                    //    }
                    //    else if (state.Array[i] is SpartaDrawable)
                    //    {
                    //        SpartaDrawable auxDrawable = state.Array[i] as SpartaDrawable;
                    //        state.TransitionAnimations.Add(new SpartaPropertyAnimation(auxDrawable, "Opacity", 1.0f, 0.0f, transition.transitionTime, transition.transitionEasing, null));
                    //    }
                    //}
                    //break;
            }
            state.TransitionAnimation.Begin();
        }
    }
}
