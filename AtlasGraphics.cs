using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtlasEngine
{
    public class AtlasGraphics : AtlasEntity
    {
        private AtlasGraphicsMode _currentMode;

        public delegate void ResolutionChangeEvent();
        public ResolutionChangeEvent onResolutionChange;

        public int ResolutionWidth { get { return _viewPort.Width; } }
        public int ResolutionHeight { get { return _viewPort.Height; } }
        public float ResolutionRatio { get { return 1f * ResolutionWidth / ResolutionHeight; } }

        public BlendState blendState = BlendState.AlphaBlend;
        public SamplerState samplerState = SamplerState.LinearClamp;

        private BlendState _lastBlendState = BlendState.AlphaBlend;
        private SamplerState _lastSamplerState = SamplerState.PointClamp;
        private Viewport _viewPort;
        public Viewport ViewPort { get { return _viewPort; } }

#if DEBUG
        public int d_sprite_draws;
        public int d_spritebatch_begins;
#endif
        public AtlasGraphics(AtlasGlobal atlas, GraphicsDeviceManager _graphicsDeviceManager)
            : base(atlas)
        {
            _currentMode = AtlasGraphicsMode.None;

            _graphicsDevice = atlas.Game.GraphicsDevice;
            this._graphicsDeviceManager = _graphicsDeviceManager;

            _graphicsDeviceManager.PreferredBackBufferFormat = SurfaceFormat.Color;

#if XNA
            _graphicsDeviceManager.PreferredBackBufferHeight = 768;
            _graphicsDeviceManager.PreferredBackBufferWidth = 1024;
#endif
            ////_graphicsDeviceManager.IsFullScreen = true;
            _graphicsDeviceManager.ApplyChanges();

            _spriteBatch = new SpriteBatch(_graphicsDevice);

            _basicEffect = new BasicEffect(_graphicsDeviceManager.GraphicsDevice);

            batch = new VertexPositionColorTexture[32][];
            batchPrimitiveCount = new int[batch.Length];

            _viewPort = new Viewport(0, 0,
                _graphicsDeviceManager.PreferredBackBufferWidth,
                _graphicsDeviceManager.PreferredBackBufferHeight);
        }

        public void Clear(Color color)
        {
            Flush();
            _graphicsDevice.Clear(color);
        }

        public void Flush()
        {
            SetGraphicsMode(AtlasGraphicsMode.None);
        }

        private void SetGraphicsMode(AtlasGraphicsMode mode)
        {
            if (_currentMode == mode)
                return;

            switch (_currentMode)
            {
                case AtlasGraphicsMode.Sprite:
                    _spriteBatch.End();
                    break;
                case AtlasGraphicsMode.Vector:
                    DrawVectorBatch();
                    break;
            }

            _currentMode = AtlasGraphicsMode.None;

            switch (mode)
            {
                case AtlasGraphicsMode.Sprite:
                    BeginSprite();
                    break;
                case AtlasGraphicsMode.Vector:
                    BeginVector();
                    break;
            }

            _currentMode = mode;
        }

        public void ResolutionCheck()
        {
            if (_viewPort.Width != _graphicsDeviceManager.PreferredBackBufferWidth
                || _viewPort.Height != _graphicsDeviceManager.PreferredBackBufferHeight)
            {
                _viewPort.Width = _graphicsDeviceManager.PreferredBackBufferWidth;
                _viewPort.Height = _graphicsDeviceManager.PreferredBackBufferHeight;

                if (onResolutionChange != null)
                    onResolutionChange();
            }
        }


        public void Update()
        {
            
#if DEBUG
            d_sprite_draws = 0;
            d_spritebatch_begins = 0;
#endif
            //takeScreenShot = false;
            ResolutionCheck();
        }
        public void Draw()
        {
            ResolutionCheck();

            _graphicsDevice.Viewport = _viewPort;
            _graphicsDevice.SetRenderTarget(null);
        }


        public RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return new RenderTarget2D(_graphicsDevice, width, height);
        }

        RenderTarget2D[] _renderTargetStack;
        int _renderTargetStackCounter;

        public void PushRenderTarget(RenderTarget2D target)
        {
            if (_renderTargetStack == null)
            {
                _renderTargetStack = new RenderTarget2D[8];
                _renderTargetStackCounter = -1;
            }


            if (target == null)
                throw new Exception("Render Target Required. Use SetRenderTargetToBackBuffer.");

            Flush();

            _renderTargetStackCounter++;
            _renderTargetStack[_renderTargetStackCounter] = target;
            _graphicsDevice.SetRenderTarget(target);
        }


        public void PopRenderTarget()
        {
            _renderTargetStackCounter--;
            Flush();

            if (_renderTargetStackCounter == -1)
                _graphicsDevice.SetRenderTarget(null);
            else if (_renderTargetStackCounter >= 0)
                _graphicsDevice.SetRenderTarget(_renderTargetStack[_renderTargetStackCounter]);
            else
                throw new Exception("Nothing in stack");


        }



        /**************/

        private BasicEffect _basicEffect;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private GraphicsDeviceManager _graphicsDeviceManager;
        private int drawCalls;


        private MatrixHandler _matrixHandler;
        public void SetMatrixHandler(MatrixHandler matrixHandler)
        {
            this._matrixHandler = matrixHandler;
            Flush();
            if (matrixHandler == null)
                _graphicsDevice.Viewport = _viewPort;
            else
                _graphicsDevice.Viewport = _matrixHandler.GetViewPort();
        }



        /*****SpriteBatch*******/

        public void BeginSprite()
        {
            BeginSprite(blendState, samplerState);
        }

        public void BeginSprite(BlendState blendState, SamplerState samplerState)
        {
            if (_currentMode == AtlasGraphicsMode.Sprite &&
                _lastBlendState == blendState &&
                _lastSamplerState == samplerState)
            {
                return;
            }
            Flush();
            _currentMode = AtlasGraphicsMode.Sprite;
            _lastBlendState = blendState;
            _lastSamplerState = samplerState;

            drawCalls = 0;

            if (_matrixHandler == null)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred,
                                blendState,
                                samplerState,
                                DepthStencilState.None,
                                RasterizerState.CullCounterClockwise,
                                null);
            }
            else
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred,
                                blendState,
                                samplerState,
                                DepthStencilState.None,
                                _matrixHandler.GetSpriteBatchRasterState(),
                                null, _matrixHandler.GetSpriteBatchMatrix());
            }
#if DEBUG
            d_spritebatch_begins++;
#endif
        }

        public void EndSprite()
        {
            Flush();
        }

#region DrawSprites


        public void DrawSprite(Texture2D texture, Vector2 position,
                            Color color)
        {
            DrawSprite(texture, position, null, color,
                             Vector2.Zero, 0, 1,
                             SpriteEffects.None);
        }
        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? source,
                            Color color)
        {
            DrawSprite(texture, position, source, color,
                             Vector2.Zero, 0, 1,
                             SpriteEffects.None);
        }
        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? source,
                            Color color,
                            Vector2 orgin,
                            float angle, float scale)
        {
            DrawSprite(texture, position, source, color,
                             orgin, angle, scale,
                             SpriteEffects.None);
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? source,
                            Color color,
                            Vector2 orgin,
                            float angle, float scale,
                            bool horizontalFlip)
        {
            DrawSprite(texture, position, source, color,
                             orgin, angle, scale,
                             (horizontalFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
        }

#endregion

        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? source,
                            Color color,
                            Vector2 orgin,
                            float angle, float scale,
                            SpriteEffects spriteEffects)
        {
            if (drawCalls > 256 * 64 - 64)
                Flush();

            SetGraphicsMode(AtlasGraphicsMode.Sprite);
            drawCalls++;

            _spriteBatch.Draw(texture, position, source, color,
                    angle, orgin, scale,
                    spriteEffects ^ (_matrixHandler == null ? SpriteEffects.None : SpriteEffects.FlipVertically), 0);

#if DEBUG
            d_sprite_draws++;
#endif
        }


        //Vector

        public void BeginVector()
        {
            if (_currentMode == AtlasGraphicsMode.Vector &&
                _lastBlendState == blendState &&
                _lastSamplerState == samplerState)
                return;

            _lastBlendState = blendState;
            _lastSamplerState = samplerState;

            Flush();
            _currentMode = AtlasGraphicsMode.Vector;

            if (_matrixHandler != null) 
                _basicEffect.Projection = _matrixHandler.GetEffectMatrix();
            else
                _basicEffect.Projection =
                    Matrix.CreateOrthographicOffCenter(0, ResolutionWidth, ResolutionHeight, 0, -100, 100);

            //_basicEffect.World = Matrix.CreateTranslation(10, 10, 0);

            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = true;
            _graphicsDevice.BlendState = blendState;
            _graphicsDevice.DepthStencilState = DepthStencilState.None;
            _graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            _graphicsDevice.SamplerStates[0] = samplerState;

            indexCounter = 0;
        }

        int indexCounter;
        VertexPositionColorTexture[][] batch;
        int[] batchPrimitiveCount;
        Texture2D currentTexture;
        PrimitiveType primitiveType = PrimitiveType.TriangleStrip;

        public void SetPrimitiveType(PrimitiveType primitiveType)
        {
            if (this.primitiveType != primitiveType)
            {
                this.primitiveType = primitiveType;
                DrawVectorBatch();
            }
        }

        private void DrawVectorBatch()
        {
            if (indexCounter == 0)
                return;

            _basicEffect.TextureEnabled = (currentTexture != null);
            _basicEffect.Texture = currentTexture;

            foreach (EffectPass e in _basicEffect.CurrentTechnique.Passes)
            {
                e.Apply();
                for (int i = 0; i < indexCounter; i++)
                {
                    _graphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(primitiveType,
                                                                                batch[i], 0, batchPrimitiveCount[i]);
                }
            }

            indexCounter = 0;
        }


        public void DrawVector(VertexPositionColorTexture[] vertex)
        {
            DrawVector(vertex, null);
        }

        public void DrawVector(VertexPositionColorTexture[] vertex, Texture2D texture)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                    if (vertex.Length / 2 > 0)
                        DrawVector(vertex, texture, vertex.Length / 2);
                    break;
                case PrimitiveType.LineStrip:
                    if (vertex.Length -1 > 0)
                        DrawVector(vertex, texture, vertex.Length - 1);
                    break;
                case PrimitiveType.TriangleList:
                    if (vertex.Length / 3 > 0)
                        DrawVector(vertex, texture, vertex.Length / 3);
                    break;
                case PrimitiveType.TriangleStrip:
                    if (vertex.Length - 2 > 0)
                        DrawVector(vertex, texture, vertex.Length - 2);
                    break;
            }
        }

        private void DrawVector(VertexPositionColorTexture[] vertex, int primitiveCount)
        {
            DrawVector(vertex, null, primitiveCount);
        }

        private void DrawVector(VertexPositionColorTexture[] vertex, Texture2D texture, int primitiveCount)
        {
            BeginVector();

            if (currentTexture != texture)
                DrawVectorBatch();

            currentTexture = texture;

            batch[indexCounter] = vertex;
            batchPrimitiveCount[indexCounter] = primitiveCount;

            indexCounter++;
            if (indexCounter == batch.Length)
                DrawVectorBatch();
        }

        public enum AtlasGraphicsMode
        {
            None,
            Sprite,
            Vector
        }

        public interface MatrixHandler
        {
            Matrix GetSpriteBatchMatrix();
            Matrix GetEffectMatrix();
            Viewport GetViewPort();
            RasterizerState GetSpriteBatchRasterState();
        }

    }


}
