using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;
using AtlasEngine.Infrastructure;
using AtlasEngine.BasicManagers.Particle;

namespace AtlasEngine.BasicManagers
{
    public class ParticleManager : AtlasManager
    {
        public Dictionary<string, Emitter> emitters;

        public ParticleManager(AtlasGlobal atlas)
            : base(atlas)
        {
            emitters = new Dictionary<string, Emitter>();
        }

        public override void Update(string arg)
        {
            foreach(Emitter e in emitters.Values)
                e.Update();
        }

        public override void Draw(int pass)
        {
            foreach (Emitter e in emitters.Values)
                e.Draw(pass);
        }

        public bool Exists(string name)
        {
            return emitters.ContainsKey(name);
        }

        public void AddEmitter(string name, IParticleDelegate pp)
        {
            emitters.Add(name, new Emitter(Atlas, pp));
        }

        public IParticleDelegate GetDelegate(string name)
        {
            return emitters[name].Delegate;
        }

        public void Emit(string name, Vector2 position)
        {
            emitters[name].Emit(position);
        }

        public void Emit(string name, Vector2 position, int count)
        {
            emitters[name].Emit(position, count);
        }

        public override void Restart(bool force)
        {
            if (force)
                foreach (Emitter e in emitters.Values)
                    e.Kill();

            base.Restart(force);
        }
    }
}
