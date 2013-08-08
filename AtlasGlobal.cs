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
        private AtlasTimer timer;

        public T GetManager<T>() where T : AtlasManager { return managerComponent.Manager[typeof(T).AssemblyQualifiedName] as T; }
        public float Elapsed { get { return timer.GetElapsedUpdate(); } }
        public float TotalTime { get { return timer.GetTotalUpdate(); } }

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


        public AtlasGlobal(AtlasComponent managerComponent, GraphicsDeviceManager graphicsManager)
        {
            this.managerComponent = managerComponent;
            this.game = managerComponent.Game as Game;

#if MONOGAME
            Windows.UI.Input.PointerVisualizationSettings.GetForCurrentView().IsContactFeedbackEnabled          = false;
            Windows.UI.Input.PointerVisualizationSettings.GetForCurrentView().IsBarrelButtonFeedbackEnabled     = false;
#endif

            game.IsFixedTimeStep = false;

            timer = new AtlasTimer();
            graphics = new AtlasGraphics(this, graphicsManager);
            content = new AtlasContent(this);
            input = new AtlasInput();
            rand = new Random();
        }

        internal void Update(GameTime gameTime)
        {
            timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            input.Update();
            graphics.Update();
        }

        internal void Draw(GameTime gameTime)
        {
            timer.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            graphics.Draw();
        }

    }
}