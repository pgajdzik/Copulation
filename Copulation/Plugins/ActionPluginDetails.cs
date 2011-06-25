using System;

namespace Copulation.Plugins
{
    public class ActionPluginDetails : IActionPluginDetails
    {
        #region Implementation of IActionPluginDetails

        public IActionPlugin ActionPlugin { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string FileName { get; set; }

        public bool Enabled { get; set; }

        #endregion
    }
}