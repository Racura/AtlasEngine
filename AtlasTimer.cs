using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasEngine
{
    public class AtlasTimer
    {
        private class FrameTimer
        {
            public int fpsCounter;
            public float total;
            public float elpased;
            public float[] lastFramesElapsed;
            public float FPS{
                get{return 1/lastFramesElapsed.Average();}
            }

            public void SetTime(float elpased)
            {
                if (lastFramesElapsed == null)
                {
                    lastFramesElapsed = new float[64];
                    fpsCounter = 0;
                }

                lastFramesElapsed[fpsCounter % lastFramesElapsed.Length] = this.elpased = Math.Min(0.25f, elpased);

                total += this.elpased;

                while (fpsCounter < lastFramesElapsed.Length)
                {
                    lastFramesElapsed[fpsCounter] = elpased;
                    fpsCounter++;
                }

                fpsCounter++;
            }
        }

        private FrameTimer updateTimer;
        private FrameTimer drawTimer;

        public float GetElapsedUpdate()
        {
            return updateTimer.elpased;
        }
        public float GetTotalUpdate()
        {
            return updateTimer.total;
        }


        public AtlasTimer()
        {
            updateTimer = new FrameTimer();
            drawTimer = new FrameTimer();
        }

        public void Update(float gameTime)
        {
            updateTimer.SetTime(gameTime);
        }

        public void Draw(float gameTime)
        {
            drawTimer.SetTime(gameTime);
        }
    }
}
