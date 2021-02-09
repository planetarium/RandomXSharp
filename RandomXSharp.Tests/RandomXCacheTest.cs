using Xunit;

namespace RandomXSharp.Tests
{
    public class RandomXCacheTest
    {
        [Fact]
        public void Constructor()
        {
            var key = new byte[] { 0x00, 0x00, 0x00, 0x01 };
            var cache = new RandomXCache(
                RandomXFlags.Argon2 | RandomXFlags.LargePages,
                key
            );
            Assert.NotNull(cache);
        }
    }
}
