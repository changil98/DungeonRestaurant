using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

/* 
 * Description
 * 데이터 목록을 검색 시간 복잡도 O(1)위해 Dictionary로 관리
 * json->List<T> 또는 Resources.ReadAll()->T[] 를 생성자 매개변수로 받아 초기화
 * 순차적으로 관리할 데이터라도 매핑 위해 이 클래스로 처음 생성하여 관리, 매핑 후 필요한 데이터 리스트로 변환
 */

public class DataDictionary<TKey, TValue>
{
    [ShowInInspector] protected Dictionary<TKey, TValue> dataDict = new Dictionary<TKey, TValue>();
    private Func<TValue, TKey> keySelector;

    // Initialize list from json
    public DataDictionary(DataSheetController controller, Func<TValue, TKey> keySelector)
    {
        this.keySelector = keySelector;
        List<TValue> list = controller.Deserialize<TValue>();
        InitializeDataDict(list, keySelector);
    }

    // Initialize list from Resources.LoadAll
    public DataDictionary(TValue[] list, Func<TValue, TKey> keySelector)
    {
        this.keySelector = keySelector;
        InitializeDataDict(list, keySelector);
    }

    // Initialize list from json
    public void InitializeDataDict(List<TValue> list, Func<TValue, TKey> keySelector)
    {
        foreach (var data in list)
        {
            try
            {
                dataDict.Add(keySelector(data), data);
            }
            catch
            {
                Debug.LogError($"key:{keySelector(data)} 는 이미 Dictionary에 있음");
            }
        }
    }

    // Initialize list from Resources.LoadAll
    public void InitializeDataDict(TValue[] list, Func<TValue, TKey> keySelector)
    {
        foreach (var data in list)
        {
            try
            {
                dataDict.Add(keySelector(data), data);
            }
            catch
            {
                Debug.LogError($"key:{keySelector(data)} 는 이미 Dictionary에 있음");
            }
        }
    }

    public TValue GetData(TKey key)
    {
        if (dataDict.ContainsKey(key))
        {
            return dataDict[key];
        }

        Debug.LogError($"key:{key} 에 해당하는 데이터가 없습니다.");
        return default(TValue);
    }

    public List<TValue> DataDictToList()
    {
        List<TValue> list = dataDict.Values.ToList();
        return list;
    }

    public void UpdateDataFromSheet() // For Debug
    {
        List<TValue> list = DataManager.Instance.DataSheetController.DeserializeData<TValue>();
        // 새로운 데이터를 임시로 저장할 Dictionary
        Dictionary<TKey, TValue> newDataList = new Dictionary<TKey, TValue>();

        // 새로운 리스트의 데이터를 임시 Dictionary에 추가
        foreach (var data in list)
        {
            var key = keySelector(data);
            if (!newDataList.ContainsKey(key))
            {
                newDataList.Add(key, data);
            }
            else
            {
                Debug.LogError($"key:{key} 는 이미 임시 Dictionary에 있음");
            }
        }

        // 기존 dataList와 비교하여 업데이트
        foreach (var key in newDataList.Keys)
        {
            if (dataDict.ContainsKey(key))
            {
                // 기존 데이터가 변경된 경우 업데이트
                if (!EqualityComparer<TValue>.Default.Equals(dataDict[key], newDataList[key]))
                {
                    dataDict[key] = newDataList[key];
                }
            }
            else
            {
                // 새로운 데이터 추가
                dataDict.Add(key, newDataList[key]);
            }
        }

        // 기존 dataList에서 삭제된 데이터 제거
        var keysToRemove = dataDict.Keys.Except(newDataList.Keys).ToList();
        foreach (var key in keysToRemove)
        {
            dataDict.Remove(key);
        }
    }
}
