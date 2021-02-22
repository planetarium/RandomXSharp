using System;
using System.Security.Cryptography;

namespace RandomXSharp
{
    public class RandomX : HashAlgorithm
    {
        private readonly VirtualMachine _vm;
        private byte[]? _hashBuffer;

        public RandomX(Flags flags, Cache? cache, Dataset? dataset)
            : base()
        {
            _vm = new VirtualMachine(flags, cache, dataset);
        }

        public override int HashSize => VirtualMachine.HashSize;

        public new static RandomX Create()
        {
            Flags flags = RecommendedFlags.Flags;
            return new RandomX(flags, new Cache(flags, new byte[0]), null);
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            // TODO: This should take the offset and the size of the array, instead of slicing
            // the array and allocating a new array of the slice.
            byte[] buffer;
            if (ibStart == 0 && cbSize == array.Length)
            {
                buffer = array;
            }
            else
            {
                buffer = new byte[cbSize];
                Array.Copy(array, ibStart, buffer, 0, cbSize);
            }

            _hashBuffer = _vm.CaculateHash(buffer);
        }

        protected override byte[] HashFinal()
        {
            return _hashBuffer ?? new byte[0];
        }

        public override void Initialize()
        {
            return;
        }
    }
}
