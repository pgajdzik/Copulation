using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copulation.Models.Configuration
{
    using Common.Configuration;

    public class GlobalSettingsSection : Section
    {
        [ConfigProperty(DefaultValue = ".\\Plugins")]
        public string PlugInPath
        {
            get; set;
        }

        [ConfigProperty]
        public SectionCollection<PluginInfoSection> RegisteredPlugins
        {
            get { return GetSectionCollection<PluginInfoSection>("RegisteredPlugIns"); }
        }


        [ConfigProperty]
        public StartupSettings StartupSettings
        {
            get { return GetSection<StartupSettings>("StartupSettings"); }
        }

        [ConfigProperty]
        public DateTime LastModification
        {
            get;
            set;
        }
    }

    public class StartupSettings : Section
    {
        [ConfigProperty(DefaultValue = "False")]
        public bool AutostartWithWindows
        {
            get;  set;
        }
    }

}

