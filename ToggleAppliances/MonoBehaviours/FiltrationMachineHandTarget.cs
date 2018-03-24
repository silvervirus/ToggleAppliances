using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ToggleAppliances.MonoBehaviours
{
    public class FiltrationMachineHandTarget : HandTarget, IHandTarget
    {
        private static readonly MethodInfo GetModuleMethod =
            typeof(BaseFiltrationMachineGeometry).GetMethod("GetModule",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private FiltrationMachine machine;
        private FiltrationMachineToggle toggle;
        private BaseFiltrationMachineGeometry geo;
        private PrefabIdentifier identifier;
        private string id;

        private bool initialized = false;
        private void Update()
        {
            if (!initialized)
                Init();
        }

        private void Init()
        {
            geo = GetComponentInParent<BaseFiltrationMachineGeometry>();

            machine = (FiltrationMachine)GetModuleMethod.Invoke(geo, new object[] { });
            identifier = machine.GetComponent<PrefabIdentifier>();
            id = identifier.Id;

            toggle = machine.GetComponent<FiltrationMachineToggle>();

            initialized = true;
        }

        public void OnHandClick(GUIHand hand)
        {
            if (machine.constructed < 1f) return;

            toggle.ToggleFiltrationMachine();
        }

        public void OnHandHover(GUIHand hand)
        {
            if (machine.constructed < 1f) return;

            var handReticle = HandReticle.main;
            handReticle.SetIcon(HandReticle.IconType.Hand);
            handReticle.SetInteractText("Toggle Filtration Machine");
        }
    }
}
