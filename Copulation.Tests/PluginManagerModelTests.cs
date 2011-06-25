using Copulation.Models;
using Copulation.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Copulation.Plugins;
using Rhino.Mocks;

namespace Copulation.Tests
{
    [TestClass]
    public class PluginManagerModelTests
    {
        [TestMethod]
        public void NotifyViewAfterPluginLoad()
        {
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out view);
            var pluginLoader = mocks.DynamicMock<IPluginLoader>();

            Expect.Call(pluginLoader.LoadFile("dummy.plugin")).Return(new ActionPluginDetails()
                                                                          {
                                                                              ActionPlugin =
                                                                                  mocks.DynamicMock<IActionPlugin>(),
                                                                              Enabled = true
                                                                          });
            Expect.Call(view.Notify).IgnoreArguments();

            mocks.ReplayAll();

            var model = new PluginManagerModel(pluginLoader);

            model.Subscribe(view);

            Assert.AreEqual(model.LoadPluginFrom("dummy.plugin"), 0);

            mocks.VerifyAll();
        }

        [TestMethod]
        public void LoadTwoDifferentPlugins()
        {
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out view);
            var pluginLoader = mocks.DynamicMock<IPluginLoader>();

            var actionPlugin1 = new DummyActionPlugin("Dummy", Guid.Parse("10000000-0000-0000-0000-000000000000"));
            var actionPlugin2 = new DummyActionPlugin("Ummmy", Guid.Parse("20000000-0000-0000-0000-000000000000"));

            Expect.Call(pluginLoader.LoadFile("dummy.plugin")).Return(new ActionPluginDetails()
                                                                          {
                                                                              ActionPlugin =
                                                                                  actionPlugin1,
                                                                              Enabled = true
                                                                          });

            Expect.Call(pluginLoader.LoadFile("dummy2.plugin")).Return(new ActionPluginDetails()
                                                                           {
                                                                               ActionPlugin =
                                                                                   actionPlugin2,
                                                                               Enabled = true
                                                                           });
            Expect.Call(view.Notify).IgnoreArguments();

            mocks.ReplayAll();

            var model = new PluginManagerModel(pluginLoader);
            
            model.Subscribe(view);

            Assert.AreEqual(0, model.LoadPluginFrom("dummy.plugin"));
            Assert.AreEqual(1, model.LoadPluginFrom("dummy2.plugin"));

            mocks.VerifyAll();
        }
        
        [TestMethod]
        public void LoadPluginsAndFilterByName()
        {
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out view);
            var pluginLoader = mocks.DynamicMock<IPluginLoader>();

            var actionPlugin1 = new DummyActionPlugin("Dummy", Guid.Parse("10000000-0000-0000-0000-000000000000"));
            var actionPlugin2 = new DummyActionPlugin("Ummmy", Guid.Parse("20000000-0000-0000-0000-000000000000"));
            var actionPlugin3 = new DummyActionPlugin("Dummmmy", Guid.Parse("30000000-0000-0000-0000-000000000000"));

            Expect.Call(pluginLoader.LoadFile("dummy.plugin")).Return(new ActionPluginDetails()
            {
                ActionPlugin =
                    actionPlugin1,
                Enabled = true
            });

            Expect.Call(pluginLoader.LoadFile("dummy2.plugin")).Return(new ActionPluginDetails()
            {
                ActionPlugin =
                    actionPlugin2,
                Enabled = true
            });

            Expect.Call(pluginLoader.LoadFile("dummy3.plugin")).Return(new ActionPluginDetails()
            {
                ActionPlugin =
                    actionPlugin3,
                Enabled = true
            });

            Expect.Call(view.Notify).IgnoreArguments();

            mocks.ReplayAll();

            var model = new PluginManagerModel(pluginLoader);

            model.Subscribe(view);
            model.SetPluginFilterText("Dumm");

            Assert.AreEqual( 0, model.LoadPluginFrom("dummy.plugin"));
            Assert.AreEqual(-1, model.LoadPluginFrom("dummy2.plugin"));
            Assert.AreEqual( 1, model.LoadPluginFrom("dummy3.plugin"));

            mocks.VerifyAll();
        }

        [TestMethod]
        public void UnloadPluginAndFilterByName()
        {
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out view);
            var pluginLoader = mocks.DynamicMock<IPluginLoader>();

            var actionPlugin1 = new DummyActionPlugin("Dummy", Guid.Parse("10000000-0000-0000-0000-000000000000"));
            var actionPlugin2 = new DummyActionPlugin("Ummmy", Guid.Parse("20000000-0000-0000-0000-000000000000"));
            var actionPlugin3 = new DummyActionPlugin("Dummmmy", Guid.Parse("30000000-0000-0000-0000-000000000000"));

            Expect.Call(pluginLoader.LoadFile("dummy.plugin")).Return(new ActionPluginDetails()
                                                                          {
                                                                              ActionPlugin =
                                                                                  actionPlugin1,
                                                                              Enabled = true
                                                                          });

            Expect.Call(pluginLoader.LoadFile("dummy2.plugin")).Return(new ActionPluginDetails()
                                                                           {
                                                                               ActionPlugin =
                                                                                   actionPlugin2,
                                                                               Enabled = true
                                                                           });

            Expect.Call(pluginLoader.LoadFile("dummy3.plugin")).Return(new ActionPluginDetails()
                                                                           {
                                                                               ActionPlugin =
                                                                                   actionPlugin3,
                                                                               Enabled = true
                                                                           });

            Expect.Call(view.Notify).Repeat.Times(4);

            mocks.ReplayAll();

            var model = new PluginManagerModel(pluginLoader);

            model.Subscribe(view);
            Assert.AreEqual(0, model.LoadPluginFrom("dummy.plugin"));
            Assert.AreEqual(1, model.LoadPluginFrom("dummy2.plugin"));
            Assert.AreEqual(2, model.LoadPluginFrom("dummy3.plugin"));

            model.SetPluginFilterText("Dumm");

            Assert.IsTrue(model.UnloadPluginBy(1));
            

            Assert.AreEqual(1, model.GetPluginsDetails().Length);

            model.SetPluginFilterText(null);

            Assert.AreEqual(2, model.GetPluginsDetails().Length);

            mocks.VerifyAll();
        }

        [TestMethod]
        public void LoadTheSamePluginsWithDifferentPathNames()
        {
            IPluginManagerView view;

            var mocks = PrepareViewModelMocks(out view);
            var pluginLoader = mocks.DynamicMock<IPluginLoader>();

            var actionPlugin1 = new DummyActionPlugin("Dummy", Guid.Parse("10000000-0000-0000-0000-000000000000"));
            var actionPlugin2 = new DummyActionPlugin("Dummy", Guid.Parse("10000000-0000-0000-0000-000000000000"));

            Expect.Call(pluginLoader.LoadFile("dummy.plugin")).Return(new ActionPluginDetails()
                                                                          {
                                                                              ActionPlugin =
                                                                                  actionPlugin1,
                                                                              Enabled = true
                                                                          });

            Expect.Call(pluginLoader.LoadFile("dummy2.plugin")).Return(new ActionPluginDetails()
                                                                           {
                                                                               ActionPlugin =
                                                                                   actionPlugin2,
                                                                               Enabled = true
                                                                           });
            Expect.Call(() => view.Notify()).Repeat.Once();

            mocks.ReplayAll();

            var model = new PluginManagerModel(pluginLoader);

            model.Subscribe(view);

            Assert.AreEqual(0, model.LoadPluginFrom("dummy.plugin"));
            Assert.AreEqual(0, model.LoadPluginFrom("dummy2.plugin"));
        }

        private static MockRepository PrepareViewModelMocks(out IPluginManagerView view)
        {
            var mocks = new MockRepository();

            view = mocks.DynamicMock<IPluginManagerView>();

            var v = view;

            return mocks;
        }

        #region Nested classes
        
        public class DummyActionPlugin : IActionPlugin
        {
            public DummyActionPlugin(string name, Guid id)
            {
                Id = id;
                Name = name;
                ShortDescription = name;
            }

            #region Implementation of IActionPlugin

            public Guid Id { get; private set; }

            public string Name { get; private set; }

            public string ShortDescription { get; private set; }

            #endregion
        }

        #endregion
    }
}
