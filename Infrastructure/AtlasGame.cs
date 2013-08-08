using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;

namespace AtlasEngine.Infrastructure
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AtlasGame : Game
    {
        public GraphicsDeviceManager graphics;

        public AtlasGame() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            // TODO: Add your initialization logic here

            base.Initialize();
        }
    }
}
