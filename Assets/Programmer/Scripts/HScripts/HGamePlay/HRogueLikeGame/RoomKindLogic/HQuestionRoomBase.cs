using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class HQuestionRoomBase : MonoBehaviour
{
    public string questionPath;
    private List<string> contents = new List<string>();
    private List<string> labels = new List<string>();
    
    // Start is called before the first frame update
    void Awake()
    {
        questionPath = "Assets/Designer/DatasetLabelling/usual_test_labeled.json";
        LoadQuestionFromDataset();
    }
    
    private void LoadQuestionFromDataset()
    {
        string jsonString = File.ReadAllText(questionPath, Encoding.UTF8);
        //seperate each object
        JArray jArray = JArray.Parse(jsonString);
        foreach (JObject obj in jArray.Children<JObject>())
        {
            foreach (JProperty singleProp in obj.Properties())
            {
                if (singleProp.Name == "content")
                {
                    contents.Add(singleProp.Value.ToString());
                }
                else if (singleProp.Name == "label")
                {
                    labels.Add(singleProp.Value.ToString());
                }
                    
            }
        }
    }

    public bool GiveAQuestionWithCurrectAnswer(out string content, out string label)
    {
        if (contents!=null && labels!=null)
        {
            int index = Random.Range(0, contents.Count);
            content = contents[index];
            label = labels[index];
            return true;
        }
        content = null;
        label = null;
        return false;
    }
    

    public bool giveAQuestion;
    // Update is called once per frame
    void Update()
    {
        if (giveAQuestion)
        {
            //HMessageShowMgr.Instance.ShowMessage();
            giveAQuestion = false;
        }
    }
    
    
}
