using System;
using Microsoft.Xna.Framework;

namespace AtlasEngine.Infrastructure
{
    public interface IAtlasGamePage
    {
        void NavigateTo(string arg);
        bool Active { get; set; }
        AtlasGame Game { get; }
    }
}
