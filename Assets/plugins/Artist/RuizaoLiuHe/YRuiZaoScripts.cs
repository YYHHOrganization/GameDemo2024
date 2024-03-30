using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRuiZaoScripts : MonoBehaviour
{
    //TODO:根据策划表读取、shader、转面、2.0版本花色、成功后不能再动
    //TODO：根据isblock给块的材质赋值
    int mBtnIndex;
    public GameObject winPanel;
    public GameObject nextLevelBtn;
    bool canPushBtn=true;//在转的时候不能push

    public GameObject[] goArray;
    public GameObject[] mappingGoArray;
    public int[] xulie = { 0, 1, 2, 3, 4, 5, 6, 7 };

    public GameObject Mcamera;
    public GameObject target;

    private int[] guiLvList = { 2, 5, 3, 4 };
    
    [Header("材质球")]
    public Material blockMat;
    public Material blockMatHighlight;
    public Material noBlockMat;
    public Material noBlockMatHighlight;
    /// <summary>
    /// 关卡1
    /// </summary>
    //读策划表
    //public int[] isBlock = {1,0,0,0
    //        ,0,0,0,0};
    ////读表得到答案对照面，这里先写si
    //int[] anser1 = {0,0,0,1,
    //    0,0,1,0 };

    /// <summary>
    /// 关卡2
    /// </summary>
    int[,] isBlock =new int[3, 8]
        {
        {1,0,0,1
            ,1,1,0,0},
        {0,0,0,1
            ,1,0,1,1},
        {1,0,0,0
            ,1,1,0,1}

        };
    int levelIndex = 0;

    //读表得到答案对照面，这里先写si
    int[,] anserMappingIsBlock = new int[3, 8]
    {
        {0,0,1,1,
        0,0,1,1 },
        {0,0,1,1,
        0,0,1,1 },
        {0,1,0,1,
        1,1,1,1 }
    };


    int[,] buttonControlArray = new int[6, 4]
    {
        {3,2,6,7 },
        {0,1,5,4 },
        {0,4,7,3 },
        {1,5,6,2 },
        {0,3,2,1 },
        {5,4,7,6 }
    };


    //int[,] buttonControlArray = new int[6, 4]
    //{
    //    {3,2,6,7 },
    //    {0,1,5,4 },
    //    {1,0,3,2 },
    //    {5,4,7,6 },
    //    {1,5,6,2 },
    //    {4,7,3,0 }
    //};

    //左右旋转相机相关
    //每个按钮控制的片会一直在变，我们以最开始默认的时候按钮指向作为他的默认片号
    //每个按钮每轮指向的分别是
    int[,] btnRotateCameraPointToPian = new int[6, 4]
    {
        {1,1,1,1 }, //上下两片怎么转都不变
        {2,2,2,2},
        {3,5,4,6 }, //按钮3每轮指向的片号
        {4,6,3,5},//按钮4每轮指向的片号
        {5,4,6,3 },//按钮5每轮指向的片号
        {6,3,5,4 }//按钮6每轮指向的片号
    };
    //我们认为它 刚开始为0 顺时针转一下为1（4+0-1）%4 逆时针转一下为3（即（4+0-1）%4）
    int curRotateCameraIndex =0;



    // Start is called before the first frame update
    void Start()
    {
        mBtnIndex = -1;
        InitMat();
        winPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(canPushBtn)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                RotateBtnClick(0);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                RotateBtnClick(1);
            }

            if(Input.GetKeyDown(KeyCode.F))
            {
                //goArray[xulie[index]].transform.Rotate(axis, rotateSpeed, Space.World);
                goArray[0].transform.Rotate(new Vector3(1.0f,0.0f,0.0f), 10, Space.World);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                //goArray[xulie[index]].transform.Rotate(axis, rotateSpeed, Space.World);
                goArray[0].transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f), -10, Space.World);
            }
        }
    }

    void InitMat()
    {
        //更新主题魔方的材质
        for(int i=0;i<8;i++)
        {
            Material[] materials = goArray[i].GetComponentInChildren<MeshRenderer>().materials;
            
            if(isBlock[levelIndex,i]==0)
            {
                //改Rendering Mode  START
                //采用材质ToonLit_Outline
                materials[0] = noBlockMat;
            }
            else
            {
                materials[0] = blockMat;
            }
            goArray[i].GetComponentInChildren<Renderer>().materials = materials;
        }

        //更新对照版面的材质
        for(int i=0;i<8;i++)
        {
            Material[] materials = mappingGoArray[i].GetComponentInChildren<MeshRenderer>().materials;

            if (anserMappingIsBlock[levelIndex, i] == 0)
            {
                //改Rendering Mode  START
                materials[0] = noBlockMat;
            }
            else
            {
                materials[0] = blockMat;
            }
            mappingGoArray[i].GetComponentInChildren<Renderer>().materials = materials;
        }

    }
    public void BtnClickSetBtnIndex(int index)
    {
        mBtnIndex = index;
        SetHightLightMat(index);
    }

    void SetHightLightMat(int index)
    {
        int btnIndex = btnRotateCameraPointToPian[index, curRotateCameraIndex]-1;;
        //buttonControlArray[btnIndex,i]的i是0-3 这几个我们高亮 其他取消高亮
        int[] hlIndex = new int[4];
        for(int i=0;i<4;i++)
        {
            hlIndex[i] = buttonControlArray[btnIndex, i];
        }
        for(int i=0;i<8;i++)
        {
            if (System.Array.IndexOf(hlIndex, i) != -1)//意思是i在hlIndex里
            {
                HightLightMat(i);
            }
            else
            {
                CancelHightLightMat(i);
            }
        }
    }

    void HightLightMat(int index)
    {
        Material materials = goArray[xulie[index]].GetComponentInChildren<MeshRenderer>().material;
        if (isBlock[levelIndex, xulie[index]] == 0)
        {
            //改Rendering Mode  START
            //采用材质ToonLit_Outline
            materials = noBlockMatHighlight;
        }
        else
        {
            materials = blockMatHighlight;
        }
        goArray[xulie[index]].GetComponentInChildren<Renderer>().material = materials;
    }
    void CancelHightLightMat(int index)
    {
        Material materials = goArray[xulie[index]].GetComponentInChildren<MeshRenderer>().material;
        if (isBlock[levelIndex, xulie[index]] == 0)
        {
            //改Rendering Mode  START
            //采用材质ToonLit_Outline
            materials = noBlockMat;
        }
        else
        {
            materials = blockMat;
        }
        goArray[xulie[index]].GetComponentInChildren<Renderer>().material = materials;
    }
    
    void HightLightMat0Ind(int index)
    {
        Material materials=goArray[xulie[index]].GetComponentInChildren<MeshRenderer>().material;
        if(isBlock[levelIndex,xulie[index]] == 0)
        {
            //改Rendering Mode  START
            //采用材质ToonLit_Outline
            materials = noBlockMatHighlight;
        }
        else
        {
            materials = blockMatHighlight;
        }
        goArray[xulie[index]].GetComponentInChildren<Renderer>().material = materials;
    }
    void CancelHightLightMat0Ind(int index)
    {
        Material materials = goArray[xulie[index]].GetComponentInChildren<MeshRenderer>().material;
        if(isBlock[levelIndex,xulie[index]] == 0)
        {
            //改Rendering Mode  START
            //采用材质ToonLit_Outline
            materials = noBlockMat;
        }
        else
        {
            materials = blockMat;
        }
        goArray[xulie[index]].GetComponentInChildren<Renderer>().material = materials;
    }
    public void RotateBtnClick(int dir)
    {
        if(canPushBtn)
        {
            if (mBtnIndex == -1) return;
            Rotate4Pieces(mBtnIndex, dir);
        }
    }
    void Rotate4Pieces(int beforeBtnIndex,int dir)
    {
        //int afterRotateCameraIndex = btnRotateCameraPointToPian[curRotateCameraIndex, btnIndex];
        int btnIndex = btnRotateCameraPointToPian[beforeBtnIndex,curRotateCameraIndex] -1;

        Vector3 axis=new Vector3(0,0,0);
        if (btnIndex == 0 || btnIndex == 1) axis = new Vector3(0, 1, 0);
        if (btnIndex == 2 || btnIndex == 3) axis = new Vector3(1, 0, 0);
        if (btnIndex == 4 || btnIndex == 5) axis = new Vector3(0, 0, 1);

        //if (btnIndex == 0 || btnIndex == 1) axis = new Vector3(0, 1, 0);
        //if (btnIndex == 2 || btnIndex == 3) axis = new Vector3(0, 0, 1);
        //if (btnIndex == 4 || btnIndex == 5) axis = new Vector3(1, 0, 0);

        for (int i=0;i<4;i++)
        {
            PieceRotate(axis, buttonControlArray[btnIndex,i],dir);
        }

        //更新序列
        //UpdateRotateSequence(btnIndex, dir);//不能传这个！ 如果传btnIndex 下面就不用①了
        UpdateRotateSequence(beforeBtnIndex, dir);
    }

    //dir=0,顺时针，=1逆时针
    void UpdateRotateSequence(int beforeBtnIndex, int dir)
    {
        //①
        int btnIndex = btnRotateCameraPointToPian[ beforeBtnIndex, curRotateCameraIndex] -1;

        int[] tmpSequence = new int[4];

        for (int i = 0; i < 4; i++)
        {
            tmpSequence[i] = xulie[buttonControlArray[btnIndex, i]]; 
        }
        for(int i=0;i<4;i++)
        {
            int index = 0;
            if (dir == 0) index = tmpSequence[(i + 1) % 4];
            else if (dir == 1) index = tmpSequence[(i + 3) % 4];
            xulie[buttonControlArray[btnIndex, i]] = index;
        } 
    }

    void PieceRotate(Vector3 axis,int index,int dir)
    {
        //TODO:协程慢慢转
        StartCoroutine(RotateObject(axis,index,dir));
        //goArray[xulie[index]].transform.Rotate(axis,a);
        
        //TEST1111 尝试在这里更改高亮
        //HightLightMat0Ind(index);
    }
    IEnumerator RotateObject(Vector3 axis, int index, int dir)
    {
        canPushBtn = false;
        int finalAngle,tmpangle=0;
        int rotateSpeed = 1;
        if (dir == 0)
        {
            rotateSpeed = 1;//todo11
            finalAngle = 90;
        }
        else
        {
            finalAngle = -90;
            rotateSpeed = -1;
        }

        while(tmpangle!=finalAngle)
        {
            goArray[xulie[index]].transform.Rotate(axis,rotateSpeed,Space.World);
            tmpangle += rotateSpeed;
            yield return null;
        }
        canPushBtn = true;
        JudgeWin();
        
        //TEST1111 尝试在这里更改高亮
        //CancelHightLightMat0Ind(index);
    }

    
    
    //bool win = false;

    int[,] mappingArray =
    {
        {2,3 },
        {6,7 },
        {0,1 },
        {4,5 },
        {3,7 },
        {2,6 },
        {0,4 },
        {1,5 }
    };
    void DebugLogWin()
    {
        int[] newdebug = { 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < 8; i++)
        {
            newdebug[i]= isBlock[levelIndex, xulie[mappingArray[i, 0]]];

        }
        int b = 0;
        //debug
    }
    bool JudgeWin()
    {
        DebugLogWin();
        for(int i=0;i<8;i++)
        {
            if (anserMappingIsBlock[levelIndex,i]==0) //没有块
            {//mappingArray[i][0]是坑的序号 序列[坑号]=真实块号 isBlock[真实块号]=是否是块 0/1
                //int kenghao = mappingArray[i, 0];
                //int realBlockIndex = xulie[mappingArray[i, 0]];
                //int isblock = isBlock[xulie[mappingArray[i, 0]]];
                if (isBlock[levelIndex,xulie[mappingArray[i, 0]]] != 0 || isBlock[levelIndex, xulie[mappingArray[i, 1]]] != 0) //有块
                {
                    //win = false;
                    return false;
                }
            }
            if (anserMappingIsBlock[levelIndex, i] == 1)
            {
                if (isBlock[levelIndex, xulie[mappingArray[i, 0]]] != 1 && isBlock[levelIndex, xulie[mappingArray[i, 1]]] != 1) //没有块
                {
                    //win = false;
                    return false;
                }
            }
        }

        winPanel.SetActive(true);
        if(levelIndex==isBlock.GetLength(0)-1)  //isBlock.Length=24
        {
            nextLevelBtn.SetActive(false);
        }
        Debug.Log("win");
        return true;
    }

    public void BtnClickChangeLevel()
    {
        winPanel.SetActive(false);
        levelIndex = levelIndex + 1;
        InitMat();
    }
    public void BtnClickRotateCamera(int dir)
    {
        StartCoroutine(rotateCamera(dir));
        //0 left 1 right
        //if (dir==0)
        //{

        //}
        //else
        //    Mcamera.transform.RotateAround(target.transform.position, Vector3.up, -90);

        if (dir == 1) curRotateCameraIndex = (curRotateCameraIndex + 1) % 4;
        else curRotateCameraIndex = (4 + curRotateCameraIndex - 1) % 4;
        Debug.Log(curRotateCameraIndex);
    }
    IEnumerator rotateCamera(int dir)
    {
        int angle = 1,sumangle=0;
        if(dir==0)
        {
            angle = 1;
            while (sumangle < 90)
            {
                sumangle =sumangle + angle;
                Mcamera.transform.RotateAround(target.transform.position, Vector3.up, angle);
                yield return null;
            }
        }
        else
        {
            angle = -1;
            while (sumangle > -90)
            {
                sumangle = sumangle + angle;
                Mcamera.transform.RotateAround(target.transform.position, Vector3.up, angle);
                yield return null;
            }
        }


        yield return null;

        SetBtnIndexAfterRotate(dir);
    }

    void SetBtnIndexAfterRotate(int dir)
    {
        if(mBtnIndex==0||mBtnIndex==1)return;//上面那两片不用管
        Debug.Log("mBtnIndex-2: " + (mBtnIndex - 2));
        //guiLvList的下一个
        int nextBtnIdxi;
        for (int i = 0; i < 4; i++)
        {
            if (guiLvList[i] == (mBtnIndex))
            {
                if (dir == 1)
                {
                    nextBtnIdxi = (i + 1) % 4;
                    mBtnIndex = guiLvList[nextBtnIdxi];
                    Debug.Log("CCC:mBtnIndex-2: " + (mBtnIndex - 2));
                    return;
                }
                else
                {
                    nextBtnIdxi = (4 + i - 1) % 4;
                    mBtnIndex = guiLvList[nextBtnIdxi];
                    Debug.Log("CCC:mBtnIndex-2: " + (mBtnIndex - 2));
                    return;
                }
            }
        }
        
        
    }
}





