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
using Normal.Realtime;

public class AvatarInitializer : MonoBehaviour
{
    public Realtime realtime; // Assign in Inspector
    public GameObject avatarPrefab; // Your networked avatar prefab
    public Material[] colorMaterials; // Assign in Inspector

    private bool hasSpawnedAvatar = false;

    void Start()
    {
        if (realtime == null)
        {
            Debug.LogError("Realtime component not assigned!");
            return;
        }

        // Load saved customization data
        CustomizationData.PlayerName = PlayerPrefs.GetString("PlayerName", "Player");
        int colorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        CustomizationData.PlayerMaterial = colorMaterials[colorIndex];

        // Subscribe to connection events
        realtime.didConnectToRoom += DidConnectToRoom;

        // Connect to the room specified by the player
        string roomName = PlayerPrefs.GetString("RoomName", "defaultRoom");
        realtime.Connect(roomName);
    }

    private void DidConnectToRoom(Realtime room)
    {
        Debug.Log("Connected to room: " + room.room.name); // Use room.room.name instead of roomName
        if (!hasSpawnedAvatar)
        {
            SpawnAvatar();
            hasSpawnedAvatar = true;
        }
    }

    public void SpawnAvatar()
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = true,
            useInstance = realtime
        };

        GameObject avatar = Realtime.Instantiate(avatarPrefab.name, options);
        if (avatar == null)
        {
            Debug.LogError("Failed to spawn avatar!");
        }
        else
        {
            Debug.Log("Avatar spawned successfully!");
        }
    }

    void OnDestroy()
    {
        if (realtime != null)
        {
            realtime.didConnectToRoom -= DidConnectToRoom;
        }
    }
}