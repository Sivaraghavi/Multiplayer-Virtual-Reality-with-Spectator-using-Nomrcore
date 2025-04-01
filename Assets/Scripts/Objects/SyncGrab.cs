/*using UnityEngine;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SyncGrab : MonoBehaviour
{
    private SyncedObject _syncedObject;
    private XRGrabInteractable _grabInteractable;

    void Awake()
    {
        _syncedObject = GetComponent<SyncedObject>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
        _grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        _grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _syncedObject.TakeOwnership();
        Debug.Log("Object grabbed, taking ownership");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _syncedObject.ReleaseOwnership();
        Debug.Log("Object released, releasing ownership");
    }

    void Update()
    {
        if (_syncedObject != null && _syncedObject._roomStateModel != null)
        {
            int currentOwner = _syncedObject._roomStateModel._objectOwners[_syncedObject.objectID];
            if (currentOwner == _syncedObject._localClientID)
            {
                _syncedObject.UpdateModelIfOwner();
            }
        }
    }
}*/

using UnityEngine;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SyncGrab : MonoBehaviour
{
    private RealtimeTransform _realtimeTransform;
    private XRGrabInteractable _grabInteractable;

    void Awake()
    {
        _realtimeTransform = GetComponent<RealtimeTransform>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
        _grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        _grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _realtimeTransform.RequestOwnership();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        Debug.Log("Object grabbed, ownership taken");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _realtimeTransform.ClearOwnership();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
        Debug.Log("Object released, ownership cleared");
    }
}