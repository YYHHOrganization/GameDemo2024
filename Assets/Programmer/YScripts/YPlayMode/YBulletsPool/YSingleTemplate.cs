using System;
using UnityEngine;
using System.Collections;
using System.Text;
//单例模板
public abstract class YSingleTemplate<T> : MonoBehaviour
    where T:MonoBehaviour//T泛型继承单例模板的类最终继承哪，约定所有单利类必须是Unity的组件
{
    private static T _instance=null;//声明一个私有字段
    public static T _Instance//提供只读属性
    {
        get { return _instance; }
    }
    //类通过Awake（）实例化单例对象，同时可以让子类重写Awake()
    protected virtual void  Awake()
    {
        _instance = this as T;
    }
 
}
