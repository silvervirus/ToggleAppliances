using ToggleAppliances.MonoBehaviours;
using System;
using UnityEngine;
using Harmony;

namespace ToggleAppliances.Patches
{
    [HarmonyPatch(typeof(BaseFiltrationMachineGeometry))]
    [HarmonyPatch("Start")]
    public class FiltrationMachine_Start_Patch
    {
        static void Postfix(BaseFiltrationMachineGeometry __instance)
        {
            foreach(var t in __instance.GetComponentsInChildren<Transform>())
            {
                if(t.name == "Capsule")
                {
                    if(t.gameObject.GetComponent<FiltrationMachineToggle>() == null)
                    {
                        t.gameObject.AddComponent<FiltrationMachineToggle>();
                        Logger.Log("Added FiltrationMachineToggle component to FiltrationMachine!");
                    }
                }
            }
        }
    }
}
