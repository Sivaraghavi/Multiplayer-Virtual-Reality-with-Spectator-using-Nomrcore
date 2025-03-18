using TMPro;
using UnityEngine;

public class DummyAvatarCustomizer : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI nameTag;  
    public MeshRenderer[] bodyMeshes;  // Array to hold head, hands, and body mesh renderers

    void Start()
    {
        if (nameTag == null) Debug.LogError("NameTag is NOT assigned in DummyAvatarCustomizer!");
        if (bodyMeshes == null || bodyMeshes.Length == 0) Debug.LogError("BodyMeshes are NOT assigned in DummyAvatarCustomizer!");
    }

    void Update()
    {
        if (nameTag != null)
            nameTag.text = CustomizationData.PlayerName;  // Update the name tag text

        if (bodyMeshes != null && bodyMeshes.Length > 0)
        {
            foreach (MeshRenderer mesh in bodyMeshes)
            {
                if (mesh != null)
                    mesh.material.color = CustomizationData.PlayerColor;  // Apply color to all body parts
            }
        }
    }
}
