using System;
using System.Collections.Generic;
using RandomXSharp.Internals;

namespace RandomXSharp
{
    public class VirtualMachine : IDisposable
    {
        public const int HashSize = 32;

        private readonly IntPtr _handle;
        private Cache? _cache;

        public VirtualMachine(Flags flags, Cache? cache, Dataset? dataset)
        {
            if (cache is null && (flags & Flags.FullMem) == 0)
            {
                throw new ArgumentNullException(
                    nameof(cache),
                    $"The cache is required unless the flag {nameof(Flags.FullMem)} is turned on."
                );
            }
            else if (cache is { } && (flags & Flags.FullMem) != 0)
            {
                throw new ArgumentException(
                    $"The cache is unavailable with the flag {nameof(Flags.FullMem)}.",
                    nameof(cache)
                );
            }
            else if (dataset is null && (flags & Flags.FullMem) != 0)
            {
                throw new ArgumentNullException(
                    nameof(dataset),
                    $"The dataset is required with the flag {nameof(Flags.FullMem)}."
                );
            }
            else if (dataset is { } && (flags & Flags.FullMem) == 0)
            {
                throw new ArgumentException(
                    $"The dataset is only available with the flag {nameof(Flags.FullMem)}.",
                    nameof(dataset)
                );
            }

            _handle = LibRandomx.Instance.randomx_create_vm(
                flags,
                cache?._handle ?? IntPtr.Zero,
                dataset?._handle ?? IntPtr.Zero
            );
            if (_handle == IntPtr.Zero)
            {
                throw new SystemException("Failed to create a machine.");
            }

            Flags = flags;
            _cache = cache;
        }

        public Flags Flags { get; }

        public Cache? Cache
        {
            get => _cache;
            set
            {
                if ((Flags & Flags.FullMem) != 0)
                {
                    if (value is null)
                    {
                        return;
                    }

                    throw new NotSupportedException(
                        $"The cache is unavailable with the flag {nameof(Flags.FullMem)}."
                    );
                }
                else if (value is null)
                {
                    throw new NullReferenceException(
                        $"The cache is required without the flag {nameof(Flags.FullMem)}."
                    );
                }

                LibRandomx.Instance.randomx_vm_set_cache(_handle, value._handle);
                _cache = value;
            }
        }

        public byte[] CaculateHash(byte[] input)
        {
            var buffer = new byte[HashSize];
            LibRandomx.Instance.randomx_calculate_hash(
                _handle,
                input,
                Convert.ToUInt32(input.Length),
                buffer
            );
            return buffer;
        }

        public IEnumerable<byte[]> CalculateHashes(IEnumerable<byte[]> inputs)
        {
            ILibRandomx librandomx = LibRandomx.Instance;
            byte[]? buffer = null;
            foreach (byte[] input in inputs)
            {
                uint inputSize = Convert.ToUInt32(input.Length);
                if (buffer is { } output)
                {
                    librandomx.randomx_calculate_hash_next(_handle, input, inputSize, output);
                    yield return output;
                }
                else
                {
                    librandomx.randomx_calculate_hash_first(_handle, input, inputSize);
                }

                buffer = new byte[HashSize];
            }

            if (buffer is { } lastOutput)
            {
                librandomx.randomx_calculate_hash_last(_handle, lastOutput);
                yield return lastOutput;
            }
        }

        public void Dispose()
        {
            LibRandomx.Instance.randomx_destroy_vm(_handle);
        }
    }
}
