using System;
using System.Globalization;
using System.Runtime.Caching;

namespace Datastore {
    public class SignaledChangeMonitor : ChangeMonitor {
        private readonly string _name;

        public SignaledChangeMonitor(string name = null) {
            _name = name;

            Signaled += OnSignalRaised;
            InitializationComplete();
        }

        public override string UniqueId => Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        private event EventHandler<SignaledChangeEventArgs> Signaled;

        public void Signal(string name = null) {
            Signaled?.Invoke(null, new SignaledChangeEventArgs(name));
        }

        protected override void Dispose(bool disposing) {
            Signaled -= OnSignalRaised;
        }

        private void OnSignalRaised(object sender, SignaledChangeEventArgs e) {
            if (!string.IsNullOrWhiteSpace(e.Name) &&
                string.Compare(e.Name, _name, StringComparison.OrdinalIgnoreCase) != 0) return;

            OnChanged(null);
        }
    }
}