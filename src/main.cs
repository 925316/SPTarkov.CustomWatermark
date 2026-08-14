using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using System.Reflection;

namespace VersionLabel;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp.bela.versionlabel";
    public string Name { get; init; } = "VersionLabel";
    public string Author { get; init; } = "Bela";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.0.2");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.2");
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; } = "https://github.com/925316/SPTarkov.CustomWatermark";
    public string License { get; init; } = "AGPL-3.0";
    public bool HasPrepatcher { get; init; } = false;
}

public class ModConfig
{
    public string? Version { get; set; } = "SPT";
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class CustomWatermark(
    ISptLogger<CustomWatermark> logger,
    CoreConfig coreConfig
    )
    : IOnLoad
{
    private static string? s_version;

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        s_version = coreConfig.CompatibleTarkovVersion;
        s_version += " Beta version";

        logger.Warning($"[VersionLabel]: {s_version}");

        new WatermarkPatch().Enable();
        return Task.CompletedTask;
    }

    public class WatermarkPatch : AbstractPatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Watermark).GetMethod("GetInGameVersionLabel");
        }

        [PatchPrefix]
        public static bool Prefix(ref string __result)
        {
            if (!string.IsNullOrEmpty(s_version))
            {
                __result = s_version;
                return false;
            }
            return true;
        }
    }
}