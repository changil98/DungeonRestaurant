using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

/* 
 * Description
 * ������ ����� �˻� �ð� ���⵵ O(1)���� Dictionary�� ����
 * json->List<T> �Ǵ� Resources.ReadAll()->T[] �� ������ �Ű������� �޾� �ʱ�ȭ
 * ���������� ������ �����Ͷ� ���� ���� �� Ŭ������ ó�� �����Ͽ� ����, ���� �� �ʿ��� ������ ����Ʈ�� ��ȯ
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
                Debug.LogError($"key:{keySelector(data)} �� �̹� Dictionary�� ����");
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
                Debug.LogError($"key:{keySelector(data)} �� �̹� Dictionary�� ����");
            }
        }
    }

    public TValue GetData(TKey key)
    {
        if (dataDict.ContainsKey(key))
        {
            return dataDict[key];
        }

        Debug.LogError($"key:{key} �� �ش��ϴ� �����Ͱ� �����ϴ�.");
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
        // ���ο� �����͸� �ӽ÷� ������ Dictionary
        Dictionary<TKey, TValue> newDataList = new Dictionary<TKey, TValue>();

        // ���ο� ����Ʈ�� �����͸� �ӽ� Dictionary�� �߰�
        foreach (var data in list)
        {
            var key = keySelector(data);
            if (!newDataList.ContainsKey(key))
            {
                newDataList.Add(key, data);
            }
            else
            {
                Debug.LogError($"key:{key} �� �̹� �ӽ� Dictionary�� ����");
            }
        }

        // ���� dataList�� ���Ͽ� ������Ʈ
        foreach (var key in newDataList.Keys)
        {
            if (dataDict.ContainsKey(key))
            {
                // ���� �����Ͱ� ����� ��� ������Ʈ
                if (!EqualityComparer<TValue>.Default.Equals(dataDict[key], newDataList[key]))
                {
                    dataDict[key] = newDataList[key];
                }
            }
            else
            {
                // ���ο� ������ �߰�
                dataDict.Add(key, newDataList[key]);
            }
        }

        // ���� dataList���� ������ ������ ����
        var keysToRemove = dataDict.Keys.Except(newDataList.Keys).ToList();
        foreach (var key in keysToRemove)
        {
            dataDict.Remove(key);
        }
    }
}
