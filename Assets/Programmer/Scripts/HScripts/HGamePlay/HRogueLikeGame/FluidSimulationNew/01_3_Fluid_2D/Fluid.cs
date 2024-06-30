/* Reference
My code was originally based on: https://github.com/Scrawk/GPU-GEMS-2D-Fluid-Simulation
Nice tutorial understanding basic fluid concept: https://www.youtube.com/watch?v=iKAVRgIrUOU
Very nice tutorial for artists to understand the maths: https://shahriyarshahrabi.medium.com/gentle-introduction-to-fluid-simulation-for-programmers-and-technical-artists-7c0045c40bac
*/

using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Fluid : MonoBehaviour
{
	public ComputeShader shader;
	public Material matResult; //输出的Material，会作为quad上的材质
	public int size = 1024;
	public Transform sphere; //represents mouse
	public int solverIterations = 50;
	public Texture2D obstacleTex;

	[Header("Force Settings")] public float forceIntensity = 200f;
	public float forceRange = 0.01f;
	private Vector2 sphere_prevPos = Vector3.zero;
	//public Color dyeColor = Color.white;

	private RenderTexture velocityTex;
	private RenderTexture densityTex;
	private RenderTexture pressureTex;
	private RenderTexture divergenceTex;

	private int dispatchSize = 0;
	private int kernelCount = 0;
	private int kernel_Init = 0;
	private int kernel_Diffusion = 0;
	private int kernel_UserInput = 0;
	private int kernel_Jacobi = 0;
	private int kernel_Advection = 0;
	private int kernel_Divergence = 0;
	private int kernel_SubtractGradient = 0;

	public bool userDefDiffusionFactor;
	public float diffusionFactor = 100f;
	
	private RenderTexture CreateTexture(GraphicsFormat format)
	{
		RenderTexture dataTex = new RenderTexture(size, size, 0, format);
		dataTex.filterMode = FilterMode.Bilinear;
		dataTex.wrapMode = TextureWrapMode.Clamp;
		dataTex.enableRandomWrite = true;
		dataTex.Create();
		return dataTex;
	}

	public Texture2D showOriginTexture;

	private void DispatchCompute(int kernel)
	{
		shader.Dispatch(kernel, dispatchSize, dispatchSize, 1);
	}

	void Start()
	{
		//Create textures
		velocityTex = CreateTexture(GraphicsFormat.R16G16_SFloat); //float2 velocity
		densityTex = CreateTexture(GraphicsFormat.R16G16B16A16_SFloat); //float3 color , float density
		pressureTex = CreateTexture(GraphicsFormat.R16_SFloat); //float pressure
		divergenceTex = CreateTexture(GraphicsFormat.R16_SFloat); //float divergence
		
		// // 创建一个ComputeBuffer用于存储纹理数据: 这个不能用
		// ComputeBuffer texBuffer = new ComputeBuffer(size * size, sizeof(float) * 4);
		// // 从纹理中获取像素数据并传递给ComputeBuffer
		// Color[] texData = showOriginTexture.GetPixels();
		// texBuffer.SetData(texData);

		//Output
		matResult.SetTexture("_MainTex", densityTex);

		//Set shared variables for compute shader
		shader.SetInt("size", size); //texture resolution
		shader.SetFloat("forceIntensity", forceIntensity);
		shader.SetFloat("forceRange", forceRange);
		if (userDefDiffusionFactor)
		{
			shader.SetFloat("diffisionFactor", diffusionFactor);
		}
		else
		{
			shader.SetFloat("diffisionFactor", size);
		}
		
		Texture2D scaledTexture = new Texture2D(size, size);
		Graphics.ConvertTexture(showOriginTexture, scaledTexture);
		//DestroyImmediate(showOriginTexture);
		
		//Set texture for compute shader
		kernel_Init = shader.FindKernel("Kernel_Init");
		kernelCount++;
		kernel_Diffusion = shader.FindKernel("Kernel_Diffusion");
		kernelCount++;
		kernel_UserInput = shader.FindKernel("Kernel_UserInput");
		kernelCount++;
		kernel_Divergence = shader.FindKernel("Kernel_Divergence");
		kernelCount++;
		kernel_Jacobi = shader.FindKernel("Kernel_Jacobi");
		kernelCount++;
		kernel_Advection = shader.FindKernel("Kernel_Advection");
		kernelCount++;
		kernel_SubtractGradient = shader.FindKernel("Kernel_SubtractGradient");
		kernelCount++;
		for (int kernel = 0; kernel < kernelCount; kernel++)
		{
			/*
			This example is not optimized, not all kernels read/write into all textures,
			but I keep it like this for the sake of convenience
			*/
			shader.SetTexture(kernel, "VelocityTex", velocityTex);
			shader.SetTexture(kernel, "DensityTex", densityTex);
			shader.SetTexture(kernel, "PressureTex", pressureTex);
			shader.SetTexture(kernel, "DivergenceTex", divergenceTex);
			shader.SetTexture(kernel, "ObstacleTex", obstacleTex);
			
			//my add
			shader.SetTexture(kernel, "inputTex", scaledTexture);
		}

		//Init data texture value
		dispatchSize = Mathf.CeilToInt(size / 16);
		DispatchCompute(kernel_Init);

		roomSize = this.transform.parent.GetComponent<YRouge_RoomBase>().RoomSpaceKeep;
		// Debug.Log("roomSIZE1!!!!!" + roomSize.length + "  " + roomSize.width);
		// GameObject objTest1 = new GameObject("objTest1");
		// objTest1.transform.position = new Vector3(roomSize.bottomLeft.x, 0, roomSize.bottomLeft.y) + YRogueDungeonManager.Instance.RogueDungeonOriginPos;
		// GameObject objTest2 = new GameObject("objTest2");
		// objTest2.transform.position = new Vector3(roomSize.topRight.x, 0, roomSize.topRight.y) + YRogueDungeonManager.Instance.RogueDungeonOriginPos;
	}

	private roomSpaceKeep roomSize;
	private Vector2 GetPlayerPos()
	{
		Vector3 playerPos = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.position;
		Vector2 tmppos = new Vector2((playerPos.x - roomSize.bottomLeft.x - YRogueDungeonManager.Instance.RogueDungeonOriginPos.x) / (roomSize.width), (playerPos.z - roomSize.bottomLeft.y - YRogueDungeonManager.Instance.RogueDungeonOriginPos.z) / (roomSize.length));
		//把npos归一化到-0.5到0.5之间
		Vector2 npos = new Vector2(tmppos.x - 0.5f, tmppos.y - 0.5f);
		//Debug.Log("PlayerPos" + npos);
		return npos;
	}


	private bool isInteractive = false;
	public void SetCheckInteractive(bool isInteractive)
	{
		this.isInteractive = isInteractive;
	}
	
	public static Color color;
	[Range(1f,20f)] public float timeSpeed = 10f;
	void FixedUpdate()
	{
		if (!isInteractive) return;
		
		Vector2 npos = GetPlayerPos();
		//Vector2 npos = new Vector2( sphere.position.x / transform.localScale.x, sphere.position.z / transform.localScale.z );
		//Debug.Log(npos);
		shader.SetVector("spherePos",npos);

		//Send sphere (mouse) velocity
		Vector2 velocity = (npos - sphere_prevPos);
		shader.SetVector("sphereVelocity",velocity);
		shader.SetFloat("_deltaTime", Time.fixedDeltaTime);
		shader.SetVector("dyeColor",color);
		Debug.Log(SetSphereColor.color);
		//Run compute shader
		DispatchCompute (kernel_Diffusion);
		DispatchCompute (kernel_Advection);
		DispatchCompute (kernel_UserInput);
		DispatchCompute (kernel_Divergence);
		for(int i=0; i<solverIterations; i++)
		{
			DispatchCompute (kernel_Jacobi);
		}
		DispatchCompute (kernel_SubtractGradient);
		
		//Save the previous position for velocity
		sphere_prevPos = npos;
		
		color = Color.HSVToRGB( 0.5f*(Mathf.Sin( Time.time * Time.fixedDeltaTime * timeSpeed )+1f) , 1f, 1f);
		
	}
}
