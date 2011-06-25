
namespace Copulation.Models.Patterns
{
    public interface IModelObservable
    {
        void Subscribe(IModelObserver observer);

        void Unsubscribe(IModelObserver observer);
    }
}