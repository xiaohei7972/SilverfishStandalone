using System.Text;
using Silverfish.Domain;
using Silverfish.Simulation;

namespace Silverfish.Cli;

internal static class CardSimGenerator
{
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
        var files = cardIds.Distinct(StringComparer.Ordinal).Select(cardId => GenerateFile(catalog.GetCard(cardId), root)).ToArray();
        if (files.Select(file => file.Path).Distinct(StringComparer.OrdinalIgnoreCase).Count() != files.Length)
            throw new ArgumentException("指定的卡牌 ID 会生成同名文件。");

        var created = 0;
        foreach (var file in files)
        {
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

    private static (string Path, string Content) GenerateFile(CardMetadata metadata, string root)
    {
        var id = metadata.CardId;
        if (id.Any(character => !char.IsAsciiLetterOrDigit(character) && character != '_'))
            throw new InvalidDataException($"卡牌 ID「{id}」不能用于生成 Sim 类名。");
        var kind = metadata.ToDefinition().Kind;
        if (kind is not (EntityKind.Hero or EntityKind.Minion or EntityKind.Spell or EntityKind.Weapon or
            EntityKind.HeroPower or EntityKind.Enchantment))
            throw new InvalidDataException($"卡牌「{id}」的类型不支持生成 Sim 骨架。");

        var enchantment = kind == EntityKind.Enchantment;
        var directory = enchantment ? Path.Combine(root, "Enchantments") : root;
        var suffix = enchantment ? ".Enchantments" : "";
        var declaration = enchantment ? "StatEnchantmentRule Rule" : "CardRules Rules";
        var instruction = enchantment ? "在此定义附魔数值和持续期。" : "在此声明本卡的目标、触发和效果。";
        var content = $$"""
            namespace Silverfish.Simulation.Cards{{suffix}};

            internal static class Sim_{{id}}
            {
                internal const string CardId = "{{id}}";

                // {{instruction}}
                internal static {{declaration}} =>
                    throw new InvalidOperationException($"卡牌「{CardId}」的 Sim 尚未实现。");
            }
            """;
        return (Path.Combine(directory, $"Sim_{id}.cs"), content + "\n");
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
