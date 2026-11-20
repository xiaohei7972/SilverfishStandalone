# EntityChoices 有序目标选择：第一阶段

这部分基于用户提交的《炉石传说卡牌多目标选择机制 — 技术分析报告》（其标注的客户端源码版本为 **20260805**）。后续如果用于 2026-10-13 或更新客户端，需要再与该版本的 Network / GameState / InputManager / ChoiceCardMgr 核对。

## 协议边界

- `OPTION_TARGET`：从 `Network.Options` 选一个目标，走单一 Option 的目标字段。**不要**将它视为 `EntityChoices`。
- `CHOICE`：单独的 `EntityChoices` 包，包含 `ID`、`ChoiceType`、`Source`、`PlayerId`、`Entities`、`UnchoosableEntities`、`CountMin` 和 `CountMax`。
- 选择由玩家按点击顺序追加；已选实体再次点击会移除，其后元素顺序前移。确认时按原顺序提交实体 **ID** 而非 CardId。
- 客户端所报告的 `IsSingleChoice()`：`CountMax == 0 || (CountMin == 1 && CountMax == 1)`。**只用于 UI / 自动提交分支**，不将 `CountMax=0` 解释为能选一个实体。
- `CountMin=0, CountMax>0` 可直接确认空列表；`CountMax=0` 只能确认空列表。
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
