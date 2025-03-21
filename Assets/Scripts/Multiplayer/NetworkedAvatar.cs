/*using Normal.Realtime;
using Normal.Realtime.Serialization;
using TMPro;
using UnityEngine;

public class NetworkedAvatar : RealtimeComponent<NetworkedAvatar.Model>
{
    // === Model Definition ===
    public class Model : RealtimeModel
    {
        // Synced properties
        private string _playerName;
        private int _colorIndex;

        public string playerName
        {
            get => _playerName;
            set
            {
                if (_playerName == value) return;
                _playerName = value;
                NotifyPropertyChanged();
            }
        }

        public int colorIndex
        {
            get => _colorIndex;
            set
            {
                if (_colorIndex == value) return;
                _colorIndex = value;
                NotifyPropertyChanged();
            }
        }

        // Serialization
        protected override void Write(WriteStream stream, StreamContext context)
        {
            base.Write(stream, context);
            stream.WriteString(_playerName);
            stream.WriteInt(_colorIndex);
        }

        protected override void Read(ReadStream stream, StreamContext context)
        {
            base.Read(stream, context);
            _playerName = stream.ReadString();
            _colorIndex = stream.ReadInt();
            NotifyModelDidChange();
        }
    }

    // === References ===
    [SerializeField] TextMeshProUGUI nameTag;
    [SerializeField] MeshRenderer[] bodyMeshes;
    [SerializeField] Material[] colorMaterials;

    // === Model Handling ===
    protected override void OnRealtimeModelReplaced(Model previousModel, Model currentModel)
    {
        if (previousModel != null)
            previousModel.modelDidChange -= OnModelDidChange;

        if (currentModel != null)
        {
            if (!currentModel.isFreshModel)
                UpdateVisuals(currentModel);

            currentModel.modelDidChange += OnModelDidChange;
        }
    }

    // === Updates ===
    private void OnModelDidChange(Model model)
    {
        UpdateVisuals(model);
    }

    private void UpdateVisuals(Model model)
    {
        nameTag.text = model.playerName;

        if (model.colorIndex >= 0 && model.colorIndex < colorMaterials.Length)
        {
            foreach (var mesh in bodyMeshes)
            {
                var materials = mesh.materials;
                materials[0] = colorMaterials[model.colorIndex];
                mesh.materials = materials;
            }
        }
    }

    // === Initialization ===
    void Start()
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            model.playerName = CustomizationData.PlayerName;
            model.colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        }
    }
}*/