/*using Normal.Realtime;
using Normal.Realtime.Serialization;
using TMPro;
using UnityEngine;

public class NetworkedAvatar : RealtimeComponent<NetworkedAvatar.Model>
{
    #region Model Definition
    public class Model : RealtimeModel
    {
        private string _playerName;
        private int _colorIndex;

        public string playerName
        {
            get => _playerName;
            set
            {
                if (_playerName == value) return;
                _playerName = value;

                // Removed marking dirty since no helper method is available.
            }
        }

        public int colorIndex
        {
            get => _colorIndex;
            set
            {
                if (_colorIndex == value) return;
                _colorIndex = value;
                // Removed marking dirty since no helper method is available.
            }
        }

        protected override void Write(WriteStream stream, StreamContext context)
        {
            stream.WriteStructString(_playerName);
            stream.WriteStructInt(_colorIndex);
        }

        protected override void Read(ReadStream stream, StreamContext context)
        {
            _playerName = stream.ReadStructString();
            _colorIndex = stream.ReadStructInt();
        }

        protected override int WriteLength(StreamContext context)
        {
            return System.Text.Encoding.UTF8.GetByteCount(_playerName) + sizeof(int);
        }
    }
    #endregion

    #region References
    [SerializeField] TextMeshProUGUI nameTag;
    [SerializeField] MeshRenderer[] bodyMeshes;
    [SerializeField] Material[] colorMaterials;
    #endregion

    #region Model Handling
    protected override void OnRealtimeModelReplaced(Model previousModel, Model currentModel)
    {
        UpdateVisuals();
    }
    #endregion

    #region Visual Updates
    private void UpdateVisuals()
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
    #endregion

    #region Initialization
    void Start()
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            model.playerName = CustomizationData.PlayerName;
            model.colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        }
    }
    #endregion
}
*/

using Normal.Realtime;
using Normal.Realtime.Serialization;
using TMPro;
using UnityEngine;

public class NetworkedAvatar : RealtimeComponent<NetworkedAvatar.Model>
{
    #region Model Definition
    public class Model : RealtimeModel
    {
        private string _playerName;
        private int _colorIndex;

        public string playerName
        {
            get => _playerName;
            set
            {
                if (_playerName == value) return;
                _playerName = value;
                // If your version requires marking the model as dirty,
                // you may need to add that call here.
            }
        }

        public int colorIndex
        {
            get => _colorIndex;
            set
            {
                if (_colorIndex == value) return;
                _colorIndex = value;
                // Mark model as dirty if required.
            }
        }

        protected override void Write(WriteStream stream, StreamContext context)
        {
            stream.WriteStructString(_playerName);
            stream.WriteStructInt(_colorIndex);
        }

        protected override void Read(ReadStream stream, StreamContext context)
        {
            _playerName = stream.ReadStructString();
            _colorIndex = stream.ReadStructInt();
        }

        protected override int WriteLength(StreamContext context)
        {
            return System.Text.Encoding.UTF8.GetByteCount(_playerName) + sizeof(int);
        }
    }
    #endregion

    #region References
    [SerializeField] TextMeshProUGUI nameTag;
    [SerializeField] MeshRenderer[] bodyMeshes;
    [SerializeField] SkinnedMeshRenderer[] bodySkinnedMeshes;
    [SerializeField] Material[] colorMaterials;
    #endregion

    #region Model Handling
    protected override void OnRealtimeModelReplaced(Model previousModel, Model currentModel)
    {
        UpdateVisuals();
    }
    #endregion

    #region Visual Updates
    private void UpdateVisuals()
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
            foreach (var skinnedMesh in bodySkinnedMeshes)
            {
                Material[] materials = skinnedMesh.materials;
                materials[0] = colorMaterials[model.colorIndex];
                skinnedMesh.materials = materials;
            }
        }
    }
    #endregion

    #region Initialization
    void Start()
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            model.playerName = CustomizationData.PlayerName;
            model.colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        }
    }
    #endregion
}
