
using UnityEngine;

public static class Settings
{
    // 持久化场景的引用
    public const string PersistentScene = "PersistentScene";

    // Obscuring Item Fading - ObscuringItemFader
    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f;

    // 图块地图
    public const float gridCellSize = 1f; // 网格单元大小（Unity单位）
    public const float gridCellDiagonalSize = 1.41f; // Unity单元中心的对角距离
    public const int maxGridWidth = 99999;
    public const int maxGridHeight = 99999;
    public static Vector2 cursorSize = Vector2.one;

    // 玩家
    public static float playerCentreYOffset = 0.875f;


    // 玩家移动
    public const float runningSpeed = 5.333f;
    public const float walkingSpeed = 2.666f;
    public static float useToolAnimationPause = 0.25f;
    public static float liftToolAnimationPause = 0.4f;
    public static float pickAnimationPause = 1f;
    public static float afterUseToolAnimationPause = 0.2f;
    public static float afterLiftToolAnimationPause = 0.4f;
    public static float afterPickAnimationPause = 0.2f;

    // NPC移动
    public static float pixelSize = 0.0625f;

    // 库存
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;

    // NPC动画参数
    public static int walkUp;
    public static int walkDown;
    public static int walkLeft;
    public static int walkRight;
    public static int eventAnimation;

    // 玩家动画参数
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;
    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;

    // 共享动画参数
    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;

    // 工具
    public const string HoeingTool = "Hoe"; // 锄头
    public const string ChoppingTool = "Axe"; // 斧头
    public const string BreakingTool = "Pickaxe"; // 镐子
    public const string ReapingTool = "Scythe"; // 镰刀
    public const string WateringTool = "Watering Can"; // 水壶
    public const string CollectingTool = "Basket"; // 篮子

    // 收获
    public const int maxCollidersToTestPerReapSwing = 15; // 每次收割挥动时要测试的最大碰撞器数量
    public const int maxTargetComponentsToDestroyPerReapSwing = 2; // 每次收割挥动时要摧毁的最大目标组件数量


    // 游戏每秒的时间时间
    public const float secondsPerGameSecond = 0.012f;


    // 静态构造函数
    static Settings()
    {
        // NPC动画参数
        walkUp = Animator.StringToHash("walkUp");
        walkDown = Animator.StringToHash("walkDown");
        walkLeft = Animator.StringToHash("walkLeft");
        walkRight = Animator.StringToHash("walkRight");
        eventAnimation = Animator.StringToHash("eventAnimation");

        // 玩家动画参数
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");

        // 共享动画参数
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");
    }
}
