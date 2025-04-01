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

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            DontDestroyOnLoad(popupPanel.transform.root.gameObject); // Make Canvas persistent
        }
        DontDestroyOnLoad(gameObject); // Persist RoomManager
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
            Vector3 offset = new Vector3((i - objectPrefabs.Length / 2f) * 0.4f, 0.1f, 0);
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
        yield return new WaitForSeconds(0.1f);
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
        if (previousModel != null)
        {
            previousModel.currentSceneDidChange -= SceneChanged;
        }
        if (currentModel != null)
        {
            currentModel.currentSceneDidChange += SceneChanged;
            if (!_roomManager.IsCreator && !string.IsNullOrEmpty(currentModel.currentScene))
            {
                _roomManager.LoadSceneForAll(currentModel.currentScene);
            }
        }
    }

    private void SceneChanged(RoomStateModel model, string sceneName)
    {
        if (!_roomManager.IsCreator && !string.IsNullOrEmpty(sceneName))
        {
            _roomManager.LoadSceneForAll(sceneName);
        }
    }

    public void SetCurrentScene(string sceneName)
    {
        if (model == null)
        {
            Debug.LogError("RoomStateComponent model is null!");
            return;
        }
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
    public GameObject[] objectPrefabs; // e.g., SpherePrefab, CubePrefab

    private string currentScene;
    public bool IsConnected { get; private set; } // Track connection state

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

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            DontDestroyOnLoad(popupPanel.transform.root.gameObject); // Persist Canvas
        }
        DontDestroyOnLoad(gameObject); // Persist RoomManager
    }

    private void DidConnectToRoom(Realtime realtime)
    {
        IsConnected = true;

        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = _realtime
        };

        // Check for RealtimeAvatarManager
        RealtimeAvatarManager avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (avatarManager == null)
        {
            Debug.Log("No RealtimeAvatarManager found, manually instantiating PlayerPrefab");
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
                    }
                }
            }
        }
        else
        {
            Debug.Log("RealtimeAvatarManager found, relying on it for avatar instantiation");
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
            Vector3 offset = new Vector3((i - objectPrefabs.Length / 2f) * 0.4f, 0.1f, 0);
            GameObject prefab = objectPrefabs[i];
            string prefabName = prefab.name;
            Realtime.Instantiate(prefabName, tablePos + offset, Quaternion.identity, options);
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        IsConnected = false;
        ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
        SceneManager.LoadScene("0_PreGame");
    }

    public void SwitchScene(string sceneName)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("Not connected to room yet!");
            return;
        }
        if (IsCreator && _roomState != null)
        {
            _roomState.SetCurrentScene(sceneName);
            LoadSceneForAll(sceneName);
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    public void LoadSceneForAll(string sceneName)
    {
        if (currentScene != sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            if (!string.IsNullOrEmpty(currentScene))
            {
                StartCoroutine(UnloadPreviousScene(currentScene));
            }
            currentScene = sceneName;
        }
    }

    private IEnumerator UnloadPreviousScene(string previousScene)
    {
        yield return new WaitForSeconds(0.1f);
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
        if (previousModel != null)
        {
            previousModel.currentSceneDidChange -= SceneChanged;
        }
        if (currentModel != null)
        {
            currentModel.currentSceneDidChange += SceneChanged;
            if (!_roomManager.IsCreator && !string.IsNullOrEmpty(currentModel.currentScene))
            {
                _roomManager.LoadSceneForAll(currentModel.currentScene);
            }
        }
    }

    private void SceneChanged(RoomStateModel model, string sceneName)
    {
        if (!_roomManager.IsCreator && !string.IsNullOrEmpty(sceneName))
        {
            _roomManager.LoadSceneForAll(sceneName);
        }
    }

    public void SetCurrentScene(string sceneName)
    {
        if (model == null)
        {
            Debug.LogError("RoomStateComponent model is null!");
            return;
        }
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
    public GameObject xrRig; // XR Rig reference for local avatar
    public GameObject[] objectPrefabs; // e.g., SpherePrefab, CubePrefab

    private string currentScene;
    public bool IsConnected { get; private set; }

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

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            DontDestroyOnLoad(popupPanel.transform.root.gameObject); // Persist Canvas
        }
        DontDestroyOnLoad(gameObject); // Persist RoomManager
    }

    private void DidConnectToRoom(Realtime realtime)
    {
        IsConnected = true;

        RealtimeAvatarManager avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (avatarManager == null)
        {
            Debug.LogError("No RealtimeAvatarManager found! Add it to the scene.");
            return;
        }
        Debug.Log("RealtimeAvatarManager found, relying on it for avatar instantiation");
        if (avatarManager.localAvatarPrefab == null)
        {
            Debug.LogError("localAvatarPrefab not set in RealtimeAvatarManager!");
            return;
        }

        // Link XR Rig to local avatar after instantiation by RealtimeAvatarManager
        if (xrRig != null && avatarManager.localAvatar != null)
        {
            XRAvatarSync sync = avatarManager.localAvatar.GetComponent<XRAvatarSync>();
            if (sync != null)
            {
                Transform cameraOffset = xrRig.transform.Find("Camera Offset");
                if (cameraOffset != null)
                {
                    sync.xrHead = cameraOffset.Find("Main Camera");
                    sync.xrLeftHand = cameraOffset.Find("Left Controller");
                    sync.xrRightHand = cameraOffset.Find("Right Controller");
                    Debug.Log("XR Rig linked to local avatar");
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
            Vector3 offset = new Vector3((i - objectPrefabs.Length / 2f) * 0.4f, 0.1f, 0);
            GameObject prefab = objectPrefabs[i];
            string prefabName = prefab.name;
            Realtime.Instantiate(prefabName, tablePos + offset, Quaternion.identity, options);
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        IsConnected = false;
        ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
        SceneManager.LoadScene("0_PreGame");
    }

    public void SwitchScene(string sceneName)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("Not connected to room yet!");
            return;
        }
        if (IsCreator && _roomState != null)
        {
            _roomState.SetCurrentScene(sceneName);
            LoadSceneForAll(sceneName);
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    public void LoadSceneForAll(string sceneName)
    {
        if (currentScene != sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            if (!string.IsNullOrEmpty(currentScene))
            {
                StartCoroutine(UnloadPreviousScene(currentScene));
            }
            currentScene = sceneName;
        }
    }

    private IEnumerator UnloadPreviousScene(string previousScene)
    {
        yield return new WaitForSeconds(0.1f);
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
        if (previousModel != null)
        {
            previousModel.currentSceneDidChange -= SceneChanged;
        }
        if (currentModel != null)
        {
            currentModel.currentSceneDidChange += SceneChanged;
            if (!_roomManager.IsCreator && !string.IsNullOrEmpty(currentModel.currentScene))
            {
                _roomManager.LoadSceneForAll(currentModel.currentScene);
            }
        }
    }

    private void SceneChanged(RoomStateModel model, string sceneName)
    {
        if (!_roomManager.IsCreator && !string.IsNullOrEmpty(sceneName))
        {
            _roomManager.LoadSceneForAll(sceneName);
        }
    }

    public void SetCurrentScene(string sceneName)
    {
        if (model == null)
        {
            Debug.LogError("RoomStateComponent model is null!");
            return;
        }
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
    public GameObject xrRig; // XR Rig reference for local avatar
    public string[] objectPrefabNames; // e.g., "SpherePrefab", "CubePrefab" (strings, not GameObject references)

    private string currentScene;
    public bool IsConnected { get; private set; }

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

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            DontDestroyOnLoad(popupPanel.transform.root.gameObject); // Persist Canvas
        }
        DontDestroyOnLoad(gameObject); // Persist RoomManager
    }

    private void DidConnectToRoom(Realtime realtime)
    {
        IsConnected = true;

        RealtimeAvatarManager avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (avatarManager == null)
        {
            Debug.LogError("No RealtimeAvatarManager found! Add it to the scene.");
            return;
        }
        Debug.Log("RealtimeAvatarManager found, relying on it for avatar instantiation");
        if (avatarManager.localAvatarPrefab == null)
        {
            Debug.LogError("localAvatarPrefab not set in RealtimeAvatarManager!");
            return;
        }

        // Link XR Rig to local avatar
        if (xrRig != null && avatarManager.localAvatar != null)
        {
            XRAvatarSync sync = avatarManager.localAvatar.GetComponent<XRAvatarSync>();
            if (sync != null)
            {
                Transform cameraOffset = xrRig.transform.Find("Camera Offset");
                if (cameraOffset != null)
                {
                    sync.xrHead = cameraOffset.Find("Main Camera");
                    sync.xrLeftHand = cameraOffset.Find("Left Controller");
                    sync.xrRightHand = cameraOffset.Find("Right Controller");
                    Debug.Log("XR Rig linked to local avatar");
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
        if (objectPrefabNames == null || objectPrefabNames.Length == 0)
        {
            Debug.LogWarning("No object prefab names assigned to RoomManager!");
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
        for (int i = 0; i < objectPrefabNames.Length; i++)
        {
            Vector3 offset = new Vector3((i - objectPrefabNames.Length / 2f) * 0.4f, 0.1f, 0);
            string prefabName = objectPrefabNames[i];
            Realtime.Instantiate(prefabName, tablePos + offset, Quaternion.identity, options);
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        IsConnected = false;
        ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
        SceneManager.LoadScene("0_PreGame");
    }

    public void SwitchScene(string sceneName)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("Not connected to room yet!");
            return;
        }
        if (IsCreator && _roomState != null)
        {
            _roomState.SetCurrentScene(sceneName);
            LoadSceneForAll(sceneName);
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    public void LoadSceneForAll(string sceneName)
    {
        if (currentScene != sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            if (!string.IsNullOrEmpty(currentScene))
            {
                StartCoroutine(UnloadPreviousScene(currentScene));
            }
            currentScene = sceneName;
        }
    }

    private IEnumerator UnloadPreviousScene(string previousScene)
    {
        yield return new WaitForSeconds(0.1f);
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
        if (previousModel != null)
        {
            previousModel.currentSceneDidChange -= SceneChanged;
        }
        if (currentModel != null)
        {
            currentModel.currentSceneDidChange += SceneChanged;
            if (!_roomManager.IsCreator && !string.IsNullOrEmpty(currentModel.currentScene))
            {
                _roomManager.LoadSceneForAll(currentModel.currentScene);
            }
        }
    }

    private void SceneChanged(RoomStateModel model, string sceneName)
    {
        if (!_roomManager.IsCreator && !string.IsNullOrEmpty(sceneName))
        {
            _roomManager.LoadSceneForAll(sceneName);
        }
    }

    public void SetCurrentScene(string sceneName)
    {
        if (model == null)
        {
            Debug.LogError("RoomStateComponent model is null!");
            return;
        }
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
    public GameObject xrRig; // XR Rig reference for local avatar
    public string[] objectPrefabNames; // e.g., "SpherePrefab", "CubePrefab" (strings)

    private string currentScene;
    public bool IsConnected { get; private set; }

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

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            DontDestroyOnLoad(popupPanel.transform.root.gameObject); // Persist Canvas
        }
        DontDestroyOnLoad(gameObject); // Persist RoomManager
    }

    private void DidConnectToRoom(Realtime realtime)
    {
        IsConnected = true;

        RealtimeAvatarManager avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (avatarManager == null)
        {
            Debug.LogError("No RealtimeAvatarManager found! Add it to the scene.");
            return;
        }
        Debug.Log("RealtimeAvatarManager found, relying on it for avatar instantiation");
        if (avatarManager.localAvatarPrefab == null)
        {
            Debug.LogError("localAvatarPrefab not set in RealtimeAvatarManager!");
            return;
        }

        // Wait briefly and check avatar
        StartCoroutine(CheckAvatarSpawn(avatarManager));

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

    private IEnumerator CheckAvatarSpawn(RealtimeAvatarManager avatarManager)
    {
        yield return new WaitForSeconds(0.5f); // Wait for instantiation
        if (avatarManager.localAvatar != null)
        {
            Debug.Log($"Local avatar spawned at position: {avatarManager.localAvatar.transform.position}");
            if (xrRig != null)
            {
                XRAvatarSync sync = avatarManager.localAvatar.GetComponent<XRAvatarSync>();
                if (sync != null)
                {
                    Transform cameraOffset = xrRig.transform.Find("Camera Offset");
                    if (cameraOffset != null)
                    {
                        sync.xrHead = cameraOffset.Find("Main Camera");
                        sync.xrLeftHand = cameraOffset.Find("Left Controller");
                        sync.xrRightHand = cameraOffset.Find("Right Controller");
                        Debug.Log("XR Rig linked to local avatar");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Local avatar not spawned by RealtimeAvatarManager!");
        }
    }

    private void SpawnSceneObjects(Realtime realtime)
    {
        if (objectPrefabNames == null || objectPrefabNames.Length == 0)
        {
            Debug.LogWarning("No object prefab names assigned to RoomManager!");
            return;
        }

        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            destroyWhenLastClientLeaves = true,
            useInstance = realtime
        };

        Vector3 tablePos = new Vector3(0, 1, 0);
        for (int i = 0; i < objectPrefabNames.Length; i++)
        {
            Vector3 offset = new Vector3((i - objectPrefabNames.Length / 2f) * 0.4f, 0.1f, 0);
            string prefabName = objectPrefabNames[i];
            GameObject instance = Realtime.Instantiate(prefabName, tablePos + offset, Quaternion.identity, options);
            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate prefab: {prefabName}");
            }
        }
    }

    private void DidDisconnectFromRoom(Realtime realtime)
    {
        IsConnected = false;
        if (gameObject.activeInHierarchy)
        {
            ShowPopup($"{PlayerPrefs.GetString("PlayerName", "Player")} has left");
            StartCoroutine(DelayedSceneSwitch("0_PreGame"));
        }
        else
        {
            SceneManager.LoadScene("0_PreGame");
        }
    }

    private IEnumerator DelayedSceneSwitch(string sceneName)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchScene(string sceneName)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("Not connected to room yet!");
            return;
        }
        if (IsCreator && _roomState != null)
        {
            _roomState.SetCurrentScene(sceneName);
            LoadSceneForAll(sceneName);
        }
        else
        {
            ShowPopup("Only the room creator can switch scenes!");
        }
    }

    public void LoadSceneForAll(string sceneName)
    {
        if (currentScene != sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            if (!string.IsNullOrEmpty(currentScene))
            {
                StartCoroutine(UnloadPreviousScene(currentScene));
            }
            currentScene = sceneName;
        }
    }

    private IEnumerator UnloadPreviousScene(string previousScene)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.UnloadSceneAsync(previousScene);
    }

    private void ShowPopup(string message)
    {
        if (popupPanel == null || popupText == null)
        {
            Debug.LogError("Popup Panel or Text not assigned!");
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("RoomManager inactive, skipping popup");
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
}