using System.IO;
using System.Reflection;
using Harmony;
using Oculus.Newtonsoft.Json;

namespace ToggleAppliances.Patches
{
    [HarmonyPatch(typeof(FiltrationMachine))]
    [HarmonyPatch("OnProtoDeserialize")]
    public class FiltrationMachine_OnProtoDeserialize_Patch
    {
        static void Prefix(FiltrationMachine __instance)
        {
            var identifier = __instance.GetComponentInParent<PrefabIdentifier>();
            var id = identifier.Id;

            var savePathDir = Main.GetSavePathDir();
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if(File.Exists(saveFile))
            {
                var rawJson = File.ReadAllText(saveFile);
                var saveData = JsonConvert.DeserializeObject<FiltrationMachineSaveData>(rawJson);

                var working = saveData.Working;

                var workingField = __instance.GetType().GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);

                var geoMethod = __instance.GetType().GetMethod("GetModel", BindingFlags.Instance | BindingFlags.NonPublic);
                var geo = (BaseFiltrationMachineGeometry)geoMethod.Invoke(__instance, new object[] { });

                if (working)
                {
                    __instance.CancelInvoke("UpdateFiltering");
                    __instance.workSound.Stop();
                    __instance.vfxController.Stop(1);

                    geo.SetWorking(false, __instance.transform.position.y);

                    workingField.SetValue(__instance, false);
                }
                else
                {
                    __instance.InvokeRepeating("UpdateFiltering", 1f, 1f);
                    __instance.workSound.Play();
                    __instance.vfxController.Play(1);

                    geo.SetWorking(true, __instance.transform.position.y);

                    workingField.SetValue(__instance, true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(FiltrationMachine))]
    [HarmonyPatch("OnProtoSerialize")]
    public class FiltrationMachine_OnProtoSerialize_Patch
    {
        static void Prefix(FiltrationMachine __instance)
        {
            var currentState = (bool)__instance.GetType().GetField("working", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

            var savePathDir = Main.GetSavePathDir();

            var identifier = __instance.GetComponentInParent<PrefabIdentifier>();
            var id = identifier.Id;

            var saveFile = Path.Combine(savePathDir, id + ".json");

            if(!Directory.Exists(savePathDir))
            {
                Directory.CreateDirectory(savePathDir);
            }

            var saveData = new FiltrationMachineSaveData()
            {
                Working = currentState
            };

            string json = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(saveFile, json);
        }
    }
}
