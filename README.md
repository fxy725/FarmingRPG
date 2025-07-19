# 🌾 Farming RPG - Unity 2D 农场模拟游戏

一个使用 Unity 6000.0.43f1 开发的全功能 2D 农场模拟 RPG 游戏，包含完整的农场经营、NPC 交互、物品管理和存档系统。

## 🎮 游戏特色

### 🌱 农场经营系统
- **作物种植与收获**：完整的作物生长周期，支持季节性种植
- **多样化工具**：锄头、浇水壶、镰刀、斧头、十字镐等农场工具
- **土地管理**：可挖掘、浇水、种植的动态地块系统
- **收获动画**：丰富的作物收获特效和动画

### 🎒 物品与库存系统
- **物品分类**：种子、商品、工具、家具等多种物品类型
- **拖拽界面**：直观的物品拖拽和管理系统
- **物品详情**：每个物品都有详细的描述和用途
- **存储容量**：可扩展的库存容量系统

### 🤖 智能 NPC 系统
- **路径寻找**：基于 A* 算法的智能导航系统
- **日程安排**：NPC 有自己的日常作息和行为模式
- **场景切换**：NPC 可以在不同场景间移动和交互
- **动画系统**：丰富的角色动画和表情系统

### ⏰ 时间与天气系统
- **动态时间**：实时的年、季、日、时、分系统
- **季节变化**：春夏秋冬的循环，影响作物生长
- **天气系统**：晴天、雨天、雪天等不同天气状态
- **游戏时钟**：可暂停的游戏时间机制

### 💾 完整存档系统
- **JSON 序列化**：使用 Newtonsoft.Json 进行数据序列化
- **场景数据保存**：每个场景的物品、NPC、作物状态都可保存
- **GUID 系统**：每个可保存对象都有唯一标识符
- **自动保存**：支持手动和自动存档功能

## 🛠️ 技术栈

### 核心引擎
- **Unity Version**: 6000.0.43f1 (Unity 6)
- **Target Framework**: .NET Standard 2.1
- **Language Version**: C# 9.0
- **Platform**: Windows Standalone (可扩展到其他平台)

### Unity 包依赖

#### 2D 渲染与动画
```json
"com.unity.2d.animation": "10.1.4"     // 2D 骨骼动画系统
"com.unity.2d.pixel-perfect": "5.0.3"  // 像素完美渲染
"com.unity.2d.sprite": "1.0.0"         // 2D 精灵系统
"com.unity.2d.spriteshape": "10.0.7"   // 2D 形状工具
"com.unity.2d.tilemap": "1.0.0"        // 瓦片地图系统
"com.unity.2d.psdimporter": "9.0.3"    // PSD 文件导入器
```

#### 相机与视觉效果
```json
"com.unity.cinemachine": "2.10.3"      // 智能相机系统
"com.unity.timeline": "1.8.7"          // 时间线和序列器
```

#### AI 与导航
```json
"com.unity.ai.navigation": "2.0.6"     // AI 导航网格系统
```

#### 用户界面
```json
"com.unity.ugui": "2.0.0"              // Unity GUI 系统
```

#### 数据处理
```json
"com.unity.nuget.newtonsoft-json": "3.2.1"  // JSON 序列化库
```

#### 开发工具
```json
"com.unity.ide.visualstudio": "2.0.23" // Visual Studio 集成
"com.unity.ide.rider": "3.0.31"        // JetBrains Rider 集成
"com.unity.test-framework": "1.4.6"    // 单元测试框架
```

## 📁 项目结构

### 脚本架构
```
Assets/Scripts/
├── Animation/          # 动画控制系统
├── AStar/             # A* 路径寻找算法
├── Crop/              # 作物生长系统
├── Enums/             # 枚举定义
├── Events/            # 事件系统
├── GameManager/       # 游戏管理器
├── HelperClasses/     # 辅助工具类
├── Inventory/         # 库存管理系统
├── Item/              # 物品系统
├── Map/               # 地图网格系统
├── Misc/              # 杂项工具
├── NPC/               # NPC 行为系统
├── Player/            # 玩家控制器
├── SaveSystem/        # 存档系统
├── Scene/             # 场景管理
├── Sounds/            # 音频管理
├── TimeSystem/        # 时间系统
├── UI/                # 用户界面
├── Utilities/         # 实用工具
└── VFX/               # 视觉特效
```

### 资源组织
```
Assets/
├── Animation/         # 动画文件
│   ├── Crop/         # 作物动画
│   ├── NPC/          # NPC 动画
│   └── Player/       # 玩家动画
├── Fonts/            # 字体资源 (LXGWMarkerGothic)
├── Prefabs/          # 预制体
│   ├── Crop/         # 作物预制体
│   ├── Item/         # 物品预制体
│   ├── NPC/          # NPC 预制体
│   └── UI/           # UI 预制体
├── Scenes/           # 游戏场景
│   ├── PersistentScene.unity    # 持久化场景
│   ├── Scene1_Farm.unity        # 农场场景
│   ├── Scene2_Field.unity       # 田野场景
│   └── Scene3_Cabin.unity       # 小屋场景
├── Sounds/           # 音频资源
│   ├── Effects/      # 音效
│   ├── Music/        # 背景音乐
│   └── AmbientSounds/ # 环境音
├── Sprites/          # 图像资源
└── Tilemap/          # 瓦片地图资源
```

## 🏗️ 核心系统架构

### 单例模式管理器
项目广泛使用单例模式来管理核心系统：
- `GameManager` - 游戏主管理器
- `InventoryManager` - 库存管理
- `GridPropertiesManager` - 网格属性管理
- `NPCManager` - NPC 管理
- `TimeManager` - 时间管理
- `UIManager` - UI 管理
- `SaveLoadManager` - 存档管理

### 事件驱动架构
使用 `EventHandler` 实现松耦合的事件系统：
```csharp
// 时间推进事件
EventHandler.AdvanceGameDayEvent += AdvanceDay;
// 场景加载事件
EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
// 库存更新事件
EventHandler.InventoryUpdatedEvent += InventoryUpdated;
```

### 数据持久化
实现 `ISaveable` 接口的对象可以自动保存：
```csharp
public interface ISaveable
{
    string ISaveableUniqueID { get; set; }
    GameObjectSave GameObjectSave { get; set; }
    void SaveableRegister();
    void SaveableDeregister();
    GameObjectSave SaveData();
    void LoadData(GameSave gameSave);
}
```

### 网格化地图系统
使用 Unity Tilemap 系统实现：
- 可挖掘土地检测
- 作物种植位置管理
- NPC 导航阻挡
- 物品放置验证

## 🎯 核心功能实现

### 作物生长系统
```csharp
// 作物详情配置
[System.Serializable]
public class CropDetails
{
    public int seedItemCode;           // 种子物品代码
    public int[] growthDays;          // 生长阶段天数
    public GameObject[] growthPrefab;  // 生长阶段预制体
    public Season[] seasons;          // 适宜生长季节
    public HarvestActionEffect harvestActionEffect; // 收获特效
}
```

### A* 寻路算法
为 NPC 实现智能导航：
```csharp
public class AStar : MonoBehaviour
{
    // 构建从起点到终点的路径
    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, 
                         Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
}
```

### 物品系统
支持多种物品类型和属性：
```csharp
public enum ItemType
{
    Seed,              // 种子
    Commodity,         // 商品
    Watering_tool,     // 浇水工具
    Hoeing_tool,       // 锄地工具
    Chopping_tool,     // 砍伐工具
    Breaking_tool,     // 破坏工具
    Reaping_tool,      // 收割工具
    Collecting_tool,   // 收集工具
    Furniture          // 家具
}
```

## 🎨 美术特色

- **像素艺术风格**：经典的 2D 像素艺术设计
- **中文本地化**：完整的中文界面和字体支持
- **丰富动画**：角色、作物、工具使用动画
- **季节主题**：不同季节的视觉表现
- **粒子特效**：收获、天气、工具使用特效

## 🔧 开发环境

### 推荐配置
- **Unity 版本**: 6000.0.43f1 或更高
- **IDE**: Visual Studio 2022 或 JetBrains Rider
- **操作系统**: Windows 10/11 (主要开发平台)
- **.NET**: .NET Standard 2.1 兼容

### 构建设置
- **目标平台**: Windows Standalone
- **架构**: x64
- **脚本后端**: IL2CPP
- **API 兼容级别**: .NET Standard 2.1

## 📈 性能优化

- **对象池**：`PoolManager` 管理可重用对象
- **LOD 系统**：根据距离调整细节级别
- **纹理压缩**：优化的精灵图集打包
- **音频压缩**：高效的音频压缩设置
- **场景分割**：多场景架构减少内存占用

## 🚀 快速开始

1. **环境准备**
   ```bash
   # 确保安装 Unity 6000.0.43f1
   # 克隆项目到本地
   git clone https://github.com/fxy725/Farming-RPG.git
   ```

2. **打开项目**
   - 使用 Unity Hub 打开项目文件夹
   - 等待项目导入完成

3. **运行游戏**
   - 打开 `PersistentScene` 场景
   - 点击播放按钮开始游戏

## 📄 许可证

本项目仅供学习和研究使用。

---

**开发者**: fxy725  
**引擎版本**: Unity 6000.0.43f1  
**最后更新**: 2025年1月  

> 这是一个展示现代Unity 2D游戏开发技术的完整项目，包含了从基础系统到高级功能的全面实现。 