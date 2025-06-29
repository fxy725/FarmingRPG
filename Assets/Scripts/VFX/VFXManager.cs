using System.Collections;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    private WaitForSeconds twoSeconds;

    [SerializeField] private GameObject deciduousLeavesFallingPrefab;
    [SerializeField] private GameObject pineConesFallingPrefab;
    [SerializeField] private GameObject choppingTreeTrunkPrefab;
    [SerializeField] private GameObject breakingStonePrefab;
    [SerializeField] private GameObject reapingPrefab;



    protected override void Awake()
    {
        base.Awake();

        twoSeconds = new WaitForSeconds(2f);

    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;
    }



    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }

    private void DisplayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {

            case HarvestActionEffect.deciduousLeavesFalling: //落叶掉落
                GameObject deciduousLeaveFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);
                deciduousLeaveFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(deciduousLeaveFalling, twoSeconds));
                break;

            case HarvestActionEffect.pineConesFalling: //松果掉落
                GameObject pineConesFalling = PoolManager.Instance.ReuseObject(pineConesFallingPrefab, effectPosition, Quaternion.identity);
                pineConesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(pineConesFalling, twoSeconds));
                break;

            case HarvestActionEffect.choppingTreeTrunk: //砍树
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;

            case HarvestActionEffect.breakingStone: //破坏石头
                GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonePrefab, effectPosition, Quaternion.identity);
                breakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(breakingStone, twoSeconds));
                break;


            case HarvestActionEffect.reaping: //收割
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;


            case HarvestActionEffect.none:
                break;

            default:
                break;
        }
    }
}
