using Copulation.Plugins;

namespace Copulation.Models
{
    public interface IPluginLoader
    {
        IActionPluginDetails LoadFile(string path);
    }
}