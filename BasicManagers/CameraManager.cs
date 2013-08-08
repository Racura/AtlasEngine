using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;


namespace AtlasEngine.BasicManagers
{
    public class CameraManager : AtlasManager, AtlasEngine.AtlasGraphics.MatrixHandler
    {
        private int _priority;

        private Matrix _spriteMatrix;
        private Matrix _effectMatrix;

        public Matrix GetSpriteBatchMatrix() { Configure(); return _spriteMatrix; }
        public Matrix GetEffectMatrix() { Configure(); return _effectMatrix; }


        private float _raduis;
        public float Raduis
        {
            get { return _raduis; }
            set { _raduis = value; _dirty = true; }
        }
        private float _wantedRaduis;
        public float WantedRaduis
        {
            get { return _wantedRaduis; }
            set { _wantedRaduis = value; }
        }

        public float WantedWidth
        {
            get { return Math.Max(_wantedRaduis * Atlas.Graphics.ResolutionRatio, _wantedRaduis) * 2; }
            set
            {
                _wantedRaduis = Math.Min(value * 0.5f / Atlas.Graphics.ResolutionRatio, value * 0.5f);
            }
        }
        public float WantedHeight
        {
            get { return Math.Max(_wantedRaduis / Atlas.Graphics.ResolutionRatio, _wantedRaduis) * 2; }
            set
            {
                _wantedRaduis = Math.Min(value * 0.5f * Atlas.Graphics.ResolutionRatio, value * 0.5f);
            }
        }

        public float Width
        {
            get { return Math.Max(_raduis * Atlas.Graphics.ResolutionRatio, _raduis) * 2; }
            set
            {
                Raduis = Math.Min(value * 0.5f / Atlas.Graphics.ResolutionRatio, value * 0.5f);
            }
        }

        public float Height
        {
            get { return Math.Max(_raduis / Atlas.Graphics.ResolutionRatio, _raduis) * 2; }
            set
            {
                Raduis = Math.Min(value * 0.5f * Atlas.Graphics.ResolutionRatio, value * 0.5f);
            }
        }

        private Vector2 _scrollFactor;
        public void SetScrollFactor(Vector2 scrollFactor)
        {
            this._scrollFactor = scrollFactor;
            Atlas.Graphics.Flush();
            _dirty = true;
        }

        private Viewport _viewport;

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; _dirty = true; }
        }
        public float X
        {
            get { return _position.X; }
            set { _position.X = value; _dirty = true; }
        }
        public float Y
        {
            get { return _position.Y; }
            set { _position.Y = value; _dirty = true; }
        }

        private Vector2 _wantedPosition;
        public Vector2 WantedPosition
        {
            get { return _wantedPosition; }
            set { _wantedPosition = value; }
        }


        private RectangleF _boundingBox;
        public RectangleF BoundingBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
        }


        private float _cameraSpeed;
        private float _raduisSpeed;
        public float CameraSpeed
        {
            get { return _cameraSpeed; }
            set { _cameraSpeed = value; }
        }
        public float RaduisSpeed
        {
            get { return _raduisSpeed; }
            set { _raduisSpeed = value; }
        }

        private bool _dirty;

        private Vector2 _lastPosition;
        private float _lastRaduis;

        public Vector2 initPosition;
        public float initRaduis;
        public RectangleF initBoundingBox;

        private float _angle;
        public float Angle
        {
            get { return _angle; }
            set { _angle = value; _dirty = true; _upDirty = true; }
        }

        private bool _upDirty;
        private Vector2 _up;
        public Vector2 Up
        {
            get
            {
                if (_upDirty)
                {
                    _up.X = (float)Math.Sin(_angle);
                    _up.Y = (float)Math.Cos(_angle);
                    _upDirty = false;
                }

                return _up;
            }
        }
        public Vector2 Right
        {
            get
            {
                return new Vector2(Up.Y, -Up.X);
            }
        }

        public CameraManager(AtlasGlobal atlas, Vector2 initPosition, float initRaduis, RectangleF initBoundingBox)
            : base(atlas)
        {
            this.initRaduis = initRaduis;
            this.initPosition = initPosition;
            this.initBoundingBox = initBoundingBox;

            Atlas.Graphics.onResolutionChange += () =>
            {
                _viewport = Atlas.Graphics.ViewPort;
                _dirty = true;
            };

            Restart(true);
        }

        public override void Restart(bool force)
        {
            _priority = -1;
            _boundingBox = initBoundingBox;

            _cameraSpeed = 1;
            _raduisSpeed = 1;

            _dirty = _upDirty = true;
            _wantedRaduis = initRaduis;
            _wantedPosition = initPosition;
            _scrollFactor = Vector2.One;

            if (force)
            {
                _raduis = _wantedRaduis;
                _position = _wantedPosition;
            }
            _viewport = Atlas.Graphics.ViewPort;

            base.Restart(force);
        }

        public Vector2 CheckBounds(Vector2 point)
        {
            if (point.X + Width / 2 > _boundingBox.X + _boundingBox.Width)
                point.X = _boundingBox.X + _boundingBox.Width - Width / 2;
            if (point.X - Width / 2 < _boundingBox.X)
                point.X = _boundingBox.X + Width / 2;

            if (point.Y + Height / 2 > _boundingBox.Y + _boundingBox.Height)
                point.Y = _boundingBox.Y + _boundingBox.Height - Height / 2;
            if (point.Y - Height / 2 < _boundingBox.Y)
                point.Y = _boundingBox.Y + Height / 2;

            return point;
        }


        public override void Update(string arg)
        {
            _priority = -1;

            if (WantedWidth > _boundingBox.Width)       WantedWidth = _boundingBox.Width;
            if (WantedHeight > _boundingBox.Height)     WantedHeight = _boundingBox.Height;

            float tmp = 1 + Math.Max(_raduis - _wantedRaduis, 0) / _raduis * _raduisSpeed;

            if (Math.Abs(_wantedRaduis - _raduis) * Atlas.Elapsed * _raduisSpeed > 0.025f)
                _raduis += (_wantedRaduis - _raduis) * Atlas.Elapsed * _raduisSpeed;
            else
                _raduis = _wantedRaduis;

            if (Width > _boundingBox.Width)             Width = _boundingBox.Width;
            if (Height > _boundingBox.Height)           Height = _boundingBox.Height;

            _wantedPosition = CheckBounds(_wantedPosition);

            if (Math.Abs(_wantedPosition.X - _position.X) * _cameraSpeed * tmp > 0.025f)
                _position.X += (_wantedPosition.X - _position.X) * Atlas.Elapsed * _cameraSpeed * tmp;
            else
                _position.X = _wantedPosition.X;

            if (Math.Abs(_wantedPosition.Y - _position.Y) * _cameraSpeed * tmp > 0.025f)
                _position.Y += (_wantedPosition.Y - _position.Y) * Atlas.Elapsed * _cameraSpeed * tmp;
            else
                _position.Y = _wantedPosition.Y;

            _position = CheckBounds(_position);

            if (!_dirty && (_position.X != _lastPosition.X ||
                            _position.Y != _lastPosition.Y ||
                            _raduis != _lastRaduis))
            {
                _dirty = true;
            }
            _lastPosition = _position;
            _lastRaduis = _raduis;
        }

        public Vector2 GetWorldPosition(Vector2 point, Vector2 scrollFactor)
        {
            Vector2 v = new Vector2();

            v.X = ((point.X / Atlas.Graphics.ResolutionWidth) - 0.5f) * Width
                + _position.X * scrollFactor.X;
            v.Y = (0.5f - (point.Y / Atlas.Graphics.ResolutionHeight)) * Height
                + _position.Y * scrollFactor.Y;

            return v;
        }

        public override void Draw(int pass)
        {
            Configure();
            Atlas.Graphics.SetMatrixHandler(this);

        }


        public Viewport GetViewPort()
        {
            return _viewport;
        }

        public void LookAt(int priority, Vector2? position, float? raduis, float speed, float raduisSpeed)
        {
            if (this._priority <= priority)
            {
                this._priority = priority;

                if (position != null)
                {
                    this._wantedPosition = (Vector2)position;
                    this._cameraSpeed = speed;
                }

                if (raduis != null)
                {
                    this.WantedRaduis = (float)raduis;
                    this._raduisSpeed = raduisSpeed;
                }
            }
        }

        public void Configure()
        {
            if (_dirty)
            {
                Atlas.Graphics.Flush();

                float tmpX = Width * 0.5f;
                float tmpY = Height * 0.5f;

                int tmp = Math.Min(Atlas.Graphics.ResolutionWidth, Atlas.Graphics.ResolutionHeight);

                Viewport tmpViewort = Atlas.Graphics.ViewPort;

                _spriteMatrix =
                    Matrix.CreateTranslation(-_position.X * _scrollFactor.X,
                                            -_position.Y * _scrollFactor.Y, 0)
                    * Matrix.CreateRotationZ(_angle)
                    * Matrix.CreateScale(tmp / _raduis * 0.5f, -tmp / _raduis * 0.5f, 0)
                    * Matrix.CreateTranslation(tmpViewort.Width / 2,
                                            tmpViewort.Height / 2, 0)
                    ;


                _effectMatrix =
                    Matrix.CreateTranslation(-_position.X * _scrollFactor.X,
                                            -_position.Y * _scrollFactor.Y, 0)
                    * Matrix.CreateRotationZ(_angle)
                    * Matrix.CreateOrthographicOffCenter(-tmpX, tmpX,
                                                        -tmpY, tmpY, -100, 100);

                _dirty = false;
            }
        }


        public void DrawTile(Texture2D texture, Vector2 offset, Color color, float scale)
        {
            VertexPositionColorTexture[] vertex = new VertexPositionColorTexture[4];
            Vector2 n = Up;

            float tmp = Math.Max(Width, Height) * 10;


            vertex[0].Position = 1.0f * new Vector3(_position.X + tmp, _position.Y + tmp, 0);
            vertex[1].Position = 1.0f * new Vector3(_position.X - tmp, _position.Y + tmp, 0);
            vertex[2].Position = 1.0f * new Vector3(_position.X + tmp, _position.Y - tmp, 0);
            vertex[3].Position = 1.0f * new Vector3(_position.X - tmp, _position.Y - tmp, 0);

            if (texture != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (scale == 0)
                    {
                        vertex[i].TextureCoordinate.X = vertex[i].TextureCoordinate.Y = 0;
                    }
                    else
                    {
                        vertex[i].TextureCoordinate.X = (vertex[i].Position.X + offset.X) / texture.Width / scale;
                        vertex[i].TextureCoordinate.Y = (vertex[i].Position.Y + offset.Y) / texture.Height / scale;
                    }
                }
            }
            vertex[0].Color = vertex[1].Color = vertex[2].Color = vertex[3].Color = color;

            Atlas.Graphics.SetPrimitiveType(PrimitiveType.TriangleStrip);
            Atlas.Graphics.DrawVector(vertex, texture);
        }




        public RasterizerState GetSpriteBatchRasterState()
        {
            return RasterizerState.CullClockwise;
        }
    }
}
