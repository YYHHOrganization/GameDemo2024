using UnityEngine;
using UnityEngine.UI;

public class YTestRecall  : MonoBehaviour
{
    public Button recallButton; // Assign this in the inspector
    public GameObject[] recallableObject; // Assign the object with the YRecallable script in the inspector

    private void Start()
    {
        // Add a listener to the button's onClick event
        recallButton.onClick.AddListener(StartRecall);
    }

    private void StartRecall()
    {
        // Get the YRecallable script from the object and start the recall
        // YRecallable recallable = recallableObject.GetComponent<YRecallable>();
        foreach (GameObject obj in recallableObject)
        {
            YRecallable recallable = obj.GetComponent<YRecallable>();
            if (recallable != null)
            {
                Debug.Log("Recalling object");
                recallable.StartCoroutine(recallable.Recall());
            }
        }
        
    }
}