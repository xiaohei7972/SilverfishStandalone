# 独立银鱼动作执行边界：仅模拟用户输入

## 重要约束

独立银鱼的职责是 **观察局面 → 搜索/评估 → 产生行动意图**，不是直接发送炉石协议或调用游戏客户端的提交方法。

**禁止用作执行路径**：
- 直接调用 `Network.SendOption()` / `Network.SendChoices()` / `SendChoices()`，以及类似网络/API 提交接口
- 修改客户端内部选项数据来伪装一次已执行的操作
- 把 `ChoiceSubmitted` 事件理解为真实游戏已经接受了提交

**预期的执行路径**（以后由独立 Input Executor 适配）：

```text
读取游戏状态 / CHOICE 选择包（只读）
   → Silverfish AI 生成选择/行动
   → UiChoiceInputPlanner 输出语义化 UI 步骤
   → 执行器在窗口中按实体 ID 定位当前可见卡牌/目标
   → 通过系统级模拟输入点击 / 拖拽 / 键盘操作
   → 检查确认按钮，若可见则模拟点击
   → 等待新快照，核对选择包是否已消失、牌局是否变化
   → 成功才继续决策；失败或窗口变化则停止并重新观察
```

不能预先固定屏幕坐标。执行器应获得当前窗口位置、分辨率、缩放、实体 ID 对应的实时可点击区域和前景状态。每次鼠标操作前重新核查 `matchId`、`snapshotSequence`、`choicePacketId`，避免误点下一轮的候选。

## 当前实现

`Silverfish.Core.UiChoiceInputPlanner` 将已验证的 `SimulationActionKind.ChooseOption` 转换成：
- 按 **OrderedChoiceTargets** 顺序的 `ClickEntity(entityId)`
- `ConfirmIfVisible`：执行器必须实际观察 UI，不能只靠 `IsSingleChoice()` 判断是否存在确认按钮；该步骤绝不执行协议调用
- `AwaitFreshSnapshot`：只有从新的权威状态确认交互成功后，才能继续规划下一步

`PendingEntityChoice.Submitted`、`ChoiceSubmitted` 是现阶段**内部记录选择意图**的旧字段/事件名称，仅供离线回放与 checkpoint 使用；**不是** OS 输入的完成状态，更不是服务端确认。下一步可以在不破坏旧存档的前提下将该含义重命名为 `Planned` 并迁移数据。

## 仍需实现

- Windows x64 `SendInput` 的输入驱动、窗口/焦点控制与坐标变换
- 从实际界面/客户端只读状态将实体 ID 映射到可点击区域，并识别确认、取消等按钮
- 按真实 UI 回执处理单选、多选、发现、泰坦和连续选择
- 失败重试、异常中断、输入后新局面对账；禁止在未观察到成功时连续点击其他动作

当前不接入 `StandaloneGameMod`、不触发实时鼠标操作；也**不承诺**仅靠现有 CHOICE 数据已能定位 UI 目标。
