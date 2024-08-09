using HarmonyLib;
using Kitchen;


namespace KitchenSuperCrane.Patches
{
    [HarmonyPatch]
    static class PlayerCraneMovementComponent_Patch
    {
        [HarmonyPatch(typeof(PlayerCraneMovementComponent), "UpdateMovement")]
        [HarmonyPrefix]
        static void UpdateMovement_Prefix(ref float base_speed)
        {
            base_speed *= Main.PrefManager.Get<float>(Main.MOVEMENT_CRANE_SPEED_FACTOR_ID);
        }
    }
}
