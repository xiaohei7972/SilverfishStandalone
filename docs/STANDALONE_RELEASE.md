# 独立模拟器 Windows x64 使用说明

在解压目录打开 PowerShell。程序内含运行时和 starter 卡牌资料。

```powershell
.\Silverfish.Cli.exe --help
.\Silverfish.Cli.exe simulate samples/simulation-combat.json
.\Silverfish.Cli.exe plan samples/simulation-random.json
```

用外置卡牌资料运行自定义规则和决策。

```powershell
.\Silverfish.Cli.exe import-card-data samples/card-data-demo.xml card-data.json
.\Silverfish.Cli.exe decide samples/simulation-decision.json --card-data card-data.json
```

导出回放结束时的存档并重新搜索。

```powershell
.\Silverfish.Cli.exe checkpoint samples/simulation-random.json | Set-Content -Encoding utf8 checkpoint.json
.\Silverfish.Cli.exe plan checkpoint.json
```

导入自己的卡牌资料时，将第一条导入命令中的 XML 路径换成实际的 `CardDefs.xml`。保存后的 JSON 可通过 `--card-data` 提供静态属性，卡牌效果由程序内置规则或场景中的 `cardRules` 指定。
