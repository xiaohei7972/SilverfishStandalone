# 独立银鱼输入执行架构：游戏内虚拟输入（不抢占 Windows 鼠标）

## 已确定的要求

- **独立银鱼**：外部 .NET 8 进程，专注状态建模、规则模拟、搜索和行动意图。
- **执行端**：必须是游戏内的虚拟输入源（`IInput → InputCollection → UniversalInputManager / InputManager`），由 Unity/Mono 模组托管。
- 不使用 `SendInput`、`SetCursorPos`、`mouse_event`、`PostMessage` 模拟桌面输入；不移动系统光标、不抢占用户正在操作的其他程序。
- 不通过 `Network.SendOption`、`Network.SendChoices`、`GameState.SendOption`、`GameState.SendChoices` 等接口直接提交游戏动作。
- 真正游戏是否接受了操作，要通过只读游戏状态和选择界面变化验证，不能把 `GameSimulator.ChoiceSubmitted` 误认为服务器确认。

## 已核对的参考实现（Ember）

仓库内部相关职责如下（作为架构参考，不从私有仓库复制实现）：

| 参考类 | 实际职责 |
| --- | --- |
| `VirtualInput` | 实现 `IInput`，虚拟输入源插到 `InputCollection.m_Inputs[0]`，仅在启用时报告鼠标位置、按下/按住/释放及键盘状态；关闭后回退原生输入 |
| `VirtualPointer` | Unity 协程跨帧产生 Click、Drag、Hover、Esc；使用 `CoroutineRunner`，清除单帧信号并在完成/异常时禁用虚拟输入 |
| `PositionHelper` | 将 Card/Actor/Collider 世界坐标投影到正确屏幕坐标；普通手牌优先使用 `ZoneHand` 的 stand-in 命中代理，避免点中相邻卡牌 |
| `ActionExecutor` | 接收外部 AI 动作，按类型分派给对应 Handler；执行互斥、取消、重试、状态刷新 |
| `ChoiceHandler` / `TargetSelect` | 选择候选、拖拽、按顺序点击多个实体，遇到真实确认按钮时点击 |
| `Confirm` | 比较 `OptionsId` / `ChoiceId`，检查界面消失或切换，判断输入是否被接受 |

Ember 的虚拟输入会在必要时维护 `PegUI` 的焦点标记，但**不要求切换 Windows 系统鼠标的位置**。它仍依赖游戏引擎每帧正常更新；游戏最小化、后台降帧或更新暂停时需要实测，不能保证任意后台环境都可运行。

## AI 与游戏内适配器之间的边界

```text
Unity 客户端的只读 GameState / Network.Options / EntityChoices
   → 快照或候选动作（IPC）
   → SilverfishStandalone / GameSimulator / AI Search
   → UiChoiceInputPlanner / GameAction 行动意图
   → IPC 下发给游戏内执行器
   → 游戏内 IInput 虚拟输入源 + Unity 协程模拟输入
   → 查询当前 Card / Collider / ConfirmButton 的命中点
   → 逐帧点击、拖拽、确认
   → 读取 OptionsId / ChoiceId 或最新游戏快照确认结果
   → 成功再搜索下一步；失败则重新观察
```

目前 `SilverfishStandalone` **只实现行动意图和 `UiChoiceInputPlanner`**，还没有上述 IPC 通道及游戏内执行器，不能宣称已能实际操作炉石。这条链路后续接入游戏内模组时应当复用或借鉴 Ember 的输入代码，而不是重新引入 Windows 鼠标驱动。

## CHOICE / OPTION_TARGET 分离

- `OPTION_TARGET`：仍是单次行为中的一个目标（出牌、攻击、技能等），由 UI 卡牌拖拽 / 目标指针完成。
- `CHOICE`：独立选择包，以 `ChoiceId` 锁定当前选择，通过 `OrderedChoiceTargets` 保留多目标点击顺序。
- `UiChoiceInputPlanner` 输出 `ClickEntity(entityId)` → `ConfirmIfVisible` → `AwaitFreshSnapshot`。执行端在每次点击前核对新的 `MatchId`、`SnapshotSequence`、`ChoiceId`，并重新计算实际目标命中位置。
- `ConfirmIfVisible` **只允许查询和点击游戏内真正存在、已启用的“确认”按钮**。不要把“隐藏/收起”按钮当确认，也不要单凭 `IsSingleChoice()` 推断 UI 是否已经提交。
- 选择后检查 `ChoiceId` 消失或切换、`OptionsId` 变化及最新快照。对连续发现不能只凭屏幕卡牌消失来判断成功。

现有 `PendingEntityChoice.Submitted` 和 `ChoiceSubmitted` 仅表示离线模型接受了一个选择意图；它们**不证明虚拟输入已执行，也不证明服务器接受**。未来可在保持存档兼容的前提下改名为 `Planned`。

## 执行端仍待实现

- 不占系统鼠标的 **Unity/Mono `IInput` 注入适配器**，以及与外部独立银鱼的动作通讯
- 点击、拖拽、确认和按钮状态感知，覆盖发现、多目标、泰坦、抉择、留牌
- 输入互斥、关闭时还原虚拟状态、取消与超时恢复、角色或局面变化后的失败处理
- 游戏后台和失去焦点时的实机验证

现在不需要把整个 Ember Bot 复制到 SilverfishStandalone，也不在外部 AI 项目中直接引用 Unity 客户端类型。
