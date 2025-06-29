using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : SingletonMonobehaviour<Player>, ISaveable
{
    // 动画暂停时间
    private WaitForSeconds liftToolAnimationPause; // 举起工具动画的暂停时间
    private WaitForSeconds afterLiftToolAnimationPause; // 在举起工具动画后暂停的时间

    private WaitForSeconds useToolAnimationPause; // 使用工具动画的暂停时间
    private WaitForSeconds afterUseToolAnimationPause; // 在使用工具动画后暂停的时间

    private WaitForSeconds pickAnimationPause; // 拾取动画的暂停时间
    private WaitForSeconds afterPickAnimationPause; // 在拾取动画后暂停的时间


    // 组件引用
    private AnimationOverrides animationOverrides;
    private Camera mainCamera;
    private Rigidbody2D rigidBody2D;
    private GridCursor gridCursor;
    private Cursor cursor;

    [Tooltip("应在预制件中填充装备物品的SpriteRenderer组件")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer;


    // 移动参数
    private float xInput;
    private float yInput;
    private float movementSpeed;
    private Direction playerDirection;


    // 状态参数
    private bool isCarrying;
    private bool isIdle;
    private bool isRunning;
    private bool isWalking;

    private bool isLiftingToolDown;
    private bool isLiftingToolLeft;
    private bool isLiftingToolRight;
    private bool isLiftingToolUp;

    private bool isUsingToolDown;
    private bool isUsingToolLeft;
    private bool isUsingToolRight;
    private bool isUsingToolUp;

    private bool isSwingingToolDown;
    private bool isSwingingToolLeft;
    private bool isSwingingToolRight;
    private bool isSwingingToolUp;

    private bool isPickingUp;
    private bool isPickingDown;
    private bool isPickingLeft;
    private bool isPickingRight;

    private bool playerToolUseDisabled;
    private bool _playerInputIsDisabled;
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }


    // 角色部件
    private List<CharacterPart> characterAttributeCustomizationList;

    private CharacterPart armsPart;
    private CharacterPart toolPart;


    // 保存数据
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    // 其他
    private ToolEffect toolEffect;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    protected override void Awake()
    {
        base.Awake();


        // 初始化字段
        rigidBody2D = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        mainCamera = Camera.main;

        armsPart = new CharacterPart(CharacterPartAnimator.arms, PartVariantColor.none, PartVariantType.none);
        toolPart = new CharacterPart(CharacterPartAnimator.tool, PartVariantColor.none, PartVariantType.hoe);
        characterAttributeCustomizationList = new List<CharacterPart>();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }


    private void OnEnable()
    {
        SaveableRegister();

        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += () => PlayerInputIsDisabled = false;
    }


    private void Start()
    {
        gridCursor = FindFirstObjectByType<GridCursor>();
        cursor = FindFirstObjectByType<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);
    }


    private void FixedUpdate()
    {
        PlayerMovement();
    }


    private void Update()
    {
        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();

            PlayerMovementInput();

            PlayerWalkInput();

            PlayerClickInput();

            PlayerTestInput();

            // Send event to any listeners for player movement input
            EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
        }
    }


    private void OnDisable()
    {
        SaveableDeregister();

        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent -= () => PlayerInputIsDisabled = false;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);

        rigidBody2D.MovePosition(rigidBody2D.position + move);
    }



    private void PlayerMovementInput()
    {
        yInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");

        if (yInput != 0 && xInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }

        if (xInput != 0 || yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;

            // Capture player direction for save game
            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }
        }
        else if (xInput == 0 && yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }


    // 测试输入的临时例程
    private void PlayerTestInput()
    {
        // Trigger Advance Time
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        // Trigger Advance Day
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }

    }

    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }

    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
                {
                    // Get Cursor Grid Position
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                    // Get Player Grid Position
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        // Get Grid property details at cursor position (the GridCursor validation routine ensures that grid property details are not null)
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        // Get Selected item details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                    }
                    break;

                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;

                case ItemType.Watering_tool:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;

                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (

            cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
            )
        {
            return Vector3Int.right;
        }
        else if (
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
            )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection)
        {
            case Direction.up:
                // set idle up trigger
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false);

                break;

            case Direction.down:
                // set idle down trigger
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false);
                break;

            case Direction.left:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false);
                break;

            case Direction.right:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
                break;

            default:
                // set idle down trigger
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false);

                break;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }



    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        // Switch on tool
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            case ItemType.Chopping_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;


            case ItemType.Collecting_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;

            case ItemType.Breaking_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;

            case ItemType.Reaping_tool:
                if (cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;


            default:
                break;
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // Process if we have cropdetails for the seed
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            // Update grid properties with seed details
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;

            // Display planted crop at grid property details
            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            // Remove item from inventory
            EventHandler.CallRemoveSelectedItemFromInventoryEvent();

            // Make planting sound
            AudioManager.Instance.PlaySound(SoundName.effectPlantingSound);

        }

    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        //Play sound
        AudioManager.Instance.PlaySound(SoundName.effectHoe);

        // Trigger animation
        StartCoroutine(HoeGroundAtCursorCoroutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator HoeGroundAtCursorCoroutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to hoe in override animation
        toolPart.partVariantType = PartVariantType.hoe;
        characterAttributeCustomizationList.Clear();
        characterAttributeCustomizationList.Add(toolPart);
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;

        // Set Grid property details for dug ground
        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        // Set grid property to dug
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display dug grid tiles
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);


        // After animation pause
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        //Play sound
        AudioManager.Instance.PlaySound(SoundName.effectWateringCan);

        // Trigger animation
        StartCoroutine(WaterGroundAtCursorCoroutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator WaterGroundAtCursorCoroutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to watering can in override animation
        toolPart.partVariantType = PartVariantType.wateringCan;
        characterAttributeCustomizationList.Clear();
        characterAttributeCustomizationList.Add(toolPart);
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

        // TODO: If there is water in the watering can
        toolEffect = ToolEffect.watering;

        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        // Set Grid property details for watered ground
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        // Set grid property to watered
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display watered grid tiles
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        // After animation pause
        yield return afterLiftToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorCoroutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorCoroutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to scythe in override animation
        toolPart.partVariantType = PartVariantType.scythe;
        characterAttributeCustomizationList.Clear();
        characterAttributeCustomizationList.Add(toolPart);
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

        // Reap in player direction
        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        // Play sound
        AudioManager.Instance.PlaySound(SoundName.effectAxe);

        // Trigger animation
        StartCoroutine(ChopInPlayerDirectionCoroutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator ChopInPlayerDirectionCoroutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to axe in override animation
        toolPart.partVariantType = PartVariantType.axe;
        characterAttributeCustomizationList.Clear();
        characterAttributeCustomizationList.Add(toolPart);
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        // After animation pause
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }



    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        // Play sound
        AudioManager.Instance.PlaySound(SoundName.effectBasket);

        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }


    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return pickAnimationPause;

        // After animation pause
        yield return afterPickAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        // Play sound
        AudioManager.Instance.PlaySound(SoundName.effectPickaxe);

        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set tool animation to pickaxe in override animation
        toolPart.partVariantType = PartVariantType.pickaxe;
        characterAttributeCustomizationList.Clear();
        characterAttributeCustomizationList.Add(toolPart);
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        // After animation pause
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Reaping_tool:
                    if (playerDirection == Vector3Int.right)
                    {
                        isSwingingToolRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        isSwingingToolLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        isSwingingToolUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        isSwingingToolDown = true;
                    }
                    break;
            }

            // Define centre point of square which will be used for collision testing
            Vector2 point = new Vector2(GetPlayerCentrePosition().x + (playerDirection.x * (equippedItemDetails.itemUseRadius / 2f)), GetPlayerCentrePosition().y + playerDirection.y * (equippedItemDetails.itemUseRadius / 2f));

            // Define size of the square which will be used for collision testing
            Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

            // Get Item components with 2D collider located in the square at the centre point defined (2d colliders tested limited to maxCollidersToTestPerReapSwing)
            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0f);

            int reapableItemCount = 0;

            // Loop through all items retrieved
            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    // Destroy item game object if reapable
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        // Effect position
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f, itemArray[i].transform.position.z);

                        // Trigger reaping effect
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                        //Play sound
                        AudioManager.Instance.PlaySound(SoundName.effectScythe);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        if (reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                            break;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Method processes crop with equipped item in player direction
    /// </summary>
    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection, ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
    {
        switch (equippedItemDetails.itemType)
        {

            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:

                if (playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }
                break;


            case ItemType.Collecting_tool:

                if (playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;

            case ItemType.none:
                break;
        }

        // Get crop at cursor grid location
        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

        // Execute Process Tool Action For crop
        if (crop != null)
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Chopping_tool:
                case ItemType.Breaking_tool:
                    crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown, isUsingToolUp);
                    break;


                case ItemType.Collecting_tool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingDown, isPickingUp);
                    break;
            }
        }
    }

    private void ResetAnimationTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }



    private void ResetMovement()
    {
        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        PlayerInputIsDisabled = true;
        ResetMovement();

        // 发送事件到用于玩家移动输入的任何监听器
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
            isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
             isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
             false, false, false, false);
    }



    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        // Apply base character arms customisation
        armsPart.partVariantType = PartVariantType.none;
        characterAttributeCustomizationList.Clear();
        characterAttributeCustomizationList.Add(armsPart);
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

        isCarrying = false;
    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            // Apply 'carry' character arms customisation
            armsPart.partVariantType = PartVariantType.carry;
            characterAttributeCustomizationList.Clear();
            characterAttributeCustomizationList.Add(armsPart);
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

            isCarrying = true;
        }
    }

    public Vector3 GetPlayerViewportPosition()
    {
        // Vector3 viewport position for player ((0,0) viewport bottom left, (1,1) viewport top right
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public Vector3 GetPlayerCentrePosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCentreYOffset, transform.position.z);
    }

    public void SaveableRegister()
    {
        SaveLoadManager.Instance.saveableObjectList.Add(this);
    }

    public void SaveableDeregister()
    {
        SaveLoadManager.Instance.saveableObjectList.Remove(this);
    }

    public GameObjectSave SaveData()
    {
        // Delete saveScene for game object if it already exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create saveScene for game object
        SceneSave sceneSave = new SceneSave();

        // Create Vector3 Dictionary
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3>();

        //  Create String Dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // Add Player position to Vector3 dictionary
        Vector3 vector3Serializable = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);

        // Add Current Scene Name to string dictionary
        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        // Add Player Direction to string dictionary
        sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());

        // Add sceneSave data for player game object
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }


    public void LoadData(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            // Get save data dictionary for scene
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // Get player position
                if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3 playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                // Get String dictionary
                if (sceneSave.stringDictionary != null)
                {
                    // Get player scene
                    if (sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                    // Get player direction
                    if (sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);

                        if (playerDirFound)
                        {
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                    }
                }
            }
        }
    }

    public void StoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }


    public void RestoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }
}