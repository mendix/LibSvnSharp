using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    abstract class SvnAuthWrapper<T> : ISvnAuthWrapper
        where T : SvnAuthenticationEventArgs
    {
        protected readonly AprBaton<SvnAuthWrapper<T>> _baton;
        protected readonly EventHandler<T> _handler;
        protected readonly SvnAuthentication _authentication;
        protected readonly AuthPromptWrappers _callbacks;
        protected int _retryLimit;

        protected SvnAuthWrapper(EventHandler<T> handler, SvnAuthentication authentication)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));

            _handler = handler;
            _baton = new AprBaton<SvnAuthWrapper<T>>(this);
            _authentication = authentication;
            _callbacks = new AuthPromptWrappers();
            _retryLimit = 128;
        }

        public void Dispose()
        {
            _baton?.Dispose();
            _callbacks?.Dispose();
        }

        public abstract svn_auth_provider_object_t GetProviderPtr(AprPool pool);

        public int RetryLimit
        {
            get => _retryLimit;
            set => _retryLimit = value;
        }

        internal void Raise(T item)
        {
            _handler(_authentication, item);
        }

        public SvnAuthentication Authentication => _authentication;
    }
}
