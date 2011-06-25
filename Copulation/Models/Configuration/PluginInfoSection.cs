using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copulation.Models.Configuration
{
    using Common.Configuration;

    public class PluginInfoSection : Section
    {
        [ConfigProperty]
        public Guid Id { get; set; }

        [ConfigProperty]
        public string PluginName { get; set; }

        [ConfigProperty]
        public string PluginFileNme { get; set; }

        [ConfigProperty]
        public bool Enabled { get; set; }
    }
}
