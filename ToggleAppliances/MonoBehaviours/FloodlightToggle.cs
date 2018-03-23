using Oculus.Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace ToggleAppliances.MonoBehaviours
{
    public class FloodlightToggle : HandTarget, IHandTarget, IProtoEventListener
    {
        private static readonly MethodInfo SetLightsActiveMethod =
            typeof(TechLight).GetMethod("SetLightsActive", BindingFlags.Instance | BindingFlags.NonPublic);

        public bool isOn;

        private TechLight techLight;
        private PrefabIdentifier identifier;
        private string id;

        private bool initialized = false;
        private void Update()
        {
            if (!initialized)
                Init();
        }

        public void Init()
        {
            isOn = true;

            techLight = GetComponent<TechLight>();
            identifier = GetComponentInParent<PrefabIdentifier>();
            id = identifier.Id;

            OnProtoDeserialize(null);

            initialized = true;
        }

        public void OnHandClick(GUIHand hand)
        {
            if(techLight.constructable.constructed)
            {
                isOn = !isOn;
                SetLightsActiveMethod.Invoke(techLight, new object[] { isOn });
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if(techLight.constructable.constructed)
            {
                var handReticle = HandReticle.main;
                handReticle.SetIcon(HandReticle.IconType.Hand);
                handReticle.SetInteractText("Toggle Floodlight");
            }
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            Logger.Log("Deserialize Called for FloodlightToggle");

            var savePathDir = Main.GetSavePathDir();
            var savePath = Path.Combine(savePathDir, id + ".json");

            if(File.Exists(savePath))
            {
                var rawJson = File.ReadAllText(savePath);
                var saveData = JsonConvert.DeserializeObject<FloodlightSaveData>(rawJson);

                isOn = saveData.IsOn;
            }
            else
            {
                isOn = true;
            }
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            Logger.Log("Serialize Called for FloodlightToggle");

            var savePathDir = Main.GetSavePathDir();
            var savePath = Path.Combine(savePathDir, id + ".json");

            if (!Directory.Exists(savePathDir))
                Directory.CreateDirectory(savePathDir);

            var saveData = new FloodlightSaveData()
            {
                IsOn = isOn
            };

            var json = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(savePath, json);
        }
    }
}
