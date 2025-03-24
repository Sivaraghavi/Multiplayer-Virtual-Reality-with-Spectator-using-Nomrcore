/*

using Normal.Realtime;
using Normal.Realtime.Serialization;
using TMPro;
using UnityEngine;

public class NetworkedAvatar : RealtimeComponent<NetworkedAvatar.AvatarModel>
{
    #region Model Definition
    [RealtimeModel]
    public partial class AvatarModel
    {
        [RealtimeProperty(1, true, true)] private string _playerName;
        [RealtimeProperty(2, true, true)] private int _colorIndex;
    }
    #endregion

    #region References
    [SerializeField] private TextMeshProUGUI nameTag;           // Assign in Inspector: Name tag UI element
    [SerializeField] private MeshRenderer[] bodyMeshes;         // Assign in Inspector: Avatar mesh renderers
    [SerializeField] private SkinnedMeshRenderer[] bodySkinnedMeshes; // Assign in Inspector: Skinned mesh renderers (if any)
    [SerializeField] private Material[] colorMaterials;         // Assign in Inspector: Array of materials (e.g., Red, Blue, etc.)
    #endregion

    #region Model Handling
    private void Awake()
    {
        if (nameTag == null || (bodyMeshes.Length == 0 && bodySkinnedMeshes.Length == 0) || colorMaterials.Length == 0)
        {
            Debug.LogError("NetworkedAvatar: Missing required references in Inspector!");
        }
    }

    protected override void OnRealtimeModelReplaced(AvatarModel previousModel, AvatarModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.playerNameDidChange -= UpdatePlayerName;
            previousModel.colorIndexDidChange -= UpdateColorIndex;
        }
        if (currentModel != null)
        {
            currentModel.playerNameDidChange += UpdatePlayerName;
            currentModel.colorIndexDidChange += UpdateColorIndex;
            UpdateVisuals(); // Initial update
        }
    }
    #endregion

    #region Visual Updates
    private void UpdatePlayerName(AvatarModel model, string value) => UpdateVisuals();
    private void UpdateColorIndex(AvatarModel model, int value) => UpdateVisuals();

    private void UpdateVisuals()
    {
        if (model == null) return;

        nameTag.text = model.playerName;

        if (model.colorIndex >= 0 && model.colorIndex < colorMaterials.Length)
        {
            Material selectedMaterial = colorMaterials[model.colorIndex];
            foreach (var mesh in bodyMeshes)
            {
                mesh.material = new Material(selectedMaterial); // Instance material to avoid shared changes
            }
            foreach (var skinnedMesh in bodySkinnedMeshes)
            {
                skinnedMesh.material = new Material(selectedMaterial);
            }
        }
    }
    #endregion

    #region Initialization
    private void Start()
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            model.playerName = CustomizationData.PlayerName;
            model.colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
            UpdateVisuals(); // Immediate local update
        }
    }
    #endregion
}*/