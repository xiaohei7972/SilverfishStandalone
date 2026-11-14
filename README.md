# SilverfishStandalone

**目标：在不依赖炉石兄弟主程序的前提下，逐步重构经典银鱼 AI 自己的实体模型、局面模拟、卡牌效果与搜索引擎。**

本项目不采用 Silverfish V2，也不把 StandaloneGameMod 接入作为当前开发阶段的前置条件。

## 已通过的里程碑

- 旧版经典银鱼参考代码可在 .NET 8 下**独立编译**、加载 CardDB、运行原版 `Ai.autoTester` 并计算出固定牌面中的斩杀攻击。
- 受限 `GameSnapshot JSON → BoardTester → 经典 Ai → BotDecision` 离线端到端回归已通过，相关 ClassicBridge 测试 9/9 通过。见 [Actions #43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029)。
- 新增 `Silverfish.Domain`：实体中心的不可变局面模型，分离卡牌基础定义、战斗属性、费用、武器属性、关键词与扩展标签。对应基础模型单元测试已通过 [Actions #43566986229](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43566986229)。
- 新增 `Silverfish.ClassicModelAdapter`：旧 `Minion`、`Handcard`、`Weapon` 到新模型的**只读状态投影**，用于迁移前差异验证，不再直接把可变旧对象暴露给新状态层。它与经典 AI 同源编译的测试由独立 Actions 工作流运行。

**明确的未完成事项：** 当前公开仓库的自动诊断使用较旧的 `xiaohei7972/Hearthbuddy_backed`，不能代表用户上传的 **2026-11-05** 最新版编译通过。完整新版源码暂未放进 GitHub Actions 可下载的 Release 资产，因此最新版的独立编译和回归尚未验收。新 `Silverfish.Domain` 也尚未取代旧 `Playfield` 以及近两万份旧卡牌模拟脚本。

## 当前重构路线

1. **锁定 2026-11-05 版基线**：SHA-256 校验、GitHub Actions 编译、`Ai.autoTester`、固定决策回归。
2. **更新实体状态模型**：把 `Minion/Handcard/Weapon` 各自堆积的字段拆成实体、战斗、费用、装备、标签和效果组件；用不可变快照避免不同搜索分支共享可变集合。
3. **重构 Playfield 和 Action**：局面分支复制改为显式状态变更，动作保存实体 ID 而非旧 `Minion/Handcard` 对象引用。
4. **逐步迁移卡牌脚本、Movegenerator、MiniSimulator、PenalityManager、Ai**：对比原版黄金局面和动作、性能、模拟结果；绝不一次性删改全部卡牌脚本。

详见 [源码实查与模型重构计划](docs/CLASSIC_MODEL_REFACTOR.md) 和 [独立源码构建说明](docs/CLASSIC_ACTIONS_BUILD.md)。

## 工程结构

- `src/Silverfish.Domain`：新不可变实体与局面核心，**不引用** Hearthbuddy、Triton 或旧 Silverfish。
- `tests/Silverfish.Domain.Tests`：新模型的分支隔离、状态不变量和基础功能测试。
- `src/Silverfish.Classic`：按 SHA/选择规则从旧源归档导入的经典银鱼源码编译项目。
- `src/Silverfish.ClassicModelAdapter`：只读旧模型投影，辅助逐步迁移。
- `tests/Silverfish.ClassicModelAdapter.Tests`：对实际编译的旧 Minion/Handcard/Weapon 的映射回归。
- `src/Silverfish.Classic.Smoke`：原版 AI 和 CardDB 离线回归程序。
- `src/Silverfish.ClassicBridge`：原版动作到合法候选动作 ID 的映射。
- `src/Silverfish.Contracts/Core/Cli`：先前的离线接口与基础评分演示（**并非**新重构模型或经典 AI 替代品）。
- `src/Silverfish.Integrations.Libmem`：早期独立的进程探测模块，当前重构阶段**暂不投入进一步开发**。

## 最新 2026-11-05 版在 GitHub Actions 构建

已经生成不含炉石兄弟主程序二进制和 V2 源码的独立 ZIP，SHA-256：

`ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c`

工作流支持 `owner-classic-work-package`，从已上传的 Release 资产核验 SHA-256，然后编译并跑原版 AI 离线回归与模型迁移单测。

如你选择把源码包发布在当前**公开**仓库的 Release 中，可用 [校验和上传脚本](tools/publish_owner_classic_source.ps1)（需要 `gh` 已登录；显式传入 `-ConfirmPublic`）：

```powershell
pwsh ./tools/publish_owner_classic_source.ps1 -ArchivePath "C:\path\ClassicSilverfish_2026-11-05_独立源码工作包.zip" -ConfirmPublic
```

脚本会核验 ZIP，上传指定 Release 资产并启动 `classic-compile.yml`。不希望公开源码时不要执行此命令，应改用私有仓库或仅本地编译。

## 快速回归

```powershell
dotnet test tests/Silverfish.Domain.Tests/Silverfish.Domain.Tests.csproj -c Release
python tools/test_import_classic.py
python tools/test_normalize_classic.py
```

真实 `Silverfish.Classic` 及其迁移适配器须先导入对应经典源文件，再在 `classic-compile.yml` 中完整验证。
