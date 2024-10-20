using System;
using System.Collections;
using System.Collections.Generic;

namespace OurGameFramework
{
    public static class Pool
    {
        public readonly static List<PoolBase> AllPool = new List<PoolBase>();

        public static void ReleaseAll()
        {
            foreach (var pool in AllPool)
            {
                pool.Dispose();
            }
            AllPool.Clear();
        }
    }

    public interface IObject
    {
        void OnRelease();
    }

    public interface PoolBase
    {
        void Dispose();
    }

    public class ObjectPool<T> : PoolBase where T : new()  //where T : new(): 这是一个约束，指明类型参数 T 必须具有一个公共无参构造函数。这意味着在 ObjectPool 中，你可以使用 new T() 来创建 T 的实例。
    {
        private static ObjectPool<T> Instance;

        private Stack<T> _pool;

        private ObjectPool() { }


        private static void Init()
        {
            if (Instance == null)
            {
                Instance = new ObjectPool<T>();
                Instance._pool = new Stack<T>();
                Pool.AllPool.Add(Instance);
            }
        }

        public static T Get()
        {
            Init();

            if (Instance._pool.Count > 0)
            {
                return Instance._pool.Pop();
            }
            else
            {
                return new T();
            }
        }

        public static void Release(T obj)
        {
            if (obj == null || Instance == null) return;

            if (obj is IObject interfac)
            {
                interfac.OnRelease();
            }
            Instance._pool.Push(obj);
        }

        public void Dispose()
        {
            if (Instance != null)
            {
                if (Instance._pool != null)
                {
                    Instance._pool.Clear();
                    Instance._pool = null;
                }
                Instance = null;
            }
        }
    }

    public class ListPool<T> : PoolBase
    {
        private static ListPool<T> Instance;

        private Stack<List<T>> _pool;

        private ListPool() { }

        private static void Init()
        {
            if (Instance == null)
            {
                Instance = new ListPool<T>();
                Instance._pool = new Stack<List<T>>();
                Pool.AllPool.Add(Instance);
            }
        }

        public static List<T> Get()
        {
            Init();

            if (Instance._pool.Count > 0)
            {
                return Instance._pool.Pop();
            }
            else
            {
                return new List<T>();
            }
        }

        public static void Release(List<T> list)
        {
            if (list == null || Instance == null) return;
            list.Clear();
            Instance._pool.Push(list);
        }

        public void Dispose()
        {
            if (Instance != null)
            {
                if (Instance._pool != null)
                {
                    Instance._pool.Clear();
                    Instance._pool = null;
                }
                Instance = null;
            }
        }
    }

    public class DictionaryPool<Key, Value> : PoolBase
    {
        private static DictionaryPool<Key, Value> Instance;

        private Stack<Dictionary<Key, Value>> _pool;

        private DictionaryPool() { }

        private static void Init()
        {
            if (Instance == null)
            {
                Instance = new DictionaryPool<Key, Value>();
                Instance._pool = new Stack<Dictionary<Key, Value>>();
                Pool.AllPool.Add(Instance);
            }
        }

        public static Dictionary<Key, Value> Get()
        {
            Init();

            if (Instance._pool.Count > 0)
            {
                return Instance._pool.Pop();
            }
            else
            {
                return new Dictionary<Key, Value>();
            }
        }

        public static void Release(Dictionary<Key, Value> dict)
        {
            if (dict == null || Instance == null) return;
            dict.Clear();
            Instance._pool.Push(dict);
        }

        public void Dispose()
        {
            if (Instance != null)
            {
                if (Instance._pool != null)
                {
                    Instance._pool.Clear();
                    Instance._pool = null;
                }
                Instance = null;
            }
        }
    }
}
