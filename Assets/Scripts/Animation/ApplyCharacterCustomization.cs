using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class colorSwap
{
    public Color fromColor;
    public Color toColor;

    public colorSwap(Color fromColor, Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}


public class ApplyCharacterCustomization : MonoBehaviour
{
    // 输入纹理
    [Header("基础纹理")]
    [SerializeField] private Texture2D maleFarmerBaseTexture = null;
    [SerializeField] private Texture2D femaleFarmerBaseTexture = null;
    [SerializeField] private Texture2D shirtsBaseTexture = null;
    [SerializeField] private Texture2D hairBaseTexture = null;
    [SerializeField] private Texture2D hatsBaseTexture = null;
    [SerializeField] private Texture2D adornmentsBaseTexture = null;
    private Texture2D farmerBaseTexture;

    // 创建纹理
    [Header("输出纹理")]
    [SerializeField] private Texture2D farmerBaseCustomised = null;
    [SerializeField] private Texture2D hairCustomised = null;
    [SerializeField] private Texture2D hatsCustomised = null;
    private Texture2D farmerBaseShirtsUpdated;
    private Texture2D farmerBaseAdornmentsUpdated;
    private Texture2D selectedShirt;
    private Texture2D selectedAdornment;

    // 选择衬衫样式
    [Header("选择衬衫样式")]
    [Range(0, 1)]
    [SerializeField] private int inputShirtStyleNo = 0;

    // 选择发型样式
    [Header("选择发型样式")]
    [Range(0, 2)]
    [SerializeField] private int inputHairStyleNo = 0;

    // 选择帽子样式
    [Header("选择帽子样式")]
    [Range(0, 1)]
    [SerializeField] private int inputHatStyleNo = 0;

    // 选择装饰样式
    [Header("选择装饰样式")]
    [Range(0, 2)]
    [SerializeField] private int inputAdornmentsStyleNo = 0;

    // 选择皮肤类型
    [Header("选择皮肤类型")]
    [Range(0, 3)]
    [SerializeField] private int inputSkinType = 0;

    // 选择性别
    [Header("选择性别: 0=男, 1=女")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    // 选择头发颜色
    [SerializeField] private Color inputHairColor = Color.black;

    // 选择裤子颜色
    [SerializeField] private Color inputTrouserColor = Color.blue;


    private Facing[,] bodyFacingArray;
    private Vector2Int[,] bodyShirtOffsetArray;
    private Vector2Int[,] bodyAdornmentsOffsetArray;

    // 尺寸
    private int bodyRows = 21;
    private int bodyColumns = 6;
    private int farmerSpriteWidth = 16;
    private int farmerSpriteHeight = 32;
    private int shirtTextureWidth = 9;
    private int shirtTextureHeight = 36;
    private int shirtSpriteWidth = 9;
    private int shirtSpriteHeight = 9;
    private int shirtStylesInSpriteWidth = 16;

    private int hairTextureWidth = 16;
    private int hairTextureHeight = 96;
    private int hairStylesInSpriteWidth = 8;

    private int hatTextureWidth = 20;
    private int hatTextureHeight = 80;
    private int hatStylesInSpriteWidth = 12;

    private int adornmentsTextureWidth = 16;
    private int adornmentsTextureHeight = 32;
    private int adornmentsStylesInSpriteWidth = 8;
    private int adornmentsSpriteWidth = 16;
    private int adornmentsSpriteHeight = 16;

    private List<colorSwap> colorSwapList;

    // 目标手臂颜色用于颜色替换
    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);  // darkest
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255); // next darkest
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255); // lightest

    // 目标皮肤颜色用于颜色替换
    private Color32 skinTargetColor1 = new Color32(145, 117, 90, 255);  // darkest
    private Color32 skinTargetColor2 = new Color32(204, 155, 108, 255);  // next darkest
    private Color32 skinTargetColor3 = new Color32(207, 166, 128, 255);  // next darkest
    private Color32 skinTargetColor4 = new Color32(238, 195, 154, 255);  // lightest


    private void Awake()
    {
        // 初始化颜色交换列表
        colorSwapList = new List<colorSwap>();

        // 处理自定义
        ProcessCustomisation();
    }

    private void ProcessCustomisation()
    {
        ProcessGender();

        ProcessShirt();

        ProcessArms();

        ProcessTrousers();

        ProcessHair();

        ProcessSkin();

        ProcessHat();

        ProcessAdornments();

        MergeCustomisations();
    }

    private void ProcessGender()
    {
        // 根据性别设置基础精灵表
        if (inputSex == 0)
        {
            farmerBaseTexture = maleFarmerBaseTexture;
        }
        else if (inputSex == 1)
        {
            farmerBaseTexture = femaleFarmerBaseTexture;
        }

        // 获取基础像素
        Color[] farmerBasePixels = farmerBaseTexture.GetPixels();

        // 设置更改后的基础像素
        farmerBaseCustomised.SetPixels(farmerBasePixels);
        farmerBaseCustomised.Apply();
    }

    private void ProcessShirt()
    {
        // 初始化身体朝向衬衫数组
        bodyFacingArray = new Facing[bodyColumns, bodyRows];

        // 填充身体朝向衬衫数组
        PopulateBodyFacingArray();

        // 初始化身体衬衫偏移数组;
        bodyShirtOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        // 填充身体衬衫偏移数组
        PopulateBodyShirtOffsetArray();

        // 创建选定的衬衫纹理
        AddShirtToTexture(inputShirtStyleNo);

        // 应用衬衫纹理到基础
        ApplyShirtTextureToBase();
    }

    private void ProcessArms()
    {
        // 获取手臂像素以重新着色
        Color[] farmerPixelsToRecolour = farmerBaseTexture.GetPixels(0, 0, 288, farmerBaseTexture.height);

        // 填充手臂颜色交换列表
        PopulateArmColorSwapList();

        // 改变手臂颜色
        ChangePixelColors(farmerPixelsToRecolour, colorSwapList);

        // 设置重新着色的像素
        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolour);

        // 应用纹理更改
        farmerBaseCustomised.Apply();
    }

    private void ProcessTrousers()
    {
        // 获取裤子像素以重新着色
        Color[] farmerTrouserPixels = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);

        // 改变裤子颜色
        TintPixelColors(farmerTrouserPixels, inputTrouserColor);

        // 设置更改后的裤子像素
        farmerBaseCustomised.SetPixels(288, 0, 96, farmerBaseTexture.height, farmerTrouserPixels);

        // 应用纹理更改
        farmerBaseCustomised.Apply();

    }

    private void ProcessHair()
    {
        // 创建选定的头发纹理
        AddHairToTexture(inputHairStyleNo);

        // 获取头发像素以重新着色
        Color[] farmerSelectedHairPixels = hairCustomised.GetPixels();

        // 着色头发像素
        TintPixelColors(farmerSelectedHairPixels, inputHairColor);

        hairCustomised.SetPixels(farmerSelectedHairPixels);
        hairCustomised.Apply();
    }


    private void ProcessSkin()
    {
        // 获取皮肤像素以重新着色
        Color[] farmerPixelsToRecolour = farmerBaseCustomised.GetPixels(0, 0, 288, farmerBaseTexture.height);

        // 填充皮肤颜色交换列表
        PopulateSkinColorSwapList(inputSkinType);

        // 改变皮肤颜色
        ChangePixelColors(farmerPixelsToRecolour, colorSwapList);

        // 设置重新着色的像素
        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolour);

        // 应用纹理更改
        farmerBaseCustomised.Apply();
    }

    private void ProcessHat()
    {
        // 创建选定的帽子纹理
        AddHatToTexture(inputHatStyleNo);
    }

    private void ProcessAdornments()
    {
        // 初始化身体装饰偏移数组;
        bodyAdornmentsOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        // 填充身体装饰偏移数组
        PopulateBodyAdornmentsOffsetArray();

        // 创建选定的装饰纹理
        AddAdornmentsToTexture(inputAdornmentsStyleNo);

        // 创建新的装饰基础纹理
        farmerBaseAdornmentsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        farmerBaseAdornmentsUpdated.filterMode = FilterMode.Point;

        // 设置装饰基础纹理为透明
        SetTextureToTransparent(farmerBaseAdornmentsUpdated);
        ApplyAdornmentsTextureToBase();
    }


    private void MergeCustomisations()
    {
        // 农民衬衫像素
        Color[] farmerShirtPixels = farmerBaseShirtsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        // 农民裤子像素
        Color[] farmerTrouserPixelsSelection = farmerBaseCustomised.GetPixels(288, 0, 96, farmerBaseTexture.height);
        // 农民装饰像素
        Color[] farmerAdornmentsPixels = farmerBaseAdornmentsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        // 农民身体像素
        Color[] farmerBodyPixels = farmerBaseCustomised.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        MergeColourArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColourArray(farmerBodyPixels, farmerShirtPixels);
        MergeColourArray(farmerBodyPixels, farmerAdornmentsPixels);

        // 粘贴合并的像素
        farmerBaseCustomised.SetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height, farmerBodyPixels);

        // 应用纹理更改
        farmerBaseCustomised.Apply();
    }

    private void ApplyAdornmentsTextureToBase()
    {
        Color[] frontAdornmentsPixels;
        Color[] rightAdornmentsPixels;

        frontAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentsSpriteHeight * 1, adornmentsSpriteWidth, adornmentsSpriteHeight);

        rightAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentsSpriteHeight * 0, adornmentsSpriteWidth, adornmentsSpriteHeight);

        // 遍历基础纹理并应用装饰像素

        for (int x = 0; x < bodyColumns; x++)
        {
            for (int y = 0; y < bodyRows; y++)
            {
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;

                if (bodyAdornmentsOffsetArray[x, y] != null)
                {
                    pixelX += bodyAdornmentsOffsetArray[x, y].x;
                    pixelY += bodyAdornmentsOffsetArray[x, y].y;
                }

                // 根据朝向切换
                switch (bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;

                    case Facing.front:
                        // 填充前装饰像素
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentsSpriteWidth, adornmentsSpriteHeight, frontAdornmentsPixels);
                        break;

                    case Facing.right:
                        // 填充右装饰像素
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentsSpriteWidth, adornmentsSpriteHeight, rightAdornmentsPixels);
                        break;

                    default:
                        break;
                }
            }
        }

        // 应用装饰纹理像素
        farmerBaseAdornmentsUpdated.Apply();
    }

    private void TintPixelColors(Color[] basePixelArray, Color tintColor)
    {
        // 遍历像素以着色
        for (int i = 0; i < basePixelArray.Length; i++)
        {
            basePixelArray[i].r = basePixelArray[i].r * tintColor.r;
            basePixelArray[i].g = basePixelArray[i].g * tintColor.g;
            basePixelArray[i].b = basePixelArray[i].b * tintColor.b;
        }
    }

    private void PopulateArmColorSwapList()
    {
        // 清除颜色交换列表
        colorSwapList.Clear();

        // 手臂替换颜色
        colorSwapList.Add(new colorSwap(armTargetColor1, selectedShirt.GetPixel(0, 7)));
        colorSwapList.Add(new colorSwap(armTargetColor2, selectedShirt.GetPixel(0, 6)));
        colorSwapList.Add(new colorSwap(armTargetColor3, selectedShirt.GetPixel(0, 5)));
    }

    private void PopulateSkinColorSwapList(int skinType)
    {
        // 清除颜色交换列表
        colorSwapList.Clear();

        // 皮肤替换颜色
        // 根据皮肤类型切换
        switch (skinType)
        {
            case 0:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;

            case 1:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(187, 157, 128, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(231, 187, 144, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(221, 186, 154, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(213, 189, 167, 255)));
                break;

            case 2:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(105, 69, 2, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(128, 87, 12, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(145, 103, 26, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(161, 114, 25, 255)));
                break;

            case 3:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(151, 132, 0, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(187, 166, 15, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(209, 188, 39, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(211, 199, 112, 255)));
                break;

            default:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }


    private void ChangePixelColors(Color[] baseArray, List<colorSwap> colorSwapList)
    {
        for (int i = 0; i < baseArray.Length; i++)
        {
            // 遍历颜色交换列表
            if (colorSwapList.Count > 0)
            {
                for (int j = 0; j < colorSwapList.Count; j++)
                {
                    if (isSameColor(baseArray[i], colorSwapList[j].fromColor))
                    {
                        baseArray[i] = colorSwapList[j].toColor;
                    }
                }
            }
        }
    }

    private bool isSameColor(Color color1, Color color2)
    {
        if ((color1.r == color2.r) && (color1.g == color2.g) && (color1.b == color2.b) && (color1.a == color2.a))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MergeColourArray(Color[] baseArray, Color[] mergeArray)
    {
        for (int i = 0; i < baseArray.Length; i++)
        {
            if (mergeArray[i].a > 0)
            {
                // 合并数组有颜色
                if (mergeArray[i].a >= 1)
                {
                    // 完全替换
                    baseArray[i] = mergeArray[i];
                }
                else
                {
                    // 插值颜色
                    float alpha = mergeArray[i].a;

                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;
                    baseArray[i].g += (mergeArray[i].g - baseArray[i].g) * alpha;
                    baseArray[i].b += (mergeArray[i].b - baseArray[i].b) * alpha;
                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }

    private void AddHairToTexture(int hairStyleNo)
    {
        // 计算头发像素的坐标
        int y = (hairStyleNo / hairStylesInSpriteWidth) * hairTextureHeight;
        int x = (hairStyleNo % hairStylesInSpriteWidth) * hairTextureWidth;

        // 获取头发像素
        Color[] hairPixels = hairBaseTexture.GetPixels(x, y, hairTextureWidth, hairTextureHeight);

        // 应用选定的头发像素到纹理
        hairCustomised.SetPixels(hairPixels);
        hairCustomised.Apply();
    }


    private void AddHatToTexture(int hatStyleNo)
    {
        // 计算帽子像素的坐标
        int y = (hatStyleNo / hatStylesInSpriteWidth) * hatTextureHeight;
        int x = (hatStyleNo % hatStylesInSpriteWidth) * hatTextureWidth;

        // 获取帽子像素
        Color[] hatPixels = hatsBaseTexture.GetPixels(x, y, hatTextureWidth, hatTextureHeight);

        // 应用选定的帽子像素到纹理
        hatsCustomised.SetPixels(hatPixels);
        hatsCustomised.Apply();
    }

    private void AddAdornmentsToTexture(int adornmentsStyleNo)
    {
        // 创建装饰纹理
        selectedAdornment = new Texture2D(adornmentsTextureWidth, adornmentsTextureHeight);
        selectedAdornment.filterMode = FilterMode.Point;

        // 计算装饰像素的坐标
        int y = (adornmentsStyleNo / adornmentsStylesInSpriteWidth) * adornmentsTextureHeight;
        int x = (adornmentsStyleNo % adornmentsStylesInSpriteWidth) * adornmentsTextureWidth;

        // 获取装饰像素
        Color[] adornmentsPixels = adornmentsBaseTexture.GetPixels(x, y, adornmentsTextureWidth, adornmentsTextureHeight);

        // 应用选定的装饰像素到纹理
        selectedAdornment.SetPixels(adornmentsPixels);
        selectedAdornment.Apply();
    }



    private void AddShirtToTexture(int shirtStyleNo)
    {
        // 创建衬衫纹理
        selectedShirt = new Texture2D(shirtTextureWidth, shirtTextureHeight);
        selectedShirt.filterMode = FilterMode.Point;

        // 计算衬衫像素的坐标
        int y = (shirtStyleNo / shirtStylesInSpriteWidth) * shirtTextureHeight;
        int x = (shirtStyleNo % shirtStylesInSpriteWidth) * shirtTextureWidth;

        // 获取衬衫像素
        Color[] shirtPixels = shirtsBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);

        // 应用选定的衬衫像素到纹理
        selectedShirt.SetPixels(shirtPixels);
        selectedShirt.Apply();
    }

    private void ApplyShirtTextureToBase()
    {
        // 创建新的衬衫基础纹理
        farmerBaseShirtsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        farmerBaseShirtsUpdated.filterMode = FilterMode.Point;

        // 设置衬衫基础纹理为透明
        SetTextureToTransparent(farmerBaseShirtsUpdated);

        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;

        frontShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);

        // 遍历基础纹理并应用衬衫像素

        for (int x = 0; x < bodyColumns; x++)
        {
            for (int y = 0; y < bodyRows; y++)
            {
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;

                if (bodyShirtOffsetArray[x, y] != null)
                {
                    if (bodyShirtOffsetArray[x, y].x == 99 && bodyShirtOffsetArray[x, y].y == 99) // do not populate with shirt
                        continue;

                    pixelX += bodyShirtOffsetArray[x, y].x;
                    pixelY += bodyShirtOffsetArray[x, y].y;
                }

                // 根据朝向切换
                switch (bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;

                    case Facing.front:
                        // 填充前衬衫像素
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, frontShirtPixels);
                        break;

                    case Facing.back:
                        // 填充后衬衫像素
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, backShirtPixels);
                        break;

                    case Facing.right:
                        // 填充右衬衫像素
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, rightShirtPixels);
                        break;

                    default:
                        break;
                }
            }
        }

        // 应用衬衫纹理像素
        farmerBaseShirtsUpdated.Apply();
    }

    private void SetTextureToTransparent(Texture2D texture2D)
    {
        // 填充纹理为透明
        Color[] fill = new Color[texture2D.height * texture2D.width];
        for (int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.clear;
        }
        texture2D.SetPixels(fill);
    }

    private void PopulateBodyFacingArray()
    {
        bodyFacingArray[0, 0] = Facing.none;
        bodyFacingArray[1, 0] = Facing.none;
        bodyFacingArray[2, 0] = Facing.none;
        bodyFacingArray[3, 0] = Facing.none;
        bodyFacingArray[4, 0] = Facing.none;
        bodyFacingArray[5, 0] = Facing.none;

        bodyFacingArray[0, 1] = Facing.none;
        bodyFacingArray[1, 1] = Facing.none;
        bodyFacingArray[2, 1] = Facing.none;
        bodyFacingArray[3, 1] = Facing.none;
        bodyFacingArray[4, 1] = Facing.none;
        bodyFacingArray[5, 1] = Facing.none;

        bodyFacingArray[0, 2] = Facing.none;
        bodyFacingArray[1, 2] = Facing.none;
        bodyFacingArray[2, 2] = Facing.none;
        bodyFacingArray[3, 2] = Facing.none;
        bodyFacingArray[4, 2] = Facing.none;
        bodyFacingArray[5, 2] = Facing.none;

        bodyFacingArray[0, 3] = Facing.none;
        bodyFacingArray[1, 3] = Facing.none;
        bodyFacingArray[2, 3] = Facing.none;
        bodyFacingArray[3, 3] = Facing.none;
        bodyFacingArray[4, 3] = Facing.none;
        bodyFacingArray[5, 3] = Facing.none;

        bodyFacingArray[0, 4] = Facing.none;
        bodyFacingArray[1, 4] = Facing.none;
        bodyFacingArray[2, 4] = Facing.none;
        bodyFacingArray[3, 4] = Facing.none;
        bodyFacingArray[4, 4] = Facing.none;
        bodyFacingArray[5, 4] = Facing.none;

        bodyFacingArray[0, 5] = Facing.none;
        bodyFacingArray[1, 5] = Facing.none;
        bodyFacingArray[2, 5] = Facing.none;
        bodyFacingArray[3, 5] = Facing.none;
        bodyFacingArray[4, 5] = Facing.none;
        bodyFacingArray[5, 5] = Facing.none;

        bodyFacingArray[0, 6] = Facing.none;
        bodyFacingArray[1, 6] = Facing.none;
        bodyFacingArray[2, 6] = Facing.none;
        bodyFacingArray[3, 6] = Facing.none;
        bodyFacingArray[4, 6] = Facing.none;
        bodyFacingArray[5, 6] = Facing.none;

        bodyFacingArray[0, 7] = Facing.none;
        bodyFacingArray[1, 7] = Facing.none;
        bodyFacingArray[2, 7] = Facing.none;
        bodyFacingArray[3, 7] = Facing.none;
        bodyFacingArray[4, 7] = Facing.none;
        bodyFacingArray[5, 7] = Facing.none;

        bodyFacingArray[0, 8] = Facing.none;
        bodyFacingArray[1, 8] = Facing.none;
        bodyFacingArray[2, 8] = Facing.none;
        bodyFacingArray[3, 8] = Facing.none;
        bodyFacingArray[4, 8] = Facing.none;
        bodyFacingArray[5, 8] = Facing.none;

        bodyFacingArray[0, 9] = Facing.none;
        bodyFacingArray[1, 9] = Facing.none;
        bodyFacingArray[2, 9] = Facing.none;
        bodyFacingArray[3, 9] = Facing.none;
        bodyFacingArray[4, 9] = Facing.none;
        bodyFacingArray[5, 9] = Facing.none;

        bodyFacingArray[0, 10] = Facing.back;
        bodyFacingArray[1, 10] = Facing.back;
        bodyFacingArray[2, 10] = Facing.right;
        bodyFacingArray[3, 10] = Facing.right;
        bodyFacingArray[4, 10] = Facing.right;
        bodyFacingArray[5, 10] = Facing.right;

        bodyFacingArray[0, 11] = Facing.front;
        bodyFacingArray[1, 11] = Facing.front;
        bodyFacingArray[2, 11] = Facing.front;
        bodyFacingArray[3, 11] = Facing.front;
        bodyFacingArray[4, 11] = Facing.back;
        bodyFacingArray[5, 11] = Facing.back;

        bodyFacingArray[0, 12] = Facing.back;
        bodyFacingArray[1, 12] = Facing.back;
        bodyFacingArray[2, 12] = Facing.right;
        bodyFacingArray[3, 12] = Facing.right;
        bodyFacingArray[4, 12] = Facing.right;
        bodyFacingArray[5, 12] = Facing.right;

        bodyFacingArray[0, 13] = Facing.front;
        bodyFacingArray[1, 13] = Facing.front;
        bodyFacingArray[2, 13] = Facing.front;
        bodyFacingArray[3, 13] = Facing.front;
        bodyFacingArray[4, 13] = Facing.back;
        bodyFacingArray[5, 13] = Facing.back;

        bodyFacingArray[0, 14] = Facing.back;
        bodyFacingArray[1, 14] = Facing.back;
        bodyFacingArray[2, 14] = Facing.right;
        bodyFacingArray[3, 14] = Facing.right;
        bodyFacingArray[4, 14] = Facing.right;
        bodyFacingArray[5, 14] = Facing.right;

        bodyFacingArray[0, 15] = Facing.front;
        bodyFacingArray[1, 15] = Facing.front;
        bodyFacingArray[2, 15] = Facing.front;
        bodyFacingArray[3, 15] = Facing.front;
        bodyFacingArray[4, 15] = Facing.back;
        bodyFacingArray[5, 15] = Facing.back;

        bodyFacingArray[0, 16] = Facing.back;
        bodyFacingArray[1, 16] = Facing.back;
        bodyFacingArray[2, 16] = Facing.right;
        bodyFacingArray[3, 16] = Facing.right;
        bodyFacingArray[4, 16] = Facing.right;
        bodyFacingArray[5, 16] = Facing.right;

        bodyFacingArray[0, 17] = Facing.front;
        bodyFacingArray[1, 17] = Facing.front;
        bodyFacingArray[2, 17] = Facing.front;
        bodyFacingArray[3, 17] = Facing.front;
        bodyFacingArray[4, 17] = Facing.back;
        bodyFacingArray[5, 17] = Facing.back;

        bodyFacingArray[0, 18] = Facing.back;
        bodyFacingArray[1, 18] = Facing.back;
        bodyFacingArray[2, 18] = Facing.back;
        bodyFacingArray[3, 18] = Facing.right;
        bodyFacingArray[4, 18] = Facing.right;
        bodyFacingArray[5, 18] = Facing.right;

        bodyFacingArray[0, 19] = Facing.right;
        bodyFacingArray[1, 19] = Facing.right;
        bodyFacingArray[2, 19] = Facing.right;
        bodyFacingArray[3, 19] = Facing.front;
        bodyFacingArray[4, 19] = Facing.front;
        bodyFacingArray[5, 19] = Facing.front;

        bodyFacingArray[0, 20] = Facing.front;
        bodyFacingArray[1, 20] = Facing.front;
        bodyFacingArray[2, 20] = Facing.front;
        bodyFacingArray[3, 20] = Facing.back;
        bodyFacingArray[4, 20] = Facing.back;
        bodyFacingArray[5, 20] = Facing.back;
    }

    private void PopulateBodyAdornmentsOffsetArray()
    {
        bodyAdornmentsOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 0] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 1] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 2] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 3] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 4] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 5] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 6] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 7] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 8] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 9] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 10] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[4, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[5, 10] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[1, 11] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[2, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 11] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 11] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 11] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 12] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[3, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[4, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 12] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 13] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 13] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[3, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[4, 13] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 13] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 14] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 14] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[4, 14] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 14] = new Vector2Int(0, 1 + 16);

        bodyAdornmentsOffsetArray[0, 15] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[1, 15] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[2, 15] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 15] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[4, 15] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 15] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 16] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 16] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[4, 16] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 16] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 17] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[1, 17] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[2, 17] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 17] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 17] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 17] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 18] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 18] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 18] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 19] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 20] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 20] = new Vector2Int(99, 99);
    }

    private void PopulateBodyShirtOffsetArray()
    {
        bodyShirtOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 0] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 1] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 2] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 3] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 4] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 5] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 6] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 7] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 8] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 9] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[1, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[3, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);
    }
}
