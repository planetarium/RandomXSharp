using System;
using System.Runtime.InteropServices;

namespace RandomXSharp
{
    internal static class Native
    {
        private const string Library = "randomx";

        [DllImport(Library)]
        public extern static IntPtr randomx_alloc_cache(RandomXFlags flags);
    }
}
