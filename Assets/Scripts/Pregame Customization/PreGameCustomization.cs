using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PreGameCustomization : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown colorDropdown; 
    public Button readyButton;
// nothing just a comment so that i can merge the file 

    void Start()
    {
        Debug.Log("[DEBUG] PreGameCustomization script started");
        nameInputField.onValueChanged.AddListener(UpdatePlayerName);
        readyButton.onClick.AddListener(OnReadyClicked);

        Debug.Log("PreGameCustomization Initialized");
    }
    void UpdatePlayerName(string newName)
    {
        CustomizationData.PlayerName = string.IsNullOrEmpty(newName) ? "Player" : newName;
        Debug.Log("Player Name Updated: " + CustomizationData.PlayerName);
    }

    void OnReadyClicked()
    {
       

        
        switch (colorDropdown.value)
        {
            case 0: CustomizationData.PlayerColor = Color.red; break;
            case 1: CustomizationData.PlayerColor = Color.blue; break;
            case 2: CustomizationData.PlayerColor = Color.green; break;
            // Add more cases as needed.
            default: CustomizationData.PlayerColor = Color.white; break;
        }

        Debug.Log($"Final Selection -> Name: {CustomizationData.PlayerName}, Color: {CustomizationData.PlayerColor}");


        SceneManager.LoadScene("1_CommonRoom");
    }
}

