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

## 迁移顺序（不一次性重写 19K 个脚本）

1. **模型冻结与回归基准**：给 2026-11-05 原始经典源码在 GitHub Actions 做 SHA-256 校验、编译、`BoardTester` 和 JSON 决策回归。目前 Actions 只能从旧版仓库自动取源码；原 ZIP/独立源码工作包尚未放在可供 Actions 下载的存储位置，不能假称最新版已通过。
2. **只读映射层**：将旧 `Minion/Handcard/Weapon` 捕获为独立 `EntityState`，并添加缺失标签警报与差异测试；旧对象不泄漏到新引擎。
3. **局面树与动作重构**：把 `Playfield` 中的分支复制逐步替换为 `BoardSnapshot` 的结构共享更新；`Action` 只保存来源/目标 ID、选择与位置，不保存旧对象引用。
4. **效果系统迁移**：逐步按卡牌分组迁移旧 `Sim_*` 脚本，加入效果执行上下文、明确的状态变更操作和原版回归金样本。不能直接将 19000+ 卡牌脚本硬性替换成新结构而不测试。
5. **搜索与估值**：在新局面模型稳定后，再逐步改造 Movegenerator、MiniSimulator、PenalityManager 和 Ai，比较可重复最佳动作与耗时/内存。

## 严格边界

这份 `Silverfish.Domain` 只是正式重构的**底层状态模型第一版**，尚未替代旧 `Minion/Handcard/Weapon`，也没有替代真实旧版 `Playfield` 算法。不能将新模型单元测试通过误称为全量银鱼模型迁移成功。

**下一次应优先检查：** 最新版源码是否能在 CI 上原样编译，然后针对旧模型的浅拷贝和可变卡牌元数据建立回归测试。
