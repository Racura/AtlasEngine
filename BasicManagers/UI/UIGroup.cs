using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;

namespace AtlasEngine.BasicManagers.UI
{
    public class UIGroup : UIEntity
    {
        public float alpha;
        public bool show;
        private List<UIEntity> _list;

        public override bool DirtyPosition
        {
            get
            {
                return base.DirtyPosition;
            }
            set
            {
                if (_list != null)
                {
                    foreach (UIEntity e in _list)
                        e.DirtyPosition = value;
                }
                base.DirtyPosition = value;
            }
        }

        public UIGroup(AtlasGlobal atlas)
            : base(atlas)
        {
            _list = new List<UIEntity>();

            alpha = 0;
            show = false;
        }

        public virtual void Show(bool animate)
        {
            show = true;

            if (!animate)
                alpha = 1;
        }
        public virtual void Hide(bool animate)
        {
            show = false;

            if (!animate)
                alpha = 0;
        }

        public override void Update(UIManager manager, bool active)
        {
            alpha = MathHelper.Clamp(alpha + Atlas.Elapsed * (show ? 1 : -1), 0, 1);

            foreach (UIEntity e in _list)
                e.Update(manager, active && show);
        }

        public override void Draw(UIManager manager, float parentAlpha)
        {
            if (DirtyPosition == true)
                ResetPosition(manager);

            foreach (UIEntity e in _list)
                e.Draw(manager, alpha * parentAlpha);
        }




        public void Add(UIEntity e)
        {
            if (e != this)
                _list.Add(e);
        }
        public void Remove(UIEntity e)
        {
            if (e != this)
                _list.Remove(e);
        }
    }
}
