using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine.BasicManagers.Particle;
using AtlasEngine;

namespace AtlasEngine.BasicManagers.Particle.ParticleDelegates
{
    public class TrailParticleDelegate : AtlasEntity, IParticleDelegate
    {
        public virtual int VectorCount
        {
            get
            {
                return 1;
            }
        }

        public Color color;
        private Vector2 _accelertation;
        private float _totalLife;

        public float Gravity { set { _accelertation.Y = value; } get { return _accelertation.Y; } }
        public float Wind { set { _accelertation.X = value; } get { return _accelertation.X; } }

        public RangeF strength;
        public float wantedAngle;
        public float wantedAngleWeight;

        public TrailParticleDelegate(AtlasGlobal atlas, float totalLife)
            : base(atlas)
        {
            _totalLife = totalLife;
            color = Color.White;

            Atlas.Content.LoadContent("part");
        }

        public void Emit(Part particle, Vector2 position)
        {
            particle.Reset();
            particle.position = position;

            float angle = Atlas.Rand * (float)Math.PI * 2;

            wantedAngle = wantedAngle % ((float)Math.PI * 2);
            if (wantedAngle < 0) wantedAngle = wantedAngle + (float)Math.PI * 2;

            while (Math.Abs(angle - wantedAngle) > (float)Math.PI) angle = angle + Math.Sign(wantedAngle - angle) * (float)Math.PI * 2;

            float power = 1 - (Math.Abs(angle - wantedAngle) / ((float)Math.PI)) * wantedAngleWeight;

            particle.vectors[0] = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle))
                * strength.ToDivision(Atlas.Rand * power);

            particle.angle = angle;
            particle.color = color;

            particle.life = _totalLife;
        }

        public void Update(Part particle)
        {
            particle.position += particle.vectors[0] * Atlas.Elapsed;

            particle.life -= Atlas.Elapsed;

            particle.color = color;

            particle.alpha = particle.life;
            particle.scale = 2 - (particle.life / _totalLife) * (particle.life / _totalLife);

            if (particle.life < 0)
                particle.alive = false;
        }

        public Vector2 Drag()
        {
            return Vector2.One;
        }

        public Vector2 Acceleration()
        {
            return _accelertation;
        }


        public void Draw(Part particle)
        {
            Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>("part"),
                                        particle.position, null, Color.White * particle.alpha, Vector2.One * 2,
                                        particle.angle, particle.scale, false);
        }


        public virtual bool DrawOnPass(int pass)
        {
            return pass == 1;
        }
    }
}
