using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;

namespace ToggleAppliances.Patches
{
    [HarmonyPatch(typeof(BaseFiltrationMachineGeometry))]
    [HarmonyPatch("OnUse")]
    public class BaseFiltrationMachineGeometry_OnUse_Patch
    {
        static void Prefix(BaseFiltrationMachineGeometry __instance)
        {
            ErrorMessage.AddMessage("[ToggleAppliances] OnUse called!");
        }
    }

}
