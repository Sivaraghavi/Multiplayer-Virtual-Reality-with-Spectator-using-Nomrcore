using UnityEngine;
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
}