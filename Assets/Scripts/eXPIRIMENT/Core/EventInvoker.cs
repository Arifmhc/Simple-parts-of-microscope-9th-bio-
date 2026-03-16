using UnityEngine;
using UnityEngine.Events;

public class EventInvoker : MonoBehaviour
{

    public UnityEvent myEvent;
     
    public void InvokeMyEvent()
    {
        if (myEvent != null)
        {
            myEvent.Invoke();
        }
    }
}
