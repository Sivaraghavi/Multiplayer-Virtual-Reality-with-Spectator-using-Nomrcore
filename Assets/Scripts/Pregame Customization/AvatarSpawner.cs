using UnityEngine;

public class AvatarSpawner : MonoBehaviour
{
    public GameObject dummyAvatarPrefab;
    public Transform xrOriginParent; // Drag XR Origin's Camera Offset here

    void Start()
    {
        GameObject avatar = Instantiate(dummyAvatarPrefab, xrOriginParent);
        avatar.transform.localPosition = Vector3.zero;
        avatar.transform.localRotation = Quaternion.identity;
    }
}