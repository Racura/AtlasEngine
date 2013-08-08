using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;
using AtlasEngine.BasicManagers.UI;
#if MONOGAME
using Microsoft.Xna.Framework.Input.Touch;
#endif



namespace AtlasEngine.BasicManagers.UI
{
    public class UIButton : UIImage
    {
        protected Rectangle hitbox;

        public bool hover;
        public bool down;

        public UIButton(AtlasGlobal atlas, string imageLocation)
            : base(atlas, imageLocation)
        {
            hitbox = rec;
        }

        public override void Update(UIManager manager, bool active)
        {
            base.Update(manager, active);

            if (!active) 
            {
                hover = false;
                this.down = false;
            }

            var touches = Atlas.Input.GetTouchCollection();

            hitbox.X = (int)Position.X - rec.Width / 2;
            hitbox.Y = (int)Position.Y - rec.Height / 2;

            bool down = false;
            hover = false;

            if (!active || touches.Count > 0)
            {
                return;
            }

            foreach (var t in Atlas.Input.GetTouchCollection())
            {
                if ((!t.HasOwner && t.State == TouchLocationState.Pressed))
                {
                    if (hitbox.Contains((int)t.Position.X, (int)t.Position.Y))
                    {
                        t.SetOwner(this);
                        down = hover = true;

                    }
                }
                else if(t.Owner == this)
                {
                    down = true;

                    if (hitbox.Contains((int)t.Position.X, (int)t.Position.Y))
                    {
                        hover = true;
                    }
                }
            }

            if (this.down && hover && !down)
            {
            }

            this.down = down;
        }

        public override void Draw(UIManager manager, float parentAlpha)
        {
            base.Draw(manager, parentAlpha);
        }
    }
}
