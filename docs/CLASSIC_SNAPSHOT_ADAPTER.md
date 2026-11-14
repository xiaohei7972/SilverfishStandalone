# 经典银鱼 Snapshot 适配：第一阶段（离线、受限）

新增 `Silverfish.ClassicBridge.ClassicSnapshotFormatter`，仅用于验证新 `GameSnapshot` 合同可以驱动**真正的**经典银鱼 `Ai.autoTester`。它不是实时游戏读取器，也不能代表所有炉石机制已受支持。

## 调用链

```
GameSnapshot JSON（schemaVersion=1）
  -> SnapshotValidator
  -> ClassicSnapshotFormatter
  -> 老 BoardTester（中文固定牌面格式）
  -> Hrtprozis / Handmanager / Playfield
  -> Ai.autoTester (classic)
  -> Ai.bestmove
  -> ClassicDecisionAdapter
  -> BotDecision（matchId、sequence、候选动作 ID）
```

## 已表达的信息

- 我方当前可用水晶/最大水晶。
- 双方英雄的职业、生命、护甲。
- 当前牌面的随从名称、攻击力、生命（仅采用经典 BoardTester 的人工实体 ID）。
- 敌方初始手牌数量和牌库数量，明确传入，避免默认假设。
- 已由其他可信来源枚举的 `LegalActions`；ClassicDecisionAdapter 对匹配非唯一或不存在的动作拒绝输出。

## 限制：必须拒绝的情况

目前旧版 BoardTester 的中文便捷格式不能表达随从不可攻击、嘲讽、圣盾、牌库/手牌详细内容、多种卡牌效果、未识别的英雄技能/装备和复杂状态，也没有完整区域/tag 的输入。适配器会拒绝这类已知无法表达的状态，而不是悄悄忽略。两位英雄实体 ID 固定为 0、1；我方第一只随从实体 ID 固定为 11，之后由旧 BoardTester 按位置生成 22、33 等；敌方位置 ID 是 101、202 等。

因为这不是游戏实体真实 ID 体系，**此阶段只能使用固定离线样本，不能直接把真实 `state.snapshot` 喂入旧 `BoardTester` 并执行动作。** 后续需以实体 ID 原样保留的 `Hrtprozis` 和 `Handmanager` 结构化适配器替代这条文本中间层。

新增示例：`samples/classic-snapshot-lethal.json`。在真正经典源码已导入的工作区运行：

```powershell
dotnet run --project src/Silverfish.Classic.Smoke/Silverfish.Classic.Smoke.csproj -c Release -- samples/classic-snapshot-lethal.json
```

当测试通过时，控制台会出现 `CLASSIC_BESTMOVE_JSON`（原版算法计算的动作）和 `CLASSIC_DECISION_JSON`（映射到合同的结果）；任何不匹配的动作均使测试失败。

目前该路径通过 GitHub Actions 对较旧公开参考源码测试；2026-11-05 的正式源码仍待通过相同构建/离线回归验证。
