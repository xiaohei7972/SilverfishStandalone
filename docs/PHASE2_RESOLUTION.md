# 阶段二：结算正确性与回归（第一批）

> 基线：阶段一已集成的 `feat/standalone-foundation` / `c2cef52`。本页记录已提交实现的范围，不将计划项冒充已完成项。

## 目标与执行策略

阶段二不再做旧 `Playfield` 的大规模结构搬迁，优先保护已落地的不可变 `GameState` 与 `GameSimulator` 的行为边界。先建立可反复执行的故障回归，再逐项改善伤害、治疗、触发、选择和存档的行为；未来拆分 `ResolutionEngine` / `TriggerDispatcher` 前必须保持这套回归全部通过。

## 本批实现

### 外部 CHOICE 与内部模拟动作严格隔离

- `PendingChoice` 只等待外部客户端的权威 CHOICE 包提交和下一帧状态；此时 `LegalActions` 继续返回空，并且 `CheckAction`、`EnsureLegal` 对普通 `PlayCard`、`UseHeroPower`、地标技能、攻击、结束回合均不放行。特别修复此前普通出牌在 `CheckAction` 中自行判定 PlayReq 后绕过 `LegalActions` 的路径。
- `Apply` 中匹配 `ChoicePacketId` 的 `ChooseOption` 仍走单独的外部选择意图提交，不在模拟器中模拟真实游戏执行。该动作不是搜索树中的普通合法动作，因此 `CheckAction` 不把它当作普通模拟动作。
- 没有外部包时，普通动作不得携带 `ChoicePacketId` 或 `OrderedChoiceTargets`；内部 `PendingPlayChoice` 的 `ChooseOption` 同样不得混入客户端选择包字段。
- `ExplainSource` 与 `SourceFailure` 明确报告外部 CHOICE 阻塞，不再错误地报告一张可使用的牌。

### 伤害来源可追溯

- 在 `SimulationEvent` 增加可选 `AttackingWeapon`，同时用于 `Damage` 与 `ShieldBroken` 事件。英雄攻击时 `Source` 仍是英雄 ID，`AttackingWeapon` 才是实际参与战斗的装备武器。法术、技能和无武器攻击保持空值。
- 这使得 Poisonous / Lifesteal / Overkill / Honorable Kill 的诊断可区分攻击者和所持武器，且不影响已有 `Source` 语义。

### 必过的回归

- 外部待决时禁止合法普通出牌，并在外部选择提交及 checkpoint 恢复之后继续禁止普通动作；确认输入未变。
- 普通出牌和内部选项拒绝夹带 `EntityChoices` 的包 ID / 有序目标。
- 吸血武器：`Damage.Source == Hero`，`Damage.AttackingWeapon == Weapon`，治疗事件仍正确指向武器和英雄。
- 范围治疗：先结算全部治疗，按进场次序产生 `Overhealed` 与对应触发，同一输入重放事件一致。
- 有序选择预演：普通局面与恢复后的 checkpoint 输出事件相同、结果相同、原始局面不变。

## 尚待继续的阶段二工作

- 精细化对伤害与治疗的事件批次、死亡波次、触发注册时点和同阶段随机事件做差分测试，特别对照 2026-11-05 经典源码/固定录像。
- 为 `PendingPlayChoice` 的多轮 Adapt、死亡导致的提前结束与 `PendingChoice` 的外部权威快照切换建立独立更多的 golden replay。
- 测试覆盖达到稳定基线后，再以逐步抽取的方式分离 `DamageResolver`、`HealResolver`、`TriggerDispatcher` 与 `ChoiceResolver`。避免机械拆分类改变效果栈和 LIFO 时序。
- 真实游戏的客户端内虚拟输入与 `StandaloneGameMod` 读取适配不在本阶段；不使用 Windows 系统鼠标，不直接调用炉石网络发送 API。

## 运行检查

```powershell
dotnet build SilverfishStandalone.sln -c Release
dotnet test SilverfishStandalone.sln -c Release --no-build
```

以本阶段分支 GitHub Actions 中的完整 offline replay 和发布检查作为最终验收，不以单独编译通过替代测试通过。

## 第二批：多轮选择与终局一致性

- 新增多轮 Adapt 固定回归：按选择回合移除已死亡接收者，活着的接收者继续下一轮；第一轮全灭时直接完成出牌；来源随从死亡时不再产生虚假的新选择。
- 新增中间 checkpoint 的 JSON 序列化、重新创建模拟器和选项恢复测试；检验选项轮次、已付费用、事件轨迹、旧状态不被修改。
- 锁定普通伤害触发、荣誉消灭奖励、死亡结算/亡语的可观察顺序，并核对同一输入重放一致。
- 修复 `GameState.Finish`：终局会清除 `PendingPlayChoice` 与 `PendingChoice`，防止产生无法再经 `RestoreGame` 校验的“已结束但仍待决”状态。离线 TARGET 选择预演若前一目标导致对局结束，就不再结算后续有序目标；保留已产生的提交意图与事件轨迹。
- 不改变真实客户端 CHOICE 的网络提交规则：这里只验证模拟结果与存档的内部一致性。

测试前请先查看 GitHub Actions 的阶段二分支最新运行结果；旧提交的成功不能替代新增测试的最终验收。

## 第三批：伤害事件观察者的注册时点

### 已复现的旧行为

- 一次 `DamageTarget.EnemyMinions` 范围伤害先记录随从 A 与 B 的伤害，随后 A 的伤害触发召唤出观察者 C。旧代码在执行 B 的 `Dispatch` 时才调用 `CaptureListeners()`，导致刚进场的 C 收到了 **发生在进场前** 的 B 的伤害事件。
- `RandomDamageMode.DistinctBatch` 同样存在该问题，并非只有普通 AOE。
- `RandomDamageMode.Sequential` 则相反：第二次命中确实发生在第一次触发效果之后，新观察者应看到第二次伤害。

### 修正范围与设计边界

- `PushOccurrence` 为已发生的伤害注册不可变触发队列：普通伤害、荣誉消灭/超杀，以及武器归因对应的致命奖励，均在事件入栈时匹配当时的观察者及事件目标。
- `Dispatch` 继续使用原先的 LIFO 顺序执行已注册的队列；`Activate` 仍检查 `IsCurrent`、活体条件和触发条件，因此沉默、离场、装备替换不会令已失效的触发器重新生效。
- 此变更只固定 **伤害事件** 的匹配时点；召唤、抽牌和外部选择的专用触发路径暂时保持原有行为，避免无对照地改变它们的结算语义。

### 回归与验证

- AOE 批次：事件中的两个目标先受伤，再召唤的 C 不响应此前的任何一个伤害事件。
- DistinctBatch：完整枚举两种随机击中次序，两种结果中 C 都不能追认已经发生的伤害。
- Sequential：两次真实依次发生的随机伤害，C 在第一次触发后进场，能够响应第二次。
- 测试先行的复现提交 `ea5626e` 在 CI #44928600612 中出现预期的两个失败，证明此前是真实行为缺陷而非仅增加了无效测试；修复后以最新一次 CI 为验收依据。

以上均属于离线模拟器，不涉及实机输入、注入执行或直接发包。
