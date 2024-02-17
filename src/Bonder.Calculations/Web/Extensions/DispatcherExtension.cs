using ComposableAsync;

namespace Web.Extensions;

public static class DispatcherExtension
{
    private sealed class DispatcherDelegatingHandler : DelegatingHandler
    {
        private readonly IDispatcher _dispatcher;

        public DispatcherDelegatingHandler(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _dispatcher.Enqueue(() => base.SendAsync(request, cancellationToken), cancellationToken);
        }
    }

    public static DelegatingHandler AsDelegate(this IDispatcher dispatcher)
    {
        return new DispatcherDelegatingHandler(dispatcher);
    }
}