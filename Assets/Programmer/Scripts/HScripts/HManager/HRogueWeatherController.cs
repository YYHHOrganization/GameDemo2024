using System.Collections;
using System.Collections.Generic;
using CartoonFX;
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
    private AudioSource audioSource;
    Camera playerCamera;
    private GameObject rainManagerObj;
    
    private Material spppMat;// material of singlePixelPostProcess
    public AnimationCurve lightningCurve;
    
    //落雷
    private string thunderAlertVFXAddress = "ThunderAlertVFX";
    private GameObject thunderAlertVFX;
    private string thunderLightingStrikeVFXAddress = "ThunderLightingStrikeVFX";
    private GameObject thunderLightingStrikeVFX;
    void Start()
    {
        dirLight = GameObject.Find("Directional Light").GetComponent<Light>();
        originalDirLightIntensity = dirLight.intensity;
        thunderAlertVFX = Addressables.LoadAssetAsync<GameObject>(thunderAlertVFXAddress).WaitForCompletion();
        thunderLightingStrikeVFX = Addressables.LoadAssetAsync<GameObject>(thunderLightingStrikeVFXAddress).WaitForCompletion();
    }

    public bool testRain = false;
    public bool resetEverything = false;
    // Update is called once per frame
    void Update()
    {
        if (testRain)
        {
            testRain = false;
            StartCoroutine(StartRainingBase());
        }

        if (resetEverything)
        {
            resetEverything = false;
            ResetEverything();
        }
    }
    
    //todo: 天气要逐渐放晴，而不是突然直接停止
    private void ResetEverything()
    {
        // 1.关闭雾效
        RenderSettings.fog = false;
        // 2.关闭单个像素后处理效果
        if (playerCamera.transform.GetChild(0).gameObject.name != "singlePixelPostProcess")
        {
            Debug.Log("Please add a singlePixelPostProcess object as the first child of the camera");
        }
        GameObject cameraMesh = playerCamera.transform.GetChild(0).gameObject;
        cameraMesh.SetActive(false);
        //3.关闭天色变暗
        dirLight.intensity = originalDirLightIntensity;
        //4.关闭音效
        audioSource.Stop();
        //5.关闭闪电
        StopAllCoroutines();
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 0.5f));
        //6.关闭rainManager
        Destroy(rainManagerObj);
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogHeight",false);
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogDistance",false);
    }
    
    IEnumerator LightningControl()
    {
        while (true)
        {
            StartCoroutine(Lightning(6f));
            HAudioManager.Instance.Play("ThunderSoundAudio", playerCamera.gameObject);
            audioSource.volume = 5;
            yield return new WaitForSeconds(32.88f);
        }
    }

    IEnumerator LightingFall()
    {
        //降下落雷
        while (true)
        {
            Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
            //在玩家周围0~5米范围内随机选择一个Position，生成ThunderAlertVFX
            Vector3 randomPos = player.position + new Vector3(Random.Range(-5f, 5f), 0.05f, Random.Range(-5f, 5f));
            GameObject thunderAlert = Instantiate(thunderAlertVFX, randomPos, Quaternion.Euler(-90f, 0f, 0f));
            yield return new WaitForSeconds(5f);
            //降下落雷
            Destroy(thunderAlert);
            Vector3 strikePos = randomPos;
            strikePos.y = 0f;
            GameObject thunderLightingStrike = Instantiate(thunderLightingStrikeVFX, strikePos, Quaternion.identity);
            HRogueCameraManager.Instance.ShakeCamera(10f, 0.5f);
            HAudioManager.Instance.Play("LightningAttackSound", thunderLightingStrike.gameObject);
            yield return new WaitForSeconds(1f);
            Destroy(thunderLightingStrike,2f);
            //随机6~12秒
            float intervalTime = Random.Range(3f, 6f);
            yield return new WaitForSeconds(intervalTime);
        }
    }
    
    //light up whole screen during the lightning
    IEnumerator Lightning(float time)
    {
        yield return new WaitForSeconds(2.375f - 0.2f);
        float i = 0f;
        float rate = 1f / time;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            // synchronize the lightning with its sound, and the random value is to make the lightning shiver
            float intense = lightningCurve.Evaluate(i) * (1f + Random.Range(-0.2f, 0.4f));
            // use this color to control lightning and darken the environment
            Color lightningColor = new Color(0.7f * intense, 0.75f * intense, 1.0f * intense, 0.5f);
            // spppMat is the material of the whole screen quad, here we change its output color
            // by the way, "sppp" stands for "Single Pixel Post Processing"
            spppMat.SetColor("_Color", lightningColor);
            yield return 0;
        }
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 0.5f));
    }

    IEnumerator StartRainingBase()
    {
        rainManagerObj = Addressables.LoadAssetAsync<GameObject>(rainManagerAddress).WaitForCompletion();
        GameObject player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer();
        playerCamera = player.GetComponent<HPlayerStateMachine>().playerCamera;
        rainManagerObj = Instantiate(rainManagerObj, playerCamera.transform);
        rainManager = rainManagerObj.GetComponent<HRogueRainManager>();
        // 1.下雨的时候要开启雾效，使用Unity的Angry Bots的Demo示例当中的下雨效果
        RenderSettings.fog = true;
        RenderSettings.fogMode = UnityEngine.FogMode.Linear;
        if (rainManager)
        {
            rainManager.gameObject.SetActive(true);
            rainBox = rainManager.transform.GetComponentInChildren<HRogueRainBox>();
            rainBox.rainSparsity = Mathf.Lerp(5f, 1f, 0f);
        }
        // 2.下雨的时候要开启单个像素后处理效果，通过在屏幕上贴一个Mesh并作Blend One SrcAlpha的材质来实现
        if (playerCamera.transform.GetChild(0).gameObject.name != "singlePixelPostProcess")
        {
            Debug.Log("Please add a singlePixelPostProcess object as the first child of the camera");
        }

        GameObject cameraMesh = playerCamera.transform.GetChild(0).gameObject;
        cameraMesh.SetActive(true);
        spppMat = cameraMesh.GetComponent<Renderer>().sharedMaterial;
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 1.0f));
        
        //3.下雨的时候天色会相对来说暗一些，并且有一些雾效（Unity自带Linear雾 + 项目里的高度雾+深度雾）
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 0f);
        fogThin = Mathf.Lerp(2f, 1f, 0f);
        RenderSettings.fogStartDistance = linearFogStart * fogThin;
        RenderSettings.fogEndDistance = linearFogEnd * fogThin;
        playerCamera.farClipPlane = RenderSettings.fogEndDistance;
        
        //4.音效
        HAudioManager.Instance.Play("RainSoundAudio", playerCamera.gameObject);
        audioSource = playerCamera.GetComponent<AudioSource>();
        audioSource.volume = 0f;
        
        // progressively intensify the rain
        float i = 0f;
        float rate = 1f / startRainTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            rainBox.rainSparsity = Mathf.Lerp(5f, 1f, i) ;
            dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, i);
            fogThin = Mathf.Lerp(2f, 1f, i);
            RenderSettings.fogStartDistance = linearFogStart * fogThin * (2.0f - i);
            RenderSettings.fogEndDistance = linearFogEnd * fogThin * (2.0f - i);
            spppMat.SetColor("_Color", new Color(0f, 0f, 0f, (0.5f + 0.5f * (1.0f - i))));
            audioSource.volume = i * 5;
            HPostProcessingFilters.Instance.SetPostProcessingWithName("FogHeight",true, 1.2f * i);
            HPostProcessingFilters.Instance.SetPostProcessingWithName("FogDistance",true, 0.03f * i);
            yield return null;
        }
        // terminal status
        rainBox.rainSparsity = Mathf.Lerp(5f, 1f, 1f);
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 1f);
        fogThin = Mathf.Lerp(2f, 1f, 1f);
        RenderSettings.fogStartDistance = linearFogStart * fogThin;
        RenderSettings.fogEndDistance = linearFogEnd * fogThin;
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 0.5f));
        // start to repeatedly play lightning
        StartCoroutine(LightningControl());
        StartCoroutine(LightingFall());
    }
}
