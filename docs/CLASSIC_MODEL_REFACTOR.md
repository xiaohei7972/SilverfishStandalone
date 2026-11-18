# 经典银鱼 AI 的状态模型重构（基于 2026-11-05 源码）

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

## 新模型：第一批已落地

`src/Silverfish.Domain` 是新的、完全独立于旧 Hearthbuddy/Triton/旧 Silverfish 类的 .NET 8 类库：

- `CardDefinition`：印刷/基础卡牌信息，与当前实体数值严格分离。
- `EntityId`、`PlayerSide`、`EntityZone`、`EntityKind`：明确区分实体身份、所有权、区域和类型。
- `EntityState`：实体中心的不可变状态值，组合 `CombatStats`、`ManaCost`、`HandStatModifiers`、`WeaponStats`。
- `Keywords`、`RawTags`、`Counters`、`EnchantmentCardIds` 使用不可变集合，保留未识别的标签，而不是每个卡牌效果都加一个公开 bool/int 字段。
- `BoardSnapshot`：不可变局面状态，支持 `Replace`、`Move` 和 `Remove`，并拒绝重复实体 ID 或不合理的结构。

第一批测试重点是**分支隔离、实体身份稳定、卡牌定义/运行时数据分离、装备/战斗/费用状态分离、未知标签保留**。

## 第二批：只读旧模型投影

`src/Silverfish.ClassicModelAdapter` 新增 `LegacyEntityProjection`，把原始 `Minion`、`Handmanager.Handcard`、`Weapon` 中能明确识别的状态投影为独立、不可变的 `EntityState`：

- 英雄和随从依然读取旧类，但转换后按不同 `EntityKind` 保存。
- 同一张牌的**基础费用/属性**来自独立 `CardDefinition`，当前费用、攻防、附魔是实体状态；不再修改 CardDB 共享卡牌定义。
- 扩展标签如果旧版本不存在，会省略为未知，不伪造数值 0。
- 2026-11-05 的 `Minion.enchs` 是卡牌 ID 列表；旧版参考源仍是字符串。投影层只在能够明确转换时接受，不会把字符串拆成字符作为错误的卡牌 ID。
- 原版 `Weapon` 缺少可靠的实体 ID、最大耐久，投影接口要求显式传入；有尚未可转换的武器附魔时拒绝转换。

`tests/Silverfish.ClassicModelAdapter.Tests` 将直接引用 CI 导入的旧银鱼源码进行回归，验证投影后的实体不再随旧对象继续变化。**它仍不是 Playfield 的完整替代实现**。

## 迁移顺序（不一次性重写 19K 个脚本）

1. **回归基准已建立**：2026-11-05 独立源码工作包已通过 SHA-256 校验、.NET 8 编译、`BoardTester` 和 JSON 决策回归。[私有 Actions #44256337489](https://github.com/xiaohei7972/Hearthbuddy_Silverfish/actions/runs/44256337489)
2. **只读映射层**：将旧 `Minion/Handcard/Weapon` 捕获为独立 `EntityState`，并添加缺失标签警报与差异测试；旧对象不泄漏到新引擎。
3. **局面树与动作重构**：把 `Playfield` 中的分支复制逐步替换为 `BoardSnapshot` 的结构共享更新；`Action` 只保存来源/目标 ID、选择与位置，不保存旧对象引用。
4. **效果系统迁移**：逐步按卡牌分组迁移旧 `Sim_*` 脚本，加入效果执行上下文、明确的状态变更操作和原版回归金样本。不能直接将 19000+ 卡牌脚本硬性替换成新结构而不测试。
5. **搜索与估值**：在新局面模型稳定后，再逐步改造 Movegenerator、MiniSimulator、PenalityManager 和 Ai，比较可重复最佳动作与耗时/内存。

## 经典模拟分支的状态隔离

2026-11-05 源码已复现三处共享写入，分别出现在 `Playfield` 复制后的兄弟分支、随从变形和手牌替换。导入脚本为这些入口复制可变的附魔列表和条件卡牌列表，并分离随从的内嵌手牌对象。复制保留列表内容及随从原有的手牌扩展字段，变形与替换保留目标实体 ID。旧版随从的字符串附魔按不可变值复制。

回归直接创建两个真实 `Playfield` 分支，修改一个分支后检查父局面和兄弟分支。经典源码编译工作流在状态投影测试中运行这些用例。

DIY 手牌的组合入口和 `BoardTester` 的模块牌解析会先复制卡牌元数据，再写入模块费用与属性。测试用同一张牌的普通版和模块版验证原版卡库和另一张手牌保留印刷数据。

武器分支保留原有 `pID`、超杀、荣耀击杀和附魔。每个分支独立复制附魔条目、标签和附件，同时保留同一武器附魔内部的共享引用和循环。回归通过真实 `Playfield` 复制验证这些状态。

经典导入器对已核对的手牌、随从内嵌手牌和武器属性写入，在修改前复制该实体持有的卡牌数据。用于临时召唤和入手的模板在应用效果前单独复制。泰坦动作生成直接使用局部技能列表。回归覆盖九蛙魔杖多次触发、释放邪能的吸血条件、意志熔炉连续召唤以及泰坦候选动作。

同视角的 `Playfield` 分支复制已知牌库的顺序与重复条目。牌库增益在写入时复制对应位置的卡牌。定向抽牌消费选中的条目，沿用原来的手牌上限和抽牌触发流程，并把该条目的属性与费用带入手牌。局面比较及 2026-11-05 版的状态哈希纳入已知牌库的卡牌 ID、费用、攻防及两个脚本计数。对手视角只使用当前可知的对手数据。

搜索根局面从宿主或离线回放读取牌库估计。同视角的分支共享只读估计，交易、已迁移的定向抽牌和帕奇斯召唤仅更新当前分支。定向抽牌按请求数量消费，优先使用已知条目及其属性。局面比较及 2026-11-05 版的状态哈希也纳入牌库估计的牌序和数量。

## 严格边界

这份 `Silverfish.Domain` 只是正式重构的**底层状态模型第一版**，尚未替代旧 `Minion/Handcard/Weapon`，也没有替代真实旧版 `Playfield` 算法。不能将新模型单元测试通过误称为全量银鱼模型迁移成功。

其余按卡牌 ID 定向抽牌、从牌库召唤以及回到卡库的属性写入仍需迁移。武器附魔需要带来源的投影，供 `BoardSnapshot` 和按实体 ID 表示的动作使用。
