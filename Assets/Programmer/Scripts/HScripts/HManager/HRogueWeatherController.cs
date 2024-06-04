using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HRogueWeatherController : MonoBehaviour
{
    //refs : https://zhuanlan.zhihu.com/p/118533357
    // Start is called before the first frame update
    private HRogueRainManager rainManager;
    private string rainManagerAddress = "RainManager";
    private HRogueRainBox rainBox;
    private Light dirLight;
    private float originalDirLightIntensity;
    public float darkestDirLightIntensity = 0.5f;
    private float fogThin;
    public float linearFogStart = 8.6f;
    public float linearFogEnd = 21.4f;
    private const float startRainTime = 8f;
    
    void Start()
    {
        dirLight = GameObject.Find("Directional Light").GetComponent<Light>();
        originalDirLightIntensity = dirLight.intensity;
    }

    public bool testRain = false;

    // Update is called once per frame
    void Update()
    {
        if (testRain)
        {
            testRain = false;
            StartCoroutine(StartRainingBase());
        }
    }

    IEnumerator StartRainingBase()
    {
        GameObject rainManagerObj = Addressables.LoadAssetAsync<GameObject>(rainManagerAddress).WaitForCompletion();
        GameObject player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer();
        Camera playerCamera = player.GetComponent<HPlayerStateMachine>().playerCamera;
        Instantiate(rainManagerObj, playerCamera.transform);
        rainManager = rainManagerObj.GetComponent<HRogueRainManager>();
        RenderSettings.fog = true;
        RenderSettings.fogMode = UnityEngine.FogMode.Linear;
        if (rainManager)
        {
            rainManager.gameObject.SetActive(true);
            rainBox = rainManager.transform.GetComponentInChildren<HRogueRainBox>();
            rainBox.rainSparsity = Mathf.Lerp(5f, 1f, 0f);
        }
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 0f);
        fogThin = Mathf.Lerp(2f, 1f, 0f);
        RenderSettings.fogStartDistance = linearFogStart * fogThin;
        RenderSettings.fogEndDistance = linearFogEnd * fogThin;
        playerCamera.farClipPlane = RenderSettings.fogEndDistance;
        
        HAudioManager.Instance.Play("RainSoundAudio", playerCamera.gameObject);
        AudioSource audioSource = playerCamera.GetComponent<AudioSource>();
        audioSource.volume = 0f;
        
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogHeight",true, 2f);
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogDistance",true, 2f);
        
        // progressively intensify the rain
        float i = 0f;
        float rate = 1f / startRainTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            rainBox.rainSparsity = Mathf.Lerp(5f, 1f, i) ;
            dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, i);
            fogThin = Mathf.Lerp(2f, 1f, i);
            RenderSettings.fogStartDistance = linearFogStart * fogThin;
            RenderSettings.fogEndDistance = linearFogEnd * fogThin;
            audioSource.volume = i * 5;
            yield return null;
        }
        // terminal status
        rainBox.rainSparsity = Mathf.Lerp(5f, 1f, 1f);
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 1f);
        fogThin = Mathf.Lerp(2f, 1f, 1f);
        RenderSettings.fogStartDistance = linearFogStart * fogThin;
        RenderSettings.fogEndDistance = linearFogEnd * fogThin;
    }
}
