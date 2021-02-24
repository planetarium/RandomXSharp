using AdvancedDLSupport;

namespace RandomXSharp.Internals
{
    internal static class LibRandomx
    {
        public static ILibRandomx Instance { get; }

        static LibRandomx()
        {
            Instance = NativeLibraryBuilder.Default.ActivateInterface<ILibRandomx>("randomx");
        }
    }
}
