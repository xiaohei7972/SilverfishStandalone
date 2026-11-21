# 经典银鱼 AI 的状态模型重构（基于 2026-11-05 源码）

当前实现路线以 `GameState` 和独立的 `Silverfish.Simulation` 为主。新卡牌效果按规则重新实现，本文所列的经典模型和脚本用作回归基线。新架构和离线回放入口见 `docs/ARCHITECTURE.md`。

## 项目定位

目标是**重构银鱼的模拟与决策内部模型**，而不只是替换炉石兄弟主程序、改读内存或制作新 UI。当前阶段不集成 StandaloneGameMod、不引入 Silverfish V2，也不实现自动执行动作。

用户提供的权威源码基准：
- 原 ZIP：`Hearthbuddy1.7.1(1).zip`
- 顶层目录：`炉石兄弟-2026.11.05`
- SHA-256：`1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071`
- 经典银鱼位置：`Routines/DefaultRoutine/Silverfish/`

## 已确认的旧模型问题

直接检查了上述 ZIP 中的真实源文件，而非推测：

| 文件 | 当前设计 | 首要风险 |
| --- | --- | --- |
| `ai/Minion.cs` | 约 1659 行，混合英雄/随从、实体 ID、攻防、附魔、关键字与特殊效果标记 | 单体对象过大、直接公开可变字段；`Minion(Minion)` 共享 `m.handcard` 和 `m.enchs`，模拟分支间可能互相污染 |
| `ai/Handmanager.cs` | `Handcard` 混合卡牌静态定义、当前费用、目标、条件计数与特殊效果 | `Handcard(Handcard)` 和 `setHCtoHC` 对 `enchs`、`conditionalList` 执行共享引用赋值 |
| `ai/Weapon.cs` | 一部分武器状态与附魔混合在一起 | 没有通用实体 ID；复制构造函数没有复制全部字段（例如 `enchants`） |
| `ai/Playfield/Playfield.Ctors.cs` | 复制局面时直接 `new Minion(m)`、`new Handcard(hc)`、`new Weapon(w)` | 同一局面的不同搜索分支可能持有共享的可变嵌套对象 |
| `ai/Action/action.cs` | 动作直接持有 `Minion`、`Handcard` 可变对象引用 | 动作与被模拟后续局面耦合，历史动作缺少稳定快照身份 |

## 当前独立模型

- `CardDefinition` 保存卡牌的印刷属性。
- `GameState` 保存玩家资源、有序区域、实体表和续跑计数。随从、武器和手牌共用 `GameEntity`，通过实体 ID 和区域访问。
- `CharacterState` 和 `WeaponStats` 分别保存角色战斗状态和武器攻耐。手牌使用区域位置及实体上的费用修正。
- 模拟动作记录来源、目标、位置和抉择。规则结算基于不可变局面，搜索分支可以共享同一个输入状态。

现有经典源码和下文的回归用于核对历史规则。新卡牌效果按独立模拟器的规则机制实现和测试。

## 目标与选牌决策（待实现）

让独立模拟器能从合法候选里选出一个目标、一组有序目标，或一个会继续结算的卡牌选项。内置单卡的出牌和激活条件遵循项目根目录 `AGENTS.md`，统一声明在对应 Sim 的 `PlayReqs` 中。抉择选项的专属条件由 `CardPlayChoice.PlayReqs` 声明。现有 `CardPlayChoice` 在出牌时随 `PlayCard` 一起确定，进行中的选牌需要单独的 `ChooseOption`。`@src/Silverfish.Simulation/SimulationCard.cs:182` `@src/Silverfish.Simulation/GameSimulator.cs:247` `@src/Silverfish.Contracts/GameSnapshot.cs:71` `@src/Silverfish.Core/SimulationDecisionEngine.cs:55`

### 从客户端状态获取候选

| 客户端状态 | 适配器需要读取的信息 | 决策时机 |
| --- | --- | --- |
| `OPTION`、`SUB_OPTION`、`OPTION_TARGET` | 当前 `Options` 包 ID、主选项和子选项、来源实体、各分支的 `Targets` 及合法状态 | 在 `OPTION` 就能比较普通单目标或「子选项 + 目标」。`SendOption()` 只有一个目标字段，候选列表里的每个目标表示一个可选项。 |
| `CHOICE`，`ChoiceType = TARGET` | 当前 `EntityChoices` 的 ID、来源、可选实体和 `CountMin`、`CountMax` | 收到当前选择后确定有序目标组，数量与成员都以当前包为准。客户端按选择顺序一次提交。 |
| `CHOICE`，`ChoiceType = GENERAL` | 当前 `EntityChoices` 及其候选实体对应的 `CardId`、已公开的实体属性和标签 | 收到候选后预演选牌结果。`ChoiceType` 描述交互，效果需要查来源与选项的模拟规则。 |

数据以当前响应模式及数据包为准。界面索引仅用于展示。候选使用实体 ID 区分同名牌或同一 `CardId` 的不同实体。适配器携带 `matchId`、快照序号和本次包 ID，沿用 `decide` 返回调用方原候选 ID 的契约。包更新或候选变化时用新快照重算。实战适配按游戏版本验证，离线阶段用记录的输入和可复现的回放。`@src/Silverfish.Core/SimulationDecisionEngine.cs:12`

客户端源码核对点见 `GetOptionsPacket()`、`IsValidTarget()`、`SendOption()`、`EntityChoices` 和 `SendChoices()`。常规选牌界面处理 `GENERAL`，场上目标选择也可能短暂显示瞄准箭头。读取和匹配 `TARGET` 时以选择包为准。`@D:/script/hbCollectionhb/炉石传说源代码/2026-10-13/Assembly-CSharp/GameState.cs:2952` `@D:/script/hbCollectionhb/炉石传说源代码/2026-10-13/Assembly-CSharp/Network.cs:12312` `@D:/script/hbCollectionhb/炉石传说源代码/2026-10-13/Assembly-CSharp/GameState.cs:5646` `@D:/script/hbCollectionhb/炉石传说源代码/2026-10-13/Assembly-CSharp/Network.cs:12412` `@D:/script/hbCollectionhb/炉石传说源代码/2026-10-13/Assembly-CSharp/GameState.cs:5578` `@D:/script/hbCollectionhb/炉石传说源代码/2026-10-13/Assembly-CSharp/ChoiceCardMgr.cs:643`

### 实施顺序

1. **接通待决选择和 `ChooseOption`。** 在 `GameState` 表示可存档的未完成选择，记录选择人、当前阶段、来源和触发来源、已公开候选、数量限制，以及恢复结算所需的上下文。结算走到选择点就交还控制权，期间只生成该选择可接受的动作。收到合法的 `ChooseOption` 后从暂停处继续，确保费用、出牌事件和已处理的触发器只结算一次。检查 `checkpoint`、重新加载和 `decide` 的一致性，并兼容省略选择字段的旧存档。`@src/Silverfish.Domain/GameState.cs:49` `@src/Silverfish.Simulation/GameSimulator.Resolution.cs:16` `@src/Silverfish.Cli/Program.cs:250`
2. **支持 `OPTION` 预选和有序目标组。** 把主选项、子选项、落点和普通单目标写进同一个候选动作。多目标沿用现有单目标规则校验每个位置，按卡牌规则判断组合条件和顺序。动作记录完整目标序列，精确比较每个目标及其顺序。客户端 `TARGET` 选择还须校验当前包的成员、数量及去重要求。若下一阶段候选尚未公开，先记录计划意图，收到新包后再约束或重算。`@src/Silverfish.Domain/SimulationAction.cs:5` `@src/Silverfish.Simulation/GameSimulator.Targeting.cs:15`
3. **实现选项效果预演。** 优先按来源 `CardId`、选项 `CardId` 和阶段查单卡 Sim，通用机制复用已有结算操作。为每个合法选项建立独立模拟分支，复用现有规划器估值。随机结果使用现有的分支预算和采样，下一阶段候选尚未公开时只基于已知信息估值。遇到资料缺失或规则未实现的选项，决策需报告缺失的来源、阶段和候选，便于补写回放和规则。`@src/Silverfish.Simulation/SimulationCard.cs:182` `@src/Silverfish.Simulation/GameSimulator.Random.cs` `@src/Silverfish.Core/SimulationTurnPlanner.cs`

### 首批机制与后续扩展

| 机制 | 实现要点 |
| --- | --- |
| 普通发现 | 使用已揭示的候选，选中后按生成牌流程进入手牌。来源牌可声明立即施放、召唤、改变费用等变体。 |
| 探底 | 将选中的牌库实例移到顶端，保留原实体和修正。同名牌按实例区分。如果来源牌还有抽牌步骤，从这里恢复结算。 |
| 进化（`ADAPT`） | 选项 ID 映射到实际增益和受益随从。随机变形类效果走独立的变形规则。 |
| 泰坦与古神技能 | 用实际技能卡 ID 绑定效果，保存主动使用次数、已用标记和技能自己的目标。被其他卡触发时保留原触发来源，按该效果的规则结算。技能数量从卡牌与当前候选读取，覆盖泰坦三个技能和古神第四个技能。 |
| 连续选择和复合效果 | 将「选技能再选目标」「发现后立即施放再选择」等表示为可继续的结算阶段。每次选择独立校验，后续效果继承来源和已结算进度。 |

优先完成普通发现、探底和一个需要目标的泰坦技能，再加入进化、古神技能及连续选择。卡面文本用于核对规则，实际预演由卡牌 Sim 和通用效果执行。[暴雪探底机制说明](https://hearthstone.blizzard.com/en-gb/news/23765277/announcing-voyage-to-the-sunken-city-hearthstone-s-next-expansion) [暴雪泰坦说明](https://hearthstone.blizzard.com/en-us/expansions-adventures/titans) [暴雪古神说明](https://hearthstone.blizzard.com/en-us/news/24297200)

### 回放与验收

- **合法性与身份。** `OPTION` 阶段能识别不同子选项各自的合法单目标。多目标用例覆盖 A→B 与 B→A、数量上下限、重复实体和失效候选。同一 `CardId` 的两个不同候选保持独立，过期选择包的候选被拒绝。
- **结算和续跑。** 发现只增加选中的手牌。探底移动原实体，紧接的抽牌拿到该实体。进化只作用于本次受益者。使用带目标的技能后准确变更目标状态及技能次数，第四个技能可单独选择。多阶段回放应能在选择点存档，重载后得到相同事件顺序和最终盘面，费用和触发只结算一次。
- **决策质量。** 用固定盘面验证费用低的解牌能胜过高费候选，探底按下一抽的收益评分，技能和目标组合能够找到斩杀。保留现有打牌抉择与随机搜索的回归，并从 `decide` 核对返回的原候选 ID。`@samples/simulation-choices.json` `@samples/simulation-random.json` `@tests/Silverfish.Domain.Tests/GameSimulationTests.Choices.cs` `@tests/Silverfish.Tests/SimulationDecisionEngineTests.cs`

## 伤害与治疗结算（待实现）

独立模拟器已有来源和受伤目标的监听、圣盾破裂、护甲吸收和疲劳伤害。英雄持武器攻击时，伤害事件只保留英雄实体。治疗目前只支持恢复己方英雄，并在事件中记录实际恢复量。

`@src/Silverfish.Simulation/GameSimulator.Resolution.cs:197`

`@src/Silverfish.Simulation/GameSimulator.Triggers.cs:12`

`@src/Silverfish.Simulation/GameSimulator.Equipment.cs:67`

### 开发计划

| 步骤 | 改动与依赖 | 本步验收 |
| --- | --- | --- |
| 1. 统一伤害入口 | 扩展 `Damage`、`Occurrence` 和回放事件。每次命中记录直接来源、操控者、当前回合方、目标、目标命中前的生命与护甲，以及当时生效的关键词。战斗区分攻击与反击，持武器攻击同时记录英雄和武器实体。法术、英雄技能、战吼、亡语保留效果来源，疲劳标记为独立原因。 | 同一随从攻击和发动战吼分别产生可区分的记录。固定群伤逐目标记录，战斗双方先交换伤害再处理监听，逐段效果保留段间触发。 |
| 2. 补全伤害结果与击杀归因 | 在步骤 1 的同一入口处理防护，记录结算伤害、护甲损失、生命变化和超出致死阈值的数值。将致死的那一笔命中与死亡检索关联，记录伤害致死、剧毒消灭和直接消灭各自的原因。 | 3 点伤害命中有 2 点护甲的英雄，事件记录护甲损失 2、生命损失 1。纯护甲损失可触发受伤监听，圣盾、免疫和零伤害均不触发。用两段伤害验证击杀归属于致死的一段。 |
| 3. 统一治疗与过量治疗 | 将 `HealFriendlyHeroEffect` 接入支持英雄和随从的治疗操作。记录来源、目标、计算后的治疗量、实际恢复量与溢出量。按实际恢复量派发普通治疗触发，按溢出量派发过量治疗触发。治疗转伤害复用前两步的伤害入口，核对转换后的归因与连锁触发。 | 对有过量治疗的 5 血上限随从治疗 3 点。原有 2、4、5 血时，实际恢复量分别为 3、1、0，溢出量分别为 0、2、3。覆盖真银圣剑在反击前回血，以及生命费用和设定生命走各自的入口。 |
| 4. 接入卡牌关键词 | 将吸血、剧毒、超杀、荣誉击杀（荣誉消灭）和过量治疗从卡牌数据接入运行时，覆盖内置卡库和外部数据。吸血使用本次结算伤害并调用步骤 3 的治疗入口。剧毒在有效伤害命中随从时登记消灭。超杀及荣誉击杀使用步骤 2 的结果，结合当前回合、目标资格和单卡奖励结算。 | 自己回合分别对 3 血敌方随从造成 2、3、4 点伤害，核对未致死、恰好致死、超额致死三种结果，再测对手回合。验证吸血武器最后一点耐久、剧毒打圣盾，以及命中后来源被沉默或离场时已登记效果的归属。 |
| 5. 接入回放与搜索 | 为每个关键词选择代表卡，使用正常出牌、攻击或英雄技能入口建固定牌面。检查旧存档缺省新增字段时仍可加载。让回放和规划器消费同一份结算结果。 | 用一个仅靠吸血才能活过反击的牌面和一个靠荣誉击杀奖励才能完成斩杀的牌面，对照事件顺序、最终生命和规划器选招。运行独立解方案的 Release 回归。 |

首个可交付改动是步骤 1。先用普通攻击、武器攻击、法术和疲劳四个固定用例验证来源记录，再扩展到群伤与反击。步骤 2 和普通治疗可以在来源字段确定后分别开发，治疗转伤害及关键词联动在两条路径接通后验收。

吸血、剧毒和超杀按伤害发生时的状态登记。涉及嵌套触发的用例同时检查来源在命中前与命中后失去关键词的结果。[暴雪伤害关键词更新](https://hearthstone.blizzard.com/en-us/news/22938499) · [超杀](https://hearthstone.blizzard.com/en-gb/news/22629915/be-ready-for-the-rumble) · [荣誉击杀](https://hearthstone.blizzard.com/en-us/news/23748034/fractured-in-alterac-valley-is-live-now) · [过量治疗](https://hearthstone.blizzard.com/en-us/news/23921497)

## 经典模拟分支的状态隔离

2026-11-05 源码已复现三处共享写入，分别出现在 `Playfield` 复制后的兄弟分支、随从变形和手牌替换。导入脚本为这些入口复制可变的附魔列表和条件卡牌列表，并分离随从的内嵌手牌对象。复制保留列表内容及随从原有的手牌扩展字段，变形与替换保留目标实体 ID。旧版随从的字符串附魔按不可变值复制。

回归直接创建两个真实 `Playfield` 分支，修改一个分支后检查父局面和兄弟分支。经典源码编译工作流通过 `Silverfish.Classic.Tests` 运行这些用例。

DIY 手牌的组合入口和 `BoardTester` 的模块牌解析会先复制卡牌元数据，再写入模块费用与属性。测试用同一张牌的普通版和模块版验证原版卡库和另一张手牌保留印刷数据。

武器分支保留原有 `pID`、超杀、荣耀击杀和附魔。每个分支独立复制附魔条目、标签和附件，同时保留同一武器附魔内部的共享引用和循环。回归通过真实 `Playfield` 复制验证这些状态。

经典导入器对已核对的手牌、随从内嵌手牌和武器属性写入，在修改前复制该实体持有的卡牌数据。用于临时召唤和入手的模板在应用效果前单独复制。泰坦动作生成直接使用局部技能列表。回归覆盖九蛙魔杖多次触发、释放邪能的吸血条件、意志熔炉连续召唤以及泰坦候选动作。

同视角的 `Playfield` 分支复制已知牌库的顺序与重复条目。牌库增益在写入时复制对应位置的卡牌。定向抽牌消费选中的条目，沿用原来的手牌上限和抽牌触发流程，并把该条目的属性与费用带入手牌。局面比较及 2026-11-05 版的状态哈希纳入已知牌库的卡牌 ID、费用、攻防及两个脚本计数。对手视角只使用当前可知的对手数据。

搜索根局面从宿主或离线回放读取牌库估计。同视角的分支共享只读估计，交易、已迁移的定向抽牌和帕奇斯召唤仅更新当前分支。定向抽牌按请求数量消费，优先使用已知条目及其属性。局面比较及 2026-11-05 版的状态哈希也纳入牌库估计的牌序和数量。

已迁移的招募、条件抽牌和奥秘入场共用牌库条目选择与扣减。它们优先按已知条目的顺序和当前属性匹配，再从估计剩余的份数选择。招募遇到满场即停止，奥秘入场跳过已经在场的同名奥秘。回归覆盖战斗号角的重复牌和满场、冰钓术与芬杰连续消费同一牌库，以及冰封秘典对重复奥秘的处理。

## 当前范围

经典 `Playfield` 及其卡牌脚本用于历史回归。独立模拟器的卡牌行为由已经绑定的单卡规则及场景规则覆盖。
