# VersionLabel

A tiny server-side mod for SPT 4.1.x that replaces the in-game version watermark with the current server version.

## How it works

A Harmony prefix patch (via SPTarkov.Reflection) on `Watermark.GetInGameVersionLabel` returns your server's `CompatibleTarkovVersion` followed by `" Beta version"` instead of the original label.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Build

Double-click `build.bat`, or run:

```powershell
dotnet build SPTarkov.CustomWatermark.sln -c Release
```

Output goes to `Build\Release\SPT_Runtime\user\mods\VersionLabel`.

## Install

- Copy the `SPT_Runtime` folder from `Build\Release` into your server root, or
- Copy only the `VersionLabel` folder into `<server root>\SPT_Runtime\user\mods\`

## License

[AGPL-3.0](LICENSE)

## Post

[sp-mod.com](https://sp-mod.com/,mod/2460/belacustomwatermark)
