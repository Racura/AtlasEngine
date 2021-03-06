﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using AtlasEngine.Infrastructure;

namespace AtlasEngine
{
    public class AtlasGlobal
    {
        private AtlasTimer _timer;
        
        public T GetManager<T>() where T : IAtlasManager { return (T)managerComponent.Manager[typeof(T).AssemblyQualifiedName]; }
        public float Elapsed { get { return _timer.ElapsedUpdate; } }
        public float TrueElapsed { get { return _timer.TrueElapsedUpdate; } }
        public float TimerScale { get { return _timer.UpdateScale; } set { _timer.UpdateScale = value; } }
        public float TotalTime { get { return _timer.TotalUpdate; } }

        private Game game;
        public Game Game { get { return game; } }


        private AtlasComponent managerComponent;
        public AtlasComponent Component { get { return managerComponent; } }

        private AtlasGraphics graphics;
        public AtlasGraphics Graphics { get { return graphics; } }

        private AtlasContent content;
        public AtlasContent Content { get { return content; } }

        private AtlasInput input;
        public AtlasInput Input { get { return input; } }

        private Random rand;
        public float Rand { get { return (float)rand.NextDouble(); } }
        public bool Debug
        {
            get;
            set;
        }


        public AtlasGlobal(AtlasComponent managerComponent, GraphicsDeviceManager graphicsManager)
        {
            this.managerComponent = managerComponent;
            this.game = managerComponent.Game as Game;

#if MONOGAME
            Windows.UI.Input.PointerVisualizationSettings.GetForCurrentView().IsContactFeedbackEnabled          = false;
            Windows.UI.Input.PointerVisualizationSettings.GetForCurrentView().IsBarrelButtonFeedbackEnabled     = false;
#endif

            game.IsFixedTimeStep = false;

            _timer = new AtlasTimer();
            graphics = new AtlasGraphics(this, graphicsManager);
            content = new AtlasContent(this);
            input = new AtlasInput();
            rand = new Random();

            Debug = false;
        }

        internal void Update(GameTime gameTime)
        {
            _timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            input.Update();
            graphics.Update();

           
#if DEBUG
            if (input.IsKeyJustPressed(Keys.F1))
                Debug = !Debug;
#endif
        }

        internal void Draw(GameTime gameTime)
        {
            _timer.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            graphics.Draw();
        }

    }
}
