using System.IO;
using Oculus.Newtonsoft.Json;
using System;
using System.Reflection;
using UnityEngine;

namespace ToggleAppliances.MonoBehaviours
{
    public class FiltrationMachineToggle : MonoBehaviour, IProtoEventListener
    {
        private FiltrationMachine filtrationMachine;
        private PrefabIdentifier identifier;
        private string id;

        private FiltrationMachineSaveData saveData;

        private bool initialized = false;
        private void Update()
        {
            if (!initialized)
                Initialize();
        }

        public void Initialize()
        {
            filtrationMachine = GetComponent<FiltrationMachine>();
            identifier = GetComponentInParent<PrefabIdentifier>();
            id = identifier.Id;

            OnProtoDeserialize(null);

            SetFiltrationMachineToggle(saveData.Working);

            initialized = true;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            var field = filtrationMachine.GetType().GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);
            var currentState = (bool)field.GetValue(filtrationMachine);

            var savePathDir = Main.GetSavePathDir();
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if (!Directory.Exists(savePathDir))
            {
                Directory.CreateDirectory(savePathDir);
            }

            var saveData = new FiltrationMachineSaveData()
            {
                Working = currentState
            };

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(saveFile, json);
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            var savePathDir = Main.GetSavePathDir();
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if(File.Exists(saveFile))
            {
                var rawJson = File.ReadAllText(saveFile);
                saveData = JsonConvert.DeserializeObject<FiltrationMachineSaveData>(rawJson);
            }
            else
            {
                saveData = new FiltrationMachineSaveData()
                {
                    Working = true
                };
            }
        }

        public void ToggleFiltrationMachine()
        {
            var field = filtrationMachine.GetType().GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);
            var working = (bool)field.GetValue(filtrationMachine);

            SetFiltrationMachineToggle(!working);
        }

        public void SetFiltrationMachineToggle(bool toggle)
        {
            var field = filtrationMachine.GetType().GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);

            var geoMethod = filtrationMachine.GetType().GetMethod("GetModel", BindingFlags.Instance | BindingFlags.NonPublic);
            var geo = (BaseFiltrationMachineGeometry)geoMethod.Invoke(filtrationMachine, new object[] { });

            if (!toggle)
            {
                filtrationMachine.CancelInvoke("UpdateFiltering");
                filtrationMachine.workSound.Stop();
                filtrationMachine.vfxController.Stop(1);

                geo.SetWorking(false, transform.position.y);

                field.SetValue(filtrationMachine, false);
            }
            else
            {
                filtrationMachine.InvokeRepeating("UpdateFiltering", 1f, 1f);
            }
        }
    }
}
