using System;

namespace Checky.Common.Datastore {
    public class SignaledChangeEventArgs : EventArgs {
        public SignaledChangeEventArgs(string name = null) {
            Name = name;
        }

        public string Name { get; private set; }
    }
}