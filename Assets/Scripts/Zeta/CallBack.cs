using UnityEngine;

public class CallBack : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Awake is called");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable is called");
    }

    private void Start()
    {
        Debug.Log("Start is called");
    }

    private void Update()
    {
        Debug.Log("Update is called");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable is called");
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy is called");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit is called");
    }
}
