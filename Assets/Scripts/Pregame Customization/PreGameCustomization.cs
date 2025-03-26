/*
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Normal.Realtime;
using System.Collections.Generic;

public class PreGameCustomization : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown colorDropdown;
    public TMP_InputField roomInputField;
    public Button createRoomButton;
    public Button joinRoomButton;
    public Material[] colorMaterials; // Assign 5 materials in Inspector (Red, Blue, Green, Yellow, Cyan)

    void Start()
    {
        InitializeColorDropdown();
        nameInputField.onValueChanged.AddListener(UpdatePlayerName);
        colorDropdown.onValueChanged.AddListener(UpdatePlayerColor);
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
    }

    void InitializeColorDropdown()
    {
        colorDropdown.ClearOptions();
        colorDropdown.AddOptions(new List<string> { "Red", "Blue", "Green", "Yellow", "Cyan" });
    }

    void UpdatePlayerName(string newName)
    {
        CustomizationData.PlayerName = string.IsNullOrEmpty(newName) ? "Player" : newName;
    }

    void UpdatePlayerColor(int colorIndex)
    {
        CustomizationData.ColorIndex = colorIndex;
        CustomizationData.PlayerColor = colorMaterials[colorIndex].color;
    }

    void OnCreateRoomClicked()
    {
        SaveCustomizationData();
        PlayerPrefs.SetString("RoomName", roomInputField.text);
        PlayerPrefs.SetInt("IsCreator", 1); // Mark as creator
        SceneManager.LoadScene("1_CommonRoom");
    }

    void OnJoinRoomClicked()
    {
        SaveCustomizationData();
        PlayerPrefs.SetString("RoomName", roomInputField.text);
        PlayerPrefs.SetInt("IsCreator", 0); // Mark as joiner
        SceneManager.LoadScene("1_CommonRoom");
    }

    void SaveCustomizationData()
    {
        PlayerPrefs.SetString("PlayerName", CustomizationData.PlayerName);
        PlayerPrefs.SetInt("ColorIndex", CustomizationData.ColorIndex);
    }
}
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Normal.Realtime;
using System.Collections.Generic;

public class PreGameCustomization : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown colorDropdown;
    public TMP_InputField roomInputField;
    public Button createRoomButton;
    public Button joinRoomButton;
    public Material[] colorMaterials; // Assign 5 materials in Inspector (Red, Blue, Green, Yellow, Cyan)

    void Start()
    {
        if (!ValidateUIComponents()) return;

        InitializeColorDropdown();
        nameInputField.onValueChanged.AddListener(UpdatePlayerName);
        colorDropdown.onValueChanged.AddListener(UpdatePlayerColor);
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        Debug.Log("PreGameCustomization initialized");
    }

    bool ValidateUIComponents()
    {
        if (nameInputField == null) Debug.LogError("Name Input Field is not assigned!");
        if (colorDropdown == null) Debug.LogError("Color Dropdown is not assigned!");
        if (roomInputField == null) Debug.LogError("Room Input Field is not assigned!");
        if (createRoomButton == null) Debug.LogError("Create Room Button is not assigned!");
        if (joinRoomButton == null) Debug.LogError("Join Room Button is not assigned!");
        if (colorMaterials == null || colorMaterials.Length < 5) Debug.LogError("Color Materials array is not properly assigned!");
        return nameInputField != null && colorDropdown != null && roomInputField != null &&
               createRoomButton != null && joinRoomButton != null && colorMaterials != null && colorMaterials.Length >= 5;
    }

    void InitializeColorDropdown()
    {
        colorDropdown.ClearOptions();
        colorDropdown.AddOptions(new List<string> { "Red", "Blue", "Green", "Yellow", "Cyan" });
    }

    void UpdatePlayerName(string newName)
    {
        CustomizationData.PlayerName = string.IsNullOrEmpty(newName) ? "Player" : newName;
    }

    void UpdatePlayerColor(int colorIndex)
    {
        CustomizationData.ColorIndex = colorIndex;
        CustomizationData.PlayerColor = colorMaterials[colorIndex].color;
    }

    void OnCreateRoomClicked()
    {
        Debug.Log("Create button clicked");
        SaveCustomizationData();
        string roomName = string.IsNullOrEmpty(roomInputField.text) ? "DefaultRoom" : roomInputField.text;
        PlayerPrefs.SetString("RoomName", roomName);
        PlayerPrefs.SetInt("IsCreator", 1);
        Debug.Log($"Creating room: {roomName}");
        SceneManager.LoadScene("1_CommonRoom");
    }

    void OnJoinRoomClicked()
    {
        Debug.Log("Join button clicked");
        SaveCustomizationData();
        string roomName = string.IsNullOrEmpty(roomInputField.text) ? "DefaultRoom" : roomInputField.text;
        PlayerPrefs.SetString("RoomName", roomName);
        PlayerPrefs.SetInt("IsCreator", 0);
        Debug.Log($"Joining room: {roomName}");
        SceneManager.LoadScene("1_CommonRoom");
    }

    void SaveCustomizationData()
    {
        PlayerPrefs.SetString("PlayerName", CustomizationData.PlayerName);
        PlayerPrefs.SetInt("ColorIndex", CustomizationData.ColorIndex);
    }
}