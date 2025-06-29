using UnityEngine;

[System.Serializable]
public class SceneItem
{
    public int itemCode;
    public Vector3 position;
    public string itemName;

    public SceneItem()
    {
        position = new Vector3();
    }
}
