using UnityEngine;
using UnityEngine.Events;


namespace PartsOfSimpleMicroscope
{
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
    
    
}