using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace CH11___Storage
{
    public class GameStorage
    {
        private static StorageContainer OpenContainer(StorageDevice storageDevice, string saveGameName)
        {
            IAsyncResult result = storageDevice.BeginOpenContainer(saveGameName, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = storageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            return container;
        }

        // get the title of this game from its assembly info
        public static string GameName
        {
            get
            {
                // get a reference to this assembly
                Assembly asm = Assembly.GetExecutingAssembly();

                // grab a reference to the title attribute
                AssemblyTitleAttribute ata =
                    (AssemblyTitleAttribute)asm.GetCustomAttributes(
                    typeof(AssemblyTitleAttribute), false)[0];

                // return the title of the assembly. you can change this
                // by editing the AssemblyInfo.cs class within your game
                // project.
                return ata.Title.Replace(":", "_").Replace("\\", "_");
            }
        }


        // save game data to a storage device
        public static void Save(StorageDevice device, GameData data, string name)
        {
            // get a container reference (hard drive or memory card)
            //StorageContainer container = device.OpenContainer(GameName);
            StorageContainer container = OpenContainer(device, GameName);

            // build the filename for the save game
            //string filename = Path.Combine(container.Path, name + ".xml");

            // open the file and write our game data
            //FileStream stream = File.Open(filename, FileMode.Create);
            //XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            //serializer.Serialize(stream, data);
            //stream.Close();

            using (Stream stream = container.OpenFile(name + ".xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                serializer.Serialize(stream, data);
                stream.Close();
            }

            // dispose of the container. data isn't truely saved until
            // you make this call. if you forget to call dispose, your
            // game will have issues if you try to load or save again.
            container.Dispose();
        }

        // load game data from a storage device
        public static GameData Load(StorageDevice device, string name)
        {
            // return a new game data object on failure
            GameData data = new GameData();

            // get a container reference (hard drive or memory card)
            //StorageContainer container = device.OpenContainer(GameName);
            StorageContainer container = OpenContainer(device, GameName);

            // build the filename for the save game
            //string filename = Path.Combine(container.Path, name + ".xml");
            string filename = name + ".xml";

            // only attempt a load if we know the file is actually there
            //if (File.Exists(filename))
            //{
            //    // open the file and read our game data
            //    FileStream stream = File.Open(filename, FileMode.Open);
            //    XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            //    data = (GameData)serializer.Deserialize(stream);
            //    stream.Close();
            //}
            if (container.FileExists(filename))
            {
                using (Stream stream = container.OpenFile(filename, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                    data = (GameData)serializer.Deserialize(stream);
                    stream.Close();
                }
            }

            // dispose of the container. if you forget to call dispose, 
            // your game will have issues if you try to load or save again.
            container.Dispose();

            // return the game data object
            return data;
        }
    }
}
