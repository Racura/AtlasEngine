using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtlasEngine.BasicManagers.Particle
{
    public interface IParticleDelegate
    {
        int VectorCount{get;}
        void Emit(Part particle, Vector2 position);
        void Update(Part particle);
        void Draw(Part particle);

        bool DrawOnPass(int pass);
    }

    public class ParticleDelegate : AtlasEntity, IParticleDelegate
    {
        public ParticleDelegate(AtlasGlobal atlas) : base(atlas) {
        }

        public virtual int VectorCount {
            get
            {
                return 0;
            }
        }

        public virtual void Emit(Part particle, Vector2 position)
        {
            particle.Reset();
            particle.position = position;
            particle.life = 1;
        }

        public virtual void Update(Part particle)
        {
            particle.life -= Atlas.Elapsed;
            particle.alpha = Math.Min(1, particle.life);

            if (particle.life < 0)
                particle.alive = false;
        }

        public virtual void Draw(Part particle)
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
            return pass == 0;
        }
    }
}
