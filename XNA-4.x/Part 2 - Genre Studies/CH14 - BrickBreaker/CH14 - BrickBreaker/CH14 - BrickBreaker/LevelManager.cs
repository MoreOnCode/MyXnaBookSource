using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace CH14___BrickBreaker
{
    public class LevelManager
    {
        // the collection of levels for the game
        protected static List<LevelData> m_Levels = new List<LevelData>();

        // read-only count of levels
        public static int LevelCount
        {
            get { return m_Levels.Count; }
        }

        // get LevelData, based on game level
        // repeat when we reach the end of the list
        public static LevelData GetLevel(int number)
        {
            return m_Levels[Math.Abs(number) % m_Levels.Count];
        }

        // load level data from a game project file
        public static void LoadProject(string filename)
        {
            // check for null filename
            if (string.IsNullOrEmpty(filename)) { return; }

            // prepend the app's path
            //string path = StorageContainer.TitleLocation + "\\" + filename;

            // if the file exists, process it
            if (!string.IsNullOrEmpty(filename))
            {
                try
                {
                    // local variable to read file, line-by-line
                    string line = null;

                    // clear any existing level data from our 
                    // in-memory collection
                    m_Levels.Clear();

                    using (Stream stream = TitleContainer.OpenStream(filename))
                    {
                        // open the file
                        StreamReader reader = new StreamReader(stream);
                            //new StreamReader(
                            //    new FileStream(path, FileMode.Open));

                        // for every line in the file, read it in,
                        // create a level from the data, and add the
                        // newly-created level to our collection
                        while ((line = reader.ReadLine()) != null)
                        {
                            m_Levels.Add(new LevelData(line));
                        }

                        // after reading all the lines, close the file
                        reader.Close();
                        stream.Close();
                    }
                }
                catch { }
            }
        }
    }
}
