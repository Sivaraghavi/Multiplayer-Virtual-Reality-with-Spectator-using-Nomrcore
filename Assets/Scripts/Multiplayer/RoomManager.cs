/*
using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;
using TMPro;

// Define the RoomStateModel
[RealtimeModel]
public partial class RoomStateModel
{
    [RealtimeProperty(1, true, true)] private string _currentScene;
}

// Subclass to manage the RoomStateModel
public class RoomStateComponent : RealtimeComponent<RoomStateModel>
{
    private RoomManager _roomManager;

    public void SetRoomManager(RoomManager manager)
    {
        _roomManager = manager;
    }

    protected override void OnRealtimeModelReplaced(RoomStateModel previousModel, RoomStateModel currentModel)
    {
        if (currentModel != null)
        {
            // Update scene when the model changes (for non-creators)
            if (!_roomManager.IsCreator && !string.IsNullOrEmpty(currentModel.currentScene))
            {
                SceneManager.LoadScene(currentModel.currentScene);
            }
        }
    }

    // Public method to set the current scene
    public void SetCurrentScene(string sceneName)
    {
        if (model != null)
        {
            model.currentScene = sceneName;
        }
    }

    // Public method to get the current scene
    public string GetCurrentScene()
    {
        return model != null ? model.currentScene : string.Empty;
    }
}

public class RoomManager : MonoBehaviour
{
    public GameObject avatarPrefab; // Assign your NetworkedAvatar prefab
    public TextMeshProUGUI popupText; // Assign a TMP_Text in the corner for pop-ups
    private Realtime _realtime;
    private RoomStateComponent _roomState;

    public bool IsCreator { get; private set; }

    void Start()
    {
        IsCreator = PlayerPrefs.GetInt("IsCreator", 0) == 1;
        string roomName = PlayerPrefs.GetString("RoomName", "DefaultRoom");
        Debug.Log($"Connecting to room: {roomName}");

        _realtime = FindObjectOfType<Realtime>();
        if (_realtime == null)
        {
            _realtime = gameObject.AddComponent<Realtime>();
        }

        _realtime.didConnectToRoom += DidConnectToRoom;
        _realtime.didDisconnectFromRoom += DidDisconnectFromRoom;

        _realtime.Connect(roomName);
    }


    private void DidConnectToRoom(Realtime realtime)
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = _realtime
        };

        Debug.Log($"Instantiating prefab: {avatarPrefab.name}");
        Realtime.Instantiate(avatarPrefab.name, Vector3.zero, Quaternion.identity, options);

        _roomState = gameObject.AddComponent<RoomStateComponent>();
        _roomState.SetRoomManager(this);

        if (IsCreator)
        {
            _roomState.SetCurrentScene("1_CommonRoom");
            ShowPopup("Room created successfully!");
        }
        else
        {
            ShowPopup("Joined room successfully!");
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        ShowPopup("Disconnected from room!");
        SceneManager.LoadScene("0_Spawning Room");
    }

    public void SwitchScene(string sceneName)
    {
        if (IsCreator)
        {
            if (_roomState != null)
            {
                _roomState.SetCurrentScene(sceneName);
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    private void ShowPopup(string message)
    {
        popupText.text = message;
        Invoke("ClearPopup", 3f);
    }

    private void ClearPopup()
    {
        popupText.text = "";
    }
}*/

using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;
using TMPro;
using Normal;

[RealtimeModel]
public partial class RoomStateModel
{
    [RealtimeProperty(1, true, true)] private string _currentScene;
}

public class RoomStateComponent : RealtimeComponent<RoomStateModel>
{
    private RoomManager _roomManager;

    public void SetRoomManager(RoomManager manager)
    {
        _roomManager = manager;
    }

    protected override void OnRealtimeModelReplaced(RoomStateModel previousModel, RoomStateModel currentModel)
    {
        if (currentModel != null && !_roomManager.IsCreator && !string.IsNullOrEmpty(currentModel.currentScene))
        {
            SceneManager.LoadScene(currentModel.currentScene);
        }
    }

    public void SetCurrentScene(string sceneName)
    {
        if (model != null)
        {
            model.currentScene = sceneName;
        }
    }
}

public class RoomManager : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    private Realtime _realtime;
    private RoomStateComponent _roomState;
    public bool IsCreator { get; private set; }

    // Assign this in the Inspector
    public NormcoreAppSettings normcoreAppSettings;

    void Start()
    {
        IsCreator = PlayerPrefs.GetInt("IsCreator", 0) == 1;
        string roomName = PlayerPrefs.GetString("RoomName", "DefaultRoom");
        Debug.Log($"Connecting to room: {roomName}");

        _realtime = GetComponent<Realtime>();
        if (_realtime == null)
        {
            _realtime = gameObject.AddComponent<Realtime>();
        }

        if (normcoreAppSettings == null)
        {
            Debug.LogError("No NormcoreAppSettings assigned in the Inspector! Please assign one.");
            return;
        }

        _realtime.normcoreAppSettings = normcoreAppSettings;

        _realtime.didConnectToRoom += DidConnectToRoom;
        _realtime.didDisconnectFromRoom += DidDisconnectFromRoom;
        _realtime.Connect(roomName);
    }

    private void DidConnectToRoom(Realtime realtime)
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = _realtime
        };

        Debug.Log("Instantiating Realtime+VR Player");
        Realtime.Instantiate("Realtime+VR Player", Vector3.zero, Quaternion.identity, options);

        _roomState = gameObject.AddComponent<RoomStateComponent>();
        _roomState.SetRoomManager(this);

        if (IsCreator)
        {
            _roomState.SetCurrentScene("1_CommonRoom");
            ShowPopup("Room created successfully!");
        }
        else
        {
            ShowPopup("Joined room successfully!");
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        ShowPopup("Disconnected from room!");
        SceneManager.LoadScene("0_PreGame");
    }

    public void SwitchScene(string sceneName)
    {
        if (IsCreator && _roomState != null)
        {
            _roomState.SetCurrentScene(sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    private void ShowPopup(string message)
    {
        Debug.Log($"Showing popup: {message}");
        if (popupText != null)
        {
            popupText.text = message;
            Invoke("ClearPopup", 3f);
        }
        else
        {
            Debug.LogError("PopupText is not assigned!");
        }
    }

    private void ClearPopup()
    {
        if (popupText != null) popupText.text = "";
    }
}