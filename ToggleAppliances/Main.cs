using Harmony;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using QModManager.API.ModLoading;

namespace ToggleMachines
{
    [QModCore]
    public class Qpatch
    {
        [QModPatch]
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.ahk1221.toggleappliances");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.Log("Successfully patched!");
        }

        public static string GetSavePathDir()
        {
             var savePathDir = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "ToggleAppliances");
            return Path.Combine(savePathDir, "ToggleAppliances")
        }
    }
}
