using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace AtlasEngine.BasicManagers.Particle
{
    public class Part : AtlasEntity
    {

        public Vector2 position;

        public float angle;
        public float scale;
        public float alpha;
        public float life;
        
        public Vector2[] vectors;

        public IParticleDelegate processor;

        public int frame;
        public float born;

        public bool alive;
        public Color color;

        public int tag;


        public Part(AtlasGlobal atlas, IParticleDelegate processor)
            : base(atlas)
        {
            this.processor = processor;

            vectors = new Vector2[processor.VectorCount];
        
            position = Vector2.Zero;
            angle = 0;
            scale = 1;

            alive = false;
        }

        public void Reset()
        {
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = Vector2.Zero;

            color = Color.White;

            alive = true;
            born = Atlas.TotalTime;

            this.position = Vector2.Zero;

            tag = (int)(Atlas.Rand * 1024);
        }
    }
}
