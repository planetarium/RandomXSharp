using System;
using System.Linq;
using System.Text;
using RandomXSharp.Internals;
using Xunit;
using Xunit.Abstractions;

namespace RandomXSharp.Tests.Internals
{
    public class LibRandomxTests
    {
        private readonly ITestOutputHelper _output;

        public LibRandomxTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ApiExample1()
        {
            // Port of:
            //   https://github.com/tevador/RandomX/blob/v1.1.8/src/tests/api-example1.c
            byte[] myKey = Encoding.ASCII.GetBytes("RandomX example key\0");
            byte[] myInput = Encoding.ASCII.GetBytes("RandomX example input\0");
            var hash = new byte[32];

            Flags flags = LibRandomx.randomx_get_flags();
            IntPtr myCache = LibRandomx.randomx_alloc_cache(flags);
            LibRandomx.randomx_init_cache(myCache, myKey, (UIntPtr)myKey.Length);
            IntPtr myMachine = LibRandomx.randomx_create_vm(flags, myCache, IntPtr.Zero);
            LibRandomx.randomx_calculate_hash(
                myMachine,
                myInput,
                (UIntPtr)myInput.Length,
                hash
            );

            LibRandomx.randomx_destroy_vm(myMachine);
            LibRandomx.randomx_release_cache(myCache);

            _output.WriteLine(string.Join("", hash.Select(b => $"{b:x02}")));

            byte[] expected =
            {
                0x8a, 0x48, 0xe5, 0xf9, 0xdb, 0x45, 0xab, 0x79,
                0xd9, 0x08, 0x05, 0x74, 0xc4, 0xd8, 0x19, 0x54,
                0xfe, 0x6a, 0xc6, 0x38, 0x42, 0x21, 0x4a, 0xff,
                0x73, 0xc2, 0x44, 0xb2, 0x63, 0x30, 0xb7, 0xc9,
            };
            Assert.Equal(expected, hash);
        }
    }
}
