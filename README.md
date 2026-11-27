# SilverfishStandalone

## 第一阶段架构整合（2026-11-22）

本仓库已将独立模拟器基础和 `EntityChoices` 有序目标/离线预演的功能纳入同一集成主线。**内部结算的 `PendingPlayChoice` 与客户端外部选择的 `PendingChoice` 是两个不同的状态，均予保留。** 真实游戏的虚拟输入执行器仍未接入，不通过网络 API 直接执行动作。详见 [第一阶段整合与回归门槛](docs/PHASE1_INTEGRATION.md)。

阶段二已完成外部 `PendingChoice` 门禁收紧、武器伤害来源追踪、多轮选择与终局清理、批量伤害与即时死亡的观察者时点快照、单效果栈内光环即时刷新、战斗双方同时受伤后的延迟吸血结算，以及 `GameSimulator.Damage.cs` 的分部抽离。详见 [阶段二：结算正确性与回归](docs/PHASE2_RESOLUTION.md)。


**执行原则**：AI 只产生行动意图，实际操作使用游戏内的 `IInput / InputCollection` 虚拟输入源（参考 Ember），**不会移动 Windows 系统鼠标，不抢占其他桌面操作**，也不调用 `Network.SendOption`、`Network.SendChoices` 等接口。现有 `UiChoiceInputPlanner` 负责将有序选择映射为点击/确认/等待步骤；游戏内输入适配器和 IPC 尚未接入。详见 [UI 模拟输入执行边界](docs/UI_INPUT_EXECUTION.md)。


## CHOICE 有序多目标选择（第一、二阶段）

`Silverfish.Cli choice samples/entity-choice-targets.json` 可在离线环境核验 EntityChoices 的数量范围、禁选实体、点击顺序、取消重排以及确认提交与包 ID 检查。现已支持把待决选择保存到局面、提交有序选择动作及 checkpoint 恢复；提交不会假定卡牌效果已结算，仍需等待后续权威局面快照；当前尚未接入真实游戏选择包和效果暂停/恢复；`OPTION_TARGET` 与 `CHOICE` 分离。详见 [EntityChoices 说明](docs/ENTITY_CHOICES.md)。示例：`dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-pending-choice.json`。


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

经典包以构建 `256589` 中 `EXPERT1` 系列的可收集牌为范围，按 DBF ID 升序开发。已全部完成至虚灵奥术师（DBF 1737，`EX1_274`），共完成 **245 / 245** 张可收集经典牌及其全部依赖衍生物、梦境牌、英雄技能与附魔。

现有文件会保留。普通卡骨架预填了注释形式的 `[CardSim("卡牌ID")]`，完成规则后取消注释。编译后，模拟器按特性里的卡牌 ID 绑定规则和静态数据。新增构建中才有的卡牌可先通过 `--card-data` 验证，或用完整 XML 更新 `Generated.g.cs` 后再次编译。附魔由施加效果的单卡 Sim 引用。

自定义场景可以在 `cardRules` 中以 `cardId` 指定卡牌并填写结算规则。模拟器默认从编译后的卡牌数据读取类型、费用、基础数值、已支持的关键词和过载值。使用 `--card-data` 时，这些属性从指定的外部目录读取。回放结果的 `cardDataBuild` 记录所用资料的构建号。场景可以同时使用 `cards` 定义独立测试卡。

效果可以通过 `conditional` 按角色生命、伤害、攻击、护甲、角色存亡状态（`characterState`）或指定区域的实体数量选分支。触发器也支持同样的 `condition`，在发动前按当时的状态判断。

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

`createCard` 按 `cardId` 创建新实体，默认加入操控方手牌。用 `recipient` 指定另一方，用 `placement` 指定牌库顶、牌库底、随机位置或加入后整副洗牌。`copyDeckCards` 从指定一方牌库按筛选条件不重复地随机复制卡牌置入手牌。`summonMatchingMinion` 从随机随从池筛选符合条件的随从召唤入场。生成可以与抽牌、招募及抽到时施放组合。

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

## 伤害结算基础：剧毒和吸血

新模拟器现在能从客户端卡牌元数据读取 `POISONOUS` / `LIFESTEAL` 标签，并支持随从、法术和主动攻击时武器的对应效果。圣盾会完全阻止本次伤害，所以不触发剧毒消灭或吸血治疗。伤害事件增加 `damageToArmor`、`damageToHealth`，并保留原攻击来源；吸血由造成伤害的卡牌控制者的英雄获得治疗，剧毒对受到有效伤害的随从标记消灭，统一进入原有死亡与亡语结算。沉默后卡牌原有关键词不再生效。

此阶段先覆盖剧毒/吸血的基本伤害场景，超杀、荣誉击杀、过量治疗和复杂的伤害改写机制仍需要后续独立实现。

## 超杀与荣誉消灭：按实际伤害触发奖励

模拟器新增 `TriggerTiming.Overkill` 与 `TriggerTiming.HonorableKill`，根据伤害来源控制者的回合和角色受击前的生命值判断：超过剩余生命的伤害是超杀，恰好致命的伤害是荣誉消灭。先扣护甲再比较生命值；圣盾完全抵消伤害，不触发两种奖励。敌方回合的反击不触发奖励。

需要额外奖励的卡牌将 `triggers` 定义为 `timing: "Overkill"` 或 `"HonorableKill"`，并指定 `focus: "DamageSource"` 和 `entityMatch: "Self"`；奖励效果通过现有结算工作队列执行，在统一死亡处理之前触发。支持造成伤害的随从、法术，以及英雄攻击时装备的武器。伤害事件增加 `isOverkill` 和 `isHonorableKill` 两个判定字段，便于测试及搜索回放。当前实现的是机制本身；各卡牌的超杀和荣誉消灭奖励仍需逐张接入单卡规则。

## 通用治疗与过量治疗（Overheal）

新模拟器支持 `HealEffect`，以卡牌和触发器规则指定 `Selected`、`Source`、`TriggerSubject`、`FriendlyHero`、`EnemyHero`、`FriendlyMinions`、`AllMinions` 和 `AllFriendlyCharacters` 等治疗目标。单次治疗根据实际缺失生命值封顶，只有**真正恢复生命**时产生 `Healed` 事件。旧的 `HealFriendlyHeroEffect` 和吸血继续复用这条治疗计算，保留原有输出。

客户端 `GAME_TAG.OVERHEAL` 映射为 `CardKeywords.Overheal`。具有该关键词的随从在一次治疗中收到**超过当前缺失生命的正数治疗量**时，记录 `Overhealed` 事件并触发自身的 `TriggerTiming.Overheal` 奖励；即使当前满血也可触发。判定以当前最大生命和伤害值为准，沉默后不生效。普通随从、英雄不因为多余治疗而触发 Overheal。此类奖励定义须使用 `focus: "EventTarget"`、`entityMatch: "Self"`，并配合 `CardKeywords.Overheal`。

群体治疗先完成所有目标的生命变化，再依照实体顺序处理 Overheal 触发器；治疗来源可以是敌方角色，但奖励归属于被治疗随从的控制者。现阶段覆盖通用模拟规则和固定测试，单张 Overheal 卡牌的具体奖励仍需逐张录入及校验。

## 已知 CHOICE 效果的独立预演

新增 `GameSimulator.PreviewChoiceDistribution` / `PreviewChoice` 和 `EntityChoiceForecastPlanner`：只有外部明确提供全部候选选项对应的 `ChoicePreviewRules`，才能离线试算 `GENERAL`（例如发现后获得一张已知卡）或 `TARGET`（按顺序逐个目标结算）的效果。随机结果按预算枚举，合法的有序选法按评估函数比较；如果不知道效果、缺包 ID、组合超预算或随机分支不精确，AI 不会猜测答案。

**预演结果始终是假想状态。** `Apply(ChooseOption)` 行为未变：只提交有序实体 ID 意图，并等待下一份权威局面快照；不自动串联生成新的 CHOICE 包、不调用网络接口或控制 Windows 鼠标。详见 [EntityChoices 第三阶段](docs/ENTITY_CHOICES.md)。

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
