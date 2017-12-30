using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CH15___Puzzle_Game
{
    class CellColors
    {
        // the normal game cell colors
        public static readonly Color[] Normal =
        {
            Color.Black,          // Empty Cell
            Color.DarkOrange,     // Orange
            Color.LawnGreen,      // Green
            Color.CornflowerBlue, // Blue
            Color.Orchid,         // Purple
            Color.OrangeRed,      // Red
            Color.Gold,           // Yellow
        };

        public const int EmptyCellColorIndex = 0;

        // while a row is being added, it's only partially visible so that 
        // the player can see what's coming up and they can plan a few 
        // moves ahead. draw this preview row in a darker shade to set it
        // apart and let the player know that the row isn't playable just
        // yet. Rather than specifying another set of colors, we'll just
        // generate our darker shades from the list of "real" colors.
        public static Color[] Dark = InitDarkCellColors();

        // since this method is marked static, and it's being assigned
        // to a static member variable, you don't need to do anything to 
        // make sure that this parallel array is initialized properly.
        // once this class is loaded from its assembly, this method will
        // be triggered, and the results will be stored in Dark[].
        private static Color[] InitDarkCellColors()
        {
            Dark = new Color[Normal.Length];
            for (int i = 0; i < Normal.Length; i++)
            {
                Dark[i] = new Color(
                    (byte)(Normal[i].R / 2),
                    (byte)(Normal[i].G / 2),
                    (byte)(Normal[i].B / 2));
            }
            return Dark;
        }

        // helper to generate random numbers
        private static Random m_rand = new Random();

        // get the next random color index
        public static int RandomIndex()
        {
            return m_rand.Next(Normal.Length - 1) + 1;
        }
    }
}
