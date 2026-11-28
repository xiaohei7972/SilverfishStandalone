using System.Text;
using System.Text.RegularExpressions;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;

namespace Silverfish.Cli;

internal static class CardSimGenerator
{
    private static readonly Regex DescriptionTemplate = new(@"\A(?:\{\d+\}\s*)+\z");

    internal static void Generate(string input, string cardsDirectory, IEnumerable<string> cardIds)
    {
        using var source = File.OpenRead(input);
        var catalog = Path.GetExtension(input).ToLowerInvariant() switch
        {
            ".xml" => CardDataCatalog.ImportXml(source),
            ".json" => CardDataCatalog.LoadJson(source),
            _ => throw new ArgumentException("卡牌资料须为 CardDefs XML 或已导出的 JSON。")
        };

        var root = Path.GetFullPath(cardsDirectory);
        var files = cardIds.Distinct(StringComparer.Ordinal).Select(cardId => GenerateFile(catalog.GetCard(cardId), catalog, root)).ToArray();
        if (files.Select(file => Path.GetFileName(file.Path)).Distinct(StringComparer.OrdinalIgnoreCase).Count() != files.Length)
            throw new ArgumentException("指定的卡牌 ID 会生成同名文件。");

        IEnumerable<string> sourceFiles = Directory.Exists(root)
            ? Directory.EnumerateFiles(root, "Sim_*.cs", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                MatchCasing = MatchCasing.CaseInsensitive,
                AttributesToSkip = FileAttributes.ReparsePoint,
                IgnoreInaccessible = false
            }) : [];
        var existingFiles = sourceFiles.ToLookup(Path.GetFileName, StringComparer.OrdinalIgnoreCase);

        var created = 0;
        foreach (var file in files)
        {
            // 文件可能还在旧目录，或属于导入数据中的另一个系列。
            var existing = existingFiles[Path.GetFileName(file.Path)].FirstOrDefault();
            if (existing is not null)
            {
                Console.WriteLine($"「{existing}」已存在，保留原文件。");
                continue;
            }
            if (WriteNewFile(file))
            {
                created++;
                Console.WriteLine($"已生成「{file.Path}」。");
            }
            else
            {
                Console.WriteLine($"「{file.Path}」已存在，保留原文件。");
            }
        }
        Console.WriteLine($"构建 {catalog.SourceBuild}，新增 {created} 个 Sim 文件，保留 {files.Length - created} 个现有文件。");
    }

    private static (string Path, string Content) GenerateFile(CardMetadata metadata, CardDataCatalog catalog, string root)
    {
        var id = metadata.CardId;
        if (id.Any(character => !char.IsAsciiLetterOrDigit(character) && character != '_'))
            throw new InvalidDataException($"卡牌 ID「{id}」不能用于生成 Sim 类名。");
        var kind = metadata.ToDefinition().Kind;
        if (kind is not (EntityKind.Hero or EntityKind.Minion or EntityKind.Spell or EntityKind.Weapon or EntityKind.Location or
            EntityKind.HeroPower or EntityKind.Enchantment))
            throw new InvalidDataException($"卡牌「{id}」的类型不支持生成 Sim 骨架。");

        var enchantment = kind == EntityKind.Enchantment;
        var directory = Path.Combine(root, CardSetDirectory.GetName(metadata.ClientCardSet));
        if (enchantment)
            directory = Path.Combine(directory, "ENCHANTMENT");
        var suffix = enchantment ? ".Enchantments" : "";
        var declaration = enchantment ? "StatEnchantmentRule Rule" : "CardRules Rules";
        var instruction = kind switch
        {
            EntityKind.Enchantment => "按附魔行为选择规则类型。属性附魔的额外移除效果在 StatEnchantmentRule.OnRemoved 中声明。",
            EntityKind.Location => "激活条件写在类的静态 PlayReqs.Ability 中，激活效果写在这里的 Activation 中。",
            _ => "出牌条件写在类的静态 PlayReqs.Play 中，触发与结算效果写在这里。"
        };
        var registration = enchantment ? "// 规则由施加附魔的单卡 Sim 引用。" : $"// [CardSim(\"{id}\")]";
        var chineseName = FormatCardText(metadata.Name?.ZhCn) ?? $"卡名未收录（{id}）";
        var englishName = FormatCardText(metadata.Name?.EnUs) ?? $"Name unavailable ({id})";
        var chineseText = FormatCardText(metadata.Text?.ZhCn);
        var englishText = FormatCardText(metadata.Text?.EnUs);
        var chineseTemplate = IsDescriptionTemplate(chineseText);
        var englishTemplate = IsDescriptionTemplate(englishText);
        if (chineseTemplate || englishTemplate)
        {
            var description = kind == EntityKind.Location ? FindLocationDescription(metadata, catalog) : null;
            if (chineseTemplate)
                chineseText = ResolveDescriptionTemplate(description?.ZhCn, "卡库仅提供描述模板。");
            if (englishTemplate)
                englishText = ResolveDescriptionTemplate(description?.EnUs, "Card data contains only a description template.");
        }
        var chineseDescription = chineseText ?? (englishText is null ? "无卡面描述。" : "中文卡面描述未收录。");
        var englishDescription = englishText ?? (chineseText is null ? "No card text." : "English card text unavailable.");
        var content = $$"""
            namespace Silverfish.Simulation.Cards{{suffix}};

            // {{chineseName}}
            // {{englishName}}
            // {{chineseDescription}}
            // {{englishDescription}}

            {{registration}}
            internal static class Sim_{{id}}
            {
                // {{instruction}}
                internal static {{declaration}} =>
                    throw new InvalidOperationException("卡牌「{{id}}」的 Sim 尚未实现。");
            }
            """;
        return (Path.Combine(directory, $"Sim_{id}.cs"), content + "\n");
    }

    private static LocalizedCardText? FindLocationDescription(CardMetadata metadata, CardDataCatalog catalog)
    {
        var matches = catalog.Cards.Where(candidate =>
            candidate.GetTag(GAME_TAG.CARDTYPE) == (int)TAG_CARDTYPE.LOCATION &&
            candidate.GetTag(GAME_TAG.COLLECTIBLE) == 1 &&
            candidate.ClientCardSet == metadata.ClientCardSet &&
            candidate.GetTag(GAME_TAG.CLASS) == metadata.GetTag(GAME_TAG.CLASS) &&
            candidate.GetTag(GAME_TAG.COST) == metadata.GetTag(GAME_TAG.COST) &&
            candidate.GetTag(GAME_TAG.HEALTH) == metadata.GetTag(GAME_TAG.HEALTH) &&
            candidate.Name == metadata.Name)
            .Take(2).ToArray();
        return matches.Length == 1 ? matches[0].Text : null;
    }

    private static bool IsDescriptionTemplate(string? text) => text is not null && DescriptionTemplate.IsMatch(text);

    private static string ResolveDescriptionTemplate(string? text, string fallback)
    {
        var description = FormatCardText(text);
        return description is null || IsDescriptionTemplate(description) ? fallback : description;
    }

    private static string? FormatCardText(string? text)
    {
        if (text is null)
            return null;

        // 展开牌面换行并去掉显示标记，保持每段文字都在单行注释内。
        var plain = text
            .Replace("\\n", " ", StringComparison.Ordinal)
            .Replace("[x]", "", StringComparison.OrdinalIgnoreCase)
            .Replace("<b>", "", StringComparison.OrdinalIgnoreCase)
            .Replace("</b>", "", StringComparison.OrdinalIgnoreCase)
            .Replace("<i>", "", StringComparison.OrdinalIgnoreCase)
            .Replace("</i>", "", StringComparison.OrdinalIgnoreCase);
        var words = plain.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return words.Length == 0 ? null : string.Join(' ', words);
    }

    private static bool WriteNewFile((string Path, string Content) file)
    {
        if (File.Exists(file.Path))
            return false;

        var directory = Path.GetDirectoryName(file.Path)!;
        Directory.CreateDirectory(directory);
        var temporary = Path.Combine(directory, $"{Guid.NewGuid():N}.tmp");
        try
        {
            File.WriteAllText(temporary, file.Content, Encoding.UTF8);
            try
            {
                // 原子移入目标路径，并保留同时写入的手工文件。
                File.Move(temporary, file.Path);
                return true;
            }
            catch (IOException) when (File.Exists(file.Path))
            {
                return false;
            }
        }
        finally
        {
            File.Delete(temporary);
        }
    }
}
