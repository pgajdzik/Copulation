using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copulation.Views
{
    using Events;
    using Plugins;

    public interface IPluginManagerView : IView
    {
        event ViewEventHandler OnLoadEvent;

        event ViewEventHandler OnUnloadEvent;

        event ViewEventHandler OnClearFilterEvent;

        event ViewEventHandler OnFilterChangeEvent;

        event ViewEventHandler OnFilterSelectEvent;
        
        event ViewEventHandler OnPluginSelectEvent;

        string FilterText { get; set; }

        bool UnloadEnabled { get; set; }

        int SelectedPluginIndex { get; set; }
        
        void ClearFilterText();

        void DisplayPluginDetails(IActionPluginDetails details);

        bool ShowConfirmUnload();
        
        bool ShowSelectPluginPath(out string path);
        
        void ShowErrorMessage(string message);
    }
}
