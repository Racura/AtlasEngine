using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasEngine
{
    public interface IAtlasManager
    {
        void Initialize();
        void Update(string arg);
        void Draw(int pass);
        void Restart(bool force);
    }
}
