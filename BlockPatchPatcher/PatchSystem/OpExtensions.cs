using wildercraftberries.PatchSystem;

namespace BlockPatchPatcher.PatchSystem;

public static class OpExtensions
{
    public static T Apply<T>(this OpType operation, T originalValue, T patchValue, string debugInfo)
    {
        try
        {
            dynamic original = originalValue;
            dynamic patch = patchValue;

            return operation switch
            {
                OpType.Multiply => (T)(original * patch),
                OpType.Add => (T)(original + patch),
                OpType.Replace => patchValue,
                _ => originalValue
            };
        }
        catch
        {
            BBPCore.Logger.Error($"Failed to apply patch operation {operation} for type {typeof(T)}: {debugInfo}");
            return originalValue;
        }
    }
}