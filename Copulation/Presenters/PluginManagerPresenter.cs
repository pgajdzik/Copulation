using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Copulation.Models.Exceptions;

namespace Copulation.Presenters
{
	using Models;
	using Views;

	public class PluginManagerPresenter : BasePresenter<IPluginManagerView, IPluginManagerModel>
	{
		public PluginManagerPresenter(IPluginManagerView view, IPluginManagerModel model)
			: base(view, model)
		{
		}

		protected override void AssignEventHandlers()
		{
			View.OnLoadEvent += OnLoadHandler;
			View.OnUnloadEvent += OnUnloadHandler;
			View.OnClearFilterEvent += OnClearFilerHandler;
			View.OnFilterChangeEvent += OnFilterChangeHandler;
			View.OnFilterSelectEvent += OnFilterSelectHandler;
			View.OnPluginSelectEvent += OnPluginSelectHandler;
		}

		protected override void BindModel()
		{
			Model.Subscribe(View);
			View.Notify();
		}

		#region View Events Handlers

		private void OnLoadHandler()
		{
			string path;

			if (View.ShowSelectPluginPath(out path))
			{
				try
				{
					int index = Model.LoadPluginFrom(path);
					var details = Model.GetPluginDetailsBy(index);

					View.SelectedPluginIndex = index;
					View.DisplayPluginDetails(details);
				}
				catch (LoadPluginException e)
				{
					View.ShowErrorMessage(e.Message);
				}
			}
		}

		private void OnUnloadHandler()
		{
			if (View.ShowConfirmUnload())
			{
				Model.UnloadPluginBy(View.SelectedPluginIndex);
			}
		}

		private void OnClearFilerHandler()
		{
			View.ClearFilterText();
            Model.SetPluginFilterText(string.Empty);
		}

		private void OnFilterChangeHandler()
		{
			Model.SetPluginFilterText(View.FilterText);
		}

		private void OnFilterSelectHandler()
		{
			Model.SetPluginFilterText(View.FilterText);
		}

		private void OnPluginSelectHandler()
		{
			var isSelected = View.SelectedPluginIndex != -1;

			if (isSelected)
			{
				var pluginDetails = Model.GetPluginDetailsBy(View.SelectedPluginIndex);
				
				View.DisplayPluginDetails(pluginDetails);
			}

			View.UnloadEnabled = isSelected;
		}

		#endregion    
	}
}
