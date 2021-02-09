using System;

namespace RandomXSharp
{
    public class RandomXCache : IDisposable
    {
        private IntPtr _handle;

        public RandomXCache(RandomXFlags randomXFlags, byte[] key)
        {
            _handle = Native.randomx_alloc_cache(randomXFlags);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
