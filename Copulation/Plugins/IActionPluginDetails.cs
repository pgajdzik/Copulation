using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copulation.Plugins
{
    public interface IActionPluginDetails
    {
        IActionPlugin ActionPlugin { get; }

        Guid Id { get; }

        string Name { get; }

        string ShortDescription { get; }

        string FileName { get; }

        bool Enabled { get; set; }
    }
}
