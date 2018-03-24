using System.IO;
using System.Reflection;
using Oculus.Newtonsoft.Json;

namespace ToggleAppliances.MonoBehaviours
{
    public class SpotlightToggle : HandTarget, IHandTarget, IProtoEventListener
    {
        private static readonly PropertyInfo PoweredProperty =
            typeof(BaseSpotLight).GetProperty("powered", BindingFlags.Instance | BindingFlags.NonPublic);

        public bool IsOn;

        private BaseSpotLight spotLight;
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
            spotLight = GetComponent<BaseSpotLight>();
            identifier = GetComponent<PrefabIdentifier>();
            id = identifier.Id;

            OnProtoDeserialize(null);

            initialized = true;
        }

        public void OnHandClick(GUIHand hand)
        {
            if (!spotLight.constructed) return;

            IsOn = !IsOn;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!spotLight.constructed) return;

            var handReticle = HandReticle.main;
            handReticle.SetIcon(HandReticle.IconType.Hand);
            handReticle.SetInteractText("Toggle Spotlight");
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            var savePathDir = Path.Combine(Main.GetSavePathDir(), "Spotlights");
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if (File.Exists(saveFile))
            {
                var json = File.ReadAllText(saveFile);
                var saveData = JsonConvert.DeserializeObject<LightSaveData>(json);

                IsOn = saveData.IsOn;
            }
            else
                IsOn = true;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            var savePathDir = Path.Combine(Main.GetSavePathDir(), "Spotlights");
            var saveFile = Path.Combine(savePathDir, id + ".json");

            if (!Directory.Exists(savePathDir))
                Directory.CreateDirectory(savePathDir);

            var saveData = new LightSaveData()
            {
                IsOn = IsOn
            };

            var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(saveFile, json);
        }
    }
}
