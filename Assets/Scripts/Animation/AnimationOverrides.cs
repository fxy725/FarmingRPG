using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character; // AnimationOverrides要应用的角色，存储对角色的引用
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray; // SO_AnimationType的数组类型字段，存储相关的所有SO_AnimationType的引用

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation; // 动画类型字典，键为AnimationClip，值为SO_AnimationType
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey; // 动画类型字典，键为字符串，值为SO_AnimationType

    private void Start()
    {
        // 初始化键为AnimationClip的动画类型字典
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        // 遍历SO_AnimationType数组，将每个SO_AnimationType的animationClip作为键，SO_AnimationType本身作为值添加到字典中
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        // 初始化键为字符串的动画类型字典
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        // 遍历SO_AnimationType数组，将每个SO_AnimationType的characterPart、partVariantColour、partVariantType和animationName组合成一个字符串作为键，SO_AnimationType本身作为值添加到字典中
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }

    }


    public void ApplyCharacterCustomizationParameters(List<CharacterPart> characterPartPropertiesList)
    {
        //Stopwatch s1 = Stopwatch.StartNew();

        // 遍历所有的角色部件属性实例，为它们设置AnimatorOverrideController
        foreach (CharacterPart characterPartProperties in characterPartPropertiesList)
        {
            Animator currentAnimator = null;
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animatorSOAssetName = characterPartProperties.characterPartAnimator.ToString(); // 获取角色部件的名称

            // 查找要应用的角色游戏对象及其所有子对象的Animator组件并存储在animatorsArray中
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animatorsArray) // 遍历所有Animator组件
            {
                // 如果Animator的名称与animatorSOAssetName相同，则将其赋值给currentAnimator
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            // 获取Animator的基动画
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            foreach (AnimationClip animationClip in animationsList)
            {
                // 在字典中查找动画
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {
                    string key = characterPartProperties.characterPartAnimator.ToString() + characterPartProperties.partVariantColor.ToString() + characterPartProperties.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;

                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            // 应用动画更新到动画覆盖控制器，然后更新Animator
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }

        // s1.Stop();
        // UnityEngine.Debug.Log("Time to apply character customization : " + s1.Elapsed + "   elapsed seconds");
    }

}
