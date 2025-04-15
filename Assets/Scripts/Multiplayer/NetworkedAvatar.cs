/*
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using TMPro;

[RealtimeModel]
public partial class AvatarModel
{
    [RealtimeProperty(1, true, true)] private string _playerName;
    [RealtimeProperty(2, true, true)] private int _colorIndex;
}

[RequireComponent(typeof(RealtimeView), typeof(RealtimeTransform))]
public class NetworkedAvatar : RealtimeComponent<AvatarModel>
{
    public TextMeshProUGUI nameTag;
    public MeshRenderer[] bodyMeshes;

    private void Start()
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            model.playerName = PlayerPrefs.GetString("PlayerName", "Player");
            model.colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        }

        // Initial update
        UpdateAvatarAppearance();
    }

    protected override void OnRealtimeModelReplaced(AvatarModel previousModel, AvatarModel currentModel)
    {
        if (previousModel != null)
        {
            // No need to unsubscribe from events since we’re not using custom delegates
        }

        if (currentModel != null)
        {
            // Update appearance when the model is set or replaced
            UpdateAvatarAppearance();
            // Since Normcore syncs automatically, we rely on Update() or a custom sync method
        }
    }

    private void Update()
    {
        // Continuously check for model updates (less efficient but works for now)
        if (model != null)
        {
            UpdateAvatarAppearance();
        }
    }

    private void UpdateAvatarAppearance()
    {
        if (nameTag != null && model != null)
        {
            nameTag.text = model.playerName;
        }

        if (bodyMeshes != null && model != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = GetColorFromIndex(model.colorIndex);
            foreach (var mesh in bodyMeshes)
            {
                if (mesh != null)
                {
                    mesh.material = mat;
                }
            }
        }
    }

    private Color GetColorFromIndex(int index)
    {
        switch (index)
        {
            case 0: return Color.red;
            case 1: return Color.blue;
            case 2: return Color.green;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            default: return Color.white;
        }
    }
}*/

/*using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using TMPro;

[RealtimeModel]
public partial class AvatarModel
{
    [RealtimeProperty(1, true, true)] private string _playerName;
    [RealtimeProperty(2, true, true)] private int _colorIndex;
}

[RequireComponent(typeof(RealtimeView), typeof(RealtimeTransform))]
public class NetworkedAvatar : RealtimeComponent<AvatarModel>
{
    public TextMeshProUGUI nameTag;
    public MeshRenderer[] bodyMeshes;
    [SerializeField] private Material[] colorMaterials; // Assign materials in Inspector (Red, Blue, Green, Yellow, Cyan)

    private void Start()
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            model.playerName = PlayerPrefs.GetString("PlayerName", "Player");
            model.colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        }

        UpdateAvatarAppearance();
    }

    protected override void OnRealtimeModelReplaced(AvatarModel previousModel, AvatarModel currentModel)
    {
        if (currentModel != null)
        {
            UpdateAvatarAppearance();
        }
    }

    private void Update()
    {
        if (model != null)
        {
            UpdateAvatarAppearance();
        }
    }

    private void UpdateAvatarAppearance()
    {
        if (nameTag != null && model != null)
        {
            nameTag.text = model.playerName;
        }

        if (bodyMeshes != null && model != null && colorMaterials != null && colorMaterials.Length > 0)
        {
            int index = Mathf.Clamp(model.colorIndex, 0, colorMaterials.Length - 1);
            Material mat = colorMaterials[index];
            foreach (var mesh in bodyMeshes)
            {
                if (mesh != null)
                {
                    mesh.material = mat;
                }
            }
        }
    }
}*/


// NetworkedAvatar.cs
// Consolidated avatar customization and synchronization
using UnityEngine;
using Normal.Realtime;
using TMPro;

[RealtimeModel]
public partial class AvatarModel
{
    [RealtimeProperty(1, true, true)] private string _playerName;
    [RealtimeProperty(2, true, true)] private int _colorIndex;
}

[RequireComponent(typeof(RealtimeView), typeof(RealtimeTransform))]
public class NetworkedAvatar : RealtimeComponent<AvatarModel>
{
    public TextMeshProUGUI nameTag;
    public MeshRenderer[] bodyMeshes;
    public Transform headTransform; // Assign head bone in Inspector
    public Vector3 nameTagOffset = new Vector3(0, 0.2f, 0);
    [SerializeField] private Material[] colorMaterials; // Assign in Inspector (Red, Blue, Green, Yellow, Cyan)
    private Camera mainCamera;

    private void Awake()
    {
        // Create material instances to avoid shared materials
        foreach (var renderer in bodyMeshes)
        {
            if (renderer != null)
                renderer.material = new Material(renderer.material);
        }
        mainCamera = Camera.main; // Cache Camera.main
    }

    private void Start()
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            model.playerName = PlayerPrefs.GetString("PlayerName", "Player");
            model.colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        }
    }

    protected override void OnRealtimeModelReplaced(AvatarModel previousModel, AvatarModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.playerNameDidChange -= UpdatePlayerName;
            previousModel.colorIndexDidChange -= UpdateColor;
        }
        if (currentModel != null)
        {
            currentModel.playerNameDidChange += UpdatePlayerName;
            currentModel.colorIndexDidChange += UpdateColor;
            UpdatePlayerName(currentModel, currentModel.playerName);
            UpdateColor(currentModel, currentModel.colorIndex);
        }
    }

    private void UpdatePlayerName(AvatarModel model, string name)
    {
        if (nameTag != null)
            nameTag.text = name;
    }

    private void UpdateColor(AvatarModel model, int index)
    {
        if (bodyMeshes == null || colorMaterials == null || colorMaterials.Length == 0) return;
        int clampedIndex = Mathf.Clamp(index, 0, colorMaterials.Length - 1);
        foreach (var mesh in bodyMeshes)
        {
            if (mesh != null)
                mesh.material.color = colorMaterials[clampedIndex].color;
        }
    }

    private void Update()
    {
        // Update name tag position and orientation
        if (nameTag != null && headTransform != null && mainCamera != null)
        {
            nameTag.transform.position = headTransform.position + nameTagOffset;
            nameTag.transform.rotation = Quaternion.LookRotation(nameTag.transform.position - mainCamera.transform.position);
        }
    }

    // Called by RoomManager to link XR Rig
    public void LinkXRRig(Transform camera, Transform leftHand, Transform rightHand)
    {
        if (realtimeView.isOwnedLocallySelf)
        {
            // Align avatar with XR Rig's camera
            if (headTransform != null && camera != null)
            {
                Transform rootTransform = transform;
                Vector3 headLocalPos = headTransform.localPosition;
                Vector3 cameraWorldPos = camera.position;
                Vector3 desiredRootPos = cameraWorldPos - rootTransform.TransformVector(headLocalPos);
                rootTransform.position = desiredRootPos;
            }
            // Optional: Adjust rotation or hand tracking here
        }
    }
}