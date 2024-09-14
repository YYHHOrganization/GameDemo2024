using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RutPainter : MonoBehaviour
{
    //公开
    public Transform playerTf;//玩家对象
    public Renderer groundRenderer;//地面渲染器
    public float paintSize = 64;//绘制矩形边长
    public float attenTime = 10;//淡化时间
    public enum RTSize
    {
       _256 = 256,
       _512 = 512,
       _1024 = 1024,
       _2048 = 2048
    }
    public RTSize rtSize = RTSize._1024;//渲染纹理尺寸

    //内置
    public RenderTexture paintRT;//轨迹渲染纹理 RGB法线 A高度
    private Material paintMat;//更新法线高度
    private Material fadeMat;//轨迹淡化计算材质
    public Material groundMat;//地形材质
    private Vector3 playerOldPos;//上次绘制时玩家的位置(经过离散化截断）
    private void Start()
    {
        
    }
    bool isInit = false;

    public void Initialize()
    {
        playerTf = YPlayModeController.Instance.curCharacter.transform;
        InitPaintProp();
        isInit = true;
    }

    private void Update()
    {
        if (!isInit) return;
        //轨迹淡化
        RenderTexture tempRT = RenderTexture.GetTemporary(paintRT.descriptor);
        Graphics.Blit(paintRT, tempRT, fadeMat, 0);
        Graphics.Blit(tempRT, paintRT);
        RenderTexture.ReleaseTemporary(tempRT);
    }

    //初始化绘制属性
    public void InitPaintProp()
    {
        if (playerTf)
        {
            Debug.Log("GetPlayerTf~~~~~1!!!!!!");
            playerOldPos = playerTf.position;

            //初始化交换纹理
            Texture2D tempTex = new Texture2D(1, 1, TextureFormat.ARGB32, 0, true);
            tempTex.SetPixel(0, 0, new Color(0.5f, 0.5f, 1, 0.5f));
            tempTex.Apply();

            //初始化轨迹RT设置
            paintRT = new RenderTexture((int)rtSize, (int)rtSize, 0, RenderTextureFormat.ARGB64, RenderTextureReadWrite.Linear);
            paintRT.wrapMode = TextureWrapMode.Clamp;
            paintRT.filterMode = FilterMode.Bilinear;
            paintRT.anisoLevel = 0;
            Graphics.Blit(tempTex, paintRT);

            //地形材质配置
            groundMat = groundRenderer.sharedMaterial;
            groundMat.SetTexture("_RutRTTex", paintRT);

            //初始化渲染材质
            paintMat = new Material(Shader.Find("Scene/SandSystem/RutPaint"));
            fadeMat = new Material(Shader.Find("Scene/SandSystem/RutFade"));
            fadeMat.SetFloat("_AttenTime", attenTime);
        }
    }

    //绘制方法
    public void Paint(Transform tfIN, Texture2D brushTex, float brushRadius, float brushInt, Texture2D brushTexMask)
    {
        Vector4 pos_Offset;

        //如果输入对象是玩家，则额外计算纹理偏移与地形材质的范围参数
        if (tfIN == playerTf)
        {
            //位移向量按纹理尺寸离散化，抵消采样时的抖动
            Vector3 deltaDir01 = (playerTf.position - playerOldPos) / paintSize;
            int tempRtSize = (int)rtSize;
            deltaDir01 = deltaDir01 * tempRtSize;
            deltaDir01.x = Mathf.Floor(deltaDir01.x) / tempRtSize;
            deltaDir01.z = Mathf.Floor(deltaDir01.z) / tempRtSize;
            pos_Offset = new Vector4(0.5f, 0.5f, deltaDir01.x, deltaDir01.z);

            //地形材质范围更新
            playerOldPos += deltaDir01 * paintSize;
            playerOldPos.y = playerTf.position.y;
            float halfSize = paintSize / 2;
            Vector3 pos00 = playerOldPos - new Vector3(halfSize, 0, halfSize);
            Vector3 pos11 = playerOldPos + new Vector3(halfSize, 0, halfSize);
            groundMat.SetVector("_PaintRect", new Vector4(pos00.x, pos00.z, pos11.x, pos11.z));
        }

        //非玩家，计算当前对象相对玩家的归一化位置
        else
        {
            Vector3 deltaDir01 = (tfIN.position - playerTf.position) / paintSize;
            pos_Offset = new Vector4(0.5f + deltaDir01.x, 0.5f + deltaDir01.z, 0, 0);
        }

        //轨迹材质参数配置
        paintMat.SetTexture("_BrushTex", brushTex);
        paintMat.SetVector("_BrushPosTS_Offset", pos_Offset);
        paintMat.SetFloat("_BrushRadius", brushRadius / paintSize);
        paintMat.SetFloat("_BrushInt", brushInt);
        paintMat.SetTexture("_BrushTexMask", brushTexMask);
        Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        if (player)
        {
            paintMat.SetVector("_playerDirection", player.forward);
        }

        //轨迹RT
        RenderTexture tempRT = RenderTexture.GetTemporary(paintRT.descriptor);
        Graphics.Blit(paintRT, tempRT, paintMat, 0);
        Graphics.Blit(tempRT, paintRT);
        RenderTexture.ReleaseTemporary(tempRT);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RutPainter))]
class RutPainterEditor : Editor
{
    RutPainter dst;
    void OnEnable()
    {
        dst = (RutPainter)target;
    }

    public override void OnInspectorGUI()
    {
        //撤销堆栈
        Undo.RecordObject(dst, "RutPainter");

        //公共参数
        GUILayout.Label("当前单位像素长度 = " + dst.paintSize / (float)dst.rtSize);
        dst.playerTf = EditorGUILayout.ObjectField("玩家", dst.playerTf, typeof(Transform), true) as Transform;
        dst.groundRenderer = EditorGUILayout.ObjectField("地面渲染器", dst.groundRenderer, typeof(Renderer), true) as Renderer;
        dst.paintSize = EditorGUILayout.Slider("绘制范围", dst.paintSize, 8, 512);
        dst.attenTime = EditorGUILayout.Slider("淡化时间", dst.attenTime, 0.5f, 60);
        dst.rtSize = (RutPainter.RTSize)EditorGUILayout.EnumPopup("渲染纹理分辨率", dst.rtSize);
        GUI.enabled = false;
        dst.paintRT = EditorGUILayout.ObjectField("轨迹纹理", dst.paintRT, typeof(RenderTexture), true) as RenderTexture;
        GUI.enabled = true;
        if (GUILayout.Button("刷新绘制状态"))
        {
            dst.InitPaintProp();
        }

        SceneView.RepaintAll();
    }

    private void OnSceneGUI()
    {
        Transform playerTf = dst.playerTf;
        if (playerTf)
        {
            Handles.color = Color.red;
            float h = 10;
            float r = dst.paintSize / 2;

            Vector3[] tempPos = new Vector3[] {
                playerTf.position + new Vector3(-r, h, -r),
                playerTf.position + new Vector3(-r, h, r),
                playerTf.position + new Vector3(r, h, r),
                playerTf.position + new Vector3(r, h, -r)
            };
            Handles.DrawAAPolyLine(10, tempPos[0], tempPos[1], tempPos[2], tempPos[3], tempPos[0]);
            for (int i = 0; i < tempPos.Length; i++)
            {
                Ray ray = new Ray(tempPos[i], Vector3.down);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    Handles.DrawAAPolyLine(10, tempPos[i], hit.point);
                }
            }
        }

        Handles.BeginGUI();
        GUI.DrawTexture(new Rect(0, 0, 200, 200), dst.paintRT);
        Handles.EndGUI();
    }
}
#endif