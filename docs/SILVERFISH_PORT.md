# 经典银鱼独立化技术计划

## 真实源码基准

以用户上传的 `Hearthbuddy1.7.1(1).zip` 为准，压缩包目录为 `炉石兄弟-2026.11.05`，SHA-256 详见 `docs/SOURCE_BASELINE.md`。不使用 V2 代码，不以任何旧公开 fork 覆盖本次提供的源码。

## 现有工作（已实际完成）

- 从源码包筛选 19,166 个经典银鱼相关源码与文本资源，其中 19,135 个 C# 文件。
- 以 `tools/import_classic.py` 做可重复、有哈希约束的来源选择和接口调整；测试工具已运行通过。
- 已建立 `src/Silverfish.Classic/Silverfish.Classic.csproj` 和 smoke runner。
- `ClassicCompatibility.cs` 提供新的日志、行为路径、Entity tag 和 Mulligan 输入边界。
- 隔离原版输入操作扩展、弃用的旧主程序 Zone 包装，以及四个直接读取 live GameState 的模拟脚本。
- 原主程序和原生 DLL 不参与构建；工作包仅包含纯文本源码和数据。
- **尚未完成** 对这批新导入经典源码的 .NET 编译、测试局面决策和实时状态适配。

## 逐项拆解（严格依赖顺序）

### 1. 原始经典源码编译

`Ai.cs`, `Hrtprozis.cs`, `Playfield/*.cs`, `CardDB/*.cs`, `Handmanager.cs`, `Movegenerator.cs`, `Minion.cs`, `Weapon.cs`, `MiniSimulator.cs` 以及卡牌脚本属于同一模拟核心，不能只复制 `Ai.cs` 就宣称 AI 独立。先运行 `dotnet build src/Silverfish.Classic/Silverfish.Classic.csproj`，按真实编译错误修复；如果有无法独立的逻辑，明确隔离而不是引用 Triton 临时过关。

### 2. 原版 BoardTester 离线回归

使用 `Silverfish.Classic.Smoke`（`Ai.Instance.autoTester`）加载一份可复现的板面测试文本，证明经典算法本身计算完成、得到 `bestmove`，记录输出和耗时。此阶段不需要游戏或 UI。

### 3. 可复现的状态转换

转换 `StandaloneGameMod state.snapshot` 为完整银鱼输入（`Hrtprozis`、`Handmanager`、`miniEnch` 的元数据等）。必须处理卡牌 ID、实体 ID、装备、光环、奥秘、墓地、手牌/牌库数量、回合标签以及尚未知的字段。数据不完整时明确报告，而非以默认值生成不可信决策。

### 4. 动作映射与安全验证

将 `Ai.bestmove` 转换成当前项目 `BotDecision`，每个决策绑定 matchId 和 sequence。只允许来自已确认合法候选动作的推荐，并在下一份快照变化后重算；不将旧点击/Hook 代码混入算法。

### 5. Libmem.NET 与 UI

Libmem.NET 保留为独立进程数据能力，不能代替银鱼完整局面语义。新 UI（Vue 3 + WebView2）显示局面、AI 计算步骤、候选动作、日志和回放。游戏交互在单独边界实现，不属于 `Silverfish.Classic`。

## 验收标准

```
一份离线牌面 -> 经典 Ai 实际计算 -> 可复现 bestmove
一份有效 state.snapshot -> 独立适配器 -> 经典 Ai -> BotDecision
```

上面的第一条目前还没有实际通过；仓库中的 BaselineStrategyEngine 只是架构演示，不能作为经典银鱼算法的替代完成标志。
