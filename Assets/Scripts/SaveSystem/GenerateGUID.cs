using UnityEngine;

[ExecuteAlways] //编辑器模式下依旧运行
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        if (!Application.IsPlaying(gameObject))
        {
            // 确保对象有独一无二的标识符
            if (_gUID == "")
            {
                _gUID = System.Guid.NewGuid().ToString();   //微软官方提供的GUID生成工具
            }
        }
    }
}
