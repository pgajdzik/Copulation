using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copulation.Plugins
{
    public interface IActionPlugin
    {
        Guid Id { get; }
        
        string Name { get; }

        string ShortDescription { get; }
    }
}
