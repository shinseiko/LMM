using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using Harmony;
using UnityEngine;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace LobotomyBaseModLib
{
    public static class ExpressionEx
    {
        public static MethodCallExpression Assign(Expression left, Expression right)
        {
            // `Assign` 메서드를 직접 호출하는 Expression.Call 사용
            var assignMethod = typeof(Assigner<>).MakeGenericType(left.Type).GetMethod("Assign");
            return Expression.Call(assignMethod, left, right);
        }

        private static class Assigner<T>
        {
            public static T Assign(ref T left, T right)
            {
                left = right; // 값 할당
                return left;  // 할당된 값을 반환
            }
        }
    }
    [Serializable]
    public class SerializeDictionary<Tkey, TValue>
    {
        public SerializeDictionary()
        {
            dic = new List<SKeyValuePair<Tkey, TValue>>();
        }
        public bool ContainsKey(Tkey key)
        {
            var value = dic.Find(x => x.key.Equals(key));
            return value != null;
        }
        public Dictionary<Tkey, TValue> ToDic()
        {
            Dictionary<Tkey, TValue> result = new Dictionary<Tkey, TValue>();
            foreach (var pair in dic)
            {
                result[pair.key] = pair.value;
            }
            return result;
        }
        public void ByDic(Dictionary<Tkey, TValue> bdic)
        {
            dic.Clear();
            foreach (var pair in bdic)
            {
                dic.Add(new SKeyValuePair<Tkey, TValue>(pair.Key, pair.Value));
            }
        }
        public TValue this[Tkey key]
        {
            get
            {
                var pair = dic.FindAll(x => x.key.Equals(key));
                if (pair.Count == 0) return default(TValue);
                return pair[0].value;
            }
            set
            {
                var pair = dic.FindAll(x => x.key.Equals(key));
                if (pair.Count > 0)
                {
                    pair[0].value = value;
                    return;
                }
                dic.Add(new SKeyValuePair<Tkey, TValue>(key, value));
            }
        }
        public List<SKeyValuePair<Tkey, TValue>> dic;
    }
    [Serializable]
    public class SKeyValuePair<Tkey, TValue>
    {
        public SKeyValuePair() { }
        public SKeyValuePair(Tkey k, TValue v)
        {
            key = k;
            value = v;
        }
        public Tkey key;

        public TValue value;
    }
    public class CacheDic<Tkey, TValue>
    {
        public bool ContainsKey(Tkey key)
        {
            if (dic.ContainsKey(key)) return true;
            TValue result = del(key);
            if (result != null)
            {
                dic[key] = result;
                return true;
            }
            return false;
        }
        public CacheDic(getdele del)
        {
            dic = new Dictionary<Tkey, TValue>();
            this.del = del;
        }
        public TValue this[Tkey key]
        {
            get
            {
                if (dic.ContainsKey(key))
                {
                    return dic[key];
                }
                TValue v = del(key);
                if (v != null)
                {
                    dic[key] = v;
                }
                return v;
            }
            set
            {
                dic[key] = value;
            }
        }
        public void PreLoading(Tkey key)
        {
            dic[key] = del(key);
        }
        public getdele del;
        public delegate TValue getdele(Tkey key);
        public Dictionary<Tkey, TValue> dic;
    }
    public static class ExtenionUtil_Lib
    {
        
        public static T JsonDeSerialize<T>(string json)
        {
            return (T)JsonConvert.DeserializeObject(json);
        }
        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static T ForceTypeChange<T>(this object obj)
        {
            return (T)obj;
        }
        public static object Invoke(this object obj, string methodname, object[] parameter)
        {
            return obj.GetType().GetMethod(methodname, AccessTools.all | BindingFlags.FlattenHierarchy).Invoke(obj, parameter);
        }
        public static T GetFieldValue<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetField(name, AccessTools.all | BindingFlags.FlattenHierarchy).GetValue(obj);
        }
        public static Func<T, R> GetFieldValueGetter<T, R>(this T obj, string name)
        {
            return CreateFieldGetter<T, R>(obj.GetType().GetField(name, AccessTools.all | BindingFlags.FlattenHierarchy));
        }
        public static Dictionary<Type, Dictionary<string, object>> GetFieldCache = new Dictionary<Type, Dictionary<string, object>>();
        public static Func<T, R> CreateFieldGetter<T, R>(FieldInfo fieldInfo)
        {
            var instance = Expression.Parameter(typeof(T), "instance");
            var field = Expression.Field(instance, fieldInfo);
            var lambda = Expression.Lambda<Func<T, R>>(field, instance);
            return lambda.Compile();
        }

        public static Action<T, R> CreateFieldSetter<T, R>(FieldInfo fieldInfo)
        {
            // 인스턴스 및 값 매개변수 생성
            var instance = Expression.Parameter(typeof(T), "instance");
            var value = Expression.Parameter(typeof(R), "value");

            // 필드 접근 표현식 생성
            var fieldAccess = Expression.Field(instance, fieldInfo);

            // Assignment 표현식 생성
            var assign = ExpressionEx.Assign(fieldAccess, value);

            // Setter Lambda 생성
            var lambda = Expression.Lambda<Action<T, R>>(assign, instance, value);
            return lambda.Compile();
        }
        public static void SetFieldValue(this object obj, string name, object value)
        {
            obj.GetType().GetField(name, AccessTools.all | BindingFlags.FlattenHierarchy).SetValue(obj, value);
        }
        public static Action<T, R> SetFieldValueSetter<T, R>(this T obj, string name)
        {
            return CreateFieldSetter<T, R>(obj.GetType().GetField(name, AccessTools.all | BindingFlags.FlattenHierarchy));
        }
        public static Dictionary<Type, Dictionary<string, object>> SetFieldCache = new Dictionary<Type, Dictionary<string, object>>();
        public static DirectoryInfo CheckNamedDir(this DirectoryInfo dir, string name)
        {
            DirectoryInfo[] directories = dir.GetDirectories();
            foreach (DirectoryInfo directoryInfo in directories)
            {
                if (directoryInfo.Name == name)
                {
                    return directoryInfo;
                }
            }

            return null;
        }
        public static void LocalEachScalingAll(this GameObject obj, float x, float y, float z = 0)
        {
            if (obj.transform.childCount > 0)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    LocalEachScalingAll(obj.transform.GetChild(i).gameObject, x, y, z);
                }
            }
            Vector3 scale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(scale.x * x, scale.y * y, scale.z * z);
        }
        public static void LocalScalingAll(this GameObject obj, float x, float y, float z = 0)
        {
            if (obj.transform.childCount > 0)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    LocalScalingAll(obj.transform.GetChild(i).gameObject, x, y, z);
                }
            }
            obj.transform.localScale = new Vector3(x, y, z);
        }
    }
    public class Singleton<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                if (Singleton<T>._instance == null)
                {
                    Singleton<T>._instance = Activator.CreateInstance<T>();
                }
                return Singleton<T>._instance;
            }
        }

        protected Singleton()
        {
        }

        private static T _instance;
    }
}
