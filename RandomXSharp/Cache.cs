using System;
using RandomXSharp.Internals;

namespace RandomXSharp
{
    public class Cache : IDisposable
    {
        internal readonly IntPtr _handle;

        public Cache(Flags flags, byte[] key)
        {
            _handle = LibRandomx.Instance.randomx_alloc_cache(flags);
            LibRandomx.Instance.randomx_init_cache(_handle, key, Convert.ToUInt32(key.Length));
        }

        public void Dispose()
        {
            LibRandomx.Instance.randomx_release_cache(_handle);
        }
    }
}
