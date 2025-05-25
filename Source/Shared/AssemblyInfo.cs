using KeyEngine;

[assembly: AssemblyVersion(KeyEngineVersion.AssemblyVersion)]
[assembly: AssemblyFileVersion(KeyEngineVersion.AssemblyVersion)]
[assembly: AssemblyInformationalVersion(KeyEngineVersion.AssemblyInformationalVersion)]

namespace KeyEngine;

internal class KeyEngineVersion
{
    public const string PublicVersion = "0.0.1";

     public const string AssemblyVersion = PublicVersion;

    public const string AssemblyInformationalVersion = PublicVersion;
}