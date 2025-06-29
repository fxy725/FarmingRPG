[System.Serializable] // 普通类或结构体可以使用特性System.Serializable来标记为可序列化
public struct CharacterPart
{

    public CharacterPartAnimator characterPartAnimator;
    public PartVariantColor partVariantColor;
    public PartVariantType partVariantType;

    // 构造函数，初始化CharacterPart的字段
    public CharacterPart(CharacterPartAnimator characterPartAnimator, PartVariantColor partVariantColor, PartVariantType partVariantType)
    {
        this.characterPartAnimator = characterPartAnimator;
        this.partVariantColor = partVariantColor;
        this.partVariantType = partVariantType;
    }
}

// 该结构体用于存储角色部件的相关信息，包括角色部位、颜色和类型等