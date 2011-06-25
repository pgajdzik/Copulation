using System.Reflection;

namespace Copulation.Models
{
    using Plugins;

    public interface IPluginManagerModel : IModel
    {
        IActionPluginDetails[] GetPluginsDetails();

        void SetPluginFilterText(string filterText);

        IActionPluginDetails GetPluginDetailsBy(int index);

        int LoadPluginFrom(string path);

        bool UnloadPluginBy(int index);
        
    }
}