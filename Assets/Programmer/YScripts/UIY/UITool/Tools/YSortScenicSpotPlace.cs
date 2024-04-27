using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YSortScenicSpotPlace : MonoBehaviour
{
    List<YScenicSpotPlace> scenicSpotPlaces = new List<YScenicSpotPlace>();
    //Start
    void Start()
    {
        SortScenicSpotPlace();
    }
    public void SortScenicSpotPlace()
    {
        scenicSpotPlaces.Clear();
        foreach (Transform child in transform)
        {
            YScenicSpotPlace scenicSpotPlace = child.GetComponent<YScenicSpotPlace>();
            if (scenicSpotPlace != null)
            {
                scenicSpotPlaces.Add(scenicSpotPlace);
                // scenicSpotPlace.gameObject.SetActive(false);
            }
        }
        //按照placeIndex从小到大排序
        scenicSpotPlaces.Sort((a, b) => a.placeIndex.CompareTo(b.placeIndex));
        // for (int i = 0; i < scenicSpotPlaces.Count; i++)
        // {
        //     scenicSpotPlaces[i].transform.SetSiblingIndex(i);
        // }
    }
    //通过curlevel 获取当前的景点，然后显示，并将imageCharacter的位置移动到当前景点的位置
    public void ShowScenicSpotPlace(int curLevel,Transform imageCharacter,float loadFakeTime)
    {
        SortScenicSpotPlace();
        if (curLevel+1 >= scenicSpotPlaces.Count)
        {
            return;
        }
        //scenicSpotPlaces[curLevel].gameObject.SetActive(true);
        scenicSpotPlaces[curLevel].gameObject.transform.DORotate(new Vector3(0,0,1), loadFakeTime);
        imageCharacter.position = scenicSpotPlaces[curLevel].transform.position;
        imageCharacter.DOMove(scenicSpotPlaces[curLevel+1].transform.position, loadFakeTime*0.8f);
        imageCharacter.DORotate(scenicSpotPlaces[curLevel+1].transform.eulerAngles, loadFakeTime);
        //先缩小为原来的0.8倍
        imageCharacter.DOScale(imageCharacter.localScale*0.8f, loadFakeTime*0.8f);
        //再放大为原来的1.2倍
        imageCharacter.DOScale(imageCharacter.localScale*1.2f, loadFakeTime*0.8f).SetDelay(loadFakeTime*0.8f);
        //再缩小为原来的1倍
        imageCharacter.DOScale(imageCharacter.localScale, loadFakeTime*0.8f).SetDelay(loadFakeTime*0.9f);

        // Transform cube = GameObject.Find("Cube").transform;
        //
        // DOTween.To(() => cube.position, x => cube.position = x, new Vector3(10, 10, 10), 1.5f);
        
        // imageCharacter.transform.position = scenicSpotPlaces[curLevel].transform.position;
    }
    
    public string GetScenicSpotPlaceName(int curLevel)
    {
        return  scenicSpotPlaces[curLevel+1].PlaceName;
    }
}
