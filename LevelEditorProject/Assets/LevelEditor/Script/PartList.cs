/// <author> Thomas Krahl </author>
/// <version>1.00</version>
/// <date>24/02/2022</date>

using System.Collections.Generic;
using UnityEngine;

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
