// ResourceHelper.cs
using System;
using System.Resources;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace EmbeddedContent
{
    public class ResourceHelper
    {
        // prep this class for use
        public static void Init(IServiceProvider provider)
        {
            m_ResourceManager = EmbeddedContent.Resources.ResourceManager;
            m_ContentManager =
                new ResourceContentManager(
                    provider,
                    ResourceManager);
        }

        // used to extract embedded content
        protected static ResourceContentManager m_ContentManager = null;
        public static ResourceContentManager ContentManager
        {
            get { return m_ContentManager; }
        }

        // used to extract (localized) embedded content
        protected static ResourceManager m_ResourceManager = null;
        public static ResourceManager ResourceManager
        {
            get { return m_ResourceManager; }
        }
    }
}