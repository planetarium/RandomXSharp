using System;
using System.Runtime.InteropServices;

namespace RandomXSharp.Internals
{
    internal static class LibRandomx
    {
        [DllImport("randomx")]
        public static extern Flags randomx_get_flags();

        [DllImport("randomx")]
        public static extern IntPtr randomx_alloc_cache(Flags flags);

        [DllImport("randomx")]
        public static extern void randomx_init_cache(IntPtr cache, byte[] key, uint keySize);

        [DllImport("randomx")]
        public static extern void randomx_release_cache(IntPtr cache);

        [DllImport("randomx")]
        public static extern IntPtr randomx_create_vm(Flags flags, IntPtr cache, IntPtr dataset);

        [DllImport("randomx")]
        public static extern void randomx_vm_set_cache(IntPtr machine, IntPtr cache);

        [DllImport("randomx")]
        public static extern void randomx_destroy_vm(IntPtr machine);

        [DllImport("randomx")]
        public static extern void randomx_calculate_hash(
            IntPtr machine,
            byte[] input,
            uint inputSize,
            byte[] output
        );

        [DllImport("randomx")]
        public static extern void randomx_calculate_hash_first(
            IntPtr machine,
            byte[] input,
            uint inputSize
        );

        [DllImport("randomx")]
        public static extern void randomx_calculate_hash_next(
            IntPtr machine,
            byte[] nextInput,
            uint nextInputSize,
            byte[] output
        );

        [DllImport("randomx")]
        public static extern void randomx_calculate_hash_last(
            IntPtr machine,
            byte[] output
        );
    }
}
