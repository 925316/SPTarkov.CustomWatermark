using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using System.Reflection;
using System.Text.Json.Nodes;

namespace VersionLabel;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp.bela.versionlabel";
    public string Name { get; init; } = "VersionLabel";
    public string Author { get; init; } = "Bela";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.1.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.2");
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; } = "https://github.com/925316/SPTarkov.CustomWatermark";
    public string License { get; init; } = "AGPL-3.0";
    public bool HasPrepatcher { get; init; } = false;
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class CustomWatermark(
    ISptLogger<CustomWatermark> logger,
    ModHelper modHelper,
    CoreConfig coreConfig
    )
    : IOnLoad
{
    private static string s_version = string.Empty;

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // Default: the original VersionLabel behavior. The patch always replaces the SPT label.
        s_version = $"{coreConfig.CompatibleTarkovVersion} Beta version";

        try
        {
            var config = modHelper.GetJsonDataFromModFile<JsonObject>("db", "config.jsonc");

            if (config?["version"] is JsonValue value && value.TryGetValue(out string? version))
            {
                s_version = version;
            }
        }
        catch (Exception ex)
        {
            logger.Warning($"[VersionLabel]: db/config.jsonc not loaded, using default label. Error: {ex.Message}");
        }

        new WatermarkPatch().Enable();
        logger.Warning($"[VersionLabel]: {(s_version == "" ? "<hidden>" : s_version)}");

        return Task.CompletedTask;
    }

    public class WatermarkPatch : AbstractPatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(SPTarkov.Server.Core.Utils.Watermark).GetMethod("GetInGameVersionLabel")
                ?? throw new MissingMethodException(nameof(SPTarkov.Server.Core.Utils.Watermark), "GetInGameVersionLabel");
        }

        [PatchPrefix]
        public static bool Prefix(ref string __result)
        {
            __result = s_version;
            return false;
        }
    }
}
