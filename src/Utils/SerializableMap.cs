using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class SerializableMap<T, M>
    {
        [SerializeField] private List<T> keys;

        [SerializeField] private List<M> values;

        public SerializableMap()
        {
            keys = new List<T>();
            values = new List<M>();
        }

        public SerializableMap<T, M> Clone()
        {
            SerializableMap<T, M> newMap = new SerializableMap<T, M>();
            foreach (T key in keys)
            {
                newMap.Add(key, Get(key));
            }

            return newMap;
        }

        public T[] GetKeys()
        {
            return keys.ToArray();
        }

        public bool IsEmpty()
        {
            return !keys.Any();
        }

        public void Add(T key, M value)
        {
            if (keys.Contains(key))
            {
                values[keys.IndexOf(key)] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
            }
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public bool Contains(T key)
        {
            return keys.Contains(key);
        }

        public void Remove(T key)
        {
            if (keys.Contains(key))
            {
                values.RemoveAt(keys.IndexOf(key));
                keys.Remove(key);
            }
        }

        public M Get(T key)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    return values[i];
                }
            }

            return default;
        }
        
        public M this[T key]
        {
            get => Get(key);
            set => Add(key, value);
        }
    }
}