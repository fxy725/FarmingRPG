public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkUp,
    walkDown,
    walkRight,
    walkLeft,
    runUp,
    runDown,
    runRight,
    runLeft,
    useToolUp,
    useToolDown,
    useToolRight,
    useToolLeft,
    swingToolUp,
    swingToolDown,
    swingToolRight,
    swingToolLeft,
    liftToolUp,
    liftToolDown,
    liftToolRight,
    liftToolLeft,
    holdToolUp,
    holdToolDown,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count
}

public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}
public enum PartVariantColour
{
    none,
    count
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}
public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}


public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

public enum Season
{
    春天,
    夏天,
    秋天,
    冬天,
    none,
    count
}


public enum ToolEffect
{
    none,
    watering
}


public enum HarvestActionEffect
{
    deciduousLeavesFalling,
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
    none
}

public enum Weather
{
    dry,
    raining,
    snowing,
    none,
    count
}


public enum Direction
{
    up,
    down,
    left,
    right,
    none
}

public enum SoundName
{
    none = 0,
    effectFootstepSoftGround = 10,
    effectFootstepHardGround = 20,
    effectAxe = 30,
    effectPickaxe = 40,
    effectScythe = 50,
    effectHoe = 60,
    effectWateringCan = 70,
    effectBasket = 80,
    effectPickupSound = 90,
    effectRustle = 100,
    effectTreeFalling = 110,
    effectPlantingSound = 120,
    effectPluck = 130,
    effectStoneShatter = 140,
    effectWoodSplinters = 150,
    ambientCountryside1 = 1000,
    ambientCountryside2 = 1010,
    ambientIndoors1 = 1020,
    musicCalm3 = 2000,
    musicCalm1 = 2010
}


public enum ItemType //物品类型
{
    Seed, //种子
    Commodity, //商品
    Watering_tool, //浇水工具
    Hoeing_tool, //锄头工具
    Chopping_tool, //砍树工具
    Breaking_tool, //破坏工具
    Reaping_tool, //收割工具
    Collecting_tool, //收集工具
    Reapable_scenary, //可收割的场景物体
    Furniture, //家具
    none, //无
    count
}

public enum Facing //人物朝向
{
    none,
    front,
    back,
    right
}
