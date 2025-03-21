// AvatarInitializer.cs
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
}