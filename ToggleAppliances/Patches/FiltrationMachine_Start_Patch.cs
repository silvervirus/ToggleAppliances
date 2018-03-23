using ToggleAppliances.MonoBehaviours;
using Harmony;

namespace ToggleAppliances.Patches
{
    [HarmonyPatch(typeof(FiltrationMachine))]
    [HarmonyPatch("Start")]
    public class FiltrationMachine_Start_Patch
    {
        static void Postfix(FiltrationMachine __instance)
        {
            __instance.gameObject.AddComponent<FiltrationMachineToggle>();
        }
    }
}
