using Harmony;
using System.Reflection;
using ToggleMachines.MonoBehaviours;

namespace ToggleMachines.Patches
{
    [HarmonyPatch(typeof(TechLight))]
    [HarmonyPatch("Start")]
    public class TechLight_Start_Patch
    {
        static void Prefix(TechLight __instance)
        {
            __instance.gameObject.AddComponent<FloodlightToggle>();
            Logger.Log("Added FloodlightToggle Component to TechLight!");
        }
    }

    [HarmonyPatch(typeof(TechLight))]
    [HarmonyPatch("UpdatePower")]
    public class TechLight_UpdatePower_Patche
    {
        //this way the reflection would be done only once, insted of getting this vars every function call.
        private static readonly FieldInfo PowerRelayInfo =
            typeof(TechLight).GetField("powerRelay", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo SetLightsActiveMethod =
            typeof(TechLight).GetMethod("SetLightsActive", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo PowerPerSecInfo =
            typeof(TechLight).GetField("powerPerSecond", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo UpdateIntervalInfo =
            typeof(TechLight).GetField("updateInterval", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo SearchingField =
            typeof(TechLight).GetField("searching", BindingFlags.Instance | BindingFlags.NonPublic);

        static bool Prefix(TechLight __instance)
        {
            var toggle = __instance.GetComponent<FloodlightToggle>();

            var powerRelay = (PowerRelay)PowerRelayInfo.GetValue(__instance);
            var powerPerSecond = (float)PowerPerSecInfo.GetValue(null);
            var updateInterval = (float)UpdateIntervalInfo.GetValue(null);

            var searching = (bool)SearchingField.GetValue(__instance);

            if (!__instance.placedByPlayer || !__instance.constructable.constructed) return false;
            if (powerRelay)
            {
                if (powerRelay.GetPowerStatus() == PowerSystem.Status.Normal && toggle.isOn)
                {
                    SetLightsActiveMethod.Invoke(__instance, new object[] { true });
                    float num;
                    powerRelay.ConsumeEnergy(powerPerSecond * updateInterval, out num);
                }
                else
                {
                    SetLightsActiveMethod.Invoke(__instance, new object[] { false });
                }
            }
            else
            {
                SetLightsActiveMethod.Invoke(__instance, new object[] { false });
                if (searching) return false;
                SearchingField.SetValue(__instance, true);
                __instance.InvokeRepeating("FindNearestValidRelay", 0f, 2f);
            }

            return false;
        }
    }
}
