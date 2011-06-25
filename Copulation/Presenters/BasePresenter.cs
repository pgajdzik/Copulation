using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copulation.Presenters
{
    using Models;
    using Views;

    public abstract class BasePresenter<TView, TModel> 
        where TView : IView 
        where TModel : IModel
    {
        protected readonly TView View;

        protected readonly TModel Model;

        protected BasePresenter(TView view, TModel model)
        {
            View = view;
            Model = model;

            Initialize();
        }

        protected void Initialize()
        {
            AssignEventHandlers();
            BindModel();
        }

        protected abstract void AssignEventHandlers();
        
        protected abstract void BindModel();
    }
}
