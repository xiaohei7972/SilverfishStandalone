# P0：2026-11-05 经典银鱼独立化（模型重构冻结）

## 范围

此阶段只让原版经典银鱼**不需要 Hearthbuddy 主程序**便能作为独立 .NET 8 类库编译、加载静态卡牌资源并完成一份固定牌面的离线搜索。

暂不开发：
- 新 Minion/Handcard/Weapon 模型或 Playfield 搜索树结构；
- StandaloneGameMod、Libmem.NET 实时数据接入；
- Vue/WebView2 界面与任何在线对局动作执行；
- Silverfish V2。

旧结构里的浅拷贝/大对象问题保留在 `docs/CLASSIC_MODEL_REFACTOR.md`，待 P0 完成后再独立排期。

## P0 验收清单

- [x] 完整经典银鱼源码从 2026-11-05 用户归档中提取：19,135 个 C# 源文件；SHA-256 有记录。
- [x] 源码包不含 Hearthbuddy.exe、Triton/GreyMagic DLL，也没有 V2；旧宿主功能仅在很少量边界上兼容处理。
- [x] 已准备标准独立 .NET 8 类库和 smoke 运行入口（包含原版通用策略初始化、真实 Ai.bestmove 输出）。
- [x] 老版参考源码在 [GitHub Actions #43531545029](https://github.com/xiaohei7972/SilverfishStandalone/actions/runs/43531545029) 完成编译、CardDB、原版 Ai.autoTester 离线运算与 JSON 决策测试。
- [x] 为最新版准备宿主单例与策略初始化、明确的固定牌面输入字段、错误抛出和 Roslyn 共享编译限制修正。
- [x] 已经制作完整的**2026-11-05 独立化源码包**；它的静态完整性检查通过。
- [ ] **最新版 2026-11-05 源码的 .NET 8 实际编译成功**（当前容器未安装 dotnet SDK；Actions 还拿不到用户 ZIP）。
- [ ] **最新版的 CardDB 和 Ai.autoTester 离线回归成功**，至少得到非空的 `attackWithMinion` (source entity 11 -> target entity 1)。
- [ ] **固定牌面发生语法错误时退出码非零且提供解释**。
- [ ] 将最新版 CLI 可执行产物保存为 GitHub Actions Artifact，并锁定源码 hash。

## 使用 GitHub Actions 验证最新版

用户提供的原始 2026-11-05 ZIP SHA-256：

`1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071`

最初的来源工作包 SHA-256：

`ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c`

重新整理的**独立运行就绪源码工作包** SHA-256：

`81f6f19e2888ba5158ea194d6bea30d1f2ad90a7e67e7fd8565ffc7d2bff75e7`

工作流 `.github/workflows/classic-compile.yml` 的 `owner-classic-work-package` 方式接受两种经过校验的工作包 SHA，并核对 `classic-import-manifest.json` 的原始来源 SHA。它仅在下载到有效 Release 资产后编译，旧版参考模式仍严格区分。

如确认可以公开 ZIP，请从开发分支运行（需要 PowerShell 7 和已登录的 `gh`）：

```powershell
pwsh ./tools/publish_owner_classic_source.ps1 -ArchivePath "C:\\path\\ClassicSilverfish_2026-11-05_独立化优先_源码版.zip" -ConfirmPublic
```

由于仓库是公开仓库，Release 资产也会公开。**这一步还没有执行**：当前 GitHub 连接不能直接上传 18MB 二进制文件。如果不希望公开，使用私有构建来源，不要传到公共 Release。

## 阿格拉玛四个卡牌脚本已临时停用

按用户要求，为避免当前 2026-11-05 版复仇者阿格拉玛和三个泰坦技能的实现阻塞独立化，`Sim_TTN_092.cs`、`Sim_TTN_092t1.cs`、`Sim_TTN_092t2.cs`、`Sim_TTN_092t3.cs` 现在保留为可编译的空 `SimTemplate` 类。每个文件的原效果源码以行注释保留在 `BEGIN DISABLED ORIGINAL` 与 `END DISABLED ORIGINAL` 之间，不再调用旧 `GameState.Get()`。两份原本就为空的相关脚本 `Sim_TTN_092t.cs` 与 `Sim_TTN_092e2t.cs` 没有修改。

该措施**不是卡牌行为修复**：战吼装备武器及三个技能效果不再由这些模拟器执行，因此使用这些卡牌时的 AI 评估可能不正确。后续独立化完成后按单独任务恢复其算法实现。

复现步骤：`tools/import_classic.py` 在从原始归档导入时自动将四个原始脚本改成“原逻辑注释 + 空 SimTemplate”；`tools/disable_aggramar.py` 为已提取的源码工作包补齐四个占位文件并保持幂等。GitHub Actions 在编译前运行该步骤。新独立源码包 SHA-256：

`4d6944304c0d224d366afda1691f0429c5d3cffbc0daae18259310a7f6f95d92`

最新版 .NET 8 编译与完整离线回归仍须在 Actions 上执行，不以静态检查代替。

## 完成标准

只有最新版本源码在 Actions 中编译通过、固定牌面真实 AI 运算通过且异常处理测试通过，才能将 P0 标为完成。其他模块的回归成功不算替代验收。
