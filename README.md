# SilverfishStandalone

**P0 独立化已通过验收。** 2026-11-05 经典银鱼在 Windows 下独立编译为 .NET 8，加载了原版 `CardDB`，并通过了原版 `Ai.autoTester` 固定牌面、JSON 决策和错误输入回归。可执行文件和日志已归档。[私有 Actions #44256337489](https://github.com/xiaohei7972/Hearthbuddy_Silverfish/actions/runs/44256337489)

本项目不采用 Silverfish V2，也不把 StandaloneGameMod 接入作为当前开发阶段的前置条件。

## 已通过的里程碑

- 旧版经典银鱼参考代码可在 .NET 8 下**独立编译**、加载 CardDB、运行原版 `Ai.autoTester` 并计算出固定牌面中的斩杀攻击。
- 受限 `GameSnapshot JSON → BoardTester → 经典 Ai → BotDecision` 离线端到端回归已通过，相关 ClassicBridge 测试 9/9 通过。见 [Actions #43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029)。
- 新增 `Silverfish.Domain`：实体中心的不可变局面模型，分离卡牌基础定义、战斗属性、费用、武器属性、关键词与扩展标签。对应基础模型单元测试已通过 [Actions #43566986229](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43566986229)。
- 新增 `Silverfish.ClassicModelAdapter`：旧 `Minion`、`Handcard`、`Weapon` 到新模型的**只读状态投影**，用于迁移前差异验证，不再直接把可变旧对象暴露给新状态层。它与经典 AI 同源编译的测试由独立 Actions 工作流运行。

## 后续重构路线

1. 以已验收的 2026-11-05 版及固定牌面决策作为回归基线。
2. **更新实体状态模型**：把 `Minion/Handcard/Weapon` 各自堆积的字段拆成实体、战斗、费用、装备、标签和效果组件；用不可变快照避免不同搜索分支共享可变集合。
3. **重构 Playfield 和 Action**：局面分支复制改为显式状态变更，动作保存实体 ID 而非旧 `Minion/Handcard` 对象引用。
4. **逐步迁移卡牌脚本、Movegenerator、MiniSimulator、PenalityManager、Ai**：对比原版黄金局面和动作、性能、模拟结果；绝不一次性删改全部卡牌脚本。

详见 [源码实查与模型重构计划](docs/CLASSIC_MODEL_REFACTOR.md) 和 [独立源码构建说明](docs/CLASSIC_ACTIONS_BUILD.md)。

## 工程结构

- `src/Silverfish.Domain`：实验性的不可变实体模型，供后续迁移使用。
- `tests/Silverfish.Domain.Tests`：新模型的分支隔离、状态不变量和基础功能测试。
- `src/Silverfish.Classic`：按 SHA/选择规则从旧源归档导入的经典银鱼源码编译项目。
- `src/Silverfish.ClassicModelAdapter`：实验性的只读旧模型投影。
- `tests/Silverfish.ClassicModelAdapter.Tests`：对实际编译的旧 Minion/Handcard/Weapon 的映射回归。
- `src/Silverfish.Classic.Smoke`：原版 AI 和 CardDB 离线回归程序。
- `src/Silverfish.ClassicBridge`：原版动作到合法候选动作 ID 的映射。
- `src/Silverfish.Contracts/Core/Cli`：先前的离线接口与基础评分演示（**并非**新重构模型或经典 AI 替代品）。
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
