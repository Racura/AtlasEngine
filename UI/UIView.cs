using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;

namespace AtlasEngine.UI
{
    public class UIView : AtlasEntity
    {
        public UIAutoResizingMask AutoResizeMask { get; set; }

        private RectangleF _lastFrame;
        private RectangleF _frame;
        public RectangleF Frame
        {
            set
            {
                _lastFrame = _frame;
                if (_frame.Equals(value))
                    return;

                _frame = value;

                LayoutSubviews();
                Bounds = new Rectangle(0, 0, (int)_frame.Width, (int)_frame.Height);
                Position = new Vector2(_frame.X, _frame.Y);
            }
            get { return _frame; }
        }
        public Rectangle Bounds { set; get; }
        public Vector2 Position { set; get; }

        public bool Hidden          { set; get; }
        public bool TouchResponder  { set; get; }

        public float Alpha { set; get; }
        protected float TrueAlpha { get { if (SuperViews == null) return Alpha; return Alpha * SuperViews.TrueAlpha; } }


        public Color BackgroundColor { set; get; }

        public UIView(AtlasGlobal atlas, RectangleF frame)
            : base(atlas)
        {
            Frame = frame;

            InitializeView();
        }


        protected virtual void InitializeView()
        {
            Hidden = false;
            TouchResponder = true;

            Alpha = 1;
            BackgroundColor = Color.Transparent;
        }

        public virtual void Update()
        {

            if (SubViews == null)
                return;

            foreach (var v in SubViews)
            {
                v.Update();
            }
        }

        public virtual UIView PointForView(Vector2 point)
        {
            if (!Bounds.Contains((int)point.X, (int)point.Y) || Hidden)
                return null;

            if (SubViews != null)
            {
                for (int i = SubViews.Length - 1; i >= 0; --i)
                {
                    var v = SubViews[i].PointForView(point - SubViews[i].Position);

                    if (v != null)
                        return v;
                }
            }

            if(TouchResponder)
                return this;

            return null;
        }

        public virtual void TouchUpdate(AtlasTouchPosition touch)
        {

        }

        public virtual void Draw(RectangleF rect)
        {

            if (BackgroundColor.A * TrueAlpha > 0)
                Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>(""), rect.Position, rect.Bounds, BackgroundColor * TrueAlpha);

            if (SubViews == null)
                return;

            foreach (var v in SubViews)
            {
                if (!v.Hidden)
                    v.Draw(v.Frame.Move(rect.Position));
            }
        }

        public virtual void LayoutSubviews()
        {
            if (SubViews == null)
                return;

            foreach (var v in SubViews)
            {
                if (v.AutoResizeMask == UIAutoResizingMask.None)
                    continue;

                RectangleF rect = v.Frame;

                if ((v.AutoResizeMask & (UIAutoResizingMask.FlexibleWidth | UIAutoResizingMask.FlexibleLeftMargin))
                    != UIAutoResizingMask.None)
                {
                    var values = new float[]
                    {
                        (rect.X),
                        (rect.Width),
                        (_lastFrame.Width - rect.X - rect.Width),
                    };

                    var flexible = new bool[]{
                        ((v.AutoResizeMask & UIAutoResizingMask.FlexibleLeftMargin)    != UIAutoResizingMask.None && values[0] != 0),
                        ((v.AutoResizeMask & UIAutoResizingMask.FlexibleWidth)         != UIAutoResizingMask.None && values[1] != 0),
                        ((v.AutoResizeMask & UIAutoResizingMask.FlexibleRightMargin)   != UIAutoResizingMask.None && values[2] != 0),
                    };

                    float totalFlex = (flexible[0] ? values[0] : 0) + (flexible[1] ? values[1] : 0) + (flexible[2] ? values[2] : 0);

                    if (flexible[0])
                        rect.X = values[0] + values[0] / totalFlex * (_frame.Width - _lastFrame.Width);

                    if (flexible[1])
                        rect.Width = values[1] + values[1] / totalFlex * (_frame.Width - _lastFrame.Width);
                } 
                if ((v.AutoResizeMask & (UIAutoResizingMask.FlexibleHeight | UIAutoResizingMask.FlexibleTopMargin))
                     != UIAutoResizingMask.None)
                {
                    var values = new float[]
                    {
                        (rect.Y),
                        (rect.Height),
                        (_lastFrame.Height - rect.Y - rect.Height),
                    };

                    var flexible = new bool[]{
                        ((v.AutoResizeMask & UIAutoResizingMask.FlexibleTopMargin)      != UIAutoResizingMask.None && values[0] != 0),
                        ((v.AutoResizeMask & UIAutoResizingMask.FlexibleHeight)         != UIAutoResizingMask.None && values[1] != 0),
                        ((v.AutoResizeMask & UIAutoResizingMask.FlexibleBottomMargin)   != UIAutoResizingMask.None && values[2] != 0),
                    };

                    float totalFlex = (flexible[0] ? values[0] : 0) + (flexible[1] ? values[1] : 0) + (flexible[2] ? values[2] : 0);

                    if (flexible[0])
                        rect.Y = values[0] + values[0] / totalFlex * (_frame.Height - _lastFrame.Height);

                    if (flexible[1])
                        rect.Height = values[1] + values[1] / totalFlex * (_frame.Height - _lastFrame.Height);
                }

                v.Frame = rect;
            }
        }


        #region Subview stuff

        private List<UIView> _subviews;
        public UIView[] SubViews { get; private set; }
        public UIView SuperViews { get; private set; }

        public void AddSubviews(UIView view)
        {
            if (view.SuperViews != null)
                throw new Exception("Element can't have multiple Views");

            if (_subviews == null)
                _subviews = new List<UIView>();

            _subviews.Add(view);
            view.SuperViews = this;

            SubViews = _subviews.ToArray();
        }

        public void RemoveFromSuperview()
        {
            if (this.SuperViews == null)
                return;

            SuperViews._subviews.Remove(this);
            SuperViews.SubViews = SuperViews._subviews.ToArray();

            this.SuperViews = null;
        }

        #endregion

    }

    [Flags]
    public enum UIAutoResizingMask
    {
        None                    = 0x00,

        FlexibleWidth           = 0x01,
        FlexibleHeight          = 0x02,

        FlexibleTopMargin       = 0x10,
        FlexibleBottomMargin    = 0x20,
        FlexibleRightMargin     = 0x40,
        FlexibleLeftMargin      = 0x80,


        FlexibleDimensions      = FlexibleWidth | FlexibleHeight,
        FlexibleMargins         = FlexibleTopMargin | FlexibleBottomMargin | FlexibleRightMargin | FlexibleLeftMargin,
        All                     = FlexibleDimensions | FlexibleMargins,
    }
}
