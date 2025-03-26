/*// Only head color change

using TMPro;
using UnityEngine;

public class DummyAvatarCustomizer : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI nameTag;
    public MeshRenderer[] bodyMeshes;


    void Start()
    {
        // Create material instances to avoid shared materials
        foreach (var renderer in bodyMeshes)
        {
            renderer.material = new Material(renderer.material);
        }
    }

    void Update()
    {
        nameTag.text = CustomizationData.PlayerName;

        foreach (var mesh in bodyMeshes)
        {
            mesh.material.color = CustomizationData.PlayerMaterial.color;
        }
    }
}


*/

using TMPro;
using UnityEngine;

public class DummyAvatarCustomizer : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI nameTag;
    public MeshRenderer[] bodyMeshes;

    void Start()
    {
        // Create material instances to avoid shared materials
        foreach (var renderer in bodyMeshes)
        {
            renderer.material = new Material(renderer.material);
        }
    }

    void Update()
    {
        nameTag.text = CustomizationData.PlayerName;

        foreach (var mesh in bodyMeshes)
        {
            mesh.material.color = CustomizationData.PlayerColor; // Use PlayerColor instead of PlayerMaterial
        }
    }
}