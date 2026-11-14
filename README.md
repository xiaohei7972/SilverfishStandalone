# SilverfishStandalone

**当前唯一优先目标：把 2026-11-05 经典银鱼完整脱离炉石兄弟主程序，完成独立 .NET 8 编译、原版 CardDB 加载、原版 Ai.autoTester 离线决策、可打包运行和错误处理。**

模型重构已冻结；这一阶段**不改动**原始 Minion、Handcard、Weapon、Playfield、Movegenerator、Ai 或约 1.9 万份卡牌脚本的设计。

本项目不采用 Silverfish V2，也不把 StandaloneGameMod 接入作为当前开发阶段的前置条件。

## 已通过的里程碑

- 旧版经典银鱼参考代码可在 .NET 8 下**独立编译**、加载 CardDB、运行原版 `Ai.autoTester` 并计算出固定牌面中的斩杀攻击。
- 受限 `GameSnapshot JSON → BoardTester → 经典 Ai → BotDecision` 离线端到端回归已通过，相关 ClassicBridge 测试 9/9 通过。见 [Actions #43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029)。
- 新增 `Silverfish.Domain`：实体中心的不可变局面模型，分离卡牌基础定义、战斗属性、费用、武器属性、关键词与扩展标签。对应基础模型单元测试已通过 [Actions #43566986229](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43566986229)。
- 新增 `Silverfish.ClassicModelAdapter`：旧 `Minion`、`Handcard`、`Weapon` 到新模型的**只读状态投影**，用于迁移前差异验证，不再直接把可变旧对象暴露给新状态层。它与经典 AI 同源编译的测试由独立 Actions 工作流运行。

**明确的未完成事项：** 当前公开仓库的自动诊断使用较旧的 `xiaohei7972/Hearthbuddy_backed`，不能代表用户上传的 **2026-11-05** 最新版编译通过。完整新版源码暂未放进 GitHub Actions 可下载的 Release 资产，因此最新版的独立编译和回归尚未验收。新 `Silverfish.Domain` 也尚未取代旧 `Playfield` 以及近两万份旧卡牌模拟脚本。

## 当前阶段：先完成独立化

1. 使用**2026-11-05 正式基线**执行 SHA-256 校验、.NET 8 类库构建与 smoke 测试，不以旧版参考代码通过冒充最新版。
2. 确保 standalone smoke 明确初始化原版通用策略、报告非空 bestmove、无宿主单例调用或异常静默退出。
3. 保留完整源码与卡牌资源，生成能直接在 Windows .NET 8 SDK 下自行编译的独立包；与 GitHub Actions 提供相同检测。
4. **只有上述全部完成后才解冻模型重构。**

见 [独立化优先验收](docs/INDEPENDENCE_FIRST.md)。

## 已暂停的后续重构路线

1. **锁定 2026-11-05 版基线**：SHA-256 校验、GitHub Actions 编译、`Ai.autoTester`、固定决策回归。
2. **更新实体状态模型**：把 `Minion/Handcard/Weapon` 各自堆积的字段拆成实体、战斗、费用、装备、标签和效果组件；用不可变快照避免不同搜索分支共享可变集合。
3. **重构 Playfield 和 Action**：局面分支复制改为显式状态变更，动作保存实体 ID 而非旧 `Minion/Handcard` 对象引用。
4. **逐步迁移卡牌脚本、Movegenerator、MiniSimulator、PenalityManager、Ai**：对比原版黄金局面和动作、性能、模拟结果；绝不一次性删改全部卡牌脚本。

详见 [源码实查与模型重构计划](docs/CLASSIC_MODEL_REFACTOR.md) 和 [独立源码构建说明](docs/CLASSIC_ACTIONS_BUILD.md)。

## 工程结构

- `src/Silverfish.Domain`：**冻结的实验分支代码**，独立化验收前暂不继续开发。
- `tests/Silverfish.Domain.Tests`：新模型的分支隔离、状态不变量和基础功能测试。
- `src/Silverfish.Classic`：按 SHA/选择规则从旧源归档导入的经典银鱼源码编译项目。
- `src/Silverfish.ClassicModelAdapter`：**冻结的实验性只读旧模型投影**，独立化验收前不作为最新版测试门槛。
- `tests/Silverfish.ClassicModelAdapter.Tests`：对实际编译的旧 Minion/Handcard/Weapon 的映射回归。
- `src/Silverfish.Classic.Smoke`：原版 AI 和 CardDB 离线回归程序。
- `src/Silverfish.ClassicBridge`：原版动作到合法候选动作 ID 的映射。
- `src/Silverfish.Contracts/Core/Cli`：先前的离线接口与基础评分演示（**并非**新重构模型或经典 AI 替代品）。
- `src/Silverfish.Integrations.Libmem`：早期独立的进程探测模块，当前重构阶段**暂不投入进一步开发**。

## 最新 2026-11-05 版在 GitHub Actions 构建

已经生成不含炉石兄弟主程序二进制和 V2 源码的独立 ZIP，SHA-256：

`ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c`

新整理的独立运行源码包（原版 Minion/Handcard/Weapon/Playfield 保留，增加 smoke 入口及退出错误修复）SHA-256：

`81f6f19e2888ba5158ea194d6bea30d1f2ad90a7e67e7fd8565ffc7d2bff75e7`

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
