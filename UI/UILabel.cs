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
    public class UILabel : UIView
    {
        private string _text;
        private SpriteFont _font;
        private UITextAlignment _alignment;

        private int _count;
        private string[] _subTexts;
        private Vector2[] _subPosition;


        public Color TextColor { get; set; }
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RecalculateLabel();
                }
            }
        }

        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                if (_font != value)
                {
                    _font = value;
                    RecalculateLabel();
                }
            }
        }

        public UITextAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                if (_alignment != value)
                {
                    _alignment = value;
                    RecalculateLabel();
                }
            }
        }

        public UILabel(AtlasGlobal atlas, RectangleF frame)
            : base(atlas, frame)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            RecalculateLabel();
        }

        private void RecalculateLabel()
        {
            if (_text == null || _text.Length == 0 || _font == null)
            {
                _subTexts = null;
                return;
            }

            _subTexts = _text.Split('\n');
            _subPosition = new Vector2[_subTexts.Length];

            float height = 0;

            float divider = Alignment == UITextAlignment.Left ? 0 : (Alignment == UITextAlignment.Center ? 0.5f : 1);
            
            for (int i = 0; i < _subTexts.Length; ++i)
            {
                var rect = _font.MeasureString(_subTexts[i]);

                _subPosition[i] = new Vector2((Bounds.Width - rect.X) * divider, height);

                height += rect.Y;
                if (height > Bounds.Height)
                {
                    height -= rect.Y;
                    break;
                }

                _count = i + 1;
            }

            _count = Math.Max(_count, 1);

            for (int i = 0; i < _count; ++i)
            {
                _subPosition[i].Y += (Bounds.Height - height) * 0.5f;
            }
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);

            if (_subTexts != null && _font != null)
            {
                for (int i = 0; i < _count; ++i)
                {
                    Atlas.Graphics.DrawString(_font, _subTexts[i], rect.Position + _subPosition[i], TextColor * TrueAlpha, 0, Vector2.Zero, 1);
                }
            }
        }
    }

    public enum UITextAlignment
    {
        Left,
        Center,
        Right
    }
}
