using Oculus.Newtonsoft.Json;
using System.IO;
using System.Reflection;
using UnityEngine;
using System;

namespace ToggleAppliances.MonoBehaviours
{
    public class FiltrationMachineToggle : HandTarget, IHandTarget, IProtoEventListener
    {
        #region Reflection
        private static readonly FieldInfo WorkingField =
            typeof(FiltrationMachine).GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo GetModuleMethod =
        typeof(BaseFiltrationMachineGeometry).GetMethod("GetModule",
            BindingFlags.Instance | BindingFlags.NonPublic);
        #endregion

        private BaseFiltrationMachineGeometry geo;
        private FiltrationMachine filtrationMachine;
        private PrefabIdentifier identifier;
        private string id;

        private bool isOn;

        private bool initialized = false;
        private void Update()
        {
            if (!initialized)
                Initialize();
        }

        public void Initialize()
        {
            geo = GetComponentInParent<BaseFiltrationMachineGeometry>();
            filtrationMachine = (FiltrationMachine)GetModuleMethod.Invoke(geo, new object[] { });

            Logger.Log("Test: " + geo);
            Logger.Log("T2: " + filtrationMachine);

            identifier = GetComponentInParent<PrefabIdentifier>();
            id = identifier.Id;

            OnProtoDeserialize(null);

            SetFiltrationMachineToggle(isOn);

            initialized = true;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            Logger.Log("Serialize Called for FiltrationMachine");
            var currentState = (bool)WorkingField.GetValue(filtrationMachine);

            var savePathDir = Main.GetSavePathDir();
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if (!Directory.Exists(savePathDir))
            {
                Directory.CreateDirectory(savePathDir);
            }

            var localSaveData = new FiltrationMachineSaveData()
            {
                Working = currentState
            };

            string json = JsonConvert.SerializeObject(localSaveData, Formatting.Indented);
            File.WriteAllText(saveFile, json);
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            Logger.Log("Deserialize Called for FiltrationMachine");
            var savePathDir = Main.GetSavePathDir();
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if (File.Exists(saveFile))
            {
                var rawJson = File.ReadAllText(saveFile);
                isOn = JsonConvert.DeserializeObject<FiltrationMachineSaveData>(rawJson).Working;
            }
            else
            {
                isOn = true;
            }
        }

        public void ToggleFiltrationMachine()
        {
            Logger.Log("ToggleFiltrationMachine called");
            var working = (bool)WorkingField.GetValue(filtrationMachine);

            SetFiltrationMachineToggle(!working);
        }

        //In this case you were using the same reflection twice...
        public void SetFiltrationMachineToggle(bool toggle)
        {
            Logger.Log("SetFiltrationMachineToggle called with " + toggle);
            if (!toggle)
            {
                filtrationMachine.CancelInvoke("UpdateFiltering");
                filtrationMachine.workSound.Stop();
                filtrationMachine.vfxController.Stop(1);

                geo.SetWorking(false, transform.position.y);

                WorkingField.SetValue(filtrationMachine, false);
            }
            else
            {
                filtrationMachine.InvokeRepeating("UpdateFiltering", 1f, 1f);
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if (filtrationMachine.constructed < 1f) return;

            var handReticle = HandReticle.main;
            handReticle.SetIcon(HandReticle.IconType.Hand);
            handReticle.SetInteractText("Toggle Filtration Machine");
        }

        public void OnHandClick(GUIHand hand)
        {
            if (filtrationMachine.constructed < 1f) return;

            ToggleFiltrationMachine();
        }
    }
}
