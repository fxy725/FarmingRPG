[System.Serializable] // 普通类或结构体可以使用特性System.Serializable来标记为可序列化
public struct CharacterPartProperties
{

    public CharacterPartAnimator characterPart;
    public PartVariantColour partVariantColour;
    public PartVariantType partVariantType;

    // 构造函数，初始化CharacterAttribute的字段
    public CharacterPartProperties(CharacterPartAnimator characterPart, PartVariantColour partVariantColour, PartVariantType partVariantType)
    {
        this.characterPart = characterPart;
        this.partVariantColour = partVariantColour;
        this.partVariantType = partVariantType;
    }
}

// 该结构体用于存储角色部件的相关信息，包括角色部位、颜色和类型等