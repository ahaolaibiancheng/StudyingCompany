using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PackageController : MonoBehaviour
{
    public static PackageController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public string FindUidByTableId(int tableId)
    {
        List<PackageLocalItem> items = PackageLocalData.Instance.LoadPackage();
        PackageLocalItem item = items.Find(i => i.id == tableId);
        return item?.uid; // 返回uid或null
    }
}