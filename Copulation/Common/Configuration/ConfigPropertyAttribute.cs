using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copulation.Common.Configuration
{
    public class ConfigPropertyAttribute : Attribute
    {
        public string DefaultValue { get; set; }
    }
}
