using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using ToggleAppliances.MonoBehaviours;
using Harmony;

namespace ToggleAppliances.Patches
{
    [HarmonyPatch(typeof(TechLight))]
    [HarmonyPatch("Start")]
    public class TechLight_Start_Patch
    {
        static void Prefix(TechLight __instance)
        {
            var mB = __instance.gameObject.AddComponent<FloodlightToggle>();
        }
    }

    [HarmonyPatch(typeof(TechLight))]
    [HarmonyPatch("UpdatePower")]
    public class TechLight_UpdatePower_Patche
    {
        static bool Prefix(TechLight __instance)
        {
            var toggle = __instance.GetComponent<FloodlightToggle>();

            ErrorMessage.AddMessage("Test " + toggle.isOn);
            var powerRelay = (PowerRelay)typeof(TechLight).GetField("powerRelay", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
            var setLightsActiveMethod = typeof(TechLight).GetMethod("SetLightsActive", BindingFlags.Instance | BindingFlags.NonPublic);
            var powerPerSecond = (float)typeof(TechLight).GetField("powerPerSecond", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var updateInterval = (float)typeof(TechLight).GetField("updateInterval", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            var searchingField = typeof(TechLight).GetField("searching", BindingFlags.Instance | BindingFlags.NonPublic);
            var searching = (bool)searchingField.GetValue(__instance);

            if (__instance.placedByPlayer && __instance.constructable.constructed)
            {
                if (powerRelay)
                {
                    if (powerRelay.GetPowerStatus() == PowerSystem.Status.Normal && toggle.isOn)
                    {
                        setLightsActiveMethod.Invoke(__instance, new object[] { true });
                        float num;
                        powerRelay.ConsumeEnergy(powerPerSecond * updateInterval, out num);
                    }
                    else
                    {
                        setLightsActiveMethod.Invoke(__instance, new object[] { false });
                    }
                }
                else
                {
                    setLightsActiveMethod.Invoke(__instance, new object[] { false });
                    if (!searching)
                    {
                        searchingField.SetValue(__instance, true);
                        __instance.InvokeRepeating("FindNearestValidRelay", 0f, 2f);
                    }
                }
            }

            return false;
        }
    }
}
