using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTriggerDialogSystem : MonoBehaviour
{
    private bool isInteracted = false;
    public void SetInteracted(bool interacted)
    {
        isInteracted = interacted;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isInteracted) return;
            HDialogSystemMgr dialogSystemMgr = yPlanningTable.Instance.gameObject.GetComponent<HDialogSystemMgr>();
            dialogSystemMgr.SetTrigger(gameObject);
            dialogSystemMgr.SetUpAndStartDialog();
        }
    }
}
