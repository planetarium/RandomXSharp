using System;
using RandomXSharp.Internals;

namespace RandomXSharp
{
    public class Cache : IDisposable
    {
        internal readonly IntPtr _handle;

        public Cache(Flags flags, byte[] key)
        {
            _handle = LibRandomx.randomx_alloc_cache(flags);
            LibRandomx.randomx_init_cache(_handle, key, (UIntPtr)key.Length);
        }

        public void Dispose()
        {
            LibRandomx.randomx_release_cache(_handle);
        }
    }
}
