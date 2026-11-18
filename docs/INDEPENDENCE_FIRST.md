# 2026-11-05 经典银鱼独立化验收

## 范围

此阶段只让原版经典银鱼**不需要 Hearthbuddy 主程序**便能作为独立 .NET 8 类库编译、加载静态卡牌资源并完成一份固定牌面的离线搜索。

暂不开发：
- 新 Minion/Handcard/Weapon 模型或 Playfield 搜索树结构；
- StandaloneGameMod、Libmem.NET 实时数据接入；
- Vue/WebView2 界面与任何在线对局动作执行；
- Silverfish V2。

旧结构里的浅拷贝/大对象问题保留在 `docs/CLASSIC_MODEL_REFACTOR.md`，待 P0 完成后再独立排期。

## P0 验收清单

- [x] 验收工作包从 2026-11-05 用户归档生成。清单记录 19,139 个 C# 文件及原始归档 SHA-256。
- [x] 源码包不含 Hearthbuddy.exe、Triton/GreyMagic DLL，也没有 V2；旧宿主功能仅在很少量边界上兼容处理。
- [x] 已准备标准独立 .NET 8 类库和 smoke 运行入口（包含原版通用策略初始化、真实 Ai.bestmove 输出）。
- [x] 老版参考源码在 [GitHub Actions #43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029) 完成编译、CardDB、原版 Ai.autoTester 离线运算与 JSON 决策测试。
- [x] 为最新版准备宿主单例与策略初始化、明确的固定牌面输入字段、错误抛出和 Roslyn 共享编译限制修正。
- [x] 已经制作完整的**2026-11-05 独立化源码包**；它的静态完整性检查通过。
- [x] 2026-11-05 版在 Windows 下通过 .NET 8 Release 编译。
- [x] `CardDB` 加载了肯瑞托法师 `EX1_612`。原版 `Ai.autoTester` 返回 `attackWithMinion`（实体 11 攻击实体 1）。JSON 快照返回 `attack-face`。
- [x] 无效文本牌面输出格式错误说明，进程退出码为 2。
- [x] 私有 Actions 归档 `classic-2026-11-05-net8-private` 和 `classic-2026-11-05-private-build-logs`。

验收于 2026-11-18 完成，构建工具版本为 `49fffbc`。导入脚本将附魔解析改为 .NET 字符串拆分，并让动作查找识别离线牌面使用的我方英雄实体 ID `0`。Windows 本地也通过了编译、打包和解压运行。[验收运行](https://github.com/xiaohei7972/Hearthbuddy_Silverfish/actions/runs/44256337489)

## 使用 GitHub Actions 验证最新版

用户提供的原始 2026-11-05 ZIP SHA-256：

`1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071`

最初的来源工作包 SHA-256：

`ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c`

重新整理的**独立运行就绪源码工作包** SHA-256：

`81f6f19e2888ba5158ea194d6bea30d1f2ad90a7e67e7fd8565ffc7d2bff75e7`

公开仓库的 `classic-compile.yml` 自动运行旧版参考源码的诊断。2026-11-05 版通过下方私有工作流进行验收。

## 阿格拉玛四个卡牌脚本已临时停用

按用户要求，为避免当前 2026-11-05 版复仇者阿格拉玛和三个泰坦技能的实现阻塞独立化，`Sim_TTN_092.cs`、`Sim_TTN_092t1.cs`、`Sim_TTN_092t2.cs`、`Sim_TTN_092t3.cs` 现在保留为可编译的空 `SimTemplate` 类。每个文件的原效果源码以行注释保留在 `BEGIN DISABLED ORIGINAL` 与 `END DISABLED ORIGINAL` 之间，不再调用旧 `GameState.Get()`。两份原本就为空的相关脚本 `Sim_TTN_092t.cs` 与 `Sim_TTN_092e2t.cs` 没有修改。

该措施**不是卡牌行为修复**：战吼装备武器及三个技能效果不再由这些模拟器执行，因此使用这些卡牌时的 AI 评估可能不正确。后续独立化完成后按单独任务恢复其算法实现。

复现步骤：`tools/import_classic.py` 在从原始归档导入时自动将四个原始脚本改成“原逻辑注释 + 空 SimTemplate”；`tools/disable_aggramar.py` 为已提取的源码工作包补齐四个占位文件并保持幂等。GitHub Actions 在编译前运行该步骤。新独立源码包 SHA-256：

`4d6944304c0d224d366afda1691f0429c5d3cffbc0daae18259310a7f6f95d92`

## 私有 GitHub Actions：最新版不公开源码的验收通道

为避免将 2026-11-05 的完整经典银鱼 ZIP 发布到公开 Release，已在账号已有的 **Private** 仓库 `xiaohei7972/Hearthbuddy_Silverfish` 的 main 分支创建 `.github/workflows/verify-classic-2026-11.yml`。

- 私有 Release 标签为 `classic-source-2026-11-05-private`，资产名为 `ClassicSilverfish_2026-11-05_Standalone.zip`。工作流默认使用这两个值。
- 强制 SHA-256 校验为 `4d6944304c0d224d366afda1691f0429c5d3cffbc0daae18259310a7f6f95d92`，再检查用户原始来源的 SHA-256。
- 从公开的 SilverfishStandalone 开发分支检出引擎构建工具，但最新源码只在私有 Windows runner 下载/编译，不向公开 repo 提交。
- 依次执行 CardDB、原版 Ai.autoTester 中文牌面、ClassicBridge JSON 决策、错误牌面非零退出，以及构建日志与 DLL Artifact。
- 上传步骤可用仓库脚本 `tools/verify_owner_classic_private.ps1` 自动完成，该脚本会再次检查目标仓库必须为 Private。

```powershell
pwsh ./tools/verify_owner_classic_private.ps1 -ArchivePath "C:\path\ClassicSilverfish_2026-11-05_Standalone.zip"
```

## 在 Windows 本地复现

在干净的开发分支检出目录中，先核对私有 Release ZIP 的 SHA-256。解压 ZIP 后，将其中的 `src/Silverfish.Classic/Imported` 目录和 `classic-import-manifest.json` 复制到检出目录的相同位置，再使用 Python 和 .NET 8 SDK 执行以下命令。

```powershell
python tools/normalize_classic.py
python tools/disable_aggramar.py
dotnet test tests/Silverfish.ClassicBridge.Tests/Silverfish.ClassicBridge.Tests.csproj -c Release
dotnet publish src/Silverfish.Classic.Smoke/Silverfish.Classic.Smoke.csproj -c Release -o artifacts/classic
./artifacts/classic/Silverfish.Classic.Smoke.exe
./artifacts/classic/Silverfish.Classic.Smoke.exe samples/classic-simple-lethal-zh.txt
./artifacts/classic/Silverfish.Classic.Smoke.exe samples/classic-snapshot-lethal.json
./artifacts/classic/Silverfish.Classic.Smoke.exe samples/classic-invalid-zh.txt
$LASTEXITCODE
```

运行发布目录中的程序需要 Windows .NET 8 Runtime。最后一条牌面测试应输出输入格式错误并返回 2。

## 完成标准

只有最新版本源码在 Actions 中编译通过、固定牌面真实 AI 运算通过且异常处理测试通过，才能将 P0 标为完成。其他模块的回归成功不算替代验收。
