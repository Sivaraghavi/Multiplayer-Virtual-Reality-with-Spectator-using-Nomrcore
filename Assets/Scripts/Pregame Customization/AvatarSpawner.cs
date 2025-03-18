using UnityEngine;

public class AvatarSpawner : MonoBehaviour
{
    public GameObject dummyAvatarPrefab;

    void Start()
    {
        Instantiate(dummyAvatarPrefab);
    }
}