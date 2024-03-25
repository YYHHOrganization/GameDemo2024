using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    private PanelManager panelManager;
    private void Awake()
    {
        panelManager = new PanelManager();
    }
    // Start is called before the first frame update
    void Start()
    {
        // panelManager.Push(new StartPanel());
        panelManager.Push(new YLoadPanel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
