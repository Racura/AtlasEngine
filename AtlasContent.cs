using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AtlasEngine
{
    public class AtlasContent
    {
        public delegate object AtlasDynamicContentBuilder(GraphicsDevice graphicsDevice, string name);

        private const string EMPTY_CONTENT = "oneemptypixel";

        private AtlasGlobal atlas;

        private ContentManager contentManager;
        private GraphicsDevice graphicsDevice;

        private List<string> queue;
        private Dictionary<string, object> assets;
        private Dictionary<string, AtlasDynamicContentBuilder> dynamicContentBuilders;

        public AtlasContent(AtlasGlobal atlas)
            : base()
        {
            this.atlas = atlas;
            queue = new List<string>();
            dynamicContentBuilders = new Dictionary<string, AtlasDynamicContentBuilder>();
            assets = new Dictionary<string, object>();
            graphicsDevice = atlas.Game.GraphicsDevice;

            contentManager = atlas.Game.Content;
        }

        public int QueueCount()
        {
            return queue.Count;
        }

        public void LoadDynamicContent(string location, AtlasDynamicContentBuilder adcb)
        {
            if (assets.ContainsKey(location) || queue.Contains(location))
                return;

            queue.Add(location);
            dynamicContentBuilders.Add(location, adcb);
        }

        public void LoadContent(string location)
        {
            if (assets.ContainsKey(location) || queue.Contains(location))
                return;

            queue.Add(location);
        }

        internal void LoadQueue()
        {
            for (int i = 0; i < queue.Count; i++)
            {
                object output = null;
                try{
                    if (dynamicContentBuilders.ContainsKey(queue[i]))
                        output = dynamicContentBuilders[queue[i]](graphicsDevice, queue[i]);
                    else
                        output = contentManager.Load<object>(queue[i]);
                }catch(Exception e){
                    e.ToString();
                }

                if (output != null)
                {
                    assets.Add(queue[i], output);

                    queue.RemoveAt(i);
                    i--;
                }
            }
        }

        public T GetContent<T>(string location) where T : class
        {
            if (assets.ContainsKey(location) )
                return assets[location] as T;
            return GetNull<T>() as T;
        }

        public object GetNull<T>() where T : class
        {
            string name = typeof(T).Name + EMPTY_CONTENT;

            if (assets.ContainsKey(name))
                return assets[name];

            object output = null;

            switch (typeof(T).Name)
            {
                case "Texture2D":
                    int size = 32;
                    Texture2D texture2d = new Texture2D(graphicsDevice, size, size);
                    Color[] c = new Color[size * size];
                    for (int i = 0; i < c.Length; i++)
                        c[i] = Color.White;
                    texture2d.SetData<Color>(c);
                    output = texture2d;
                    break;
                default:
                    throw new Exception("Unsupported Null type");
            }

            assets.Add(name, output);

            return output;
        }
    }

    
}
