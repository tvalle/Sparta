using System;
using System.Collections.Generic;
//using FarseerPhysics.Dynamics;
using Sparta.Graphics.Transitions;
using Sparta.Input;
//using Sparta.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if WINDOWS_PHONE
using Microsoft.Phone.Shell;
#endif

namespace Sparta
{
    public class SpartaGame : Game
    {
        private Color backgroundColor = Color.CornflowerBlue;
        private Texture2D backgroundImage;
        private string backgroundImageAssetName;
        internal bool contentLoaded { get; private set; }
        private readonly SpartaList<SpartaState> states;
        private const string stateKey = "sparta_state";

        public SpriteSortMode SpriteSortMode { get; set; }
        public BlendState BlendState { get; set; }
        public int ScreenWidth { get { return Graphics.PreferredBackBufferWidth; } }
        public int ScreenHeight { get { return Graphics.PreferredBackBufferHeight; } }
        public ContentManager ContentManager { get { return Content; } }

        public Dictionary<string, Texture2D> Textures { get; protected set; }
        public Dictionary<string, SpriteFont> Fonts { get; protected set; }
        public Dictionary<string, SoundEffect> SoundEffects { get; protected set; }
        public Dictionary<string, Song> Songs { get; protected set; }

        public float DeltaTime { get; private set; }

        public Texture2D PixelTexture { get; private set; }

        private Random random;
        public Random Random
        {
            get
            {
                if (random == null)
                {
                    random = new Random();
                }
                return random;
            }
        }

        //FPS
        public enum FPSValue
        {
            FPS30, FPS60
        }
        private FPSValue desiredFps;
        public FPSValue DesiredFPS 
        {
            get { return desiredFps; }
            set
            {
                desiredFps = value;
                if (desiredFps == FPSValue.FPS30)
                {
                    TargetElapsedTime = TimeSpan.FromTicks(333333);
                }
                else if (desiredFps == FPSValue.FPS60)
                {
                    TargetElapsedTime = TimeSpan.FromTicks(166667);
                }
            }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set 
            { 
                backgroundColor = value;
                backgroundImage = null;
                BackgroundImageAssetName = string.Empty;
            }
        }

        public string BackgroundImageAssetName
        {
            get { return backgroundImageAssetName; }
            set 
            {
                backgroundImageAssetName = value;
                if (contentLoaded && !string.IsNullOrEmpty(backgroundImageAssetName))
                {
                    backgroundImage = LoadTexture(backgroundImageAssetName);
                }
            }
        }
        public SpartaState ActiveState
        {
            get
            {
                SpartaState[] spartaStates = states.Array;
                int count = states.Count;

                for (int i = 0; i < count; i++)
                {
                    if (spartaStates[i].Mode == SpartaState.SpartaStateMode.Activated)
                    {
                        return spartaStates[i];
                    }
                }

                return null;
            }
        }

        public int TotalStates { get { return states.Count; } }

        protected GraphicsDeviceManager Graphics { get; private set; }
        protected SpriteBatch SpriteBatch { get; private set; }
        
        public static SpartaGame Instance { get; private set; }
#if DEBUG
        public readonly SpartaDebug SpartaDebug = new SpartaDebug();
#endif
        public SpartaGame()
            : this(null)
        {
        }

        private SpartaGame(IGraphicsDeviceService service)
        {
            if (service == null)
            {
                Graphics = new GraphicsDeviceManager(this);
#if WINDOWS_PHONE
                Graphics.IsFullScreen = true;
                Graphics.PreferredBackBufferWidth = 480;
                Graphics.PreferredBackBufferHeight = 800;
#endif
                Graphics.PreparingDeviceSettings += graphicsPreparingDeviceSettings;
            }
            else
            {
                this.Services.AddService(typeof(IGraphicsDeviceService), service);
            }

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            DesiredFPS = FPSValue.FPS60;

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            SpriteSortMode = SpriteSortMode.Deferred;
            BlendState = BlendState.NonPremultiplied;

            Textures = new Dictionary<string, Texture2D>();
            Fonts = new Dictionary<string, SpriteFont>();
            SoundEffects = new Dictionary<string, SoundEffect>();
            Songs = new Dictionary<string, Song>();

            states = new SpartaList<SpartaState>();
            Instance = this;

            //Physics initializations
            //SpartaPhysics.MeterPerPixel = 100f;
            //SpartaPhysics.world = new World(new Vector2(0f, 20f));
            
#if WINDOWS_PHONE
            // Set up application lifecycle event handlers
            PhoneApplicationService.Current.Launching += gameLaunchingEvent;
            PhoneApplicationService.Current.Closing += gameClosingEvent;
            PhoneApplicationService.Current.Deactivated += gameDeactivatedEvent;
            PhoneApplicationService.Current.Activated += gameActivatedEvent;
#endif
        }

        public void SetState(SpartaState state)
        {
            AddState(state, SpartaState.SpartaStateMode.Loading, SpartaTransition.Default, SpartaTransition.Default);
        }

        public void SetState(SpartaState state, SpartaTransition transitionIn, SpartaTransition transitionOut)
        {
            AddState(state, SpartaState.SpartaStateMode.Loading, transitionIn, transitionOut);
        }

        protected void AddState(SpartaState state, SpartaState.SpartaStateMode mode, SpartaTransition transitionIn, SpartaTransition transitionOut)
        {
            if (state == null)
            {
                return;
            }
            
            if (states.Count > 0 && (mode == SpartaState.SpartaStateMode.Loading))
            {
                SpartaState[] spartaStates = this.states.Array;
                int count = this.states.Count;
                for (int i = 0; i < count; i++)
                {
                    if (spartaStates[i].Mode == SpartaState.SpartaStateMode.Loading)
                    {
                        spartaStates[i].Mode = SpartaState.SpartaStateMode.Deactivated;
                    }
                    else if (spartaStates[i].Mode == SpartaState.SpartaStateMode.Activated || spartaStates[i].Mode == SpartaState.SpartaStateMode.TransitionIn)
                    {
                        if (transitionOut.transitionType == SpartaTransitionType.None)
                        {
                            spartaStates[i].Mode = SpartaState.SpartaStateMode.Deactivated;
                        }
                        else
                        {
                            spartaStates[i].TransitionOut = transitionOut;
                            spartaStates[i].Mode = SpartaState.SpartaStateMode.TransitionOff;
                        }
                    }
                }
            }

            state.TransitionIn = transitionIn;
            state.Mode = mode;
            
            if (!states.Contains(state))
            {
                states.Add(state);
            }
        }

        public Texture2D LoadTexture(string texture)
        {
            if (!Textures.ContainsKey(texture))
            {
                Textures[texture] = ContentManager.Load<Texture2D>(texture);
            }
            Texture2D texture2D = Textures[texture];
            if (texture2D.IsDisposed)
            {
                throw new Exception("texture disposed");
            }
            return texture2D;
        }

        public void RemoveTexture(string texture)
        {
            if (Textures.ContainsKey(texture))
            {
                Textures[texture].Dispose();
                if (!Textures.Remove(texture))
                {
                    throw new Exception("texture not found");
                }
            }
        }

        public SoundEffect LoadSoundEffect(string sound)
        {
            if (!SoundEffects.ContainsKey(sound))
            {
                SoundEffects[sound] = ContentManager.Load<SoundEffect>(sound);
            }
            return SoundEffects[sound];
        }

        public void RemoveSoundEffect(string sound)
        {
            if (SoundEffects.ContainsKey(sound))
            {
                SoundEffects[sound].Dispose();
                SoundEffects.Remove(sound);
            }
        }

        public Song LoadSong(string song)
        {
            if (!Songs.ContainsKey(song))
            {
                Songs[song] = ContentManager.Load<Song>(song);
            }
            return Songs[song];
        }

        public void RemoveSong(string song)
        {
            if (Songs.ContainsKey(song))
            {
                Songs[song].Dispose();
                Songs.Remove(song);
            }
        }

        public SpriteFont LoadSpriteFont(string font)
        {
            if (!Fonts.ContainsKey(font))
            {
                Fonts[font] = ContentManager.Load<SpriteFont>(font);
            }
            return Fonts[font];
        }

        public void RemoveSpriteFont(string font)
        {
            if (Fonts.ContainsKey(font))
            {
                GC.SuppressFinalize(Fonts[font]);
                Fonts.Remove(font);
            }
        }

        public void SetBackgroundTexture(string backgroundImageAssetName)
        {
            this.BackgroundImageAssetName = backgroundImageAssetName;
            backgroundImage = LoadTexture(this.BackgroundImageAssetName);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            contentLoaded = true;

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            if (!String.IsNullOrEmpty(BackgroundImageAssetName))
            {
                backgroundImage = ContentManager.Load<Texture2D>(BackgroundImageAssetName);
            }

            PixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            PixelTexture.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Textures.Clear();
            Fonts.Clear();
            SoundEffects.Clear();
            Songs.Clear();

            ContentManager.Unload();
#if DEBUG
            SpartaDebug.DebugFont = null;
#endif
            backgroundImage = null;

            base.UnloadContent();

            GC.Collect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
#if DEBUG
            SpartaDebug.Update(gameTime);
#endif
#if WINDOWS_PHONE
            SpartaTouch.Update();
#endif
#if WINDOWS
            SpartaMouse.Update();
            SpartaKeyboard.Update();
#endif
            bool backButtonPressed = GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (states.Count > 0)
            {
                SpartaState toBeRemoved = null;
                SpartaState[] spartaStates = states.Array;
                int count = states.Count;

                for (int i = 0; i < count; i++)
                {
                    switch (spartaStates[i].Mode)
                    {
                        case SpartaState.SpartaStateMode.Activated:
                            if (backButtonPressed)
                            {
                                spartaStates[i].OnBackButtonPressed();
                            }
                            spartaStates[i].Update(gameTime);
                            break;
                        case SpartaState.SpartaStateMode.Loading:
                            spartaStates[i].Initialize();
                            spartaStates[i].LoadContent();
                            if (spartaStates[i].TransitionIn.transitionType == SpartaTransitionType.None
                                && spartaStates[i].TransitionOut.transitionType == SpartaTransitionType.None)
                            {
                                spartaStates[i].Mode = SpartaState.SpartaStateMode.Activated;
                            }
                            else if (spartaStates[i].TransitionIn.transitionType != SpartaTransitionType.None)
                            {
                                spartaStates[i].Mode = SpartaState.SpartaStateMode.TransitionIn;
                            }
                            else
                            {
                                spartaStates[i].Mode = SpartaState.SpartaStateMode.TransitionOff;
                            }
                            spartaStates[i].Update(gameTime);
                            break;
                        case SpartaState.SpartaStateMode.TransitionIn:
                            spartaStates[i].Update(gameTime);
                            break;
                        case SpartaState.SpartaStateMode.TransitionOff:
                            spartaStates[i].Update(gameTime);
                            break;
                        case SpartaState.SpartaStateMode.Deactivated:
                            spartaStates[i].UnloadContent();
                            toBeRemoved = spartaStates[i];
                            break;
                    }
                }

                if (toBeRemoved != null)
                {
                    states.Remove(toBeRemoved);
                    toBeRemoved.Dispose();
                    toBeRemoved = null;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            SpartaState[] spartaStates = states.Array;
            int count = states.Count;

            SpartaState activatedState = null;
            SpartaState transitionInState = null;
            SpartaState transitionOutState = null;

            for (int i = 0; i < count; i++)
            {
                if (spartaStates[i].Mode == SpartaState.SpartaStateMode.Activated)
                {
                    activatedState = spartaStates[i];
                    break;
                }
                else if (spartaStates[i].Mode == SpartaState.SpartaStateMode.TransitionIn)
                {
                    transitionInState = spartaStates[i];
                }
                else if (spartaStates[i].Mode == SpartaState.SpartaStateMode.TransitionOff)
                {
                    transitionOutState = spartaStates[i];
                }
            }

            if (activatedState != null)
            {
                activatedState.OnBeforeDraw(gameTime, SpriteBatch);
                GraphicsDevice.Clear(BackgroundColor);
                DrawBackgroundImage();
                activatedState.Draw(gameTime, SpriteBatch);
                activatedState.OnPostDraw(gameTime, SpriteBatch);
            }
            else
            {
                if (transitionOutState != null)
                {
                    transitionOutState.OnBeforeDraw(gameTime, SpriteBatch);
                }
                if (transitionInState != null)
                {
                    transitionInState.OnBeforeDraw(gameTime, SpriteBatch);
                }
                GraphicsDevice.Clear(BackgroundColor);
                DrawBackgroundImage();
                bool stopDrawing = false;
                if (transitionInState != null && transitionOutState != null)
                {
                    if (transitionInState.DrawPriority)
                    {
                        DrawState(gameTime, transitionOutState);
                        DrawState(gameTime, transitionInState);
                        stopDrawing = true;
                    }
                    else if (transitionOutState.DrawPriority)
                    {
                        DrawState(gameTime, transitionInState);
                        DrawState(gameTime, transitionOutState);
                        stopDrawing = true;
                    }
                }
                if (!stopDrawing)
                {
                    if (transitionOutState != null)
                    {
                        DrawState(gameTime, transitionOutState);
                    }
                    if (transitionInState != null)
                    {
                        DrawState(gameTime, transitionInState);
                    }
                }
            }
#if DEBUG
            SpartaDebug.Draw(SpriteBatch);
#endif
        }

        protected void DrawBackgroundImage()
        {
            if (backgroundImage != null)
            {
                SpriteBatch.Begin(SpriteSortMode, BlendState);
                SpriteBatch.Draw(backgroundImage, Vector2.Zero, Color.White);
                SpriteBatch.End();
            }
        }

        protected void DrawState(GameTime gameTime, SpartaState state)
        {
            //state.OnBeforeDraw(gameTime, SpriteBatch);
            state.Draw(gameTime, SpriteBatch);
            state.OnPostDraw(gameTime, SpriteBatch);
        }
#if DEBUG
        public void DebugDraw(bool enabled)
        {
            DebugDraw(enabled, null);
        }

        public void DebugDraw(bool enabled, SpriteFont font)
        {
            SpartaDebug.DebugDraw = enabled;
            SpartaDebug.DebugFont = font;
            SpartaDebug.DrawLog = false;
        }
#endif

        /// <summary>
        /// Virtual function to allow the game to handle the Launching event
        /// </summary>
        protected virtual void GameLaunching()
        {
        }

        /// <summary>
        /// Virtual function to allow the game to handle the Activated event
        /// </summary>
        protected virtual void GameActivated()
        {
#if WINDOWS_PHONE
            readGameObjectsFromPhoneState();
#endif
        }

        /// <summary>
        /// Virtual function to allow the game to handle the Deactivated event
        /// </summary>
        protected virtual void GameDeactivated()
        {
#if WINDOWS_PHONE
            writeGameObjectsToPhoneState();
#endif
        }

        /// <summary>
        /// Virtual function to allow the game to handle the Closing event
        /// </summary>
        protected virtual void GameClosing()
        {
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Handle the Launching event
        /// </summary>
        private void gameLaunchingEvent(object sender, LaunchingEventArgs e)
        {
            // Call the virtual function so that the game can provide its own handling code
            GameLaunching();
        }

        /// <summary>
        /// Handle the Closing event
        /// </summary>
        private void gameClosingEvent(object sender, ClosingEventArgs e)
        {
            // Call the virtual function so that the game can provide its own handling code
            GameClosing();
        }

        /// <summary>
        /// Handle the Deactivated event
        /// </summary>
        private void gameDeactivatedEvent(object sender, DeactivatedEventArgs e)
        {
            // Call the virtual function so that the game can provide its own handling code
            GameDeactivated();
        }

        /// <summary>
        /// Handle the Activated event
        /// </summary>
        private void gameActivatedEvent(object sender, ActivatedEventArgs e)
        {
            // Call the virtual function so that the game can provide its own handling code
            GameActivated();
        }
        
        private void writeGameObjectsToPhoneState()
        {
            PhoneApplicationService.Current.State.Clear();
            if (states.Count > 0)
            {
                SpartaState[] spartaStates = this.states.Array;
                int count = this.states.Count;
                for (int i = 0; i < count; i++)
                {
                    SpartaState state = spartaStates[i];
                    if (state.Mode == SpartaState.SpartaStateMode.Loading
                        || state.Mode == SpartaState.SpartaStateMode.TransitionIn
                        || state.Mode == SpartaState.SpartaStateMode.Activated)
                    {
                        state.PrepareForTombstone();
                        PhoneApplicationService.Current.State.Add(stateKey, state);
                        return;
                    }
                }
            }
        }

        private void readGameObjectsFromPhoneState()
        {
            if (states.Count == 0 
                && PhoneApplicationService.Current.State.Count > 0
                && PhoneApplicationService.Current.State.ContainsKey(stateKey))
            {
                SpartaState state = PhoneApplicationService.Current.State[stateKey] as SpartaState;
                if (state != null)
                {
                    if (state.Mode == SpartaState.SpartaStateMode.Loading || state.Mode == SpartaState.SpartaStateMode.TransitionIn)
                    {
                        state = Activator.CreateInstance(state.GetType()) as SpartaState;
                    }
                    state.Initialize();
                    state.LoadContent();
                    state.Mode = SpartaState.SpartaStateMode.Activated;
                    states.Add(state);
                }
            }
        }
#endif
        private void graphicsPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            var pp = e.GraphicsDeviceInformation.PresentationParameters;
            if (DesiredFPS == FPSValue.FPS60)
            {
                pp.PresentationInterval = PresentInterval.One;
            }
            pp.BackBufferFormat = SurfaceFormat.Color;
            //pp.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}
