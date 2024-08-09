using HarmonyLib;
using Kitchen;

namespace KitchenSuperCrane.Patches
{
    [HarmonyPatch]
    static class ActivateCraneModeViaPing_Patch
    {
        [HarmonyPatch(typeof(ActivateCraneModeViaPing), "Initialise")]
        [HarmonyPrefix]
        static void Initialise_Prefix(ref ActivateCraneModeViaPing __instance)
        {
            __instance.RequireSingletonForUpdate<SGameplayMarker>();
            __instance.RequireSingletonForUpdate<SKitchenMarker>();
        }
    }
}
