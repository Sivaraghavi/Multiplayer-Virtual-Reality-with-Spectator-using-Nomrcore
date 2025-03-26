/*// AvatarInitializer.cs
using UnityEngine;
using Normal.Realtime;

public class AvatarInitializer : MonoBehaviour
{
    public Realtime realtime;
    public GameObject avatarPrefab;
    public Material[] colorMaterials;

    void Start()
    {
        // Load customization data
        CustomizationData.PlayerName = PlayerPrefs.GetString("PlayerName", "Player");
        int colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        CustomizationData.PlayerMaterial = colorMaterials[colorIndex];

        // Connect to Normcore
        realtime.Connect("e4e872e1-9fc5-443d-9dba-ad06b4ab40b7"); // Get from Normcore dashboard
    }

    public void SpawnAvatar()
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = true,
            useInstance = realtime
        };

        Realtime.Instantiate(avatarPrefab.name, options);
    }
}*/

using UnityEngine;

public class AvatarInitializer : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer[] bodyMeshes; // Assign the avatar's mesh renderers in the Inspector

    void Start()
    {
        // Create material instances to avoid shared materials
        foreach (var renderer in bodyMeshes)
        {
            renderer.material = new Material(renderer.material);
        }

        // Apply customization data to the avatar
        ApplyCustomization();
    }

    void Update()
    {
        // Optional: Update in real-time if customization changes during pre-game
        ApplyCustomization();
    }

    private void ApplyCustomization()
    {
        foreach (var mesh in bodyMeshes)
        {
            mesh.material.color = CustomizationData.PlayerColor; // Use PlayerColor instead of PlayerMaterial
        }
    }
}