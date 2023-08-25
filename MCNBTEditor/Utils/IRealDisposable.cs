using System;

namespace MCNBTEditor.Utils {
    public interface IRealDisposable : IDisposable {
        bool IsDisposed { get; }

        void Dispose(bool isDisposing);
    }
}