using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ToggleAppliances.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class BaseFiltrationMachineGeometry_OnHover_Patch
    {
        static void Prefix()
        {
            var go = default(GameObject);
            var dist = 0f;
            if (Targeting.GetTarget(Player.main.gameObject, 1f, out go, out dist))
            {
                if (go.GetComponentInParent<BaseFiltrationMachineGeometry>() != null)
                {
                    var geo = go.GetComponentInParent<BaseFiltrationMachineGeometry>();

                    var getModuleMethod = geo.GetType().GetMethod("GetModule", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    var machine = (FiltrationMachine)getModuleMethod.Invoke(geo, new object[] { });
                    var handReticle = HandReticle.main;

                    handReticle.SetIcon(HandReticle.IconType.Hand);
                    handReticle.SetInteractText("Toggle Filtration Machine");

                    if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
                    {
                        var working = (bool)machine.GetType().GetField("working", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(machine);

                        if(working)
                        {
                            machine.CancelInvoke("UpdateFiltering");
                            machine.workSound.Stop();
                            machine.vfxController.Stop(1);

                            var shownModelVFXScan = machine.GetType().GetField("shownModelVfxScan", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(machine) as VFXScan;
                            var shownModel = machine.GetType().GetField("shownModel", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(machine) as GameObject;

                            if (shownModel != null && shownModelVFXScan != null)
                                geo.SetWorking(false, shownModelVFXScan.GetCurrentYPos());

                            machine.GetType().GetField("working", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(machine, false);
                        }
                        else
                        {
                            machine.InvokeRepeating("UpdateFiltering", 1f, 1f);

                            var shownModelVFXScan = machine.GetType().GetField("shownModelVfxScan", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(machine) as VFXScan;
                            var shownModel = machine.GetType().GetField("shownModel", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(machine) as GameObject;

                            if (shownModel != null && shownModelVFXScan != null)
                                geo.SetWorking(true, shownModelVFXScan.GetCurrentYPos());

                            machine.GetType().GetField("working", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(machine, true);
                        }
                    }
                }
            }
        }
    }
}
