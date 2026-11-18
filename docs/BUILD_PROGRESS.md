# 经典银鱼独立化进度（2026-11-18）

## 2026-11-05 正式基线

私有 Windows Actions 使用 SHA-256 锁定的 2026-11-05 源码工作包，完成了 .NET 8 编译、`CardDB` 加载、`ClassicBridge`（9/9）、原版 `Ai.autoTester` 和非法输入回归。构建产物与日志已归档。[私有 Actions #44256337489](https://github.com/xiaohei7972/Hearthbuddy_Silverfish/actions/runs/44256337489)

新版固定牌面返回 `attackWithMinion`（实体 11 攻击实体 1）。同一局面的 JSON 决策为 `attack-face`，评分 23141。无效文本牌面返回退出码 2。

## 旧版参考源码回归

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

## 回归覆盖范围

目前受限格式使用固定的中文卡牌名、测试用实体 ID、已知手牌/牌库/奥秘信息。拒绝未表示的卡牌状态或复杂局面，详见 `docs/CLASSIC_SNAPSHOT_ADAPTER.md`。

## 最新版来源

- 用户原 ZIP：`Hearthbuddy1.7.1(1).zip`（2026-11-05），SHA-256 `1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071`。
- 已验收的私有 Release 源码工作包 SHA-256 为 `4d6944304c0d224d366afda1691f0429c5d3cffbc0daae18259310a7f6f95d92`。

## 下一阶段

1. 以 2026-11-05 版独立构建与固定决策回归为基准，分阶段迁移状态模型。
2. 为 `CardResearchLab/StandaloneGameMod` 的 `state.snapshot` 设计完整的输入版本检查、实体映射与特征缺失诊断，不能再依赖简化的中文 BoardTester 文本接口。
3. 与独立 `Libmem.NET` 进程能力整合，最终由 Vue 3/WebView2 UI 显示局面、计算建议、回放与日志。

不引入 Silverfish V2。
