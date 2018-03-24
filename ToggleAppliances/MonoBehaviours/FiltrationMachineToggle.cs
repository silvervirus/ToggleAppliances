using Oculus.Newtonsoft.Json;
using System.IO;
using System.Reflection;
using UnityEngine;
using System;

namespace ToggleMachines.MonoBehaviours
{
    public class FiltrationMachineToggle : MonoBehaviour, IProtoEventListener
    {
        #region Reflection
        private static readonly FieldInfo WorkingField =
            typeof(FiltrationMachine).GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo GetModelMethod =
            typeof(FiltrationMachine).GetMethod("GetModel",
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
            filtrationMachine = GetComponent<FiltrationMachine>();
            geo = (BaseFiltrationMachineGeometry)GetModelMethod.Invoke(filtrationMachine, new object[] { });

            identifier = GetComponentInParent<PrefabIdentifier>();
            id = identifier.Id;

            OnLoad();

            SetFiltrationMachineToggle(isOn);

            initialized = true;
        }

        public void OnSave()
        {
            Logger.Log("Serialize Called for FiltrationMachine");
            var currentState = (bool)WorkingField.GetValue(filtrationMachine);

            var savePathDir = Path.Combine(Main.GetSavePathDir(), "FiltrationMachines");
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

        public void OnLoad()
        {
            Logger.Log("Deserialize Called for FiltrationMachine");
            var savePathDir = Path.Combine(Main.GetSavePathDir(), "FiltrationMachines");
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

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            OnSave();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            OnLoad();
        }
    }
}
