/*
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
    public GameObject popupPanel; // Assign "Panel" in Inspector
    public TextMeshProUGUI popupText; // Assign "Popup Text" in Inspector
    private Realtime _realtime;
    private RoomStateComponent _roomState;
    public bool IsCreator { get; private set; }

    // Assign this in the Inspector
    public NormcoreAppSettings normcoreAppSettings;
    public GameObject xrRig;

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
        if (popupPanel != null) popupPanel.SetActive(false);
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

        Debug.Log("Attempting to instantiate PlayerPrefab from Resources");
        GameObject avatarInstance = Realtime.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity, options);
        if (avatarInstance == null)
        {
            Debug.LogError("Failed to instantiate PlayerPrefab!");
            return;
        }

        if (xrRig != null)
        {
            Debug.Log($"XR Rig found: {xrRig.name}");
            Transform cameraOffset = xrRig.transform.Find("Camera Offset");
            if (cameraOffset != null)
            {
                XRAvatarSync sync = avatarInstance.GetComponent<XRAvatarSync>();
                if (sync != null)
                {
                    sync.xrHead = cameraOffset.Find("Main Camera");
                    sync.xrLeftHand = cameraOffset.Find("Left Controller");
                    sync.xrRightHand = cameraOffset.Find("Right Controller");

                    Debug.Log($"XR Head: {(sync.xrHead != null ? sync.xrHead.name : "null")}");
                    Debug.Log($"XR Left Hand: {(sync.xrLeftHand != null ? sync.xrLeftHand.name : "null")}");
                    Debug.Log($"XR Right Hand: {(sync.xrRightHand != null ? sync.xrRightHand.name : "null")}");

                    if (sync.xrHead == null || sync.xrLeftHand == null || sync.xrRightHand == null)
                    {
                        Debug.LogError("One or more XR Rig child transforms not found! Check XR Rig hierarchy.");
                    }
                    else
                    {
                        Debug.Log("XR Rig linked to RealtimeAvatar");
                    }
                }
                else
                {
                    Debug.LogError("XRAvatarSync component missing on instantiated avatar!");
                }
            }
            else
            {
                Debug.LogError("Camera Offset not found under XR Rig!");
            }
        }
        else
        {
            Debug.LogError("XR Rig not assigned in RoomManager Inspector!");
        }

        _roomState = gameObject.AddComponent<RoomStateComponent>();
        _roomState.SetRoomManager(this);

        if (IsCreator)
        {
            _roomState.SetCurrentScene("1_CommonRoom");
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has created the room");
        }
        else
        {
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has joined");
        }
        realtime.room.clientConnected += ClientConnected;
        realtime.room.clientDisconnected += ClientDisconnected;
    }
    private void DidDisconnectFromRoom(Realtime realtime)
    {
        ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
        SceneManager.LoadScene("0_PreGame");
    }
    private void ClientConnected(Room room, int clientID)
    {
        if (clientID != _realtime.clientID)
        {
            RealtimeView view = room.GetRealtimeViewForClient(clientID);
            if (view != null)
            {
                PlayerSync sync = view.GetComponent<PlayerSync>();
                if (sync != null)
                {
                    ShowPopup($"{sync.PlayerName} has joined");
                }
            }
        }
    }

    private void ClientDisconnected(Room room, int clientID)
    {
        RealtimeView view = room.GetRealtimeViewForClient(clientID);
        if (view != null)
        {
            PlayerSync sync = view.GetComponent<PlayerSync>();
            if (sync != null)
            {
                ShowPopup($"{sync.PlayerName} has left");
            }
        }
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
        if (popupPanel == null || popupText == null)
        {
            Debug.LogError("Popup Panel or Text not assigned!");
            return;
        }

        Debug.Log($"Showing popup: {message}");
        popupText.text = message;
        popupPanel.SetActive(true);

        // Fade in
        CanvasGroup canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = popupPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f).setOnComplete(() =>
        {
            // Fade out after 2 seconds
            LeanTween.alphaCanvas(canvasGroup, 0f, 0.5f).setDelay(2f).setOnComplete(() =>
            {
                popupPanel.SetActive(false);
            });
        });
    }

    private void ClearPopup()
    {
        if (popupText != null) popupText.text = "";
    }
}*/
/*

using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;
using TMPro;
using System.Collections; // For coroutine
using Normal;


[System.Serializable]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
}

[RealtimeModel]
public partial class RoomStateModel
{
    [RealtimeProperty(2, true, true)] private Dictionary<string, TransformData> _objectTransforms = new Dictionary<string, TransformData>();
    [RealtimeProperty(3, true, true)] private Dictionary<string, int> _objectOwners = new Dictionary<string, int>(); // objectID to clientID
}

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
    public GameObject popupPanel; // Assign "Panel" in Inspector
    public TextMeshProUGUI popupText; // Assign "PopText" in Inspector
    private Realtime _realtime;
    private RoomStateComponent _roomState;
    public bool IsCreator { get; private set; }
    public NormcoreAppSettings normcoreAppSettings;
    public GameObject xrRig;

    void Start()
    {
        IsCreator = PlayerPrefs.GetInt("IsCreator", 0) == 1;
        string roomName = PlayerPrefs.GetString("RoomName", "DefaultRoom");
        Debug.Log($"Connecting to room: {roomName}");

        _realtime = GetComponent<Realtime>();
        if (_realtime == null) _realtime = gameObject.AddComponent<Realtime>();

        if (normcoreAppSettings == null)
        {
            Debug.LogError("No NormcoreAppSettings assigned in the Inspector!");
            return;
        }

        _realtime.normcoreAppSettings = normcoreAppSettings;
        _realtime.didConnectToRoom += DidConnectToRoom;
        _realtime.didDisconnectFromRoom += DidDisconnectFromRoom;
        _realtime.Connect(roomName);

        if (popupPanel != null) popupPanel.SetActive(false);
    }

    */
/*private void DidConnectToRoom(Realtime realtime)
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = _realtime
        };

        Debug.Log("Attempting to instantiate PlayerPrefab from Resources");
        GameObject avatarInstance = Realtime.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity, options);
        if (avatarInstance == null)
        {
            Debug.LogError("Failed to instantiate PlayerPrefab!");
            return;
        }

        if (xrRig != null)
        {
            Debug.Log($"XR Rig found: {xrRig.name}");
            Transform cameraOffset = xrRig.transform.Find("Camera Offset");
            if (cameraOffset != null)
            {
                XRAvatarSync sync = avatarInstance.GetComponent<XRAvatarSync>();
                if (sync != null)
                {
                    sync.xrHead = cameraOffset.Find("Main Camera");
                    sync.xrLeftHand = cameraOffset.Find("Left Controller");
                    sync.xrRightHand = cameraOffset.Find("Right Controller");

                    Debug.Log($"XR Head: {(sync.xrHead != null ? sync.xrHead.name : "null")}");
                    Debug.Log($"XR Left Hand: {(sync.xrLeftHand != null ? sync.xrLeftHand.name : "null")}");
                    Debug.Log($"XR Right Hand: {(sync.xrRightHand != null ? sync.xrRightHand.name : "null")}");

                    if (sync.xrHead == null || sync.xrLeftHand == null || sync.xrRightHand == null)
                    {
                        Debug.LogError("One or more XR Rig child transforms not found!");
                    }
                    else
                    {
                        Debug.Log("XR Rig linked to RealtimeAvatar");
                    }
                }
                else
                {
                    Debug.LogError("XRAvatarSync component missing on instantiated avatar!");
                }
            }
            else
            {
                Debug.LogError("Camera Offset not found under XR Rig!");
            }
        }
        else
        {
            Debug.LogError("XR Rig not assigned in RoomManager Inspector!");
        }

        _roomState = gameObject.AddComponent<RoomStateComponent>();
        _roomState.SetRoomManager(this);

        if (IsCreator)
        {
            _roomState.SetCurrentScene("1_CommonRoom");
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has created the room");
        }
        else
        {
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has joined");
        }
    }
*/
/*
    private void DidConnectToRoom(Realtime realtime)
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = _realtime
        };

        Debug.Log("Attempting to instantiate PlayerPrefab from Resources");
        GameObject avatarInstance = Realtime.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity, options);
        if (avatarInstance == null)
        {
            Debug.LogError("Failed to instantiate PlayerPrefab!");
            return;
        }

        if (xrRig != null)
        {
            Debug.Log($"XR Rig found: {xrRig.name}");
            Transform cameraOffset = xrRig.transform.Find("Camera Offset");
            if (cameraOffset != null)
            {
                XRAvatarSync sync = avatarInstance.GetComponent<XRAvatarSync>();
                if (sync != null)
                {
                    sync.xrHead = cameraOffset.Find("Main Camera");
                    sync.xrLeftHand = cameraOffset.Find("Left Controller");
                    sync.xrRightHand = cameraOffset.Find("Right Controller");

                    Debug.Log($"XR Head: {(sync.xrHead != null ? sync.xrHead.name : "null")}");
                    Debug.Log($"XR Left Hand: {(sync.xrLeftHand != null ? sync.xrLeftHand.name : "null")}");
                    Debug.Log($"XR Right Hand: {(sync.xrRightHand != null ? sync.xrRightHand.name : "null")}");

                    if (sync.xrHead == null || sync.xrLeftHand == null || sync.xrRightHand == null)
                    {
                        Debug.LogError("One or more XR Rig child transforms not found!");
                    }
                    else
                    {
                        Debug.Log("XR Rig linked to RealtimeAvatar");
                    }
                }
            }
        }

        _roomState = gameObject.AddComponent<RoomStateComponent>();
        _roomState.SetRoomManager(this);

        if (IsCreator)
        {
            _roomState.SetCurrentScene("1_CommonRoom");
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has created the room");

            // Spawn objects (only creator)
            SpawnObjects(realtime);
        }
        else
        {
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has joined");
        }
    }

    private void SpawnObjects(Realtime realtime)
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = _realtime
        };

        // Example: Spawn a sphere and cube on the table
        */
/*Vector3 tablePos = new Vector3(0, 0, 4); // Adjust to your table’s position
        Realtime.Instantiate("Totem2", tablePos + new Vector3(0.2f, 0.1f, 0), Quaternion.identity, options);
        Realtime.Instantiate("Cube", tablePos + new Vector3(-0.2f, 0.1f, 0), Quaternion.identity, options);*/
/*
    }
    private void DidDisconnectFromRoom(Realtime realtime)
    {
        ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
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
        if (popupPanel == null || popupText == null)
        {
            Debug.LogError("Popup Panel or Text not assigned!");
            return;
        }

        Debug.Log($"Showing popup: {message}");
        popupText.text = message;
        popupPanel.SetActive(true);

        CanvasGroup canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = popupPanel.AddComponent<CanvasGroup>();
        StartCoroutine(FadePopup(canvasGroup));
    }

    private System.Collections.IEnumerator FadePopup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        float fadeTime = 0.5f;
        float displayTime = 2f;

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Wait
        yield return new WaitForSeconds(displayTime);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        popupPanel.SetActive(false);
    }

    private void ClearPopup()
    {
        if (popupText != null) popupText.text = "";
    }
}*/

/*using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;
using TMPro;
using System.Collections;
using UnityEngine.UI;
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
        model.currentScene = sceneName;
    }
}

public class RoomManager : MonoBehaviour
{
    public GameObject popupPanel; // Assign "Panel" in Inspector
    public TextMeshProUGUI popupText; // Assign "PopText" in Inspector
    private Realtime _realtime;
    private RoomStateComponent _roomState;
    public bool IsCreator { get; private set; }
    public NormcoreAppSettings normcoreAppSettings;
    public GameObject xrRig;

    void Start()
    {
        IsCreator = PlayerPrefs.GetInt("IsCreator", 0) == 1;
        string roomName = PlayerPrefs.GetString("RoomName", "DefaultRoom");
        Debug.Log($"Connecting to room: {roomName}");

        _realtime = GetComponent<Realtime>();
        if (_realtime == null) _realtime = gameObject.AddComponent<Realtime>();

        if (normcoreAppSettings == null)
        {
            Debug.LogError("No NormcoreAppSettings assigned in the Inspector!");
            return;
        }

        _realtime.normcoreAppSettings = normcoreAppSettings;
        _realtime.didConnectToRoom += DidConnectToRoom;
        _realtime.didDisconnectFromRoom += DidDisconnectFromRoom;
        _realtime.Connect(roomName);

        if (popupPanel != null) popupPanel.SetActive(false);
        DontDestroyOnLoad(gameObject); // Persist across scenes
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

        Debug.Log("Attempting to instantiate PlayerPrefab from Resources");
        GameObject avatarInstance = Realtime.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity, options);
        if (avatarInstance == null)
        {
            Debug.LogError("Failed to instantiate PlayerPrefab!");
            return;
        }

        if (xrRig != null)
        {
            Debug.Log($"XR Rig found: {xrRig.name}");
            Transform cameraOffset = xrRig.transform.Find("Camera Offset");
            if (cameraOffset != null)
            {
                XRAvatarSync sync = avatarInstance.GetComponent<XRAvatarSync>();
                if (sync != null)
                {
                    sync.xrHead = cameraOffset.Find("Main Camera");
                    sync.xrLeftHand = cameraOffset.Find("Left Controller");
                    sync.xrRightHand = cameraOffset.Find("Right Controller");

                    Debug.Log($"XR Head: {(sync.xrHead != null ? sync.xrHead.name : "null")}");
                    Debug.Log($"XR Left Hand: {(sync.xrLeftHand != null ? sync.xrLeftHand.name : "null")}");
                    Debug.Log($"XR Right Hand: {(sync.xrRightHand != null ? sync.xrRightHand.name : "null")}");

                    if (sync.xrHead == null || sync.xrLeftHand == null || sync.xrRightHand == null)
                    {
                        Debug.LogError("One or more XR Rig child transforms not found!");
                    }
                    else
                    {
                        Debug.Log("XR Rig linked to RealtimeAvatar");
                    }
                }
            }
        }

        _roomState = gameObject.AddComponent<RoomStateComponent>();
        _roomState.SetRoomManager(this);

        if (IsCreator)
        {
            _roomState.SetCurrentScene("1_CommonRoom");
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has created the room");
            InitializeSceneObjects(realtime);
        }
        else
        {
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has joined");
        }
    }

    private void InitializeSceneObjects(Realtime realtime)
    {
        SyncedObject[] sceneObjects = FindObjectsOfType<SyncedObject>();
        foreach (SyncedObject obj in sceneObjects)
        {
            obj.Initialize(realtime);
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
        SceneManager.LoadScene("0_PreGame");
    }

    public void SwitchScene(string sceneName)
    {
        if (IsCreator && _roomState != null)
        {
            _roomState.SetCurrentScene(sceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            StartCoroutine(UnloadPreviousScene(SceneManager.GetActiveScene().name));
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    private IEnumerator UnloadPreviousScene(string previousScene)
    {
        yield return new WaitForSeconds(0.1f); // Brief delay to ensure new scene loads
        SceneManager.UnloadSceneAsync(previousScene);
    }

    private void ShowPopup(string message)
    {
        if (popupPanel == null || popupText == null)
        {
            Debug.LogError("Popup Panel or Text not assigned!");
            return;
        }

        Debug.Log($"Showing popup: {message}");
        popupText.text = message;
        popupPanel.SetActive(true);

        CanvasGroup canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = popupPanel.AddComponent<CanvasGroup>();
        StartCoroutine(FadePopup(canvasGroup));
    }

    private IEnumerator FadePopup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        float fadeTime = 0.5f;
        float displayTime = 2f;

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        popupPanel.SetActive(false);
    }

    private void ClearPopup()
    {
        if (popupText != null) popupText.text = "";
    }
}*/

using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;
using TMPro;
using System.Collections;
using UnityEngine.UI;
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
        model.currentScene = sceneName;
    }
}

public class RoomManager : MonoBehaviour
{
    public GameObject popupPanel; // Assign "Panel" in Inspector
    public TextMeshProUGUI popupText; // Assign "PopText" in Inspector
    private Realtime _realtime;
    private RoomStateComponent _roomState;
    public bool IsCreator { get; private set; }
    public NormcoreAppSettings normcoreAppSettings;
    public GameObject xrRig;

    // Prefabs to spawn (assign in Inspector)
    public GameObject[] objectPrefabs; // e.g., SpherePrefab, CubePrefab

    void Start()
    {
        IsCreator = PlayerPrefs.GetInt("IsCreator", 0) == 1;
        string roomName = PlayerPrefs.GetString("RoomName", "DefaultRoom");
        Debug.Log($"Connecting to room: {roomName}");

        _realtime = GetComponent<Realtime>();
        if (_realtime == null) _realtime = gameObject.AddComponent<Realtime>();

        if (normcoreAppSettings == null)
        {
            Debug.LogError("No NormcoreAppSettings assigned in the Inspector!");
            return;
        }

        _realtime.normcoreAppSettings = normcoreAppSettings;
        _realtime.didConnectToRoom += DidConnectToRoom;
        _realtime.didDisconnectFromRoom += DidDisconnectFromRoom;
        _realtime.Connect(roomName);

        if (popupPanel != null) popupPanel.SetActive(false);
        DontDestroyOnLoad(gameObject); // Persist across scenes
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

        Debug.Log("Attempting to instantiate PlayerPrefab from Resources");
        GameObject avatarInstance = Realtime.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity, options);
        if (avatarInstance == null)
        {
            Debug.LogError("Failed to instantiate PlayerPrefab!");
            return;
        }

        if (xrRig != null)
        {
            Debug.Log($"XR Rig found: {xrRig.name}");
            Transform cameraOffset = xrRig.transform.Find("Camera Offset");
            if (cameraOffset != null)
            {
                XRAvatarSync sync = avatarInstance.GetComponent<XRAvatarSync>();
                if (sync != null)
                {
                    sync.xrHead = cameraOffset.Find("Main Camera");
                    sync.xrLeftHand = cameraOffset.Find("Left Controller");
                    sync.xrRightHand = cameraOffset.Find("Right Controller");

                    Debug.Log($"XR Head: {(sync.xrHead != null ? sync.xrHead.name : "null")}");
                    Debug.Log($"XR Left Hand: {(sync.xrLeftHand != null ? sync.xrLeftHand.name : "null")}");
                    Debug.Log($"XR Right Hand: {(sync.xrRightHand != null ? sync.xrRightHand.name : "null")}");

                    if (sync.xrHead == null || sync.xrLeftHand == null || sync.xrRightHand == null)
                    {
                        Debug.LogError("One or more XR Rig child transforms not found!");
                    }
                    else
                    {
                        Debug.Log("XR Rig linked to RealtimeAvatar");
                    }
                }
            }
        }

        _roomState = gameObject.AddComponent<RoomStateComponent>();
        _roomState.SetRoomManager(this);

        if (IsCreator)
        {
            _roomState.SetCurrentScene("1_CommonRoom");
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has created the room");
            SpawnSceneObjects(realtime);
        }
        else
        {
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has joined");
        }
    }

    private void SpawnSceneObjects(Realtime realtime)
    {
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogWarning("No object prefabs assigned to RoomManager!");
            return;
        }

        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = realtime
        };

        Vector3 tablePos = new Vector3(0, 1, 0); // Adjust to your table’s position
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            Vector3 offset = new Vector3((i - objectPrefabs.Length / 2f) * 0.4f, 0.1f, 0); // Space objects out
            GameObject prefab = objectPrefabs[i];
            string prefabName = prefab.name;
            Realtime.Instantiate(prefabName, tablePos + offset, Quaternion.identity, options);
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
        SceneManager.LoadScene("0_PreGame");
    }

    public void SwitchScene(string sceneName)
    {
        if (IsCreator && _roomState != null)
        {
            _roomState.SetCurrentScene(sceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            StartCoroutine(UnloadPreviousScene(SceneManager.GetActiveScene().name));
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    private IEnumerator UnloadPreviousScene(string previousScene)
    {
        yield return new WaitForSeconds(0.1f); // Brief delay to ensure new scene loads
        SceneManager.UnloadSceneAsync(previousScene);
    }


    private void ShowPopup(string message)
    {
        if (popupPanel == null || popupText == null)
        {
            Debug.LogError("Popup Panel or Text not assigned!");
            return;
        }

        Debug.Log($"Showing popup: {message}");
        popupText.text = message;
        popupPanel.SetActive(true);

        CanvasGroup canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = popupPanel.AddComponent<CanvasGroup>();
        StartCoroutine(FadePopup(canvasGroup));
    }

    private IEnumerator FadePopup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        float fadeTime = 0.5f;
        float displayTime = 2f;

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        popupPanel.SetActive(false);
    }

    private void ClearPopup()
    {
        if (popupText != null) popupText.text = "";
    }
}