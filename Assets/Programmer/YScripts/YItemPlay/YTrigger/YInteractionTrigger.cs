using UnityEngine;

public class YInteractionTrigger : MonoBehaviour
{
    private int activatedTriggers = 0;//比如人数
    public bool activated=false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Puppet"))
        {
            Debug.Log("OnTriggerEnter！！！");
            activatedTriggers++;
            if (IsActivated())
            {
                if(activated==false)
                {
                    activated = true;
                    Debug.Log("OnTriggerEnter！！！IsActivated");
                    YTriggerEvents.RaiseOnTriggerStateChanged(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit！！！");
        if (other.CompareTag("Player")||other.CompareTag("Puppet"))
        {
            activatedTriggers--;
            if (!IsActivated())
            {
                Debug.Log("OnTriggerExit！！！IsActivated");
                YTriggerEvents.RaiseOnTriggerStateChanged(false);
                activated = false;
            }
        }
    }
    
    private bool IsActivated()
    {
        return activatedTriggers > 0;
    }
}