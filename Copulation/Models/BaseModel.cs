using System.Collections.Generic;
using System.Collections.ObjectModel;
using Copulation.Models.Patterns;

namespace Copulation.Models
{
    public class BaseModel : IModel
    {
        private readonly ICollection<IModelObserver> _observers = new Collection<IModelObserver>();

        #region Implementation of IModelObservable

        public void Subscribe(IModelObserver observer)
        {
            lock (_observers)
            {
                if (!_observers.Contains(observer))
                    _observers.Add(observer);
            }
        }

        public void Unsubscribe(IModelObserver observer)
        {
            lock (_observers)
            {
                _observers.Remove(observer);
            }
        }

        #endregion

        public void NotifyObservers()
        {
            lock (_observers)
            {
                foreach (var observer in _observers)
                {
                    observer.Notify();
                }
            }
        }
    }
}