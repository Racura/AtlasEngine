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
    public class UIEntity : AtlasEntity
    {
        private UIRelativePositionType _relativePositionType;
        public UIRelativePositionType RelativePositionType
        {
            get { return _relativePositionType; }
            set { _relativePositionType = value; DirtyPosition = true; }
        }
        private Vector2 _relativePosition;
        public Vector2 RelativePosition
        {
            get { return _relativePosition; }
            set { _relativePosition = value; DirtyPosition = true; }
        }
        private Vector2 _offset;
        public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; DirtyPosition = true; }
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
        }

        public virtual bool DirtyPosition
        {
            get;
            set;
        }

        public UIEntity(AtlasGlobal atlas)
            : base(atlas)
        {
            DirtyPosition = false;
        }

        public virtual void Update(UIManager manager, bool active)
        {
            ResetPosition(manager);
        }

        public virtual void Draw(UIManager manager, float parentAlpha)
        {
            ResetPosition(manager);
        }

        protected virtual void ResetPosition(UIManager manager)
        {
            if (DirtyPosition != true)
                return;

            float width =  manager.Width;
            float height = manager.Height;

            DirtyPosition = false;

            if (_relativePositionType == UIRelativePositionType.None)
                _position = _offset;

            _position = Vector2.Zero;

            if ((_relativePositionType & UIRelativePositionType.PixelsFromRight) == UIRelativePositionType.PixelsFromRight)
                _position.X = width + _relativePosition.X + _offset.X;
            if ((_relativePositionType & UIRelativePositionType.PixelsFromLeft) == UIRelativePositionType.PixelsFromLeft)
                _position.X = 0 + _relativePosition.X + _offset.X;
            if ((_relativePositionType & UIRelativePositionType.VerticalPercent) == UIRelativePositionType.VerticalPercent)
                _position.X = width * _relativePosition.X * 0.01f + _offset.X;

            if ((_relativePositionType & UIRelativePositionType.PixelsFromBottom) == UIRelativePositionType.PixelsFromBottom)
                _position.Y = height - _relativePosition.Y + _offset.Y;
            if ((_relativePositionType & UIRelativePositionType.PixelsFromTop) == UIRelativePositionType.PixelsFromTop)
                _position.Y = 0 + _relativePosition.Y + _offset.Y;
            if ((_relativePositionType & UIRelativePositionType.HorizontalPercent) == UIRelativePositionType.HorizontalPercent)
                _position.Y = height * _relativePosition.Y * 0.01f + _offset.Y;
        }

    }

    [Flags]
    public enum UIRelativePositionType
    {
        None                = 0x0,
        PixelsFromTop       = 0x01,
        PixelsFromBottom    = 0x02,
        PixelsFromRight     = 0x04,
        PixelsFromLeft      = 0x08,
        HorizontalPercent   = 0x10,
        VerticalPercent     = 0x20,
    }
}
