
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PreGameCustomization : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown colorDropdown;
    public Button readyButton;
    public Material[] colorMaterials; // Assign 5 materials in inspector

    void Start()
    {
        InitializeColorDropdown();
        nameInputField.onValueChanged.AddListener(UpdatePlayerName);
        readyButton.onClick.AddListener(OnReadyClicked);
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

    void OnReadyClicked()
    {
        PlayerPrefs.SetString("PlayerName", CustomizationData.PlayerName);
        PlayerPrefs.SetInt("ColorIndex", colorDropdown.value);
        SceneManager.LoadScene("1_CommonRoom");
    }
}