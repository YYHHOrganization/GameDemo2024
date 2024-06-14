using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRogueDamageCalculator : MonoBehaviour
{
    // 单例模式
    private static HRogueDamageCalculator instance;
    public static HRogueDamageCalculator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HRogueDamageCalculator>();
            }
            return instance;
        }
    }
    
    // 环境中目前的元素
    private ElementType currentEnvironmentElement;
    public void SetElementTypeInEnvironment(ElementType element)
    {
        currentEnvironmentElement = element;
    }

    private ElementType UpdateDestElementTypeWithEnvironment(ElementType destElement)
    {
        if (currentEnvironmentElement == ElementType.Hydro) //当前下雨了，敌人挂水元素
        {
            return ElementType.Hydro;
        }

        return destElement; //返回默认的元素
    }
    
    // 伤害计算, 考虑到元素反应，但是比较简单，只考虑了基础伤害，不考虑持续伤害这种问题
    public int CalculateBaseDamage(int sourceDamage, ElementType sourceElement, ElementType destElement, out ElementReaction reaction)
    {
        int finalDamage = sourceDamage;
        reaction = ElementReaction.None;
        destElement = UpdateDestElementTypeWithEnvironment(destElement); //比如下雨的话，敌方会被挂上水元素
        
        if (sourceElement == ElementType.Pyro)  //发射的是火元素
        {
            finalDamage = PyroRelatedDamage(true, sourceDamage, destElement, out reaction);
        }
        else if (sourceElement == ElementType.Electro) //发射的是雷元素
        {
            finalDamage = ElectroRelatedDamage(true, sourceDamage, destElement, out reaction);
        }

        return finalDamage;
    }

    //与雷元素相关的反应
    private int ElectroRelatedDamage(bool isSource, int sourceDamage, ElementType destElement,
        out ElementReaction reaction)
    {
        int finalDamage = sourceDamage;
        reaction = ElementReaction.None;
        if (isSource)
        {
            switch (destElement)
            {
                case ElementType.None: //敌方目标是无属性的
                    finalDamage = sourceDamage;
                    break;
                case ElementType.Hydro:
                    //感电反应打出1.5倍伤害
                    finalDamage = sourceDamage;
                    reaction = ElementReaction.ElectroCharged; //打出感电反应
                    break;
            }
        }

        return finalDamage;
    }

    private int PyroRelatedDamage(bool isSource, int sourceDamage, ElementType destElement, out ElementReaction reaction)
    {
        int finalDamage = sourceDamage;
        reaction = ElementReaction.None;
        if (isSource)
        {
            switch (destElement)
            {
                case ElementType.None: //敌方目标是无属性的
                    finalDamage = sourceDamage;
                    break;
                case ElementType.Hydro:
                    //蒸发反应打出1.5倍伤害
                    finalDamage = (int)(sourceDamage * 1.5f);
                    reaction = ElementReaction.Vaporize; //打出蒸发反应
                    break;
            }
        }
        
        return finalDamage;
    }
    
}
