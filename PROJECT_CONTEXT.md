# PROJECT_CONTEXT.md —— 项目交接说明

> 这份文档是给「下一个 AI / 协作者」看的入口。读完它就能理解项目现状、继续开发，无需重读全部历史对话。
> 更细的每日记录在 `.workbuddy/memory/`（按日期），需要时再翻。

---

## 一、项目是什么

**HalconMeasure**（解决方案 `Measure.Halcon.Demo.slnx`，项目 `Measure.Halcon.Demo`）——基于 HALCON 的**视觉检测上位机**。

完整业务流：

```
读本地图像 / 相机采图
   → 创建模板（画矩形区域，做形状匹配）
   → 画圆 ROI（卡尺取边缘点）
   → 拟合圆（得到像素半径/圆心）
   → 标定（像素当量 = 实际直径mm ÷ 像素直径px，一次性）
   → 测量（模板匹配定位 + ROI 跟随 + 拟合圆 + 判直径是否在范围内 → OK/NG）
```

**一句话**：模板匹配负责「找到零件」，圆卡尺负责「量直径」，标定负责「像素换算成毫米」。

---

## 二、技术栈与环境

| 项 | 值 |
|---|---|
| 语言/框架 | C# / WinForms，**.NET Framework 4.8**（老式 csproj + `packages.config`，**非 SDK 风格**） |
| 视觉库 | HALCON 25.11（`halcondotnet.dll`） |
| 日志 | Serilog 4.4.0 + Serilog.Sinks.File 7.0.0 |
| 配置 | ini-parser-netstandard 2.5.3（`INIFileParser.dll`，命名空间 `IniParser` / `IniParser.Model`） |
| 语言版本 | 默认 C# 7.3（别用 C# 8+ 语法，如 nullable、switch 表达式、`var x = 方法名` 方法组推断会报 CS8370） |

**编译验证命令**（重要，`dotnet build` 会因 resx 二进制资源报 MSB3822）：

```bash
cd Measure.Halcon.Demo/Measure.Halcon.Demo
dotnet build -t:"ResolveReferences;CoreCompile"
```

只编译代码、跳过资源生成，能真实校验语法/类型。VS 里 F5 正常。

---

## 三、文件结构与职责

```
Measure.Halcon.Demo/
├── Program.cs             Serilog 初始化 + Main（try/catch/finally）
├── FormMain.cs            主窗体：全部 UI 事件 + 参数字段 + DataBindings + 业务编排
├── FormMain.Designer.cs   窗体控件（VS 生成）
├── OperationResult.cs     结果对象（IsSuccess/Msg/Error + 工厂方法）
├── MatchParams.cs         模板匹配参数（原 MathParams，已改名 MatchParams）
├── CircleParams.cs        圆测量参数
└── Helper/
    ├── HalconHelper.cs    HALCON 算子封装（HalconInitial/draw_spoke3/spoke/pts_to_best_circle/Fit_Circle/disp_message）
    ├── ImageHelper.cs     相机（OpenCam/CloseCam/GrabImage/ReadImage/SaveImage/Get·Set 参数）
    ├── ShapeModelHelper.cs 模板（CreateShapeModel/FindShapeMode/ImportShapeModel/SaveShapeModel/ClearShapeModel）
    ├── LogHelper.cs       日志（Serilog 写文件 + ListView 刷界面）
    └── IniConfigHelper.cs 【已废弃】旧 P/Invoke INI 读写（thinger 教程抄的），已改用 ini-parser
```

---

## 四、核心对象与设计（理解现状的关键）

### 1. OperationResult（结果模式）

```csharp
public class OperationResult {
    public bool IsSuccess; public string Msg; public Exception Error;
    static CreateSuccessResult(msg) / CreateFailResult(msg, ex = null)
}
```

**铁律**：意料之内的失败用 Result（模板没匹配上、边缘点太少），意料之外的崩溃用异常（空引用、HALCON 异常）。`Error != null` 是「技术故障」与「业务失败」的分界。

### 2. 日志系统（已收敛完成）

`LogHelper` 对外只有 5 个方法：

```csharp
Info(string) / Warn(string) / Error(string)     // 事件型（无 result 的日志）
Result(OperationResult)                          // 动作型：成功→Info，失败→(有ex?Error:Warn)
Fail(OperationResult)                            // 静默型：只记失败，成功不记（连续采集/调参）
```

- **级别三分**：成功 Info / 业务失败(无 ex) Warn / 技术故障(有 ex) Error。
- **原则**：内层（Helper）不记日志、只返回 Result；日志在「边界」（按钮事件）用 `log.Result(result)` 记一次。
- 写文件走 Serilog（线程安全），刷 UI 走 `BeginInvoke` 封送（不是同步 Invoke，避免被模态框卡死）。

### 3. 参数 + UI 绑定（DataBindings，MVVM 雏形）

- `MatchParams` / `CircleParams` 里**要绑 UI 的字段已改成属性**（`{ get; set; }`）。DataBinding 只认属性、不认字段。
- `FormMain.BindParams()` 在构造函数里用 `BindDouble/BindInt` 把 TextBox 和参数双向绑定。
- **两个方向不对称**：控件→数据源（输入）自动；数据源→控件（代码改属性）**需要 INotifyPropertyChanged，目前没实现**，所以加载参数后要手动 `RefreshBindings(this)`（递归 `Binding.ReadValue()`）。
- ComboBox 用 `Enum.GetNames` 填字符串 + `SelectedItem` 选中（**不要用 DataSource 绑枚举对象**，Text 会被数据绑定接管、显示不可靠）。

### 4. 参数持久化（ini + HALCON 文件）

`SaveShapeModel(path, ...)` 保存：`param.ini`（数值参数，ini-parser）+ `model.she`（WriteShapeModel）+ `model_region.tif`（WriteRegion）+ `roi_row/col/dir.tup`（WriteTuple）。

`ImportShapeModel(path, ...)` 读取：先 `ClearShapeModel` 释放旧句柄 → `ReadShapeModel`/`ReadRegion`/`ReadTuple` → 赋回参数。

**注意**：`circleRadius`/圆心/距离是「运行时结果」，**不保存**——导入后它是 null，标定前必须先现场拟合圆。

### 5. 仿射变换（ROI 跟随）

`FindShapeMode` 里算 `homMat2d_T = VectorAngleToRigid(row1, col1, 0, row, col, angle)`，`Fit_Circle` 里用 `AffineTransPoint2d` 把 ROI 点搬到匹配位置。**方向是「旧参考点 → 新匹配点」+ 带旋转 angle**（曾写反 + 丢 angle，已修）。

**坐标约定**：HALCON `Row`=行=纵向(y)，`Column`=列=横向(x)。本项目里 `circleParams.rOI_Y` 存行、`rOI_X` 存列（命名没反，别去"修正"）。所有 HALCON 角度都是**弧度**。

---

## 五、已完成的重构（按时间，便于理解代码现状）

1. **Serilog 接入 + LogHelper 抽取**（独立类，33 处调用点收敛）
2. **OperationResult 模式** + 全项目 catch 记录完整异常（带栈，不只 Message）
3. **日志收敛**：`Result(result)` / `Fail(result)` 统一入口，删掉散落的 if-else 和级别硬编码
4. **ini 配置**：用 `ini-parser-netstandard` 替换旧的 `IniConfigHelper`（P/Invoke，中文易乱码、命名空间还是 `thinger.HalconDemo`）
5. **参数绑定**：字段→属性 + DataBindings，删掉手写 Load/Initial 同步
6. **参数保存/加载**：SaveShapeModel / ImportShapeModel 完整实现
7. **仿射变换修正**：ROI 跟随匹配

---

## 六、待办（下一步方向，按优先级）

从 `项目问题.md` + 历史遗留：

1. **全局异常兜底**：`Program.cs` 加 `Application.ThreadException` + `AppDomain.UnhandledException`（已讲解，未落地）。单按钮出错现在会崩整个程序。
2. **UI 数据没实现 INotifyPropertyChanged**：现在加载参数要手动 `RefreshBindings`。彻底方案是参数类实现 `INotifyPropertyChanged` 或用 `CommunityToolkit.Mvvm`（但旧式 csproj + C#7.3 上源生成器有摩擦）。
3. **检测结果不包含是哪张图**：测量结果没关联到具体图像文件名。
4. **系统检测精度问题**：待分析（像素当量、卡尺参数、亚像素精度）。
5. **权限系统**：未开始。
6. **「保存/加载参数」菜单事件**：`SaveShapeModel`/`ImportShapeModel` 已实现，但 Designer 里「文件→保存/加载」菜单项（控件名有 `保存ToolStripMenuItem`/`保存ToolStripMenuItem1` 的错位）事件还没接完。

**历史小坑（低优先级）**：
- `TrackBar`（曝光/增益）高度被系统主题锁定，要套 Panel。
- `ShapeModelHelper.FindShapeMode` 每次 new 3 个 HObject 不 Dispose（连续查找会累积）。
- `ImageHelper.ReadImage(out hImage)` 直接覆盖字段，旧 HObject 不 Dispose。
- `ImageHelper.GetCamParamDouble` 读不到返回 0（吞异常，应该返回 Result）。

---

## 七、用户编程习惯与协作约定（务必遵守）

- **教学型协作**：用户是学习者，偏好「先讲原理 → 再动手」。不要一上来就大段改代码。
- **先读实际代码再回答**：用户会质疑「你读我代码了吗」，回答前务必用 Read/Grep 看真实代码，别凭记忆或旧行号（行号会因他手动改代码而漂移）。
- **最小改动**：能小改不大改；说清楚「改了哪些、为什么」。
- **列出改动清单**：改完要总结「改了哪些文件、哪几处」。
- **有些地方用户要自己写**：他会说「改过来我自己写」，此时不要代劳。
- **中文交流**。
- **重视规范/可维护性**：日志一致性、异常处理、命名、MVVM 都想做规范，会追根究底问「为什么」「是不是主流」。

---

## 八、构建与环境坑（避免重蹈覆辙）

- `dotnet build` 报 MSB3822/MSB3823（resx 含 Icon/Color 二进制资源）→ 用 `dotnet build -t:"ResolveReferences;CoreCompile"` 绕过。
- 老式 csproj 新增 .cs 文件**不会自动编译**，要手动在 csproj 的 `<ItemGroup>` 加 `<Compile Include="..." />`。
- 写文件工具（Edit/Write）对带「隐藏」属性的文件报 EPERM，先 `attrib -h` 去掉隐藏。
- `App.config` 里的 bindingRedirect 是 NuGet 自动写的，**别手删**（否则 FileLoadException）。
- Serilog 日志相对路径以工作目录为基准，必须用 `Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app-.log")` 绝对路径。
- 中文 Windows 代码页 936，控制台中文经管道会乱码。
- 本机 PowerShell 工具会拦截含 `cmd`/`Add-Type`/`MSBuild` 字样的命令。

---

## 九、用户反复关心的核心概念（面试/深入都会问到）

- **日志三分法**：成功 Info / 业务失败 Warn / 技术故障 Error，靠 `Error==null` 区分。
- **Result vs 异常的分工**：意料之内 vs 意料之外。
- **Result vs Fail**：成功值不值得记（动作型 vs 静默型）。
- **一致性四维度**：文案/级别/结构/决策位置。
- **MVVM 与 DataBindings**：参数类 + UI 绑定，数据源→控件刷新需 INotifyPropertyChanged。
- **卫语句链**：多步骤流程（Measure = 匹配→拟合→判直径）失败立即 return + 前缀累加。
- **仿射变换**：VectorAngleToRigid 算矩阵、AffineTransXXX 执行搬运、角度一律弧度。
- **短路求值**：null 检查要放 `||`/`&&` 最前面。
- **typeof / nameof / GetType** 区别。

---

*文档最后更新：2026-11-02*
