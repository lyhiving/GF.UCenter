using System;
using System.Threading;

namespace GF.UCenter.Common
{
    public abstract class DisposableObjectSlim : IDisposable
    {
        private const int NotDisposed = 0;

        private const int Disposed = 1;

        private int disposedFlag = NotDisposed;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposedFlag, Disposed, NotDisposed) == NotDisposed)
            {
                this.DisposeInternal();
                GC.SuppressFinalize(this);
            }
        }

        protected abstract void DisposeInternal();
    }
}
