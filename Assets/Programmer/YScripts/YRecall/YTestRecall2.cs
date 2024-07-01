using UnityEngine;
using UnityEngine.UI;
public class YTestRecall2 : MonoBehaviour
{
    // public Button recallButton; // Assign this in the inspector
    public Button generateRecallObjButton; 
    // Start is called before the first frame update
    void Start()
    {
        generateRecallObjButton.onClick.AddListener(generateRecallObj);
        // Add a listener to the button's onClick event
        // recallButton.onClick.AddListener(StartRecall);
    }

    private GameObject player;
    void generateRecallObj()
    {
        
        if(player==null)player = YPlayModeController.Instance.curCharacter;
        string id = "33310000";
        GameObject go = YObjectPool._Instance.Spawn(id);
        go.transform.position = player.transform.position+new Vector3(Random.Range(-4,4),Random.Range(1,5),Random.Range(-4,4));
        go.SetActive(true);
    }
    private void StartRecall()
    {
        
        
    }
}
