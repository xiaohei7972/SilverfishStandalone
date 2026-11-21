# SilverfishStandalone

## 构建与发布

使用 .NET 8 SDK 构建新模拟器和 CLI，并运行独立模拟器的测试。

```powershell
dotnet build SilverfishStandalone.sln -c Release
dotnet test SilverfishStandalone.sln -c Release --no-build
```

发布内含 .NET 运行时的 Windows x64 单文件可执行程序。

```powershell
dotnet publish src/Silverfish.Cli/Silverfish.Cli.csproj -c Release -p:PublishProfile=WindowsX64 -o artifacts/standalone-win-x64
.\artifacts\standalone-win-x64\Silverfish.Cli.exe plan samples/simulation-random.json
```

## 回放样例

新模拟器按玩家、区域和实体 ID 建模。可以用下面的命令回放从抽牌到亡语的完整样例，并查看最终局面、结算事件和可选动作。

```powershell
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-combat.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-stats.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-equipment.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-lifecycle.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-combat-keywords.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-secrets.json
dotnet run --project src/Silverfish.Cli -c Release -- plan samples/simulation-random.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-mana.json
dotnet run --project src/Silverfish.Cli -c Release -- plan samples/simulation-choices.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-conditions.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-cost-auras.json
dotnet run --project src/Silverfish.Cli -c Release -- plan samples/simulation-combat.json
```

## 卡牌数据

客户端的 `GAME_TAG`、`TAG_ZONE`、`TAG_RACE`、`TAG_SPELL_SCHOOL`、`TAG_CARDTYPE`、`TAG_CARD_ALTERNATE_COST` 和 `TAG_CARD_SET` 从客户端源码生成。更新客户端版本时先同步枚举，再生成卡牌数据。加 `--check` 可核对当前枚举。

```powershell
$client = 'D:/script/hbCollectionhb/炉石传说源代码/2026-10-13/Assembly-CSharp'
python tools/generate_client_enums.py $client
```

构建 `256589` 的 23,452 项模拟属性已编译进程序。`starter` 启动时按卡牌 ID 查找已注册 Sim 所需的数据。更新卡库时，先生成 `Generated.g.cs`，再重新编译。

```powershell
$cardDefs = 'D:/script/hbCollectionhb/Hearthbuddy_backed_release/Routines/DefaultRoutine/Silverfish/data/CardDefs.xml'
dotnet run --project src/Silverfish.Cli -c Release -- import-card-data $cardDefs src/Silverfish.Simulation/Data/Generated.g.cs
dotnet run --project src/Silverfish.Cli -c Release -- import-card-data $cardDefs artifacts/card-data/card-data-256589.json
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-mana.json --card-data artifacts/card-data/card-data-256589.json
```

JSON 导出保留构建号、中英文名称与牌面文本，以及实体自身的数字标签，支持按卡牌 ID 或 DBF ID 查询。回放和搜索可通过 `--card-data` 挂载这份资料。

生成的卡牌属性包含随从种族和法术派系，包含双种族与全部种族。Sim 可以用 `cardFilter` 在目标、区域计数、触发器、属性光环或费用光环中指定 `minionType` 或 `spellSchool`。

替代费用类型随卡牌数据和存档保留。打出卡牌和使用英雄技能可以消耗法力、己方生命、护甲或尸体。支付生命后至少要剩 1 点生命。尸体累计值用 `corpses` 记录，已花费量用 `corpsesSpentThisGame` 记录。用 `LeavesNoCorpse` 声明死亡时不产生尸体的随从。

武器和地标的初始耐久只读取 `HEALTH`（45）。只有旧耐久标签的条目仍可在导出的原始标签中查询。

## 单卡 Sim

单卡 Sim 按 `CARD_SET` 归档到 `src/Silverfish.Simulation/Cards/<系列编号>-<中文系列名>/`。普通卡直接放在系列目录，附魔放在所属系列的 `ENCHANTMENT` 子目录。每个附魔保留独立 Sim，由施加效果的卡牌引用。局面记录附魔 ID 和来源实体，便于在回放和存档里追踪。

`generate-sim` 使用 XML 或 JSON 目录里的 `CARD_SET` 选择路径。缺少标签时使用 `0000-失效`，新系列尚未命名时使用其编号加 `-未知系列`。输出目录下已有同一 ID 的 Sim 时保留原文件。

**卡牌注释是单卡 Sim 的必需项。** 普通卡、附魔和仅包含 `PlayReq` 的骨架都在命名空间后保留四行非空注释，依次写中文卡名、英文卡名、中文牌面描述和英文牌面描述。直接写内容，省略字段前缀。对应语言未收录时注明资料缺失。没有牌面描述时分别写「无卡面描述。」和「No card text.」。

```powershell
dotnet run --project src/Silverfish.Cli -c Release -- generate-sim $cardDefs artifacts/card-sims CS2_092 CS2_092e
python tools/sim_comments.py --check
```

CI 会检查所有独立 Sim 的四行注释。可用 `python tools/sim_comments.py --repair $cardDefs` 按卡牌 ID 从 XML 补齐旧文件的缺失注释。

内置单卡在对应 Sim 中集中声明 `PlayReqs`。合法动作、解释和结算共用目标判定。迁移表记录旧条件，供校验和追溯。新增机制和注册单卡须遵守项目根目录 `AGENTS.md` 的规则和验收要求。

经典包以构建 `256589` 中 `EXPERT1` 系列的可收集牌为范围，按 DBF ID 升序开发。首批依次完成先知维伦（9）、魔瘾者（12）、怒火中烧（22）。下一张是末日灾祸（23）。

现有文件会保留。普通卡骨架预填了注释形式的 `[CardSim("卡牌ID")]`，完成规则后取消注释。编译后，模拟器按特性里的卡牌 ID 绑定规则和静态数据。新增构建中才有的卡牌可先通过 `--card-data` 验证，或用完整 XML 更新 `Generated.g.cs` 后再次编译。附魔由施加效果的单卡 Sim 引用。

自定义场景可以在 `cardRules` 中以 `cardId` 指定卡牌并填写结算规则。模拟器默认从编译后的卡牌数据读取类型、费用、基础数值、已支持的关键词和过载值。使用 `--card-data` 时，这些属性从指定的外部目录读取。回放结果的 `cardDataBuild` 记录所用资料的构建号。场景可以同时使用 `cards` 定义独立测试卡。

效果可以通过 `conditional` 按角色生命、伤害、攻击、护甲或指定区域的实体数量选分支。触发器也支持同样的 `condition`，在发动前按当时的状态判断。

有目标的牌、英雄技能和抉择分支可使用 `targetFilter`。合法目标须同时满足 `comparisons` 里的属性比较和可选的 `frozen` 状态。伤害值大于零可用来筛选已受伤的角色。

场上英雄、随从以及已装备的武器可通过 `costAuras` 改变手牌或英雄技能的费用。合法动作和扣费共用同一计算，费用叠加后最低为零。

`changeHandCost` 增减手牌的费用数值，`changeCostType` 改变支付资源。两者都可以作用于抽到的牌自身、抽牌事件涉及的牌或筛选后的己方手牌。`currentTurn` 修正在回合结束时到期。卡牌离开手牌区时清除费用修正。

```powershell
dotnet run --project src/Silverfish.Cli -c Release -- import-card-data samples/card-data-demo.xml artifacts/card-data/demo.json
dotnet run --project src/Silverfish.Cli -c Release -- plan samples/simulation-card-data.json --card-data artifacts/card-data/demo.json
dotnet run --project src/Silverfish.Cli -c Release -- decide samples/simulation-decision.json --card-data artifacts/card-data/demo.json
```

`decide` 接收进行中对局的完整状态、对局 ID、快照序号和调用方提供的候选动作。新模拟器验证候选动作的来源、目标、落点和抉择，再搜索首步并返回原动作 ID。取得动作结果后的新快照可以再次提交。

地标与随从共享战场位置。用 `playCard` 支付法力并选择落点，再用 `activateLocation` 执行 `activation` 中声明的目标和效果。激活免费，消耗一次使用次数。地标在下一次己方回合处于冷却。需要在最后一次激活先腾出位置的单卡，可声明 `removeBeforeFinalEffects`。

`discard` 按随机、最高或最低当前费用选择手牌，费用相同时由随机分支决定。`discardHand` 弃掉整手。整批移入墓地后，按原手牌顺序结算，每张先处理场上的 `cardDiscarded` 触发器，再处理它自身的 `whenDiscarded`。随从可以用 `summonDiscardedMinion` 让原实体进入战场。

`drawMatching` 从牌库随机抽取符合 `cardFilter` 的牌，没有候选时结束。`recruit` 从牌库随机召唤符合条件的随从，沿用原实体 ID。筛选可指定 `kind`、种族、派系和基础费用区间。多次抽牌或招募会在本次触发器结算后重新检查牌库与战场。

`transformMinionByCost` 从随机随从池选取目标费用档，沿用变形对象的实体 ID。场景可传入 `randomMinionPool` 指定当前对局允许生成的卡牌 ID，存档会保存这个列表。省略该字段时使用内置构建的可收集随从池，它覆盖多个系列。只含静态资料的结果按牌面进入场上，单卡效果标记为待实现。

带 `CastsWhenDrawn` 的法术成功抽到后会免费施放，执行单卡 Sim 的 `OnPlay`，需要目标时从合法目标里随机选择。抽牌流程随后从牌库顶补抽一张。手牌已满时仍走烧牌流程。

`createCard` 按 `cardId` 创建新实体，默认加入操控方手牌。用 `recipient` 指定另一方，用 `placement` 指定牌库顶、牌库底、随机位置或加入后整副洗牌。生成可以与抽牌、招募及抽到时施放组合。

`checkpoint` 导出最后一个动作结算后的 JSON 存档，包含已绑定的卡牌规则、随机状态和对局计数。可直接用 `plan` 读取存档，向存档添加 `actions` 后可继续回放，也可将存档作为 `decide` 请求的 `state`。

扎罗格的王冠在发现时暂停出牌结算。存档中的 `pendingPlayChoice` 保存来源、候选实体和随后要检查的触发来源。用 `chooseOption` 提交来源及候选的实体 ID，随后完成原法术的召唤和结算。回放或规划输出的 `legalActions` 会列出当时的有效选项。

```powershell
New-Item -ItemType Directory -Force artifacts | Out-Null
dotnet run --project src/Silverfish.Cli -c Release -- checkpoint samples/simulation-random.json |
    Set-Content -Encoding utf8 artifacts/checkpoint.json
dotnet run --project src/Silverfish.Cli -c Release -- plan artifacts/checkpoint.json
```

**P0 独立化已通过验收。** 2026-11-05 经典银鱼在 Windows 下独立编译为 .NET 8，加载了原版 `CardDB`，并通过了原版 `Ai.autoTester` 固定牌面、JSON 决策和错误输入回归。可执行文件和日志已归档。[私有 Actions #44256337489](https://github.com/xiaohei7972/Hearthbuddy_Silverfish/actions/runs/44256337489)

本项目不采用 Silverfish V2，也不把 StandaloneGameMod 接入作为当前开发阶段的前置条件。

## 已通过的里程碑

- 旧版经典银鱼参考代码可在 .NET 8 下**独立编译**、加载 CardDB、运行原版 `Ai.autoTester` 并计算出固定牌面中的斩杀攻击。
- 受限 `GameSnapshot JSON → BoardTester → 经典 Ai → BotDecision` 离线端到端回归已通过，相关 ClassicBridge 测试 9/9 通过。见 [Actions #43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029)。
- 新增 `Silverfish.Domain`：实体中心的不可变局面模型，分离卡牌基础定义、战斗属性、费用、武器属性、关键词与扩展标签。对应基础模型单元测试已通过 [Actions #43566986229](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43566986229)。
- 新模拟器统一使用 `GameState` 和 `GameEntity` 表示随从、手牌和武器。旧 `Minion`、`Handcard`、`Weapon` 类型留在经典引擎的独立回归路径中。
- 新模拟器已支持抽牌、亡语、附魔、位置光环、沉默、武器、基础英雄技能、回手、变形、消灭、复活、战斗关键词、过载、奥秘、随机效果、临时法力和带独立目标规则的抉择。`starter` 的已注册卡牌、衍生物和技能使用共用结算操作。

## 后续重构路线

1. 以已验收的 2026-11-05 版及固定牌面决策作为回归基线。
2. 用 `GameState`、玩家区域和实体 ID 表示新局面，动作由新模拟器直接执行。
3. 用通用结算操作重写卡牌效果，按机制扩充回放覆盖。经典引擎和脚本作为历史回归参考。
4. 在新模拟器上扩展搜索和估值，用固定局面核对行动、结算轨迹和性能。
5. 扩展目标与选牌决策。沿用经典 `PlayReq` 的条件语义，支持 `OPTION` 单目标预选、有序多目标，以及发现、进化、探底、泰坦、古神和连续选择的效果预演。实施顺序和验收场景见 `@docs/CLASSIC_MODEL_REFACTOR.md` 的「目标与选牌决策」一节。
6. 完善独立模拟器的伤害与治疗结算。记录每笔伤害的来源和结果，接入吸血、剧毒、超杀、荣誉击杀及过量治疗。实施顺序和验收场景见 `@docs/CLASSIC_MODEL_REFACTOR.md` 的「伤害与治疗结算」一节。

详见 [源码实查与模型重构计划](docs/CLASSIC_MODEL_REFACTOR.md) 和 [独立源码构建说明](docs/CLASSIC_ACTIONS_BUILD.md)。

## 工程结构

- `src/Silverfish.Domain`：新模拟器的不可变局面与实体模型。
- `src/Silverfish.Simulation`：合法动作生成、规则结算和卡牌定义。
- `tests/Silverfish.Domain.Tests`：新模型的分支隔离、状态不变量和基础功能测试。
- `src/Silverfish.Classic`：按 SHA/选择规则从旧源归档导入的经典银鱼源码编译项目。
- `tests/Silverfish.Classic.Tests`：经典 `Playfield` 的分支隔离、牌库与单卡效果回归。
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
dotnet test SilverfishStandalone.sln -c Release
```

经典导入工具的回归入口如下。

```powershell
python tools/test_import_classic.py
python tools/test_normalize_classic.py
```

经典引擎和相关回归须先导入对应经典源文件，再在 `classic-compile.yml` 中完整验证。
