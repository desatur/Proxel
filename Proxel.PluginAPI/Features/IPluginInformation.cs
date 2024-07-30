using Version = Proxel.Common.Structs.Version;

namespace Proxel.PluginAPI.Features
{
    public interface IPluginInformation
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        Version Version { get; }
    }
}
