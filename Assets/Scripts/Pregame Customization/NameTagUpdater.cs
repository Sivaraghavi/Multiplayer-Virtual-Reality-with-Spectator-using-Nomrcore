using TMPro;
using UnityEngine;

public class NameTagUpdater : MonoBehaviour
{
    public TextMeshProUGUI nameTag;
    public Transform headTransform;
    public Vector3 offset = new Vector3(0, 0.2f, 0);

    void Update()
    {
        if (nameTag == null || headTransform == null || Camera.main == null)
        {
            Debug.LogError("NameTagUpdater is missing a reference!");
            return;
        }
        nameTag.text = CustomizationData.PlayerName;
        nameTag.transform.position = headTransform.position + offset;
        nameTag.transform.rotation = Quaternion.LookRotation(
            nameTag.transform.position - Camera.main.transform.position);
    }
}