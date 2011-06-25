using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Copulation.Models;
using Copulation.Presenters;
using Copulation.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Copulation.Tests
{
    [TestClass]
    public class PluginManagerPresenterTests
    {
        [TestMethod]
        public void EnableUnloadOnPluginSelection()
        {
            var mocks = new MockRepository();

            var view = mocks.DynamicMock<IPluginManagerView>();
            var model = mocks.DynamicMock<IPluginManagerModel>();

            Expect.Call(() => model.Subscribe(view));
            Expect.Call(view.Notify).IgnoreArguments();
            
            SetupResult.For(view.UnloadEnabled).PropertyBehavior();
            SetupResult.For(view.SelectedPluginIndex).PropertyBehavior();

            view.SelectedPluginIndex = 0;

            var onPluginSelectEvent = view.GetEventRaiser(v => v.OnPluginSelectEvent += null);

            mocks.ReplayAll();

            new PluginManagerPresenter(view, model);
            
            onPluginSelectEvent.Raise();

            Assert.IsTrue(view.UnloadEnabled);

            mocks.VerifyAll();
        }

        [TestMethod]
        public void DisableUnloadOnPluginUnselection()
        {
            IPluginManagerModel model;
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out model, out view);

            SetupResult.For(view.UnloadEnabled).PropertyBehavior();
            SetupResult.For(view.SelectedPluginIndex).PropertyBehavior();
            
            view.SelectedPluginIndex = -1;

            var onPluginSelectEvent = view.GetEventRaiser(v => v.OnPluginSelectEvent += null);

            mocks.ReplayAll();

            new PluginManagerPresenter(view, model);

            onPluginSelectEvent.Raise();

            Assert.IsFalse(view.UnloadEnabled);

            mocks.VerifyAll();
        }

        [TestMethod]
        public void ClearFilterTextOnClearButtonPressed()
        {
            IPluginManagerModel model;
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out model, out view);

            Expect.Call(view.ClearFilterText);
            Expect.Call(() => model.SetPluginFilterText(string.Empty));

            var onClearFilterEvent = view.GetEventRaiser(v => v.OnClearFilterEvent += null);

            mocks.ReplayAll();

            new PluginManagerPresenter(view, model);

            onClearFilterEvent.Raise();

            mocks.VerifyAll();
        }

        [TestMethod]
        public void DisplayPluginInformationOnPluginSelection()
        {
            IPluginManagerModel model;
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out model, out view);

            Expect.Call(model.GetPluginDetailsBy(0)).Return(null);
            Expect.Call(() => view.DisplayPluginDetails(null));

            SetupResult.For(view.SelectedPluginIndex).PropertyBehavior();
            SetupResult.For(view.UnloadEnabled).PropertyBehavior();

            view.SelectedPluginIndex = 0;

            var onPluginSelectEvent = view.GetEventRaiser(v => v.OnPluginSelectEvent += null);

            mocks.ReplayAll();

            new PluginManagerPresenter(view, model);

            onPluginSelectEvent.Raise();

            mocks.VerifyAll();
        }

        [TestMethod]
        public void LoadPluginAndDisplayInformation()
        {
            IPluginManagerModel model;
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out model, out view);

            string path = string.Empty;

            Expect.Call(view.ShowSelectPluginPath(out path)).Return(true).OutRef(new object[] {string.Empty});
            Expect.Call(model.LoadPluginFrom(path)).Return(1);
            Expect.Call(model.GetPluginDetailsBy(1)).Return(null);
            
            SetupResult.For(view.SelectedPluginIndex).PropertyBehavior();

            Expect.Call(() => view.DisplayPluginDetails(null));

            var onLoadEvent = view.GetEventRaiser(v => v.OnLoadEvent += null);

            mocks.ReplayAll();

            new PluginManagerPresenter(view, model);

            onLoadEvent.Raise();

            mocks.VerifyAll();
        }

        [TestMethod]
        public void UpdatePluginListOnFilterTextChange()
        {
            IPluginManagerModel model;
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out model, out view);

            SetupResult.For(view.FilterText).PropertyBehavior();
            view.FilterText = "pattern";

            Expect.Call(() => model.SetPluginFilterText(view.FilterText));

            var onFilterChangeEvent = view.GetEventRaiser(v => v.OnFilterChangeEvent += null);

            mocks.ReplayAll();

            new PluginManagerPresenter(view, model);

            onFilterChangeEvent.Raise();

            mocks.VerifyAll();
        }

        private static MockRepository PrepareViewModelMocks(out IPluginManagerModel model, out IPluginManagerView view)
        {
            var mocks = new MockRepository();
            
            view = mocks.DynamicMock<IPluginManagerView>();
            model = mocks.DynamicMock<IPluginManagerModel>();

            var m = model;
            var v = view;

            Expect.Call(() => m.Subscribe(v));
            Expect.Call(view.Notify).IgnoreArguments();
            
            return mocks;
        }

    }
}
