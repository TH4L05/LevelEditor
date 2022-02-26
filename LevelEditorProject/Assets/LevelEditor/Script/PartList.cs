/// <author> Thomas Krahl </author>

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    [System.Serializable]
    public class LevelPart
    {
        [SerializeField] string partName;
        [SerializeField] GameObject template;

        public string PartName => partName;
        public GameObject Template => template;
    }

    public class PartList : ScriptableObject
    {
        public List<LevelPart> parts = new List<LevelPart>();
    }
}