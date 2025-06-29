using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneSave
{
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, bool> boolDictionary;
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3> vector3Dictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    public List<SceneItem> listSceneItem;
    public List<InventoryItem>[] listInvItemArray;
}
