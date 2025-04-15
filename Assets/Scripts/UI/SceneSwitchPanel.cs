/*using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class SceneSwitchPanel : MonoBehaviour
{
    public Button commonRoomButton;
    public Button showroomButton;
    public Button clinicroomButton;
    public Button natureButton;
    private RoomManager _roomManager;

    void Start()
    {
        _roomManager = FindObjectOfType<RoomManager>();
        if (_roomManager == null)
        {
            Debug.LogError("RoomManager not found!");
            return;
        }

        // Wait for RoomManager to connect before setting up buttons
        StartCoroutine(SetupButtonsWhenConnected());
    }

    private IEnumerator SetupButtonsWhenConnected()
    {
        while (!_roomManager.IsConnected)
        {
            yield return null;
        }

        gameObject.SetActive(_roomManager.IsCreator);

        commonRoomButton.onClick.AddListener(() => _roomManager.SwitchScene("1_CommonRoom"));
        showroomButton.onClick.AddListener(() => _roomManager.SwitchScene("2_Showroom"));
        clinicroomButton.onClick.AddListener(() => _roomManager.SwitchScene("3_Clinic"));
        natureButton.onClick.AddListener(() => _roomManager.SwitchScene("4_Nature"));
    }
}*/

// SceneSwitchPanel.cs
// UI for creator to switch scenes
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneSwitchPanel : MonoBehaviour
{
    public Button commonRoomButton;
    public Button showroomButton;
    public Button clinicroomButton;
    public Button natureButton;
    private RoomManager _roomManager;

    void Start()
    {
        _roomManager = FindObjectOfType<RoomManager>();
        if (_roomManager == null)
        {
            Debug.LogError("RoomManager not found!");
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(SetupButtonsWhenConnected());
    }

    private IEnumerator SetupButtonsWhenConnected()
    {
        while (!_roomManager.IsConnected)
        {
            yield return null;
        }

        gameObject.SetActive(_roomManager.IsCreator);

        if (commonRoomButton != null)
            commonRoomButton.onClick.AddListener(() => _roomManager.SwitchScene("1_CommonRoom"));
        else
            Debug.LogError("Common Room Button not assigned!");

        if (showroomButton != null)
            showroomButton.onClick.AddListener(() => _roomManager.SwitchScene("2_Showroom"));
        else
            Debug.LogError("Showroom Button not assigned!");

        if (clinicroomButton != null)
            clinicroomButton.onClick.AddListener(() => _roomManager.SwitchScene("3_Clinic"));
        else
            Debug.LogError("Clinic Room Button not assigned!");

        if (natureButton != null)
            natureButton.onClick.AddListener(() => _roomManager.SwitchScene("4_Nature"));
        else
            Debug.LogError("Nature Button not assigned!");
    }

    void Update()
    {
        // Disable buttons if not connected
        bool interactable = _roomManager != null && _roomManager.IsConnected && _roomManager.IsCreator;
        if (commonRoomButton != null) commonRoomButton.interactable = interactable;
        if (showroomButton != null) showroomButton.interactable = interactable;
        if (clinicroomButton != null) clinicroomButton.interactable = interactable;
        if (natureButton != null) natureButton.interactable = interactable;
    }
}