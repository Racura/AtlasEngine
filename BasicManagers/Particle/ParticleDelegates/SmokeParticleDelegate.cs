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
    public class SmokeParticleDelegate : AtlasEntity, IParticleDelegate
    {
        public int VectorCount
        {
            get
            {
                return 2;
            }
        }

        private Vector2 _accelertation;
        private float _totalLife;
        
        public float Gravity { set { _accelertation.Y = value; } get { return _accelertation.Y; } }
        public float Wind { set { _accelertation.X = value; } get { return _accelertation.X; } }

        public RangeF strength;
        public float drag;

        public SmokeParticleDelegate(AtlasGlobal atlas, float totalLife)
            : base(atlas)
        {
            _totalLife = totalLife;
            drag = 1;
            strength = new RangeF();
        }

        public void Emit(Part particle, Vector2 position)
        {
            particle.Reset();
            particle.position = position;

            float angle = (float)(Math.PI * 2 * Atlas.Rand);

            if (strength.Max != 0 || strength.Min != 0)
            {
                particle.vectors[1] = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle))
                    * strength.ToDivision(Atlas.Rand);
            }

            particle.angle = (float)(Math.PI * 2 * Atlas.Rand);
            particle.color = Color.Lerp(Color.White, Color.Gray, Atlas.Rand);

            particle.life = _totalLife;
        }

        public void Update(Part particle)
        {
            particle.vectors[0] += _accelertation * Atlas.Elapsed;
            particle.vectors[1] -= particle.vectors[1] * (drag * Atlas.Elapsed);


            particle.position += (particle.vectors[0] + particle.vectors[1]) * Atlas.Elapsed;

            particle.life -= Atlas.Elapsed;

            particle.alpha = Math.Min(particle.life / _totalLife, (_totalLife - particle.life) / 4);


            particle.scale = 2 - (particle.life / _totalLife) * (particle.life / _totalLife);

            if (particle.life < 0)
                particle.alive = false;
        }


        public void Draw(Part particle)
        {
            if (particle.scale > 0 && particle.alpha > 0)
            {
                Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>("images/debug_arrow"),
                    particle.position, null,
                    particle.color * Math.Min(particle.alpha, 1),
                    new Vector2(16, 16),
                    particle.angle,
                    particle.scale, false);
            }
        }


        public virtual bool DrawOnPass(int pass)
        {
            return pass ==  1;
        }
    }
}
