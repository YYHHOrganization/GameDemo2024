using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YChooseCharacterPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YChooseCharacterPanelNew";
    static readonly string pathPlace = "Prefabs/YPlace/YChooseCharacterShowPlace";
    public YChooseCharacterPanel() : base(new UIType(path)){}
    public GameObject YChooseCharacterShowPlace;
    public GameObject curCharacter;
    public int curChooseCharacterIndex=-1;

    private bool isInteractMode = true;
    public override void OnEnter()
    {
        //YChooseCharacterShowPlace = GameObject.Find("YChooseCharacterShowPlace");
        YChooseCharacterShowPlace = GameObject.Instantiate(Resources.Load<GameObject>(pathPlace));
        
        uiTool.GetOrAddComponentInChilden<Button>("BeginButton").onClick.AddListener(()=>
        {
            Pop();
            //Push(new YChooseScreenplayPanel());//其实并不是 而是应该直接让角色在场景中活动，但是先这样吧
            
            Push(new YMainPlayModeOriginPanel());
            //销毁YChooseCharacterShowPlace
            GameObject.Destroy(YChooseCharacterShowPlace);
            //然后应该把那些加载出来 
            YPlayModeController.Instance.SetCharacter(curChooseCharacterIndex);

        });
        Debug.Log("角色数量"+yPlanningTable.Instance.GetCharacterNum());
        //循环遍历角色列表
        for (int i = 0; i < yPlanningTable.Instance.GetCharacterNum(); i++)
        {
            //不可以直接传i 因为i会变，因为是引用类型
            int index = i;
            //给每个角色按钮添加点击事件
            uiTool.GetOrAddComponentInChilden<Button>("CharacterButton"+index).onClick.AddListener(()=>
            {
                SetCharacter(index);
            });
        }
        SetCharacter(0);

        isInteractMode = true;
        var interactButton = uiTool.GetOrAddComponentInChilden<Button>("SelectPatternButton");
        interactButton.onClick.AddListener(() =>
        {
            isInteractMode = !isInteractMode;
            if (isInteractMode)
            {
                //change text
                interactButton.GetComponentInChildren<TMP_Text>().text = "交互模式";
            }
            else
            {
                interactButton.GetComponentInChildren<TMP_Text>().text = "鉴赏模式";
            }

            if (curCharacter)
            {
                curCharacter.GetComponent<HCharacterInteracionInShow>().SetInteractMode(isInteractMode);
            }
        });
    }
    public void SetCharacter(int index)
    {
        if(index==curChooseCharacterIndex)
        {
            return;
        }
        if (curCharacter!=null)
        {
            GameObject.Destroy(curCharacter);
        }
        curChooseCharacterIndex = index;
        
        Debug.Log("选择了角色"+index);
        int id = yPlanningTable.Instance.selectNames2Id["character"];
        string path = "Prefabs/YCharacter/"+yPlanningTable.Instance.SelectTable[id][index]+"Show";
        //string path = "Prefabs/YCharacter/2";
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(path));
        go.transform.parent = YChooseCharacterShowPlace.transform;
        go.transform.localPosition = Vector3.zero;
        curCharacter = go;

        go.GetComponent<HCharacterInteracionInShow>().SetInteractMode(isInteractMode);
        
        //设置角色
        //yPlanningTable.Instance.SetCharacter(i);
    }
}
