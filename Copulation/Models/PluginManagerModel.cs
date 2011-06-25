using System;
using System.Collections.Generic;
using System.Linq;

namespace Copulation.Models
{
    using Common.Configuration;
    using Models.Configuration;
    using Models.Exceptions;
    using Plugins;

    public class PluginManagerModel : BaseModel, IPluginManagerModel
    {
        private readonly IPluginLoader _pluginLoader;
        private readonly IDictionary<Guid, IActionPluginDetails> _actionPlugins;
        
        private string _filterText;

        public PluginManagerModel(IPluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader;
            _actionPlugins = new Dictionary<Guid, IActionPluginDetails>();
        }

        public void LoadConfiguration(GlobalSettingsSection config)
        {
            //TODO: Out that list.
            var loadErrors = new List<string>();

            foreach (var info in config.RegisteredPlugins)
            {
                try
                {
                    _actionPlugins.Add(info.Id, GetActionPluginDetails(info));
                }
                catch (LoadPluginException e)
                {
                    loadErrors.Add(e.Message);
                }
            }

            //TODO: Return true if OK, or false if any of errors occurred.
        }

        public void SaveConfiguration(GlobalSettingsSection config)
        {
            config.RegisteredPlugins.RemoveAll();

            foreach (var details in _actionPlugins.Values)
            {
                var pluginInfo = config.RegisteredPlugins.Add();

                pluginInfo.Id = details.Id;
                pluginInfo.PluginName = details.Name;
                pluginInfo.PluginFileNme = details.FileName;
            }
        }

        private IActionPluginDetails GetActionPluginDetails(PluginInfoSection pluginInfo)
        {
            var details = _pluginLoader.LoadFile(pluginInfo.PluginFileNme);


            if (details.Id == pluginInfo.Id)
            {
                details.Enabled = pluginInfo.Enabled;

                return details;
            }

            throw new LoadPluginException(string.Format("Wrong plugin ID: (Configuration){0} != (Loaded){1}",
                                            pluginInfo.Id,
                                            details.Id));
        }

        #region Implementation of IPluginManagerModel

        public IActionPluginDetails[] GetPluginsDetails()
        {
            return _actionPlugins
                .Where(ap => string.IsNullOrEmpty(_filterText) || IsMatchingToFilter(ap.Value.ActionPlugin.Name))
                .Select(ap => ap.Value)
                .ToArray();
        }

        private bool IsMatchingToFilter(string key)
        {
            return !string.IsNullOrEmpty(key) && key.Contains(_filterText);
        }

        public void SetPluginFilterText(string filterText)
        {
            _filterText = filterText ?? string.Empty;
            
            NotifyObservers();
        }

        public IActionPluginDetails GetPluginDetailsBy(int index)
        {
            var all = GetPluginsDetails();

            if (index < 0 || index >= all.Length)
                throw new IndexOutOfRangeException();

            return all[index];
        }

        public int LoadPluginFrom(string path)
        {
            try
            {
                var actionPluginDetails = _pluginLoader.LoadFile(path);

                var id = actionPluginDetails.ActionPlugin.Id;

                if (_actionPlugins.ContainsKey(id))
                {
                    _actionPlugins[id] = actionPluginDetails;
                }
                else
                {
                    _actionPlugins.Add(id, actionPluginDetails);
                }

                var all = GetPluginsDetails();
                var detailsCouldBeDisplayed = all.FirstOrDefault(ap => ap.ActionPlugin.Id == id);

                NotifyObservers();

                return detailsCouldBeDisplayed != null
                           ? all.ToList().IndexOf(detailsCouldBeDisplayed)
                           : -1;
            }
            catch (Exception e)
            {
                throw new LoadPluginException(e);
            }
        }

        public bool UnloadPluginBy(int index)
        {
            var all = GetPluginsDetails();

            if (index < 0 || index >= all.Length)
                throw new IndexOutOfRangeException();

            var details = all[index];
            
            //TODO: Release Action Plugin resources.

            var result = _actionPlugins.Remove(details.ActionPlugin.Id);

            if (result)
            {
                NotifyObservers();
            }

            return result;
        }

        #endregion
    }
}