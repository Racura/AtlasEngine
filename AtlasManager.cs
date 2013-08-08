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

        public virtual void Update(string arg)
        {
        }

        public virtual void Draw(int pass)
        {

        }

        public virtual void Restart(bool force)
        {
        }
    }
}
