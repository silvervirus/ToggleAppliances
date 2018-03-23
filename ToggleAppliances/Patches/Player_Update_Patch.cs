using Harmony;
using System.Reflection;
using ToggleAppliances.MonoBehaviours;
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
                    var toggle = machine.GetComponent<FiltrationMachineToggle>();

                    var handReticle = HandReticle.main;
                    handReticle.SetIcon(HandReticle.IconType.Hand);
                    handReticle.SetInteractText("Toggle Filtration Machine");

                    if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
                    {
                        toggle.ToggleFiltrationMachine();
                    }
                }
            }
        }
    }
}
