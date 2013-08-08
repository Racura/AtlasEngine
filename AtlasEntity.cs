using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasEngine
{
    public class AtlasEntity
    {
        private AtlasGlobal a;
        protected AtlasGlobal Atlas { get { return a; } }

        public AtlasEntity(AtlasGlobal atlas)
        {
            a = atlas;
        }
    }
}
