using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Caching;

namespace Datastore {
    public static class CachePolicy {
        public static readonly CacheItemPolicy CommonPolicy = new CacheItemPolicy {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(24)
        };

        public static readonly CacheItemPolicy Environments = GetNamedPolicy(nameof(Environments));
        public static readonly CacheItemPolicy HttpTests = GetNamedPolicy(nameof(HttpTests));

        private static CacheItemPolicy GetNamedPolicy(string name) {
            var policy = CommonPolicy;
            policy.ChangeMonitors.Add(new SignaledChangeMonitor(name));
            return policy;
        }
    }

    /// <summary>
    ///     Cache change monitor that allows an app to fire a change notification
    ///     to all associated cache items.
    /// </summary>
    public class SignaledChangeMonitor : ChangeMonitor {
        private readonly string _name;
        private readonly string _uniqueId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        public SignaledChangeMonitor(string name = null) {
            _name = name;
            // Register instance with the shared event
            Signaled += OnSignalRaised;
            InitializationComplete();
        }

        public override string UniqueId {
            get { return _uniqueId; }
        }

        // Shared across all SignaledChangeMonitors in the AppDomain
        private static event EventHandler<SignaledChangeEventArgs> Signaled;

        public static void Signal(string name = null) {
            if (Signaled != null) {
                // Raise shared event to notify all subscribers
                Signaled(null, new SignaledChangeEventArgs(name));
            }
        }

        protected override void Dispose(bool disposing) {
            Signaled -= OnSignalRaised;
        }

        private void OnSignalRaised(object sender, SignaledChangeEventArgs e) {
            if (string.IsNullOrWhiteSpace(e.Name) || string.Compare(e.Name, _name, true) == 0) {
                Debug.WriteLine(
                    _uniqueId + " notifying cache of change.", "SignaledChangeMonitor");
                // Cache objects are obligated to remove entry upon change notification.
                OnChanged(null);
            }
        }
    }
}