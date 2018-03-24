using ToggleMachines.MonoBehaviours;
using System;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace ToggleMachines.Patches
{
    [HarmonyPatch(typeof(BaseFiltrationMachineGeometry))]
    [HarmonyPatch("Start")]
    public class BaseFiltrationMachineGeometry_Start_Patch
    {
        static void Postfix(BaseFiltrationMachineGeometry __instance)
        {
            foreach(var t in __instance.GetComponentsInChildren<Transform>())
            {
                if(t.name == "Capsule")
                {
                    if(t.gameObject.GetComponent<FiltrationMachineHandTarget>() == null)
                    {
                        t.gameObject.AddComponent<FiltrationMachineHandTarget>();
                        Logger.Log("Added FiltrationMachineHandTarget component to FiltrationMachine!");
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(FiltrationMachine))]
    [HarmonyPatch("Start")]
    public class FiltrationMachine_Start_Patch
    {
        static void Prefix(FiltrationMachine __instance)
        {
            if(__instance.gameObject.GetComponent<FiltrationMachineToggle>() == null)
            {
                __instance.gameObject.AddComponent<FiltrationMachineToggle>();
                Logger.Log("Added FiltrationMachineToggle component to FiltrationMachine!");
            }
        }
    }
}
