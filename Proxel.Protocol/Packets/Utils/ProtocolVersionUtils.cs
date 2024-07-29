using Proxel.Protocol.Enums;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Proxel.Protocol.Packets.Utils
{
    /// <summary>
    /// Provides utility methods for working with the ProtocolVersion enum.
    /// </summary>
    public static class ProtocolVersionUtils
    {
        // Using a dictionary to map protocol versions to a list of ProtocolVersion enums to handle duplicates
        private static readonly Dictionary<int, List<ProtocolVersion>> protocolVersionMap = Enum.GetValues(typeof(ProtocolVersion))
            .Cast<ProtocolVersion>()
            .GroupBy(v => (int)v)
            .ToDictionary(g => g.Key, g => g.ToList());

        /// <summary>
        /// Converts a ProtocolVersion enum value to a version string (e.g., v1_8 to 1.8).
        /// </summary>
        /// <param name="protocolVersion">The ProtocolVersion enum value.</param>
        /// <returns>A string representing the version.</returns>
        public static string ProtocolVersionToVersionString(ProtocolVersion protocolVersion)
        {
            return protocolVersion.ToString().TrimStart('v').Replace('_', '.');
        }

        /// <summary>
        /// Retrieves the ProtocolVersion enum value corresponding to a given protocol version integer.
        /// </summary>
        /// <param name="version">The protocol version integer.</param>
        /// <returns>A list of corresponding ProtocolVersion enum values, or null if not found.</returns>
        public static List<ProtocolVersion>? GetProtocolVersion(int version)
        {
            if (protocolVersionMap.TryGetValue(version, out var protocolVersions))
            {
                return protocolVersions;
            }
            return null;
        }

        /// <summary>
        /// Converts a version string to the corresponding ProtocolVersion enum value (e.g., 1.8 to v1_8).
        /// </summary>
        /// <param name="versionString">The version string.</param>
        /// <returns>The corresponding ProtocolVersion enum value, or null if not found.</returns>
        public static ProtocolVersion? GetProtocolFromVersionString(string versionString)
        {
            string formattedString = "v" + versionString.Replace('.', '_');
            if (Enum.TryParse(typeof(ProtocolVersion), formattedString, out var protocolVersion))
            {
                return (ProtocolVersion)protocolVersion;
            }
            return null;
        }

        /// <summary>
        /// Checks if a given protocol version integer is supported.
        /// </summary>
        /// <param name="version">The protocol version integer.</param>
        /// <returns>True if the protocol version is supported, otherwise false.</returns>
        public static bool IsProtocolSupported(int version)
        {
            return protocolVersionMap.ContainsKey(version);
        }

        /// <summary>
        /// Checks if a given version string is supported.
        /// </summary>
        /// <param name="versionString">The version string.</param>
        /// <returns>True if the version string corresponds to a supported protocol version, otherwise false.</returns>
        public static bool IsProtocolSupported(string versionString)
        {
            return GetProtocolFromVersionString(versionString) != null;
        }
    }
}
