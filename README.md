# SilverfishStandalone

新模拟器按玩家、区域和实体 ID 建模。可以用下面的命令回放从抽牌到亡语的完整样例，并查看最终局面、结算事件和可选动作。

```powershell
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-combat.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-stats.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-equipment.json
dotnet run --project src/Silverfish.Cli -c Release -- plan samples/simulation-combat.json
```

**P0 独立化已通过验收。** 2026-11-05 经典银鱼在 Windows 下独立编译为 .NET 8，加载了原版 `CardDB`，并通过了原版 `Ai.autoTester` 固定牌面、JSON 决策和错误输入回归。可执行文件和日志已归档。[私有 Actions #44256337489](https://github.com/xiaohei7972/Hearthbuddy_Silverfish/actions/runs/44256337489)

本项目不采用 Silverfish V2，也不把 StandaloneGameMod 接入作为当前开发阶段的前置条件。

## 已通过的里程碑

- 旧版经典银鱼参考代码可在 .NET 8 下**独立编译**、加载 CardDB、运行原版 `Ai.autoTester` 并计算出固定牌面中的斩杀攻击。
- 受限 `GameSnapshot JSON → BoardTester → 经典 Ai → BotDecision` 离线端到端回归已通过，相关 ClassicBridge 测试 9/9 通过。见 [Actions #43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029)。
- 新增 `Silverfish.Domain`：实体中心的不可变局面模型，分离卡牌基础定义、战斗属性、费用、武器属性、关键词与扩展标签。对应基础模型单元测试已通过 [Actions #43566986229](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43566986229)。
- 新增 `Silverfish.ClassicModelAdapter`：旧 `Minion`、`Handcard`、`Weapon` 到新模型的**只读状态投影**，用于迁移前差异验证，不再直接把可变旧对象暴露给新状态层。它与经典 AI 同源编译的测试由独立 Actions 工作流运行。
- 新模拟器已支持基础战斗、抽牌、亡语、附魔、位置光环和沉默，以及武器装备、耐久结算和基础英雄技能。`starter` 的 29 项卡牌、衍生物和技能用新的通用结算操作实现。

## 后续重构路线

1. 以已验收的 2026-11-05 版及固定牌面决策作为回归基线。
2. 用 `GameState`、玩家区域和实体 ID 表示新局面，动作由新模拟器直接执行。
3. 用通用结算操作重写卡牌效果，按机制扩充回放覆盖。经典引擎和脚本作为历史回归参考。
4. 在新模拟器上扩展搜索和估值，用固定局面核对行动、结算轨迹和性能。

详见 [源码实查与模型重构计划](docs/CLASSIC_MODEL_REFACTOR.md) 和 [独立源码构建说明](docs/CLASSIC_ACTIONS_BUILD.md)。

## 工程结构

- `src/Silverfish.Domain`：新模拟器的不可变局面与实体模型。
- `src/Silverfish.Simulation`：合法动作生成、规则结算和卡牌定义。
- `tests/Silverfish.Domain.Tests`：新模型的分支隔离、状态不变量和基础功能测试。
- `src/Silverfish.Classic`：按 SHA/选择规则从旧源归档导入的经典银鱼源码编译项目。
- `src/Silverfish.ClassicModelAdapter`：实验性的只读旧模型投影。
- `tests/Silverfish.ClassicModelAdapter.Tests`：对实际编译的旧 Minion/Handcard/Weapon 的映射回归。
- `src/Silverfish.Classic.Smoke`：原版 AI 和 CardDB 离线回归程序。
- `src/Silverfish.ClassicBridge`：原版动作到合法候选动作 ID 的映射。
- `src/Silverfish.Contracts`：离线输入与决策接口。
- `src/Silverfish.Core`：基础决策接口实现和新模拟器的独立搜索与评分。
- `src/Silverfish.Cli`：离线决策、模拟回放和回合搜索入口。
- `src/Silverfish.Integrations.Libmem`：早期独立的进程探测模块，当前重构阶段**暂不投入进一步开发**。

## 最新 2026-11-05 版在 GitHub Actions 构建

私有工作流使用 `ClassicSilverfish_2026-11-05_Standalone.zip`，SHA-256 为 `4d6944304c0d224d366afda1691f0429c5d3cffbc0daae18259310a7f6f95d92`。公开仓库的自动诊断继续使用旧版参考源码。

固定牌面返回 `attackWithMinion`（实体 11 攻击实体 1），快照返回 `attack-face`。无效牌面输出诊断并返回退出码 2。阿格拉玛的 4 个效果脚本仍为空实现，涉及这些牌的 AI 评估可能有偏差。[完整验收记录与复现方法](https://github.com/xiaohei7972/SilverfishStandalone/blob/feat/standalone-foundation/docs/INDEPENDENCE_FIRST.md)

## 快速回归

```powershell
dotnet test tests/Silverfish.Domain.Tests/Silverfish.Domain.Tests.csproj -c Release
python tools/test_import_classic.py
python tools/test_normalize_classic.py
```

真实 `Silverfish.Classic` 及其迁移适配器须先导入对应经典源文件，再在 `classic-compile.yml` 中完整验证。
