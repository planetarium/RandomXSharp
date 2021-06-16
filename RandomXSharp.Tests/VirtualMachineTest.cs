using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace RandomXSharp.Tests
{
    public class VirtualMachineTest
    {
        private readonly ITestOutputHelper _output;

        public VirtualMachineTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SimpleCase()
        {
            Flags flags = Flags.Default;
            byte[] hash;
            using (var cache = new Cache(flags, Encoding.ASCII.GetBytes("cache key")))
            using (var vm = new VirtualMachine(flags, cache, null))
            {
                hash = vm.CaculateHash(Encoding.ASCII.GetBytes("RandomX example input"));
            }

            _output.WriteLine("{0}", Hex(hash));

            byte[] expected =
            {
                0x3d, 0x7a, 0x7a, 0x44, 0xf6, 0xf3, 0x1a, 0x41,
                0x27, 0x79, 0x4c, 0xcf, 0x35, 0xea, 0x05, 0x36,
                0xd0, 0xae, 0xa0, 0xbc, 0x08, 0xe7, 0x41, 0x69,
                0x00, 0xb5, 0x4f, 0x0c, 0x8c, 0xf2, 0x87, 0xbf,
            };
            Assert.Equal(expected, hash);
        }

        [Fact]
        public void ApiExample1()
        {
            // Port of:
            //   https://github.com/tevador/RandomX/blob/v1.1.8/src/tests/api-example1.c
            byte[] myKey = Encoding.ASCII.GetBytes("RandomX example key\0");
            byte[] myInput = Encoding.ASCII.GetBytes("RandomX example input\0");
            byte[] hash;

            Flags flags = RecommendedFlags.Flags;
            using (var myCache = new Cache(flags, myKey))
            using (var myMachine = new VirtualMachine(flags, myCache, null))
            {
                hash = myMachine.CaculateHash(myInput);
            }

            _output.WriteLine("{0}", Hex(hash));

            byte[] expected =
            {
                0x8a, 0x48, 0xe5, 0xf9, 0xdb, 0x45, 0xab, 0x79,
                0xd9, 0x08, 0x05, 0x74, 0xc4, 0xd8, 0x19, 0x54,
                0xfe, 0x6a, 0xc6, 0x38, 0x42, 0x21, 0x4a, 0xff,
                0x73, 0xc2, 0x44, 0xb2, 0x63, 0x30, 0xb7, 0xc9,
            };
            Assert.Equal(expected, hash);
        }

        [Fact]
        public void MultipleHashes()
        {
            Encoding ascii = Encoding.ASCII;
            byte[] key = ascii.GetBytes("RandomX example key\0");
            byte[][] inputs =
            {
                ascii.GetBytes("RandomX example input\0"),
                ascii.GetBytes("second round\0"),
                ascii.GetBytes("third round\0"),
            };
            byte[][] hashes;

            Flags flags = RecommendedFlags.Flags;
            using (var cache = new Cache(flags, key))
            using (var vm = new VirtualMachine(flags, cache, null))
            {
                hashes = vm.CalculateHashes(inputs).ToArray();
            }

            foreach ((byte[] input, byte[] hash) in inputs.Zip(hashes, (a, b) => (a, b)))
            {
                _output.WriteLine("{0}: {1}", ascii.GetString(input, 0, input.Length - 1), Hex(hash));
            }

            byte[][] expectedHashes =
            {
                new byte[] {
                    0x8a, 0x48, 0xe5, 0xf9, 0xdb, 0x45, 0xab, 0x79,
                    0xd9, 0x08, 0x05, 0x74, 0xc4, 0xd8, 0x19, 0x54,
                    0xfe, 0x6a, 0xc6, 0x38, 0x42, 0x21, 0x4a, 0xff,
                    0x73, 0xc2, 0x44, 0xb2, 0x63, 0x30, 0xb7, 0xc9,
                },
                new byte[]
                {
                    0x59, 0x44, 0x24, 0x33, 0x50, 0x15, 0x4b, 0x11,
                    0x10, 0x6a, 0x7a, 0xec, 0x6f, 0x13, 0x05, 0x51,
                    0xb4, 0x6d, 0xdc, 0x81, 0xb5, 0x67, 0xdd, 0xb2,
                    0xa4, 0xcd, 0x27, 0xb2, 0x3f, 0x12, 0x64, 0x1b,
                },
                new byte[]
                {
                    0x9b, 0x2f, 0xa7, 0x22, 0x84, 0x5a, 0x9e, 0x53,
                    0xd0, 0x6a, 0x48, 0xf4, 0xb3, 0xe6, 0x4f, 0x91,
                    0x5e, 0x56, 0x1c, 0xd5, 0x65, 0xa2, 0x45, 0x59,
                    0xa8, 0xff, 0x50, 0xb9, 0x84, 0xb0, 0xe3, 0x3a,
                },
            };
            foreach ((byte[] expect, byte[] actual) in expectedHashes.Zip(hashes, (a, b) => (a, b)))
            {
                Assert.Equal(expect, actual);
            }
        }

        private static string Hex(byte[] bytes)
        {
            return string.Join("", bytes.Select(b => $"{b:x02}"));
        }
    }
}
