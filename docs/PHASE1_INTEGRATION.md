# Phase 1 integration / 第一阶段分支整合

日期：2026-11-22

## 基线与范围

- 基础分支：`feat/standalone-foundation` @ `fcc904552b3713fec7dd28ce6477b71518f22b4d`
- 功能分支：`feat/ordered-entity-choices` @ `64a39058e8e6906058ddabb9770e57b5baef4461`
- 集成分支：`feat/phase1-integration`。首次集成提交 `11093954891718977797da6a7cf7448b5141d3db` 是以上两分支的双父提交。
- 保留基础分支的统一 `PlayReq`、`GameSimulator.EvaluateBranch`、待决出牌恢复、经典包逐卡开发约定和独立卡牌数据。
- 纳入有序 EntityChoices、外部选择包校验、已知效果预演、UI 虚拟输入意图规划、治疗/过量治疗与伤害关键词回归。

## 两种 pending 状态不能互相覆盖

- `GameState.PendingPlayChoice`：**模拟器内部**的暂停结算点。由 `ChooseAfterPlay` 产生；模拟 `ChooseOption` 在原规则栈继续执行，保留已支付费用和已触发的前置效果。
- `GameState.PendingChoice`：**外部客户端**公布的 `EntityChoices` 包。包含包 ID、顺序选择、数量限制和已提交意图；这种 `ChooseOption` 只产生 `ChoiceSubmitted`，不修改游戏实体、资源或直接执行单卡效果。必须等待新的权威快照。
- `OPTION_TARGET` 是普通出牌/技能目标，不是 `EntityChoices`；两套候选数据和动作字段不能混用。
- 外部 `PendingChoice` 存在时，普通 `LegalActions` 返回空；内部 `PendingPlayChoice` 在没有外部包时仍会生成内部选项。
- 存档同时保留两类字段，旧版省略新增字段的存档仍然可以读取。
- `UiChoiceInputPlanner` 只生成 `ClickEntity → ConfirmIfVisible → AwaitFreshSnapshot` 语义步骤。**未实现**真实虚拟输入、网络发送或 Windows 鼠标驱动，不能把本地提交意图当成实际游戏确认。

## 合并时特别保留的行为

1. 伤害入口继续使用统一的效果量计算、圣盾附魔层移除和剧毒死亡事件顺序；新增伤害来源、武器关键词、吸血、超杀、荣誉消灭及详细伤害事件。
2. `GameSimulator.Random` 继续接收基础分支的 `resolvesEffects` 合法性判定；为已知 CHOICE 效果预演复用相同的精确随机分布与采样引擎。
3. `GameState` 的实体与区域不可变更新同时传播两类 pending 状态，且仍保留英雄生命变化等计数。
4. CLI 同时保留 `explain`、`choice`、`simulate`、`plan`、`decide` 和 `checkpoint`。

## 验收门槛

```powershell
dotnet build SilverfishStandalone.sln -c Release
dotnet test SilverfishStandalone.sln -c Release --no-build
python tools/sim_comments.py --check
```

同时运行 `.github/workflows/ci.yml` 的离线回放、CHOICE 顺序选择和待决选择存档恢复。经典源码的导入/编译另由独立诊断工作流以及 2026-11-05 私有正式源码工作流核验，不应把公开诊断误写成所有私有历史卡牌均已验证。

## 下一步

本阶段集成稳定后，统一 PR 文档及分支入口，再决定是否将 `feat/standalone-foundation` 经 PR 合入 `main`。不要直接将未验证的规则骨架视为可执行效果；实时游戏快照适配、游戏内虚拟输入、复杂选择的真实效果和搜索质量优化仍在后续阶段。
