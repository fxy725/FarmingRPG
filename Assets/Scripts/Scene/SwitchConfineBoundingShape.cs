using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    }

    /// <summary>
    /// 切换cinemachine使用的collider来定义屏幕的边缘
    /// </summary>
    private void SwitchBoundingShape()
    {
        // 获取'boundsconfiner'游戏对象上的polygon collider，该collider由Cinemachine使用来防止摄像机超出屏幕边缘
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        // 由于confiner边界已更改，需要调用此方法来清除缓存;

        cinemachineConfiner.InvalidatePathCache();
    }
}
