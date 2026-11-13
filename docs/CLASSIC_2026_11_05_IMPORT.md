# 经典银鱼独立化：2026-11-05 实际源码基准

## 已核实的源码包

来自用户在对话中提供的文件 \`Hearthbuddy1.7.1(1).zip\`。

- 压缩包 SHA-256：\`1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071\`
- ZIP 顶层目录：\`炉石兄弟-2026.11.05/\`
- 压缩包内容：19,405 个条目，解压总大小约 137 MB。
- 经典银鱼目录：\`Routines/DefaultRoutine/Silverfish/\`
- 经典银鱼 C# 源文件：19,141 个，其中 19,058 个在 \`cards/\` 下。
- **无 Silverfish_v2 目录。** 本项目不迁移 V2。
- 银鱼卡牌数据库：\`Silverfish/data/CardDefs.xml\`，约 23 MB。

## 已执行的第一轮源码解耦

使用 \`tools/import_classic.py\` 从实际 ZIP 提取：

| 项目 | 结果 |
|---|---|
| 导入源码及文本数据 | 19,166 文件 |
| 其中 C# 源码 | 19,135 文件 |
| 已实施路径、using 或 API 替换 | 27 文件 |
| 暂时隔离的源文件 | 6 文件 |
| 被复制的 Hearthbuddy 主程序 / Triton DLL / GreyMagic DLL | **0** |
| 独立 .NET 8 工程文件 | \`src/Silverfish.Classic/Silverfish.Classic.csproj\` |
| 独立 smoke 测试入口 | \`src/Silverfish.Classic.Smoke/\` |

已处理的依赖边界：
- \`Triton.Common.LogUtilities.Logger\` / \`log4net.ILog\` 改用仓库内的无 UI 日志兼容接口。
- \`TritonHs.GameState.GetEntity\` 入口替换为离线 \`DetachedEntityRegistry.GetEntity\`，由独立状态适配器负责喂数据。
- \`Silverfish.Instance.BehaviorPath\` 换为 \`ClassicBehaviorPaths.Paths\`。
- \`DefaultRoutineSettings.Instance.MaxWide\` 改从经典 \`Settings\` 读取。
- 在非 \`APPLICATION_MODE\` 下，原 UI 更新代码不参与编译，不需要 WPF UI。

暂不纳入独立编译范围的文件：
- \`Helpers/Extensions.cs\`：主程序 \`HSCard\` / 输入交互扩展。
- \`ai/Zones.cs\`：旧 Triton 卡牌映射包装类，未被其余银鱼模拟源码引用。
- \`cards/1858-泰坦诸神/Sim_TTN_092*.cs\` 四个脚本：直接读取旧的 live GameState，后续必须将逻辑改为读模拟局面 \`Playfield\`。**不能视为已经支持的卡牌效果**。

## 如何复现

在仓库根目录执行（Python 3.10+）：

\`\`\`powershell
python tools/test_import_classic.py
python tools/import_classic.py "D:\Downloads\Hearthbuddy1.7.1(1).zip" --dest . --sha256 1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071
dotnet build src/Silverfish.Classic/Silverfish.Classic.csproj -c Release
dotnet run --project src/Silverfish.Classic.Smoke/Silverfish.Classic.Smoke.csproj -c Release
\`\`\`

可选地给 smoke 程序传入经典 \`BoardTester\` 格式的牌面文件，尝试调用**原始经典** \`Ai.Instance.autoTester\`。

\`\`\`powershell
dotnet run --project src/Silverfish.Classic.Smoke/Silverfish.Classic.Smoke.csproj -- "D:\boards\test.txt"
\`\`\`

\`Imported/\` 默认在 \`.gitignore\` 中，避免未经核对就把约两万份原始卡牌文件发布到公开仓库；**提取器和兼容层已经提交**。如需合并实质源码，应先在本地编译和回归。

## 验证边界

- 已验证实际 ZIP 的 SHA-256、目录内容、选择性导入数量以及提取器的合成用例。
- 原有 \`Silverfish.Contracts -> Silverfish.Core -> Silverfish.Cli\` 离线演示和单测此前通过 GitHub Actions。
- **还没有对这一批新提取的经典源码完成 dotnet 构建验证**：当前执行环境没有 .NET SDK，而且约 1.9 万个源文件尚未加入 GitHub 远程分支。
- 因此不能宣称经典 \`Ai\` 已经独立运行或接入 \`GameSnapshot\`，更不能宣称实战可用。

下一道技术门槛：解决独立编译错误、把新版 \`state.snapshot\` 映射到 \`Hrtprozis/Handmanager\`、使真实经典 \`Ai\` 在固定离线局面中产生决策，然后才接 UI。
