using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;
using AtlasEngine.BasicManagers.UI;



namespace AtlasEngine.BasicManagers.UI
{
    public class UIImage : UIEntity
    {
        public float alpha;
        protected Vector2 offset;
        protected string imageLocation;
        protected Rectangle rec;

        private bool _loaded;

        public UIImage(AtlasGlobal atlas, string imageLocation)
            : base(atlas)
        {
            this.imageLocation = imageLocation;

            alpha = 1;

            Atlas.Content.LoadContent(imageLocation);
            _loaded = false;
        }

        public override void Update(UIManager manager, bool active) {
        
            base.Update(manager, active);

        }

        public override void Draw(UIManager manager, float parentAlpha)
        {
            base.Draw(manager, parentAlpha);

            if (_loaded)
            {
                Texture2D t = Atlas.Content.GetContent<Texture2D>(imageLocation);

                offset = new Vector2(t.Width / 2, t.Height / 2);
                rec = new Rectangle(0, 0, t.Width, t.Height);
            }

            if (Math.Min(parentAlpha, alpha) > 0)
            {
                Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>(imageLocation),
                                        Position, rec,
                                        Color.White * (parentAlpha * alpha), offset,
                                        0, 1, false);
            }
        }
    }
}
