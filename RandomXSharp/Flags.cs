namespace RandomXSharp
{
    [System.Flags]
    public enum Flags
    {
        Default = 0,
        LargePages = 1,
        HardAes = 2,
        FullMem = 4,
        Jit = 8,
        Secure = 16,
        Argon2Ssse3 = 32,
        Argon2Avx2 = 64,
        Argon2 = 96,
    }
}
