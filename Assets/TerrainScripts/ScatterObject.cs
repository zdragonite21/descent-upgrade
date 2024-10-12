using System;
using UnityEngine;

public class ScatterObject : MonoBehaviour
{
    private Action<ScatterObject> _killAction;

    public void Init(Action<ScatterObject> killAction, ref Action onChunkDelete)
    {
        Debug.Log("Init called");
        _killAction = killAction;
        onChunkDelete += HandleChunkDelete;
    }
    public void HandleChunkDelete()
    {
        Debug.Log("Delete Message Recieved");
        _killAction(this);
    }
}
