
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PreGameCustomization : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown colorDropdown;
    public TMP_InputField roomInputField; // Add this in Inspector
    public Button createRoomButton; // Add this in Inspector
    public Button joinRoomButton;
   // public Button readyButton;
    public Material[] colorMaterials; // Assign 5 materials in inspector

    void Start()
    {
        InitializeColorDropdown();
        nameInputField.onValueChanged.AddListener(UpdatePlayerName);
        colorDropdown.onValueChanged.AddListener(UpdatePlayerColor);
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
       //eadyButton.onClick.AddListener(OnReadyClicked);
    }

    void InitializeColorDropdown()
    {
        colorDropdown.ClearOptions();
        colorDropdown.AddOptions(new List<string> { "Red", "Blue", "Green", "Yellow", "Cyan" });
        colorDropdown.onValueChanged.AddListener(UpdatePlayerColor);
    }

    void UpdatePlayerName(string newName)
    {
        CustomizationData.PlayerName = string.IsNullOrEmpty(newName) ? "Player" : newName;
    }

    void UpdatePlayerColor(int colorIndex)
    {
        if (colorIndex >= 0 && colorIndex < colorMaterials.Length)
        {
            CustomizationData.PlayerMaterial = colorMaterials[colorIndex];
        }
    }
    void OnCreateRoomClicked()
    {
        SaveCustomizationData();
        PlayerPrefs.SetString("RoomName", roomInputField.text);
        PlayerPrefs.SetInt("IsCreatingRoom", 1); // Flag for creating
        SceneManager.LoadScene("1_CommonRoom");
    }

    void OnJoinRoomClicked()
    {
        SaveCustomizationData();
        PlayerPrefs.SetString("RoomName", roomInputField.text);
        PlayerPrefs.SetInt("IsCreatingRoom", 0); // Flag for joining
        SceneManager.LoadScene("1_CommonRoom");
    }
    void SaveCustomizationData()
    {
        PlayerPrefs.SetString("PlayerName", CustomizationData.PlayerName);
        PlayerPrefs.SetInt("ColorIndex", colorDropdown.value);
    }

    /*void OnReadyClicked()
    {
        PlayerPrefs.SetString("PlayerName", CustomizationData.PlayerName);
        PlayerPrefs.SetInt("ColorIndex", colorDropdown.value);
        SceneManager.LoadScene("1_CommonRoom");
    }*/
}