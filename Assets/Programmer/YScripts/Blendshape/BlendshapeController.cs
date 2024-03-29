using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlendshapeController 
{
    //int[] BlinkIndex = {24,0,};//qiyu xina (应该当道csv里面，yplanning那个去读，新的csv里面有表情的名字，然后根据名字去找index)
    private List<List<List<int>>> bsIndex;//= new int[3, 2]{{24,0},{13,15},{5,0}};//每个表情的index，{不同角色}  //todo：目前只有24是有效的  其他都是随便写的
    public BlendshapeController()
    {
        bsIndex = yPlanningTable.Instance.blendshapeIndexs;
    }

    public List<int> GetIndexes(int characterId, int selectId)
    {
        return bsIndex[selectId][characterId];
    }
    
    SkinnedMeshRenderer skinnedMeshRenderers;
    public void SetBlendshape(int characterId,SkinnedMeshRenderer skinnedMeshRenderer,int selectId,bool isOn)
    {
        skinnedMeshRenderers = skinnedMeshRenderer;
        SetBlendShapeValueNoSearch(bsIndex[selectId][characterId], isOn?100:0);
    }
    public void SetBlendShapeValueNoSearch(List<int> blendshapeInd ,float value)
    {
        //skinnedMeshRenderers.SetBlendShapeWeight(blendshapeInd, value);
        for (int i = 0; i < blendshapeInd.Count; i++)
        {
            if(blendshapeInd[i]<0 || blendshapeInd[i]>=skinnedMeshRenderers.sharedMesh.blendShapeCount) return;
            skinnedMeshRenderers.SetBlendShapeWeight(blendshapeInd[i], value);
        }
    }
    
    public void ResetBlendshape(SkinnedMeshRenderer currentSkinnedMeshRenderer,int characterId)
    {
        List<int> blendshapeWithCharacter = yPlanningTable.Instance.characterExpressionIndices[characterId];
        //将他们全部置为0
        for (int i = 0; i < blendshapeWithCharacter.Count; i++)
        {
            if(blendshapeWithCharacter[i]<0 || blendshapeWithCharacter[i]>=currentSkinnedMeshRenderer.sharedMesh.blendShapeCount) return;
            currentSkinnedMeshRenderer.SetBlendShapeWeight(blendshapeWithCharacter[i], 0);
        }
    }
}
