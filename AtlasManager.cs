using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasEngine
{
    public abstract class AtlasManager : AtlasEntity
    {
        public AtlasManager(AtlasGlobal atlas)
            : base(atlas)
        {
        }

        public abstract void Update(string arg);

        public abstract void Draw(int pass);

        public abstract void Restart(bool force);
    }
}
