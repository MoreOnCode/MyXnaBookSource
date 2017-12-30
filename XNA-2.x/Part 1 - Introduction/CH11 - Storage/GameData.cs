// GameData.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Chapter11
{
    // mark our game data as serializable so that we can easily
    // persist its contents as an XML data file
    [Serializable]
    public class GameData
    {
        // the list of the player's stamp coordinates
        public List<Vector2> Stamps = new List<Vector2>();
    }
}
