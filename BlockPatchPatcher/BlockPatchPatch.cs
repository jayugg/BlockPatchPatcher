using System;
using System.Linq;
using System.Reflection;
using BlockPatchPatcher.PatchSystem;
using Newtonsoft.Json;
using Vintagestory.API.MathTools;
using Vintagestory.ServerMods;
using Vintagestory.ServerMods.NoObf;
using wildercraftberries.PatchSystem;

namespace BlockPatchPatcher;

public class BlockPatchPatch
{
    [JsonProperty]
    public string Code;
    [JsonProperty]
    public OpType Op;
    [JsonProperty]
    public float Chance = 0.05f;
    [JsonProperty]
    public int MinTemp = -30;
    [JsonProperty]
    public int MaxTemp = 40;
    [JsonProperty]
    public float MinRain;
    [JsonProperty]
    public float MaxRain = 1f;
    [JsonProperty]
    public float MinForest;
    [JsonProperty]
    public float MaxForest = 1f;
    [JsonProperty]
    public float MinShrub;
    [JsonProperty]
    public float MaxShrub = 1f;
    [JsonProperty]
    public float MinFertility;
    [JsonProperty]
    public float MaxFertility = 1f;
    [JsonProperty]
    public float MinY = -0.3f;
    [JsonProperty]
    public float MaxY = 1f;
    [JsonProperty]
    public EnumBlockPatchPlacement Placement;
    [JsonProperty]
    public EnumTreeType TreeType;
    [JsonProperty]
    public NatFloat OffsetX;
    [JsonProperty]
    public NatFloat OffsetZ;
    [JsonProperty]
    public NatFloat BlockCodeIndex;
    [JsonProperty]
    public NatFloat Quantity;
    [JsonProperty]
    public string MapCode;
    [JsonProperty]
    public string[] RandomMapCodePool;
    [JsonProperty]
    public int MinWaterDepth = 3;
    [JsonProperty]
    public int MaxHeightDifferential = 8;

    public void Patch(BlockPatch blockPatch)
    {
        var patchFields = GetType().GetFields();
        var blockPatchFields = blockPatch.GetType().GetFields();
        foreach (var patchField in patchFields)
        {
            if (patchField.GetValue(this) == patchField.GetValue(new BlockPatchPatch())) continue; // Skip default values
            var blockPatchField = blockPatchFields.FirstOrDefault(f => f.Name == patchField.Name);
            if (blockPatchField != null && blockPatchField.GetCustomAttribute(typeof(JsonPropertyAttribute), false) != null)
            { 
                //BBPCore.Logger.Warning($"Patching {blockPatchField.Name} with Type {patchField.FieldType}");
                if (blockPatchField.FieldType != typeof(string) &&
                    blockPatchField.FieldType != typeof(string[]) &&
                    blockPatchField.FieldType != typeof(Single) &&
                    blockPatchField.FieldType != typeof(NatFloat) &&
                    blockPatchField.FieldType != typeof(double) &&
                    blockPatchField.FieldType != typeof(float)
                    ) continue;
                var originalValue = blockPatchField.GetValue(blockPatch);
                var patchValue = patchField.GetValue(this);
                var patchedValue = Op.Apply(originalValue, patchValue, blockPatchField.Name);
                blockPatchField?.SetValue(blockPatch, patchedValue);
            }
        }
    }
}