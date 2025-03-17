/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    public Transform playerTarget;
    public Transform mirror;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localPlayer = mirror.InverseTransformPoint(playerTarget.position);
        transform.position = mirror.TransformPoint(new Vector3(localPlayer.x,localPlayer.y, -localPlayer.z));

        Vector3 lookatmirror = mirror.TransformPoint(new Vector3(-localPlayer.x,localPlayer.y,localPlayer.z));
        transform.LookAt(lookatmirror);
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    public Transform playerTarget;  // The player object (or camera)
    public Transform mirror;        // The mirror object
    public Camera mirrorCamera;     // The camera used for rendering the reflection

    void Start()
    {
        if (mirrorCamera == null)
        {
            Debug.LogError("Mirror Camera is not assigned!");
        }
    }

    void Update()
    {
        // Get the player's position relative to the mirror
        Vector3 localPlayer = mirror.InverseTransformPoint(playerTarget.position);

        // Adjust the reflection camera position
        // Reflect the player's position along the plane of the mirror (inverse the Z position)
        mirrorCamera.transform.position = mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));

        // Make the reflection camera look at the correct reflection of the player
        Vector3 lookAtMirror = mirror.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        mirrorCamera.transform.LookAt(lookAtMirror);

        // Optionally, reflect the camera's rotation (to make the player's rotation mirrored)
        Vector3 reflectedRotation = new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z);
        transform.LookAt(mirror.TransformPoint(reflectedRotation));
    }
}
