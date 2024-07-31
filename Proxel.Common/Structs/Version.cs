namespace Proxel.Common.Structs
{
    public readonly struct Version(int major, int minor, int patch, int build)
    {
        public int Major { get; } = major;
        public int Minor { get; } = minor;
        public int Patch { get; } = patch;
        public int Build { get; } = build;

        public readonly string ToString(bool includeBuild = false)
        {
            if (includeBuild) return Major.ToString() + "." + Minor.ToString() + "." + Patch.ToString() + "_" + Build.ToString();
            else return Major.ToString() + "." + Minor.ToString() + "." + Patch.ToString();
        }
    }
}
