using Harmony;
using System.Reflection;
using UnityEngine;

namespace ToggleAppliances.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class Player_Update_patch
    {
        static void Prefix()
        {
            var go = default(GameObject);
            var dist = 0f;

            if (Targeting.GetTarget(Player.main.gameObject, 1f, out go, out dist))
            {
                if (go.GetComponentInParent<BaseFiltrationMachineGeometry>() != null && go.name != "HandTarget")
                {
                    var geo = go.GetComponentInParent<BaseFiltrationMachineGeometry>();
                    var getModuleMethod = geo.GetType().GetMethod("GetModule", BindingFlags.Instance | BindingFlags.NonPublic);
                    var machine = (FiltrationMachine)getModuleMethod.Invoke(geo, new object[] { });

                    var handReticle = HandReticle.main;
                    handReticle.SetIcon(HandReticle.IconType.Hand);
                    handReticle.SetInteractText("Toggle Filtration Machine");

                    if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
                    {
                        var workingField = machine.GetType().GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);
                        var working = (bool)workingField.GetValue(machine);

                        if(working)
                        {
                            machine.CancelInvoke("UpdateFiltering");
                            machine.workSound.Stop();
                            machine.vfxController.Stop(1);

                            geo.SetWorking(false, go.transform.position.y);

                            workingField.SetValue(machine, false);
                        }
                        else
                        {
                            machine.InvokeRepeating("UpdateFiltering", 1f, 1f);
                            machine.workSound.Play();
                            machine.vfxController.Play(1);

                            geo.SetWorking(true, go.transform.position.y);

                            workingField.SetValue(machine, true);
                        }
                    }
                }
            }
        }
    }
}
