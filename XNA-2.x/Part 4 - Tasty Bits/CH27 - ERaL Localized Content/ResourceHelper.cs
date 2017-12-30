using System;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LocalizedContent
{
    public class ResourceHelper
    {
#region "Content Managers"

        //// prep this class for use
        //public static void Init(IServiceProvider provider)
        //{
        //    m_ResourceManager = LocalizedContent.Localized.ResourceManager;

        //    m_ContentManager =
        //        new ResourceContentManager(
        //            provider,
        //            ResourceManager);

        //    InitProperties();
        //}

        public static void Init(IServiceProvider provider, CultureInfo culture)
        {
            m_ResourceManager = LocalizedContent.Localized.ResourceManager;

            m_ContentManager =
                new ResourceContentManager(
                    provider,
                    ResourceManager);

            InitProperties(culture);
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

        // used to extract localized content
        protected static CultureInfo m_Culture = null;
        public static CultureInfo Culture
        {
            get { return m_Culture; }
        }

#endregion

#region "Localized Content"

        //// get references to localized content, especially
        //// after a culture change (player pressed Y button)
        //protected static void InitProperties()
        //{
        //    CultureInfo culture = CultureInfo.CurrentUICulture;
        //    InitProperties(culture);
        //}

        //protected static void InitProperties(string name)
        //{
        //    CultureInfo culture = CultureInfo.CreateSpecificCulture(name);
        //    InitProperties(culture);
        //}

        protected static void InitProperties(CultureInfo culture)
        {
            if (ResourceManager != null)
            {
                // get localized content
                m_AllowCrude =
                    ResourceManager.GetString("AllowCrude", culture)
                    .Equals("True", StringComparison.OrdinalIgnoreCase);
                m_FlagPath = ResourceManager.GetString("Flag", culture);
                m_Goodbye = ResourceManager.GetString("Goodbye", culture);
                m_Hello = ResourceManager.GetString("Hello", culture);
                m_HowAreYou = ResourceManager.GetString("HowAreYou", culture);
                m_Language = ResourceManager.GetString("Language", culture);
            }
            else
            {
                // no resource manager? use safe default values
                m_AllowCrude = false;
                m_FlagPath = String.Empty;
                m_Goodbye = String.Empty;
                m_Hello = String.Empty;
                m_HowAreYou = String.Empty;
                m_Language = String.Empty;
            }

            // system-provided culture strings
            m_OfficialLanguage = culture.EnglishName;
            m_NativeLanguage = culture.NativeName;
        }

        // does this culture allow our crude easter egg?
        protected static bool m_AllowCrude = false;
        public static bool AllowCrude { get { return m_AllowCrude; } }

        // content pipeline path to the flag image
        protected static string m_FlagPath = string.Empty;
        public static string FlagPath { get { return m_FlagPath; } }

        // "GoodBye"
        protected static string m_Goodbye = string.Empty;
        public static string Goodbye { get { return m_Goodbye; } }

        // "Hello"
        protected static string m_Hello = string.Empty;
        public static string Hello { get { return m_Hello; } }

        // "How are you?"
        protected static string m_HowAreYou = string.Empty;
        public static string HowAreYou { get { return m_HowAreYou; } }

        // our name for this language
        protected static string m_Language = string.Empty;
        public static string Language { get { return m_Language; } }

        // the proper name of this language
        protected static string m_OfficialLanguage = string.Empty;
        public static string OfficialLanguage
        {
            get { return m_OfficialLanguage; }
        }

        // the proper name of this language, translated
        protected static string m_NativeLanguage = string.Empty;
        public static string NativeLanguage
        {
            get { return m_NativeLanguage; }
        }

#endregion
    }
}
