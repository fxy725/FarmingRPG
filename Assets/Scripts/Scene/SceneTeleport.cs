using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto = SceneName.scene1_Farm;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();


    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            // 计算玩家的新位置

            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f)
                ? player.transform.position.x
                : scenePositionGoto.x;

            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f)
                ? player.transform.position.y
                : scenePositionGoto.y;

            float zPosition = 0f;

            // 传送到新场景
            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(),
                new Vector3(xPosition, yPosition, zPosition));
        }
    }
}
