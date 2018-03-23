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
        private static readonly MethodInfo GetModuleMethod =
                typeof(BaseFiltrationMachineGeometry).GetMethod("GetModule",
                    BindingFlags.Instance | BindingFlags.NonPublic)
            ;
        static void Prefix()
        {
            //this are just my preference (^o^)
            GameObject go = null;
            var dist = 0f;

            if (Targeting.GetTarget(Player.main.gameObject, 1f, out go, out dist))
            {
                if (go.GetComponentInParent<BaseFiltrationMachineGeometry>() != null && go.name != "HandTarget")
                {
                    var geo = go.GetComponentInParent<BaseFiltrationMachineGeometry>();
                    var machine = (FiltrationMachine)GetModuleMethod.Invoke(geo, new object[] { });
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
