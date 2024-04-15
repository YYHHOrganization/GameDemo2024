using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YShopPlatformRogue : MonoBehaviour
{
    public TMP_Text[] PriceText;

    public void SetPriceStr(string priceStr)
    {
        for (int i = 0; i < PriceText.Length; i++)
        {
            PriceText[i].text = priceStr;
        }
    }
}
