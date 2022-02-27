/// <author> Thomas Krahl </author>

using System.Collections.Generic;
using UnityEngine;


namespace LevelEditor
{
    [System.Serializable]
    public class AssetData
    {
        [SerializeField] AssetList assetList;
        [SerializeField] string path;

        public AssetList AssetList => assetList;
        public string Path => path;


        public void SetList(AssetList list, string path)
        {
            assetList = list;
            this.path = path;
        }
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