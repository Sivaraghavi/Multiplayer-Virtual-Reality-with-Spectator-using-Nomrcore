/*using UnityEngine;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit; // For XR Rig

public class XRAvatarSync : MonoBehaviour
{
    public RealtimeAvatar avatar; // Assign in Inspector or find at runtime
    public Transform xrHead;      // Assign XR Rig's Main Camera
    public Transform xrLeftHand;  // Assign XR Rig's LeftHand Controller
    public Transform xrRightHand; // Assign XR Rig's RightHand Controller

    void Start()
    {
        if (avatar == null)
        {
            avatar = GetComponent<RealtimeAvatar>();
            if (avatar == null)
            {
                Debug.LogError("No RealtimeAvatar found on this GameObject!");
                return;
            }
        }

        // Create LocalPlayer instance and assign XR transforms
        var localPlayer = new RealtimeAvatar.LocalPlayer
        {
            head = xrHead,
            leftHand = xrLeftHand,
            rightHand = xrRightHand
        };
        avatar.localPlayer = localPlayer;

        Debug.Log("XR Avatar Sync initialized");
    }
}*/