﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnibusEvent
{
    public delegate void OnEvent<T>(T action);

    public delegate void OnEventWrapper(object _object);

    class DictionaryKey
    {
        public Type Type;
        public object Tag;

        public DictionaryKey(object tag, Type type)
        {
            this.Tag = tag;
            this.Type = type;
        }

        public override int GetHashCode()
        {
            return this.Tag.GetHashCode() ^ this.Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DictionaryKey)
            {
                var key = (DictionaryKey)obj;
                return this.Tag.Equals(key.Tag) && this.Type.Equals(key.Type);
            }

            return false;
        }
    }

    public class UnibusObject : SingletonMonoBehaviour<UnibusObject>
    {
        public const string DefaultTag = "default";

        private Dictionary<DictionaryKey, Dictionary<int, OnEventWrapper>> observerDictionary =
            new Dictionary<DictionaryKey, Dictionary<int, OnEventWrapper>>();

        public void Subscribe<T>(OnEvent<T> eventCallback)
        {
            this.Subscribe(DefaultTag, eventCallback);
        }

        public void Subscribe<T>(object tag, OnEvent<T> eventCallback)
        {
            var key = new DictionaryKey(tag, typeof(T));

            if (!observerDictionary.ContainsKey(key))
            {
                observerDictionary[key] = new Dictionary<int, OnEventWrapper>();
            }

            observerDictionary[key][eventCallback.GetHashCode()] = (object _object) =>
            {
                var obj = _object;
                eventCallback((T)obj);
            };
        }

        public void Unsubscribe<T>(OnEvent<T> eventCallback)
        {
            this.Unsubscribe(DefaultTag, eventCallback);
        }

        public void Unsubscribe<T>(object tag, OnEvent<T> eventCallback)
        {
            var key = new DictionaryKey(tag, typeof(T));

            if (observerDictionary[key] != null)
            {
                observerDictionary[key].Remove(eventCallback.GetHashCode());
            }
        }

        public void Dispatch<T>(T action)
        {
            this.Dispatch(DefaultTag, action);
        }


        private List<List<OnEventWrapper>> mTempPooledList = new List<List<OnEventWrapper>>();
        public void Dispatch<T>(object tag, T action)
        {
            var key = new DictionaryKey(tag, typeof(T));
            if (observerDictionary.ContainsKey(key))
            {
                List<OnEventWrapper> mTempList = null;
                foreach (var lst in mTempPooledList)
                {
                    if (lst.Count == 0)
                    {
                        mTempList = lst;
                        break;
                    }
                }
                if (mTempList == null)
                {
                    mTempList = new List<OnEventWrapper>();
                    mTempPooledList.Add(mTempList);
                    // Debug.Log($"pooled list size: {mTempPooledList.Count}");
                }
                foreach (var c in observerDictionary[key].Values)
                {
                    mTempList.Add(c);
                }
                foreach (var caller in mTempList)
                {
                    caller(action);
                }
                mTempList.Clear();
            }
            else
            {
                string tagAndAction = string.Format("(tag:{0}, action:{1})", tag, action);
                Debug.LogWarning("Unibus.Dispatch failed to send: " + tagAndAction);
            }
        }
    }
}
