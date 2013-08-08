using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;
using AtlasEngine.Infrastructure;
using AtlasEngine.BasicManagers;

namespace AtlasEngine.BasicManagers.Particle
{
    public class Emitter : AtlasEntity
    {
        private IParticleDelegate particleProcessor;
        public IParticleDelegate Delegate
        {
            get { return particleProcessor; }
        }

        private bool alive;

        private List<Part> particles;

        public Emitter(AtlasGlobal atlas, IParticleDelegate particleProcessor)
            : base(atlas)
        {
            alive = false;

            this.particleProcessor = particleProcessor;
            particles = new List<Part>();
        }

        public void Update()
        {
            if (!alive) return;

            alive = false;

            foreach (Part p in particles)
            {
                if (p.alive)
                {
                    alive = true;

                    particleProcessor.Update(p);
                }
            }
        }

        public void Draw(int pass)
        {
            if (!alive || !particleProcessor.DrawOnPass(pass)) return;

            foreach (Part p in particles)
                if(p.alive)
                    particleProcessor.Draw(p);
        }

        public void Emit(Vector2 position, int count)
        {
            for (int i = 0; i < count; i++)
                Emit(position);
        }

        public void Kill()
        {
            foreach (Part p in particles)
                p.alive = false;
        }

        public void Emit(Vector2 position)
        {
            alive = true;

            foreach (Part p in particles)
            {
                if (!p.alive)
                {
                    particleProcessor.Emit(p, position);
                    return;
                }
            }

            Part pt = new Part(Atlas, particleProcessor);
            particles.Add(pt);
            particleProcessor.Emit(pt, position);
        }
    }
}
