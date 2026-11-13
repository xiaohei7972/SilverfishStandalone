# 经典银鱼 Actions 编译进度

日期：2026-11-14。

## 已经完成的真正编译

- 源码模式：`old-reference-diagnostic`。
- 源：`xiaohei7972/Hearthbuddy_backed`（较旧的经典银鱼参考源码，**不是** 2026-11-05 版）。
- Actions 运行：https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43527236305
- `dotnet build src/Silverfish.Classic/Silverfish.Classic.csproj -c Release`：**成功**。
- 旧版源码编译错误从多处语法/合并冲突缩减到旧 `Ai.cs` 的 2 处宿主单例依赖，再改为独立 `Hrtprozis.updateCThunInfo` 后，核心类库通过编译。
- 原工作流的 `CardDB` smoke 失败并不是 C# 或 CardDB 本身的错误，而是 Actions 启动命令将 `--nologo` 作为应用的文件路径参数传入，导致 `FileNotFoundException`。
- **已通过**：GitHub Actions [43527562637](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43527562637) 的经典 `Silverfish.Classic` .NET 8 构建和 `CardDB` 离线 smoke 均成功。
- 已移除这个误传参数，并补充编译成功后的 DLL Artifact 上传步骤。新的 smoke 结果要以新的 Actions 运行记录为准。

## 为什么这仍不是最终验收

真正的源码基准为用户上传的 `Hearthbuddy1.7.1(1).zip`（2026-11-05）。其原版 SHA-256：

`1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071`

最新独立源码工作包 SHA-256：

`ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c`

**2026-11-05 版尚未交由 Actions 编译**，因为它仍在会话文件/独立工作包内，尚无供 GitHub runner 下载的可信来源。工作流已支持从本仓库 Release 资产加载这个工作包、强制验 SHA-256，再编译源文件。不要将旧参考版本通过编译误认为最新版本通过。

## 接下来的验收

1. 新旧版均先编译真实 `Ai`、`Hrtprozis`、`Playfield`、`CardDB` 及卡牌脚本。
2. `Silverfish.Classic.Smoke` 运行 `CardDB` 初始化；不应再误传命令行参数。
3. 加入一个有效的 `BoardTester` 固定局面，运行 `Ai.autoTester` 并记录 `bestmove`。
4. 最终再实现 `GameSnapshot` 到经典银鱼输入格式的转换和 UI。

没有引入任何 Silverfish V2 代码。

## 新增：实际 AI 离线牌面决策测试

已经提交 `samples/classic-simple-lethal-zh.txt`，包含一个最小测试：我方 4 攻可攻击随从，对手英雄 3 血。专门用 `Silverfish.Classic.Smoke` 调用原版 `Ai.autoTester`，目标是验证它能独立搜索棋盘并输出 `bestmove`。该测试的完成结果单独以 GitHub Actions 的 `Run classic AI autoTester against a fixed offline board` 步骤判定，不能用基础评分器或 CardDB smoke 的通过来代替。
