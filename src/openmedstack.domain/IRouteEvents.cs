using System;

namespace OpenMedStack.Domain
{
    public interface IRouteEvents : IDisposable
    {
        void Register<T>(Action<T> handler);

        void Register(object aggregate);

        void Dispatch(object eventMessage);
    }
}