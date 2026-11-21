# EntityChoices 有序目标选择：第一阶段

这部分基于用户提交的《炉石传说卡牌多目标选择机制 — 技术分析报告》（其标注的客户端源码版本为 **20260805**）。后续如果用于 2026-10-13 或更新客户端，需要再与该版本的 Network / GameState / InputManager / ChoiceCardMgr 核对。

## 协议边界

- `OPTION_TARGET`：从 `Network.Options` 选一个目标，走单一 Option 的目标字段。**不要**将它视为 `EntityChoices`。
- `CHOICE`：单独的 `EntityChoices` 包，包含 `ID`、`ChoiceType`、`Source`、`PlayerId`、`Entities`、`UnchoosableEntities`、`CountMin` 和 `CountMax`。
- 选择由玩家按点击顺序追加；已选实体再次点击会移除，其后元素顺序前移。确认时按原顺序提交实体 **ID** 而非 CardId。
- 客户端所报告的 `IsSingleChoice()`：`CountMax == 0 || (CountMin == 1 && CountMax == 1)`。**只用于 UI / 自动提交分支**，不将 `CountMax=0` 解释为能选一个实体。
- `CountMin=0, CountMax>0` 可直接确认空列表；`CountMax=0` 只能确认空列表。
- 单选就绪标记只代表可以考虑自动提交；`DoNetworkChoice` 中仍有其他客户端判断，不通过此库直接发送网络请求。
- 所有外部包数据必须验证：不接受重复 ID、未提供的目标、禁选目标、超过上限的目标或不满足最小数量的提交。
- 确认时检查选择包 ID，拒绝把旧包的有序目标提交到新包。

## 已实现

- `Silverfish.Domain.EntityChoicePacket`：客户端选择包及边界校验。
- `Silverfish.Domain.EntityChoiceSelection`：不可变点击/取消/确认状态机。
- `Silverfish.Cli choice <选择包.json>`：对离线 CHOICE 输入重放点击与确认，输出选择顺序与提交内容。
- `tests/Silverfish.Domain.Tests/EntityChoiceSelectionTests.cs`：覆盖顺序变更、单/多选、禁选、数量和过期包 ID。

```powershell
dotnet run --project src/Silverfish.Cli -c Release -- choice samples/entity-choice-targets.json
```

预期 `selectedEntityIds` 和 `submission.entityIds` 为 `[11,33,22]`。

## 仍未实现

本轮是**CHOICE 包适配及有序选择状态机**，不发送任何游戏网络消息，也**不宣称**支持自动预演发现、泰坦、定向多目标卡牌结算。

下一阶段：把 `EntityChoicePacket` 与包含 `matchId` / 快照序号的真实对局响应绑定，在 `GameState` 保存可恢复的 pending choice；使用 `ChooseOption` / 连续选择动作接入模拟器的暂停/恢复栈。需要在真正的选择包录制样本上验证执行顺序、重入、同名实体以及异步失效处理。对 `OPTION_TARGET` 保持独立适配。

## 第二阶段：持久待决状态（PendingEntityChoice）

本阶段在真正的模拟器局面中增加可选的 `pendingChoice` 字段；旧的 `stateVersion=1` 存档省略该字段时保持兼容。状态同时保存原始选择包，以及已提交的有序实体 ID。

- `GameSimulator.CreateGame / RestoreGame` 能载入尚未确认或已经确认的 `PendingEntityChoice`，并检查玩家、选择包、数量、重复项和过期包 ID。
- `SimulationActionKind.ChooseOption` 的 `choicePacketId` 与 `orderedChoiceTargets` 是独立的选择动作字段，与出牌动作的单目标 `Target` 不混用。
- `GameSimulator.Apply` 对 `ChooseOption` **只记录提交、发出 `ChoiceSubmitted` 事件**。游戏状态中原有血量、实体、资源、回合及区域不会变化。状态仍然带有 `PendingChoice.Submitted`，必须等待**权威的新局面快照**才能继续模拟；这是有意为之。
- 不会生成假想的后续动作：待决或已提交选择期间 `LegalActions` 返回空；`SimulationDecisionEngine` 对不能准确估值的选项明确放弃选择。
- `checkpoint` 可以序列化已提交的选择；`simulate` 能从存档恢复。此阶段不包含真正的卡牌效果工作栈暂停/恢复，亦不连接网络。

```powershell
dotnet run --project src/Silverfish.Cli -c Release -- simulate samples/simulation-pending-choice.json
dotnet run --project src/Silverfish.Cli -c Release -- checkpoint samples/simulation-pending-choice.json
```

后续必须在已录制的真实选择包和规则上实现 `PendingResolution` 续跑工作栈，以及 `GENERAL` / `TARGET` 的不同价值估算；在没有这些规则之前**不**将提交候选的能力称为已经能够自主选牌。

## 执行路径澄清：不通过 API 提交

使用 `SimulationActionKind.ChooseOption` 是描述 AI 意图，不等于执行一次网络提交。选择真实执行时应交给 `UiChoiceInputPlanner` 构造按顺序点击实体、确认按钮检测和新状态校验的输入计划。所有实际操作由游戏内 `IInput / InputCollection` 虚拟输入执行器完成（参考 Ember 的 `VirtualInput`、`VirtualPointer`），不移动 Windows 系统鼠标，禁止使用 `SendInput` 等桌面输入驱动，也禁止直接调用游戏协议或客户端 `Network.SendChoices`。详见 [UI_INPUT_EXECUTION.md](UI_INPUT_EXECUTION.md)。

## 第三阶段：已知效果离线预演（非权威状态）

本阶段新增 **显式已知规则** 下的选择效果预测，但 `Apply(ChooseOption)` **仍然只提交意图**，继续等待新的权威快照；`SimulationDecisionEngine` 对未知选择保持拒绝猜测。预演不会更改传入的 `GameState`，返回的仍是标记为已提交的**假想分支**，不能作为客户端真实状态或后续动作的依据。

- `ChoicePreviewRules` 是每个选择包的**独立规则输入**，必须匹配 `ChoiceId` 和 `EntityChoiceType`。若没有覆盖所有可选实体、缺少来源卡、或者不是已支持的 `GENERAL` / `TARGET`，预演拒绝执行。
- `GENERAL` 可以为每个被展示的 EntityId 绑定明确的 `CardEffect` 数组；例如，已核实的发现候选通过 `CreateCardEffect("已知CardId")` 加入假想手牌。**不能**仅根据 CHOICE 的实体 ID 推断其 CardId 或效果。
- `TARGET` 在已存在于局面中的场上角色之间执行有序选择，按提交顺序逐个结算明确的 `PerTargetEffects`；每次结算均走现有伤害、触发器、死亡和亡语流程。 `OPTION_TARGET` 仍然是另外一种动作，不混用。
- `GameSimulator.PreviewChoiceDistribution` 利用模拟器同一套概率分支引擎，尝试精确枚举随机结果；预算不足时返回采样分布并标记非精确。`PreviewChoice` 只允许结果唯一且概率精确为 1 的选择。
- `EntityChoiceForecastPlanner` 枚举所有合法的**有序**选法（包括允许的空选），按用户提供的局面评估函数对精确的选择效果分布打分；若组合数超过 `candidateLimit`，或某个选择的随机结果无法精确枚举，则**拒绝返回可能误导的最佳选择**。
- 真实客户端发来下一个连续选择包后，可以用**新权威快照**再预演下一轮；本阶段没有自动伪造下一次 CHOICE 包，也没有通用的服务端结算暂停/恢复栈。特殊发现、泰坦、目标顺序和复杂效果仍须按真实卡牌逐张补规则并验证。

示例（调用方已先核实发现候选和实际卡牌效果）：

```csharp
var rules = new ChoicePreviewRules(choiceId, EntityChoiceType.General)
{
    PerOptionEffects = ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty
        .Add(candidateA, [new CreateCardEffect("KNOWN_CARD_A")])
        .Add(candidateB, [new CreateCardEffect("KNOWN_CARD_B")])
};
var forecast = new EntityChoiceForecastPlanner(simulator, SimulationMaterialEvaluator.Evaluate)
    .Plan(pendingGameState, rules);
if (forecast.IsComplete && forecast.Action is { } choice)
{
    // 这里只产生离线决策；Apply(choice) 仍不会自动执行卡牌效果或操作游戏客户端。
    var intent = simulator.Apply(pendingGameState, choice);
}
```

此阶段的 `CreateCardEffect` 是已知规则预演的具体例子，不代表所有发现都必然走这一种效果路径，也不证明程序能从未知客户端包自动识别候选卡。

