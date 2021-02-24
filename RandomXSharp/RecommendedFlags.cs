using RandomXSharp.Internals;

namespace RandomXSharp
{
    public static class RecommendedFlags
    {
        public static Flags Flags =>
            LibRandomx.Instance.randomx_get_flags();
    }
}
