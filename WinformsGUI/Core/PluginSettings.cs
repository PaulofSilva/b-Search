using System;
using System.IO;

using bSearch.Common;

namespace bSearch.Core
{
    /// <summary>
    /// Used to access the search option settings.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public sealed class PluginSettings
    {
        // This class is fully static.
        private PluginSettings() { }

        #region Declarations
        private static PluginSettings __MySettings = null;

        private const string VERSION = "1.0";

        private string AllPlugins = string.Empty;
        #endregion

        /// <summary>
        /// Contains the static reference of this class.
        /// </summary>
        private static PluginSettings MySettings
        {
            get
            {
                if (__MySettings == null)
                {
                    __MySettings = new PluginSettings();
                    SettingsIO.Load(__MySettings, Location, VERSION);
                }
                return __MySettings;
            }
        }

        /// <summary>
        /// Gets the full location to the config file.
        /// </summary>
        static public string Location
        {
            get
            {
                return Path.Combine(ApplicationPaths.DataFolder, "bSearch.plugins.config");
            }
        }

        /// <summary>
        /// Save the search options.
        /// </summary>
        /// <returns>Returns true on success, false otherwise</returns>
        public static bool Save()
        {
            return SettingsIO.Save(MySettings, Location, VERSION);
        }

        /// <summary>
        /// Gets/Sets the string containing all plugins.
        /// </summary>
        static public string Plugins
        {
            get { return MySettings.AllPlugins; }
            set { MySettings.AllPlugins = value; }
        }
    }
}
