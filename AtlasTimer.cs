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

            public float scale;

            public float total;
            public float elapsed;

            public float trueTotal;
            public float trueElapsed;

            public float[] lastFramesElapsed;
            public float FPS{
                get{return 1/lastFramesElapsed.Average();}
            }

            public void SetTime(float elapsed)
            {
                if (lastFramesElapsed == null)
                {
                    lastFramesElapsed = new float[64];
                    fpsCounter = 0;
                }

                lastFramesElapsed[fpsCounter % lastFramesElapsed.Length] = this.trueElapsed = Math.Min(0.25f, elapsed);

                this.elapsed = trueElapsed * scale;

                trueTotal += this.trueElapsed;

                total += this.elapsed;

                while (fpsCounter < lastFramesElapsed.Length)
                {
                    lastFramesElapsed[fpsCounter] = elapsed;
                    fpsCounter++;
                }

                fpsCounter++;
            }
        }

        private FrameTimer updateTimer;
        private FrameTimer drawTimer;

        public float ElapsedUpdate { get { return updateTimer.elapsed; } }
        public float TotalUpdate { get { return updateTimer.total; } }
        public float TrueElapsedUpdate { get { return updateTimer.trueTotal; } }
        public float TrueTotalUpdate { get { return updateTimer.trueTotal; } }

        public float UpdateScale { get { return updateTimer.scale; } set { updateTimer.scale = Math.Max(0, value); } }


        public AtlasTimer()
        {
            updateTimer = new FrameTimer();
            drawTimer = new FrameTimer();

            updateTimer.scale = 1;
            drawTimer.scale = 1;
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
