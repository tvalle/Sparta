#if DEBUG
using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if WINDOWS_PHONE
using Microsoft.Phone.Info;
#endif

namespace Sparta
{
    public class SpartaDebug
    {
        public bool DebugDraw;
        public bool DrawLog;
        public SpriteFont DebugFont;
        public Vector2 FpsDrawPosition;
        public Vector2 LogDrawPosition;

        private static StringBuilder logOutput;

        public int FPS { get; private set; }
        private TimeSpan elapsedTime;
        private TimeSpan oneSecond = TimeSpan.FromSeconds(1d);
        private int drawCounter;

        public string Memory { get; private set; }
        public string PeakMemory { get; private set; }
        private int lastSafetyBand = -1;
        private bool alreadyFailedPeak = false;
        private long lastPeakMemory;
        private const long MAX_MEMORY = 90 * 1024 * 1024; // 90MB, per marketplace
        
#if WINDOWS_PHONE
        private long currentMemoryUsage
        {
            get
            {
                return (long)DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage");
            }
        }

        private long peakMemoryUsage
        {
            get
            {
                return (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
            }
        }
#endif

        public SpartaDebug()
        {
            FpsDrawPosition = new Vector2(5f, 5f);
            LogDrawPosition = new Vector2(5f, 125f);
        }

        public static void Log(object obj)
        {
            if (logOutput == null)
            {
                logOutput = new StringBuilder();
            }
            logOutput.Append(obj);
            logOutput.Append(Environment.NewLine);
        }

        public void Update(GameTime gameTime)
        {
            if (elapsedTime >= oneSecond)
            {
                elapsedTime = TimeSpan.Zero;
                FPS = drawCounter;
                drawCounter = 0;
            }
            elapsedTime += gameTime.ElapsedGameTime;
            updateCurrentMemoryUsage();
            updatePeakMemoryUsage();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawCounter++;
            if (DebugDraw && DebugFont != null)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.DrawString(DebugFont, FPS + " " + Memory + " " + PeakMemory , FpsDrawPosition, Color.White);
                if (DrawLog) 
                    spriteBatch.DrawString(DebugFont, SpartaDebug.logOutput, LogDrawPosition, Color.White);
                spriteBatch.End();
            }
        }

        private void updatePeakMemoryUsage()
        {
#if WINDOWS_PHONE
            if (alreadyFailedPeak)
                return;

            long peak = peakMemoryUsage;
            if (peak != lastPeakMemory)
            {
                lastPeakMemory = peak;
                PeakMemory = string.Format("{0:N}", peak / 1024);
            }
            if (peak >= MAX_MEMORY)
            {
                alreadyFailedPeak = true;
                if (Debugger.IsAttached)
                    Debug.Assert(false, "Peak memory condition violated");
            }
#endif
        }

        private void updateCurrentMemoryUsage()
        {
#if WINDOWS_PHONE
            long memory = currentMemoryUsage;
            Memory = string.Format("{0:N}", memory / 1024);
            int safetyBand = getSafetyBand(memory);
            if (safetyBand != lastSafetyBand)
            {
                lastSafetyBand = safetyBand;
            }
#endif
        }

        private int getSafetyBand(long mem)
        {
            double percent = (double)mem / (double)MAX_MEMORY;
            if (percent <= 0.75f)
                return 0;

            if (percent <= 0.90f)
                return 1;

            return 2;
        }
    }
}
#endif
