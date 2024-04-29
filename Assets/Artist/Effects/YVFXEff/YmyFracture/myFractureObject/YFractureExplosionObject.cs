using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class YFractureExplosionObject : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject fractureObject;

    public GameObject mexplosionVFX;

    private GameObject fractureGO;

    public float minExplosionForce, maxExplosionForce;

    public float mExplosionRadius;

    private Transform vfxPlace;
    private Vector3 vfxV3;
    [Header("shrink")]
    public float delaytime = 2f;

    public float SrinkScaleRefresh = 10f;
    bool isBroken = false;

    GameObject collider;
    [SerializeField] private bool brokenAndGenerateItem = false;
    [SerializeField] private string itemId ;
    // Start is called before the first frame update
    void Start()
    {
        // vfxPlace.position = new Vector3(originalObject.transform.position.x, originalObject.transform.position.y,
        //     originalObject.transform.position.z);
        vfxV3 = new Vector3(originalObject.transform.position.x, originalObject.transform.position.y,
            originalObject.transform.position.z);
        print(originalObject.transform.position.x + " " + originalObject.transform.position.y + " " +
            originalObject.transform.position.z);
        //在当前位置找到一个名为Collider的子物体
        collider = transform.Find("Collider").gameObject;
    }

    // Update is called once per frame 
    void Update()
    {
        // if (isBroken==false)
        // {
        //     if (Input.GetKeyDown(KeyCode.Space))//当然这是可以改的
        //     {
        //         print("l");
        //         Explode();
        //         isBroken = true;
        //     }
        // }
        
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (isBroken == false)
    //     {
    //         if(other.tag=="triggerExplosion")
    //         {
    //             print("Onnn");
    //             Explode(other.bounds.ClosestPoint(transform.position));
    //             //传入碰撞点
    //             
    //             //InstantiateBroken();
    //             isBroken = true; //test
    //         }
    //     }
    //     //在这个位置炸了 往他来的方向
    // }
    
    public void TriggerExplosion(Vector3 hitVector3)
    {
        if (isBroken == false)
        {
            Explode(hitVector3);
            
            //有概率生成道具 比如心啥的
            if (brokenAndGenerateItem)
            {
                Broken2GenerateItem();
            }
            
            isBroken = true; //test
            Destroy(collider);
        }
    }

    [SerializeField] private int ProbabilityOfBroken2GenerateItem = 50;
    private void Broken2GenerateItem()
    {
        Class_RogueCommonItemCSVFile commonItemCsvFile  = SD_RogueCommonItemCSVFile.Class_Dic[itemId];
        GenerateRoomItemDaojuNew(commonItemCsvFile);
        // int randomKind = Random.Range(0, 100);
        // if (randomKind < ProbabilityOfBroken2GenerateItem)  
        // {
        //     //生成道具
        //     print("生成道具");
        //     string itemId = "80000011";
        //     HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(itemId, transform,
        //         new Vector3(0, 0, 0));
        // }
    }
    
    private void GenerateRoomItemDaojuNew(Class_RogueCommonItemCSVFile commonItemCsvFile)
    {
        int couldBroken = commonItemCsvFile._CouldBrokenAndGenerateItem();
        if (couldBroken == 0)
        {
            return;
        }
        
        string[] DropItemIDs = commonItemCsvFile.BrokenAndGenerateItemList.Split(':'); 
        string[] DropItemProbabilitys = commonItemCsvFile.GenerateItemProbabilityList.Split(':');
        for (int i = 0; i < DropItemIDs.Length; i++)
        {
            float probability = float.Parse(DropItemProbabilitys[i]);
            if (Random.Range(0, 1f) < probability)
            {
                string DropItemID = DropItemIDs[i];
                Vector3 position = new Vector3(0, 0.1f, 0);
                //80000000-80000073 如果是这种 表示从这个范围内随机一个
                if (DropItemID.Contains("-"))
                {
                    string[] range = DropItemID.Split('-');
                    int min = int.Parse(range[0]);
                    int max = int.Parse(range[1]);
                    int randomID = Random.Range(min, max);
                    DropItemID = randomID.ToString();
                    
                    GameObject lumine = HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(DropItemID, transform,position);
                    lumine.GetComponent<HRogueItemBase>().SetBillboardEffect();
                }
                else if (DropItemID == "all")
                {
                    GameObject lumine = HRoguePlayerAttributeAndItemManager.Instance.RollingARandomItem(transform, position);
                    lumine.GetComponent<HRogueItemBase>().SetBillboardEffect();
                }
                else
                {
                    GameObject lumine = HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(DropItemID, transform,position);
                    lumine.GetComponent<HRogueItemBase>().SetBillboardEffect();
                }
                //只让其生成一个最多
                return;
            }
        }
        
    }

    public void InstantiateBroken()
    {
        if (originalObject != null)
        {
            originalObject.SetActive(false);
        }
        if (fractureObject != null)
        {
            fractureGO = Instantiate(fractureObject, vfxV3, quaternion.identity) as GameObject;
            Destroy(fractureGO, 10f);
        }
        if (mexplosionVFX != null)
        {
            GameObject exploVFX = Instantiate(mexplosionVFX, vfxV3, Quaternion.identity) as GameObject;
            print("ll");
            Destroy(exploVFX, 10f);
        }
    }
    public void Explode(Vector3 hitVector3)
    {
        if (originalObject != null)
        {
            originalObject.SetActive(false);
        }

        if (fractureObject != null)
        {
            fractureGO = Instantiate(fractureObject, vfxV3, quaternion.identity) as GameObject;

            foreach (Transform tPiece in fractureGO.transform)
            {
                Rigidbody rb = tPiece.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    //让他们崩开
                    // rb.AddExplosionForce(Random.Range(minExplosionForce, maxExplosionForce), vfxV3, mExplosionRadius);
                    rb.AddExplosionForce(Random.Range(minExplosionForce, maxExplosionForce), hitVector3, mExplosionRadius);
                }
                //慢慢变小小时
                StartCoroutine(Shrink(tPiece));
            }
            Destroy(fractureGO, 10f);
        }

        if (mexplosionVFX != null)
        {
            // GameObject exploVFX = Instantiate(mexplosionVFX, vfxV3, Quaternion.identity) as GameObject;
            GameObject exploVFX = Instantiate(mexplosionVFX, hitVector3, Quaternion.identity) as GameObject;
            print("ll");
            Destroy(exploVFX, 10f);
        }
    }

    IEnumerator Shrink(Transform tpiece)
    {
        yield return new WaitForSeconds(delaytime);
        Vector3 newScale = tpiece.localScale;

        while (newScale.x >= 0)
        {
            newScale -= new Vector3(SrinkScaleRefresh, SrinkScaleRefresh, SrinkScaleRefresh);
            tpiece.localScale = newScale;
            yield return new WaitForSeconds(0.05f);
        }

        if (newScale.x - SrinkScaleRefresh < 0)
        {
            tpiece.localScale = new Vector3(0, 0, 0);
        }

        // yield return new WaitForSeconds(1f);
    }
}
