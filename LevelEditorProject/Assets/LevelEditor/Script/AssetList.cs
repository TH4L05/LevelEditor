/// <author> Thomas Krahl </author>

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    [System.Serializable]
    public class LevelAsset
    {
        [SerializeField] string partName;
        [SerializeField] GameObject template;

        public string PartName => partName;
        public GameObject Template => template;
    }

    public class AssetList : ScriptableObject
    {
        public List<LevelAsset> parts = new List<LevelAsset>();
    }
}