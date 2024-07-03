using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTriggerDialogSystem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HDialogSystemMgr dialogSystemMgr = yPlanningTable.Instance.gameObject.GetComponent<HDialogSystemMgr>();
            dialogSystemMgr.SetTrigger(other.gameObject);
            dialogSystemMgr.SetUpAndStartDialog();
        }
    }
}
