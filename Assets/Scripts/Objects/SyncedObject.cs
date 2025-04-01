/*using UnityEngine;
using Normal.Realtime;

public class SyncedObject : MonoBehaviour
{
    private RealtimeTransform _realtimeTransform;
    private int _localClientID;

    void Awake()
    {
        _realtimeTransform = GetComponent<RealtimeTransform>();
        if (_realtimeTransform == null) _realtimeTransform = gameObject.AddComponent<RealtimeTransform>();
    }

    public void Initialize(Realtime realtime)
    {
        _localClientID = realtime.clientID;

        // Since this is a scene object, we need to ensure it's linked to the Realtime instance
        // Normcore doesn't natively support this, so we use a workaround
        RealtimeView view = GetComponent<RealtimeView>();
        if (view == null) view = gameObject.AddComponent<RealtimeView>();
        view.ownerIDSelf = -1; // Unowned initially
    }

    public void TakeOwnership()
    {
        _realtimeTransform.RequestOwnership();
        Debug.Log("Ownership requested");
    }

    public void ReleaseOwnership()
    {
        _realtimeTransform.ClearOwnership();
        Debug.Log("Ownership cleared");
    }

    public bool IsOwnedLocally()
    {
        return _realtimeTransform.ownerID == _localClientID;
    }
}*/