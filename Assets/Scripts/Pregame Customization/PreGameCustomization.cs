using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PreGameCustomization : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Dropdown colorDropdown; 
    public Button readyButton;

    void Start()
    {
        readyButton.onClick.AddListener(OnReadyClicked);
    }

    void OnReadyClicked()
    {
        
        CustomizationData.PlayerName = string.IsNullOrEmpty(nameInputField.text) ? "Player" : nameInputField.text;

        
        switch (colorDropdown.value)
        {
            case 0: CustomizationData.PlayerColor = Color.red; break;
            case 1: CustomizationData.PlayerColor = Color.blue; break;
            case 2: CustomizationData.PlayerColor = Color.green; break;
            // Add more cases as needed.
            default: CustomizationData.PlayerColor = Color.white; break;
        }

        
        SceneManager.LoadScene("1_CommonRoom");
    }
}
