using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;


namespace AtlasEngine.BasicManagers
{
    public class CameraManager : AtlasEntity,  IAtlasManager, AtlasGraphics.MatrixHandler
    {
        private Matrix _spriteMatrix;
        private Matrix _effectMatrix;


        private float _raduis;
        public float Raduis
        {
            get { return _raduis; }
            set { _raduis = value; _dirty = true; }
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

        private bool _dirty;

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

        public CameraManager(AtlasGlobal atlas, float raduis)
            : base(atlas)
        {
            this._raduis = raduis;

            Atlas.Graphics.onResolutionChange += () =>
            {
                _viewport = Atlas.Graphics.ViewPort;
                _dirty = true;
            };

            Restart(true);
        }



        public virtual void Initialize()
        {

        }

        public virtual void Restart(bool force)
        {
            _dirty = _upDirty = true;
            _scrollFactor = Vector2.One;

            _position = Vector2.Zero;
            _viewport = Atlas.Graphics.ViewPort;

          
        }


        public virtual void Update(string arg)
        {
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

        public virtual void Draw(int pass)
        {
            Configure();
            Atlas.Graphics.SetMatrixHandler(this);

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


            vertex[0].Position = new Vector3(_position.X + tmp, _position.Y + tmp, 0);
            vertex[1].Position = new Vector3(_position.X - tmp, _position.Y + tmp, 0);
            vertex[2].Position = new Vector3(_position.X + tmp, _position.Y - tmp, 0);
            vertex[3].Position = new Vector3(_position.X - tmp, _position.Y - tmp, 0);

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
                        vertex[i].TextureCoordinate.X = (vertex[i].Position.X + offset.X) / (texture.Width * scale);
                        vertex[i].TextureCoordinate.Y = (vertex[i].Position.Y + offset.Y) / (texture.Height * scale);
                    }
                }
            }
            vertex[0].Color = vertex[1].Color = vertex[2].Color = vertex[3].Color = color;

            Atlas.Graphics.SetPrimitiveType(PrimitiveType.TriangleStrip);
            Atlas.Graphics.DrawVector(vertex, texture);
        }




        public RasterizerState RasterState { get { return RasterizerState.CullClockwise; } }
        public Viewport ViewPort { get { return _viewport; } }
        public SpriteEffects SpriteEffect{ get { return SpriteEffects.FlipVertically; } }

        public Matrix GetSpriteBatchMatrix() { Configure(); return _spriteMatrix; }
        public Matrix GetEffectMatrix() { Configure(); return _effectMatrix; }


        public float PixelToPointRation { get { return Height / Atlas.Graphics.ResolutionHeight; } }
    }
}
