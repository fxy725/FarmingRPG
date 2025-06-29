using UnityEngine;

[ExecuteAlways] //编辑模式与预制件编辑模式下也运行
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        if (!Application.IsPlaying(gameObject)) //判断是否在运行模式下
        {
            if (string.IsNullOrEmpty(_gUID))
            {
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
