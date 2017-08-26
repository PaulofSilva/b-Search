namespace libbSearch.Plugin
{
    /// <summary>
    /// Wrapper class for IbSearchPlugins.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class PluginWrapper
    {

        #region Declarations

        #endregion

        /// <summary>
        /// Initializes a new instance of the PluginWrapper class.
        /// </summary>

        public PluginWrapper()
        {
            AssemblyName = string.Empty;
            AssemblyPath = string.Empty;
            Internal = false;
            Enabled = false;
            Index = -1;
        }

        /// <summary>
        /// Initializes a new instance of the PluginWrapper class.
        /// </summary>
        /// <param name="plugin">IbSearchPlugin</param>
        /// <param name="assemblyPath">Fully qualified file path</param>
        /// <param name="assemblyName">Name of assembly</param>
        /// <param name="internalPlugin">If true the plugin is internal, False is external.</param>
        /// <param name="enabled">If true the plugin is enabled, False is disabled.</param>

        public PluginWrapper(IbSearchPlugin plugin, string assemblyPath,
                             string assemblyName, bool internalPlugin, bool enabled, int index)
        {
            Plugin = plugin;
            AssemblyPath = assemblyPath;
            AssemblyName = assemblyName;
            Internal = internalPlugin;
            Enabled = enabled;
            Index = index;
        }

        /// <summary>
        /// Contains the IbSearchPlugin.
        /// </summary>

        public IbSearchPlugin Plugin { get; set; }

        /// <summary>
        /// Determines whether the plugin is enabled or disabled.
        /// </summary>

        public bool Enabled { get; set; }

        /// <summary>
        /// Determines whether the plugin is an Internal or External plugin.
        /// </summary>

        public bool Internal { get; set; }

        /// <summary>
        /// Contains the fully qualified path to the plugin.
        /// </summary>

        public string AssemblyPath { get; set; }

        /// <summary>
        /// Contains the full assembly name of the plugin.
        /// </summary>

        public string AssemblyName { get; set; }

        /// <summary>
        /// Index of plugin in collection.
        /// </summary>

        public int Index { get; set; }
    }
}