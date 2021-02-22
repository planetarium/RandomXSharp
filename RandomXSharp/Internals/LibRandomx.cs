using AdvancedDLSupport;

namespace RandomXSharp.Internals
{
    internal static class LibRandomx
    {
        private static ILibRandomx? _instance = null;

        public static ILibRandomx Instance
        {
            get
            {
                if (!(_instance is { } i))
                {
                    i = NativeLibraryBuilder.Default.ActivateInterface<ILibRandomx>("randomx");
                    _instance = i;
                }

                return i;
            }
        }
    }
}
