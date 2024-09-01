using Controllers;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace KitchenSuperCrane.Patches
{
    [HarmonyPatch]
    static class Maps_Patch
    {
        [HarmonyPatch(typeof(Maps), "NewGamepad")]
        [HarmonyPostfix]
        static void NewGamepad_Postfix(ref InputActionMap __result)
        {
            if (__result.FindAction(Controls.Interact1Crane).bindings.Count <= 0)
                __result.FindAction(Controls.Interact1Crane).AddBinding("<Mouse>/leftButton");
            if (__result.FindAction(Controls.Interact2Crane).bindings.Count <= 0)
                __result.FindAction(Controls.Interact2Crane).AddBinding("<Mouse>/rightButton");
        }
    }
}
