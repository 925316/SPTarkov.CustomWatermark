using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using System.Reflection;

namespace Bela.CustomWatermark;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.sp-tarkov.Bela.CustomWatermark";
    public override string Name { get; init; } = "CustomWatermark";
    public override string Author { get; init; } = "Bela";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.1");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://github.com/925316/SPTarkov.CustomWatermark";
    public override bool? IsBundleMod { get; init; }
    public override string? License { get; init; } = "AGPL-3.0";
}

public class ModConfig
{
    public string? Version { get; set; } = "SPT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class CustomWatermark(
    ISptLogger<CustomWatermark> logger,
    ConfigServer configServer
    )
    : IOnLoad
{
    private static string? s_version;

    public Task OnLoad()
    {
        var coreConfig = configServer.GetConfig<CoreConfig>();
        s_version = coreConfig.CompatibleTarkovVersion;
        s_version += " Beta version";

        logger.Warning($"[Bela.CustomWatermark]: {s_version}");

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