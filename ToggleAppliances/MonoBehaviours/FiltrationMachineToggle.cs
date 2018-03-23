using Oculus.Newtonsoft.Json;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ToggleAppliances.MonoBehaviours
{
    public class FiltrationMachineToggle : MonoBehaviour, IProtoEventListener
    {
        #region Reflection
        private static readonly FieldInfo WorkingField =
            typeof(FiltrationMachine).GetField("working", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo GetModelMethod =
            typeof(FiltrationMachine).GetMethod("GetModel", BindingFlags.Instance | BindingFlags.NonPublic);
        #endregion

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
            var savePathDir = Main.GetSavePathDir();
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if (File.Exists(saveFile))
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
            var working = (bool)WorkingField.GetValue(filtrationMachine);

            SetFiltrationMachineToggle(!working);
        }

        //In this case you were using the same reflection twice...
        public void SetFiltrationMachineToggle(bool toggle)
        {
            var geo = (BaseFiltrationMachineGeometry)GetModelMethod.Invoke(filtrationMachine, new object[] { });

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
    }
}
