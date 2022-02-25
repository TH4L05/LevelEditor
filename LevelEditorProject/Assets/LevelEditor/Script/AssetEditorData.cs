/// <author> Thomas Krahl </author>
/// <version>1.00</version>
/// <date>24/02/2022</date>

using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AssetData
{
    public PartList assetList;
    public string path;
}

public class AssetEditorData : ScriptableObject
{
    public List<AssetData> createdPartsLists = new List<AssetData>();
    public float LastUsedListIndex {get; set;}


    public void AddPartList(AssetData partsList)
    {
        createdPartsLists.Add(partsList);
    }

    public void RemovePartList(AssetData partList)
    {
        createdPartsLists.Remove(partList);
    }

}
