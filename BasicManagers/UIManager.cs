using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;
using AtlasEngine.BasicManagers.UI;

namespace AtlasEngine.BasicManagers
{
    
    public abstract class UIManager : AtlasManager, AtlasEngine.AtlasGraphics.MatrixHandler
    {
        private Dictionary<string, UIGroup> _uiElements;
        protected string _activeElement;
        protected float _radius;
        private float _width;
        private float _height;

        public float Width
        {
            get { Check(); return _width; }
        }
        public float Height
        {
            get { Check(); return _height; }
        }

        public UIManager(AtlasGlobal atlas, float radius)
            : base(atlas)
        {
            _radius = radius;
            _uiElements = new Dictionary<string, UIGroup>();

            Atlas.Graphics.onResolutionChange += () =>
            {
                foreach (KeyValuePair<string, UIGroup> g in _uiElements)
                    g.Value.DirtyPosition = true;
            };
        }

        public override void Restart(bool force)
        {
            base.Restart(force);
        }

        public override void Update(string arg)
        {
            Check();

            foreach (KeyValuePair<string, UIGroup> g in _uiElements)
                g.Value.Update(this, _activeElement.Equals(g.Key));

            base.Update(arg);
        }

        public override void Draw(int arg)
        {
            Atlas.Graphics.SetMatrixHandler(this);

            Check();

            foreach (KeyValuePair<string, UIGroup> g in _uiElements)
                g.Value.Draw(this, 1);

            base.Draw(arg);
        }

        public void Focus(string key, bool force, bool hideOthers, bool hidePrevious)
        {
            if (hidePrevious)
                _uiElements[_activeElement].Hide(force);


            foreach (KeyValuePair<string, UIGroup> g in _uiElements)
            {
                if (key.Equals(g.Key))
                    g.Value.Show(force);
                else if (hideOthers)
                    g.Value.Hide(force);
            }

            _activeElement = key;
        }

        public void Add(string key, UIGroup value)
        {
            _uiElements.Add(key, value);
        }


        private void Check()
        {
            float tmpX = Atlas.Graphics.ResolutionHeight / Math.Min(Atlas.Graphics.ResolutionWidth, Atlas.Graphics.ResolutionHeight) * _radius;
            float tmpY = Atlas.Graphics.ResolutionWidth / Math.Min(Atlas.Graphics.ResolutionWidth, Atlas.Graphics.ResolutionHeight) * _radius;

            if (tmpX != _width || tmpY != _height)
            {
                _width = tmpX;
                _height = tmpY;

                foreach (KeyValuePair<string, UIGroup> g in _uiElements)
                    g.Value.DirtyPosition = true;
            }
        }

        public Matrix GetSpriteBatchMatrix()
        {
            return Matrix.Identity;
        }

        public Matrix GetEffectMatrix()
        {
            return Matrix.Identity;
        }

        public Viewport GetViewPort()
        {
            return Atlas.Graphics.ViewPort;
        }

        public RasterizerState GetSpriteBatchRasterState()
        {
            return RasterizerState.CullCounterClockwise;
        }
    }
}
