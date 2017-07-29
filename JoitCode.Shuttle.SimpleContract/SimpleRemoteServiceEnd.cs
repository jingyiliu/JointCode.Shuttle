using System;
using JointCode.Shuttle;

namespace JoitCode.Shuttle.SimpleContract
{
    /// <summary>
    /// A remote service end
    /// </summary>
    /// <seealso cref="System.MarshalByRefObject" />
    public abstract class SimpleRemoteServiceEnd : MarshalByRefObject
    {
        protected ShuttleDomain _shuttleDomain;

        /// <summary>
        /// Creates a ShuttleDomain instance in current AppDomain, so that we can communicate with other AppDomains.
        /// </summary>
        public void CreateShuttleDomain()
        {
            var key = this.GetType().Name;
            _shuttleDomain = ShuttleDomainHelper.Create(key, key);
        }

        /// <summary>
        /// Unregister all services registered to the global registry from the current AppDomain, and disposes the ShuttleDomain.
        /// </summary>
        public void DisposeShuttleDomain()
        {
            if (_shuttleDomain != null)
                _shuttleDomain.Dispose();
        }

        /// <summary>
        /// Registers services to a global registy from current AppDomain.
        /// </summary>
        public abstract void RegisterServices(); // provide services
    }
}
