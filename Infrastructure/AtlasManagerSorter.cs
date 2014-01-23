using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasEngine.Infrastructure
{
    public class AtlasManagerSorter
    {
        public string arg;

        public bool draw;
        public bool update;

        public int drawWeight;
        public int updateWeight;

        public IAtlasManager manager;

        public int pass;

        public AtlasManagerSorter(IAtlasManager manager, string arg, int updateWeight, int drawWeight)
        {
            this.manager = manager;
            this.arg = arg;

            draw = (drawWeight >= 0);
            update = (updateWeight >= 0);

            this.drawWeight = drawWeight;
            this.updateWeight = updateWeight;
            pass = 0;
        }


        public static int SortByDraw(AtlasManagerSorter p1, AtlasManagerSorter p2)
        {
            return p1.drawWeight.CompareTo(p2.drawWeight);
        }

        public static int SortByUpdate(AtlasManagerSorter p1, AtlasManagerSorter p2)
        {
            return p1.updateWeight.CompareTo(p2.updateWeight);
        }
    }
}
