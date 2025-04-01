/*using UnityEngine;
using UnityEngine.UI;

public class SceneSwitchPanel : MonoBehaviour
{
    public Button showroomButton;
    public Button classroomButton;
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

        gameObject.SetActive(_roomManager.IsCreator);

        showroomButton.onClick.AddListener(() => _roomManager.SwitchScene("2_Showroom"));
        classroomButton.onClick.AddListener(() => _roomManager.SwitchScene("3_Classroom"));
        natureButton.onClick.AddListener(() => _roomManager.SwitchScene("4_Nature"));
    }
}*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Added for IEnumerator

public class SceneSwitchPanel : MonoBehaviour
{
    public Button commonRoomButton;
    public Button showroomButton;
    public Button classroomButton;
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
        classroomButton.onClick.AddListener(() => _roomManager.SwitchScene("3_Classroom"));
        natureButton.onClick.AddListener(() => _roomManager.SwitchScene("4_Nature"));
    }
}