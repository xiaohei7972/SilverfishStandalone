# GitHub Actions：经典银鱼独立编译

新增 .github/workflows/classic-compile.yml。这个工作流会实际编译提取后的 Silverfish.Classic.csproj，而不只是编译此前的 baseline 测试器。

## 目前可运行：旧源码兼容性诊断

推送相关文件到 feat/** 分支时，Actions 会自动检出 xiaohei7972/Hearthbuddy_backed 的 main 分支，单独打包 Routines/DefaultRoutine/Silverfish，运行 tools/import_classic.py，再运行 dotnet build。

注意：这份 GitHub 参考仓库比你上传的 2026-11-05 ZIP 更旧。此模式用于识别早期解耦的编译问题，不能据此宣布最新版银鱼已通过测试，也不能用旧文件覆盖最终版代码。

## 推荐：使用独立源码工作包（不包含旧主程序）

已经从用户提供的最新版 ZIP 生成了 ClassicSilverfish_2026-11-05_独立源码工作包.zip（约 18 MB）。它包含经典银鱼源码/数据，不包含 Hearthbuddy.exe、GreyMagic 等主程序文件。

SHA-256：

    ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c

当确认该源码包可以公开发布时，可将此 ZIP 上传到本仓库 tag 为 classic-source-2026-11-05 的 GitHub Release 资产，然后在 Actions 中选择 owner-classic-work-package。工作流会验证文件 SHA-256，直接从归档导入实际 2026-11-05 经典源码，再编译。此模式比上传完整旧主程序安装包更合适。

若源码不希望公开，请不要使用公开 Release，改在私有仓库中运行工作流或通过有权限的私有制品仓库连接。

## 一条命令上传独立源码包并启动最新版本构建

仓库已加入 `tools/publish_owner_classic_source.ps1`：

```powershell
pwsh ./tools/publish_owner_classic_source.ps1 -ArchivePath "C:\\path\\ClassicSilverfish_2026-11-05_独立源码工作包.zip" -ConfirmPublic
```

该脚本会依次核验哈希、确认经典 `Ai.cs` 与 `Minion.cs`、检查不含 V2 或旧主程序 exe/dll、上传到 `classic-source-2026-11-05` Release 资产，并手动触发 `owner-classic-work-package` Actions。需要 GitHub CLI `gh` 已登录。因仓库是公开的，必须显式输入 `-ConfirmPublic`。

此步骤**仍未在 GitHub 端完成**：当前连接的 GitHub 工具不能上传此 18MB 二进制 Release 资产；请勿把旧版 CI 通过当成最新版本的结果。

## 重构相关回归

经典工作流在原版编译之后，还会测试 `Silverfish.ClassicModelAdapter`：只读地把旧 Minion、Handcard、Weapon 投影到 `Silverfish.Domain`，验证状态复制与身份映射。后续对原版模型动手改造时，这些测试用于识别新旧状态差异。

## 最新版 2026-11-05 的正式构建

工作流支持从当前仓库的 GitHub Release 下载原始 ZIP，并要求 SHA-256 必须等于以下值：

    1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071

原始安装包模式仍保留：将原版 ZIP 作为同一 Release 资产上传后，在 Actions > Classic Silverfish compile (diagnostic) > Run workflow 中选择 owner-2026-11-05-release 并将 release_asset 指定为 Hearthbuddy1.7.1(1).zip。优先使用上面的独立源码包模式。

注意本仓库是公开的。不要直接公开完整安装包中可能含有的账号配置、私有资料或不想发布的二进制。若不希望发布，建议在私有仓库托管该 ZIP 并使用最小权限的 GitHub Actions 凭证接入，或先确认归档内容及公开范围。

## 日志和判断标准

- 每次运行生成 classic-import-manifest.json，说明实际源码 SHA-256、导入数量及隔离清单。
- 直接运行 dotnet build src/Silverfish.Classic/Silverfish.Classic.csproj -c Release。
- 无论成功还是失败，都尝试上传 classic-silverfish-build-logs Artifact，其中包含 classic-console.log、classic-build.log。
- 若编译成功，再运行 Silverfish.Classic.Smoke 的 CardDB 加载检查。
- 最终目标仍是从固定牌面启动经典 Ai.autoTester 并输出真实 bestmove。

**旧参考代码的编译失败并不意味着 2026-11-05 版也会失败；同样，旧版通过也不证明最新版已通过。**
