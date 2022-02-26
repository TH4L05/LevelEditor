/// <author> Thomas Krahl </author>

using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    [System.Serializable]
    public class AssetData
    {
        public PartList assetList;
        public string path;
    }

    public class AssetEditorData : ScriptableObject
    {
        public List<AssetData> createdPartsLists = new List<AssetData>();
        public float LastUsedListIndex { get; set; }


        public void AddPartList(AssetData partsList)
        {
            createdPartsLists.Add(partsList);
        }

        public void RemovePartList(AssetData partList)
        {
            createdPartsLists.Remove(partList);
        }
    }
}