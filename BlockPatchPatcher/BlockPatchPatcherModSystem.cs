using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.ServerMods.NoObf;

[assembly: ModInfo(name: "BlockPatchPatcher", modID: "blockpatchpatcher", Side = "Server", Version = "1.0.0", Authors = new string[] { "jayugg" },
    Description = "Dynamic blockpatch patching")]

namespace BlockPatchPatcher
{
public class BBPCore : ModSystem
{
    public override double ExecuteOrder() => 0.6;
    public static ILogger Logger;
    public BlockPatchPatch[] BlockPatchPatches;
    public override void StartPre(ICoreAPI api)
    {
        Logger = Mod.Logger;
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);
        if ((api.Side & EnumAppSide.Server) != 0) LoadBlockPatchPatches(api);
    }

    public void LoadBlockPatchPatches(ICoreAPI api)
    {
        Dictionary<AssetLocation, BlockPatchPatch[]> blockPatchPatchAssets = api.Assets.GetMany<BlockPatchPatch[]>(Logger,"config/blockpatchpatches.json");
        BlockPatchPatches = blockPatchPatchAssets.Values.SelectMany(val => val).ToArray();
    }
    
    public IAsset[] LoadPatchableBlockPatches(ICoreAPI api)
    {
        List<AssetLocation> blockPatchAssets = api.Assets.GetLocations("worldgen/blockpatches/");
        var assets = blockPatchAssets.Select(val => api.Assets.Get(val)).ToList();
        return assets.ToArray();
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        base.AssetsFinalize(api);
        
        if (api is ICoreServerAPI sapi)
        {
            var blockPatchConfigAsset = api.Assets.Get("worldgen/blockpatches.json");
            var bpc = blockPatchConfigAsset.ToObject<BlockPatchConfig>();
            PatchBlockPatches(bpc.Patches, out var anyPatch1);
            if (anyPatch1)
            {
                var output1 = JToken.FromObject(bpc).ToString();
                blockPatchConfigAsset.Data = Encoding.UTF8.GetBytes(output1);
            }

            foreach (var blockPatchAsset in LoadPatchableBlockPatches(sapi))
            {
                BlockPatch[] blockPatches = blockPatchAsset.ToObject<BlockPatch[]>();
                PatchBlockPatches(blockPatches, out var anyPatch2);
                if (anyPatch2) continue;
                var output2 = JToken.FromObject(blockPatches).ToString();
                blockPatchAsset.Data = Encoding.UTF8.GetBytes(output2);
            }
        }
    }

    private void PatchBlockPatches(BlockPatch[] blockPatches, out bool anyPatch)
    {
        foreach (var patch in blockPatches)
        {
            foreach (var blockPatchPatch in BlockPatchPatches)
            {
                if (patch.blockCodes.Any(code => WildcardUtil.Match(blockPatchPatch?.Code, code.ToString())))
                {
                    anyPatch = true;
                    //Logger.Warning($"Patch: {patch.blockCodes[0]} Op: {blockPatchPatch?.Op} Chance: {patch?.Chance},{blockPatchPatch?.Chance}");
                    blockPatchPatch?.Patch(patch);
                    //Logger.Warning($"AfterPatch: {patch.blockCodes[0]} Chance: {patch?.Chance}");
                }
            }
        }
        anyPatch = false;
    }
}
}