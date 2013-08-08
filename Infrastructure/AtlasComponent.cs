using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if MONOGAME
using Microsoft.Xna.Framework.Input.Touch;
#endif

using AtlasEngine;

namespace AtlasEngine.Infrastructure
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AtlasComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Dictionary<string, AtlasManager> managers;
        public Dictionary<string, AtlasManager> Manager { get { return managers; } }

        private List<AtlasManagerSorter> managerDrawSorter;
        private List<AtlasManagerSorter> managerUpdateSorter;

        private bool dirty;
        private AtlasGlobal atlas;
        public AtlasGlobal Atlas { get { return atlas; } }

        private IAtlasGamePage gamePage;
        private bool drawThisFrame;

        public AtlasComponent(IAtlasGamePage gamePage, GraphicsDeviceManager graphicsDeviceManager)
            : base(gamePage.Game)
        {
            this.gamePage = gamePage;

            managers = new Dictionary<string, AtlasManager>();
            managerDrawSorter = new List<AtlasManagerSorter>();
            managerUpdateSorter = new List<AtlasManagerSorter>();
            dirty = true;

            atlas = new AtlasGlobal(this, graphicsDeviceManager);
        }

        public void AddManager(AtlasManagerSorter sorter)
        {
            dirty = true;

            if (!managers.ContainsValue(sorter.manager))
                managers.Add(sorter.manager.GetType().AssemblyQualifiedName, sorter.manager);

            if (sorter.draw)
                managerDrawSorter.Add(sorter);

            if (sorter.update)
                managerUpdateSorter.Add(sorter);
        }

        private void Sort()
        {
            if (dirty) {
                managerDrawSorter.Sort(AtlasManagerSorter.SortByDraw);
                managerUpdateSorter.Sort(AtlasManagerSorter.SortByUpdate);

                managerDrawSorter.Reverse();
                managerUpdateSorter.Reverse();

                for (int i = 1; i < managerDrawSorter.Count; i++)
                {
                    managerDrawSorter[i].pass = 0;

                    for (int j = 0; j < i; j++)
                        if (managerDrawSorter[i].manager == managerDrawSorter[j].manager)
                            managerDrawSorter[i].pass++;

                }

                    dirty = false;
            }
        }
        public virtual void Restart(bool force)
        {
            foreach (AtlasManager m in Manager.Values)
                m.Restart(force);
        }

        public sealed override void Update(GameTime gameTime){

            drawThisFrame = gamePage.Active && Atlas.Content.QueueCount() == 0;

            Atlas.Content.LoadQueue();

            if (!drawThisFrame)
                return;

            atlas.Update(gameTime);

            PreUpdate();

            Sort();
            foreach (AtlasManagerSorter ms in managerUpdateSorter)
                ms.manager.Update(ms.arg);

            PostUpdate();

            base.Update(gameTime);
        }

        public virtual void PreUpdate() { }
        public virtual void PostUpdate(){}
        public virtual void PreDraw(){}
        public virtual void PostDraw(){}

        public sealed override void Draw(GameTime gameTime){

            if (!drawThisFrame)
                return;

            atlas.Draw(gameTime);

            PreDraw();

            Sort();

            foreach (AtlasManagerSorter ms in managerDrawSorter)
            {
                ms.manager.Draw(ms.pass);
            }
                

            atlas.Graphics.Flush();
            PostDraw();
            atlas.Graphics.Flush();


            base.Draw(gameTime);
        }

        public virtual void Resume()
        {
        }

        public virtual void GotoIdle()
        {
        }

        public virtual void Pause()
        {
        }
    }
}
