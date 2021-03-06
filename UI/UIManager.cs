﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;

namespace AtlasEngine.UI
{
    public abstract class UIManager : AtlasEntity, IAtlasManager, AtlasEngine.AtlasGraphics.MatrixHandler
    {
        public UIView View { get; private set; }



        public UIManager(AtlasGlobal atlas)
            : base(atlas)
        {
            View = new UIView(atlas, new RectangleF(0, 0, 640, 480));

            Atlas.Graphics.onResolutionChange += () =>
            {
                View.Frame = new RectangleF(0, 0, Atlas.Graphics.ResolutionWidth, Atlas.Graphics.ResolutionHeight);
            };
        }

        public virtual void Initialize()
        {
        }

        public virtual void Restart(bool force)
        {
        }

        public virtual void Update(string arg)
        {
            View.Frame = new RectangleF(0, 0, Atlas.Graphics.ResolutionWidth, Atlas.Graphics.ResolutionHeight);

            foreach (var t in Atlas.Input.GetTouchCollection())
            {
                if (t.State == AtlasTouchState.Pressed)
                {
                    var v = View.PointForView(t.Position);

                    if (v != null && v != View) {
                        t.SetOwner(v);
                        v.TouchUpdate(t);
                    }
                }
                else if (t.State != AtlasTouchState.Invalid && t.Owner is UIView)
                {
                    var view = t.Owner as UIView;

                    view.TouchUpdate(t);
                }
            }

            View.Update();
        }

        public virtual void Draw(int arg)
        {
            Atlas.Graphics.SetMatrixHandler(this);

            View.Frame = new RectangleF(0, 0, Atlas.Graphics.ResolutionWidth, Atlas.Graphics.ResolutionHeight);
            View.Draw(View.Frame);
        }

        public Matrix GetSpriteBatchMatrix()
        {
            return Matrix.Identity;
        }

        public Matrix GetEffectMatrix()
        {
            return Matrix.Identity;
        }

        public RasterizerState RasterState { get { return RasterizerState.CullCounterClockwise; } }
        public Viewport ViewPort { get { return Atlas.Graphics.ViewPort; ; } }
        public SpriteEffects SpriteEffect { get { return SpriteEffects.None; } }
    }
}
