using Harmony;
using System;
using System.IO;
using System.Reflection;

namespace ToggleAppliances
{
    public class Main
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.ahk1221.toggleappliances");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.Log("Successfully patched!");
        }

        public static string GetSavePathDir()
        {
            var savePathDir = Path.Combine(@".\SNAppData\SavedGames\", Utils.GetSavegameDir());
            return Path.Combine(savePathDir, "ToggleAppliances");
        }
    }
}
