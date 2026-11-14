# 经典银鱼独立化进度（2026-11-14）

## 已通过：完整离线闭环

**GitHub Actions 运行**：[43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029)（旧版参考源码，**不是**用户上传的最终版）。

此轮以下步骤全部通过：

1. 从参考仓库自动选择性提取经典 Silverfish，并运行幂等的宿主依赖适配。
2. `dotnet build src/Silverfish.Classic/Silverfish.Classic.csproj -c Release`：成功；不需要旧版 Hearthbuddy.exe、Triton DLL、GreyMagic DLL。
3. 初始化真实经典 `CardDB`：成功。
4. `ClassicBridge` 单测：**9/9 通过**。
5. 原版 `Ai.autoTester` 从 `samples/classic-simple-lethal-zh.txt` 完成真实搜索：成功。
6. `samples/classic-snapshot-lethal.json` 经受限 `ClassicSnapshotFormatter` 转为原版 BoardTester 输入，再调用原版 `Ai`、由 `ClassicDecisionAdapter` 匹配为 `BotDecision`：成功。
7. 编译产物和日志已上传为 Actions Artifact：`classic-silverfish-net8`、`classic-silverfish-build-logs`。

固定盘面实际决策：

```json
{
  "matchId": "offline-classic-snapshot-001",
  "snapshotSequence": 8,
  "engine": "classic-silverfish",
  "chosenAction": {
    "id": "attack-face",
    "kind": "attack",
    "sourceEntityId": 11,
    "targetEntityId": 1
  },
  "score": 23121
}
```

这是原版银鱼得出的随从攻击对方英雄（4 攻对 3 血），不是 `BaselineStrategyEngine` 的虚拟评分输出。

## 真实完成边界

当前工作证明**老版经典银鱼能够脱离原主程序完成离线决策**。它不证明：

- 2026-11-05 最新版已经在 CI 通过编译；
- 任意真实炉石对局状态都能通过 `ClassicSnapshotFormatter` 转换；
- 可直接执行游戏动作或完成新的完整 UI。

目前受限格式使用固定的中文卡牌名、测试用实体 ID、已知手牌/牌库/奥秘信息。拒绝未表示的卡牌状态或复杂局面，详见 `docs/CLASSIC_SNAPSHOT_ADAPTER.md`。

## 最新版来源

- 用户原 ZIP：`Hearthbuddy1.7.1(1).zip`（2026-11-05），SHA-256 `1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071`。
- 仅包含经典银鱼源码和文本数据的独立工作包 SHA-256：`ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c`。
- Actions 已支持从本仓库指定 Release 资产加载并验证独立源码包。该资产目前尚未上传，且仓库是公开的；若有公开范围顾虑，请使用私有构建来源。

## 下一阶段

1. 对 2026-11-05 最终版执行完全相同的源码构建、BoardTester 与 JSON 回归，并处理只在最新版出现的差异。
2. 为 `CardResearchLab/StandaloneGameMod` 的 `state.snapshot` 设计完整的输入版本检查、实体映射与特征缺失诊断，不能再依赖简化的中文 BoardTester 文本接口。
3. 与独立 `Libmem.NET` 进程能力整合，最终由 Vue 3/WebView2 UI 显示局面、计算建议、回放与日志。

不引入 Silverfish V2。
