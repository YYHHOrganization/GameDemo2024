using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HTimelineTriggerSomething : MonoBehaviour
{
    private YLoadPanel thisPanel;
    //主要是加载界面用来播放timeline和放帕姆的
    public void LoadScenePlayDirector(YLoadPanel panel, float time)
    {
        thisPanel = panel;
        var director = GetComponent<PlayableDirector>();
        var character = GameObject.FindWithTag("Puppet");
        
        Animator animationController = character.gameObject.GetComponent<Animator>();
        animationController.runtimeAnimatorController = null;
        if (director)
        {
            director.Play();
            //Debug.Log("director is playing");
        }
        else
        {
            thisPanel.ShowEnterGameButton();
        }
        
    }

    public void ShowEnterGameButton()
    {
        if (thisPanel!=null)
        {
            thisPanel.ShowEnterGameButton();
        }
        else
        {
            thisPanel = GameObject.Find("YLoadPanel").GetComponent<YLoadPanel>();
            thisPanel.ShowEnterGameButton();
        }
    }
}
