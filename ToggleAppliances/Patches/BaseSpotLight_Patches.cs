using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToggleMachines.MonoBehaviours;
using Harmony;

namespace ToggleMachines.Patches
{
    [HarmonyPatch(typeof(BaseSpotLight))]
    [HarmonyPatch("Start")]
    public class BaseSpotLight_Start_Patch
    {
        static void Prefix(BaseSpotLight __instance)
        {
            __instance.gameObject.AddComponent<SpotlightToggle>();
            Logger.Log("Added SpotlightToggle component to BaseSpotLight");
        }
    }

    [HarmonyPatch(typeof(BaseSpotLight))]
    [HarmonyPatch("GetLightsActive")]
    public class BaseSpotLight_GetLightsActive_Patch
    {
        static bool Prefix(BaseSpotLight __instance, ref bool __result)
        {
            var toggle = __instance.GetComponent<SpotlightToggle>();

            if (toggle != null && !toggle.IsOn)
            {
                __result = false;
                return false;
            }
            else
                return true;
        }
    }
}
