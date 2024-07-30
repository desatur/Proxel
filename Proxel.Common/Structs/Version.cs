namespace Proxel.Common.Structs
{
    public readonly struct Version
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Build { get; }

        public Version(int major, int minor, int patch, int build)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Build = build;
        }

        public readonly string ToString(bool includeBuild = false)
        {
            if (includeBuild) return Major.ToString() + "." + Minor.ToString() + "." + Patch.ToString() + "_" + Build.ToString();
            else return Major.ToString() + "." + Minor.ToString() + "." + Patch.ToString();
        }
    }
}
