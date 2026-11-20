using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed record LocalizedCardText(string? EnUs, string? ZhCn);

public sealed record CardMetadata(
    string CardId, int DbfId, LocalizedCardText? Name, LocalizedCardText? Text,
    [property: JsonRequired] ImmutableSortedDictionary<int, int> Tags)
{
    public int GetTag(int tag) => Tags.GetValueOrDefault(tag);

    public CardDefinition ToDefinition()
    {
        const int cardType = 202, cost = 48, attack = 47, health = 45;
        var kind = GetTag(cardType) switch
        {
            0 => EntityKind.Unknown,
            3 => EntityKind.Hero,
            4 => EntityKind.Minion,
            5 => EntityKind.Spell,
            6 => EntityKind.Enchantment,
            7 => EntityKind.Weapon,
            10 => EntityKind.HeroPower,
            39 => EntityKind.Location,
            _ => EntityKind.Other
        };

        // 武器和地标的初始耐久取自 HEALTH。
        return kind is EntityKind.Weapon or EntityKind.Location
            ? new(CardId, kind, GetTag(cost), GetTag(attack), PrintedDurability: GetTag(health))
            : new(CardId, kind, GetTag(cost), GetTag(attack), GetTag(health));
    }
}

/// <summary>通过卡牌 ID 或 DBF ID 查询同一构建的静态卡牌资料。</summary>
public sealed class CardDataCatalog
{
    private const int SchemaVersion = 1;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    private static readonly JsonSerializerOptions IndentedJsonOptions = new(JsonOptions) { WriteIndented = true };
    private readonly ImmutableDictionary<string, CardMetadata> _byCardId;
    private readonly ImmutableDictionary<int, CardMetadata> _byDbfId;

    private CardDataCatalog(int sourceBuild, IEnumerable<CardMetadata> cards)
    {
        if (sourceBuild <= 0)
            throw new InvalidDataException("卡牌数据的来源构建号必须大于零。");

        var byCardId = ImmutableDictionary.CreateBuilder<string, CardMetadata>(StringComparer.Ordinal);
        var byDbfId = ImmutableDictionary.CreateBuilder<int, CardMetadata>();
        foreach (var card in cards)
        {
            if (card is null || string.IsNullOrWhiteSpace(card.CardId) || card.DbfId <= 0 || card.Tags is null)
                throw new InvalidDataException("卡牌资料必须包含卡牌 ID、正整数 DBF ID 和数值标签。");
            if (!byCardId.TryAdd(card.CardId, card) || !byDbfId.TryAdd(card.DbfId, card))
                throw new InvalidDataException($"卡牌「{card.CardId}」（DBF ID {card.DbfId}）的卡牌 ID 或 DBF ID 重复。");
        }
        if (byCardId.Count == 0)
            throw new InvalidDataException("卡牌目录至少要包含一张卡牌。");

        SourceBuild = sourceBuild;
        _byCardId = byCardId.ToImmutable();
        _byDbfId = byDbfId.ToImmutable();
        Cards = [.. byCardId.Values.OrderBy(card => card.CardId, StringComparer.Ordinal)];
    }

    public int SourceBuild { get; }
    public ImmutableArray<CardMetadata> Cards { get; }

    public CardMetadata GetCard(string cardId) => _byCardId.TryGetValue(cardId, out var card)
        ? card : throw new InvalidDataException($"构建 {SourceBuild} 的目录中找不到卡牌「{cardId}」。");

    public CardMetadata GetCard(int dbfId) => _byDbfId.TryGetValue(dbfId, out var card)
        ? card : throw new InvalidDataException($"构建 {SourceBuild} 的目录中找不到 DBF ID {dbfId}。");

    public CardDataCatalog SelectCards(IEnumerable<string> cardIds) => new(SourceBuild, cardIds.Select(GetCard));

    public static CardDataCatalog ImportXml(Stream source)
    {
        using var reader = XmlReader.Create(source, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit });
        var root = XDocument.Load(reader).Root;
        if (root?.Name != "CardDefs" || (int?)root.Attribute("build") is not int build)
            throw new InvalidDataException("请输入带有构建号的 CardDefs 文档。");

        return new(build, root.Elements("Entity").Select(ReadCard));
    }

    private static CardMetadata ReadCard(XElement entity)
    {
        var cardId = (string?)entity.Attribute("CardID") ?? throw new InvalidDataException("有卡牌实体缺少 CardID。");
        var dbfId = (int?)entity.Attribute("ID") ?? throw new InvalidDataException($"卡牌「{cardId}」缺少 DBF ID。");
        var tags = ImmutableSortedDictionary.CreateBuilder<int, int>();
        LocalizedCardText? name = null;
        LocalizedCardText? text = null;

        // ReferencedTag 描述牌面文本提及的机制。这里只保留实体本身的 Tag。
        foreach (var tag in entity.Elements("Tag"))
        {
            var id = (int?)tag.Attribute("enumID");
            if (id is 184 or 185)
            {
                var localized = new LocalizedCardText((string?)tag.Element("enUS"), (string?)tag.Element("zhCN"));
                if (id == 185)
                    name = localized;
                else
                    text = localized;
            }
            if (id is int key && (int?)tag.Attribute("value") is int value && !tags.TryAdd(key, value))
                throw new InvalidDataException($"卡牌「{cardId}」的数值标签 {key} 重复。");
        }
        return new(cardId, dbfId, name, text, tags.ToImmutable());
    }

    public static CardDataCatalog LoadJson(Stream source)
    {
        var data = JsonSerializer.Deserialize<CatalogFile>(source, JsonOptions);
        if (data is null || data.SchemaVersion != SchemaVersion || data.Cards.IsDefault)
            throw new InvalidDataException($"卡牌目录必须使用格式版本 {SchemaVersion} 并包含 cards 列表。");
        return new(data.SourceBuild, data.Cards);
    }

    public void WriteJson(Stream destination, bool indented = false) =>
        JsonSerializer.Serialize(destination, new CatalogFile(SchemaVersion, SourceBuild, Cards),
            indented ? IndentedJsonOptions : JsonOptions);

    private sealed record CatalogFile(int SchemaVersion, int SourceBuild, ImmutableArray<CardMetadata> Cards);
}
