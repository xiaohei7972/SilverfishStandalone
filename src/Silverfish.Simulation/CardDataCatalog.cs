using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed record LocalizedCardText(string? EnUs, string? ZhCn);

public sealed record CardMetadata(
    string CardId, int DbfId, LocalizedCardText? Name, LocalizedCardText? Text,
    [property: JsonRequired] ImmutableSortedDictionary<int, int> Tags)
{
    public int GetTag(int tag) => Tags.GetValueOrDefault(tag);
    public int GetTag(GAME_TAG tag) => GetTag((int)tag);

    [JsonIgnore] public TAG_CARDTYPE ClientCardType => (TAG_CARDTYPE)GetTag(GAME_TAG.CARDTYPE);
    [JsonIgnore] public TAG_CARD_SET ClientCardSet => (TAG_CARD_SET)GetTag(GAME_TAG.CARD_SET);
    [JsonIgnore] public TAG_RACE ClientRace => (TAG_RACE)GetTag(GAME_TAG.CARDRACE);
    [JsonIgnore] public TAG_SPELL_SCHOOL ClientSpellSchool => (TAG_SPELL_SCHOOL)GetTag(GAME_TAG.SPELL_SCHOOL);
    [JsonIgnore] public TAG_CARD_ALTERNATE_COST ClientAlternateCost => (TAG_CARD_ALTERNATE_COST)GetTag(GAME_TAG.CARD_ALTERNATE_COST);
    [JsonIgnore] public TAG_ZONE ClientZone => (TAG_ZONE)GetTag(GAME_TAG.ZONE);

    public CardDefinition ToDefinition()
    {
        var kind = ClientCardType switch
        {
            TAG_CARDTYPE.INVALID => EntityKind.Unknown,
            TAG_CARDTYPE.HERO => EntityKind.Hero,
            TAG_CARDTYPE.MINION => EntityKind.Minion,
            TAG_CARDTYPE.SPELL => EntityKind.Spell,
            TAG_CARDTYPE.ENCHANTMENT => EntityKind.Enchantment,
            TAG_CARDTYPE.WEAPON => EntityKind.Weapon,
            TAG_CARDTYPE.HERO_POWER => EntityKind.HeroPower,
            TAG_CARDTYPE.LOCATION => EntityKind.Location,
            _ => EntityKind.Other
        };

        // 武器和地标的初始耐久取自 HEALTH。
        var equipment = kind is EntityKind.Weapon or EntityKind.Location;
        var health = GetTag(GAME_TAG.HEALTH);
        var alternateCost = ClientAlternateCost;
        var classes = GetTag(GAME_TAG.MULTIPLE_CLASSES);
        return new(CardId, kind, GetTag(GAME_TAG.COST), GetTag(GAME_TAG.ATK),
            PrintedHealth: equipment ? 0 : health, PrintedDurability: equipment ? health : 0)
        {
            MinionTypes = kind == EntityKind.Minion ? CardTraitReader.ReadMinionTypes(this) : null,
            SpellSchool = CardTraitReader.ReadSpellSchool(this),
            AlternateCost = alternateCost == TAG_CARD_ALTERNATE_COST.MANA ? null : alternateCost,
            ClassMask = classes != 0 ? unchecked((uint)classes) : ClassBit((TAG_CLASS)GetTag(GAME_TAG.CLASS))
        };
    }

    internal static uint ClassBit(TAG_CLASS value) => (int)value is >= 1 and <= 32 ? 1U << ((int)value - 1) : 0;
}

/// <summary>通过卡牌 ID 或 DBF ID 查询同一构建的静态卡牌资料。</summary>
public sealed class CardDataCatalog
{
    private const int SchemaVersion = 1;
    // 卡名标签来自 CardDefs.xml，客户端 GAME_TAG 中未声明该成员。
    private const int CardNameTag = 185;
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
        if (!reader.IsStartElement("CardDefs", "") || reader.GetAttribute("build") is not { } build)
            throw new InvalidDataException("请输入带有构建号的 CardDefs 文档。");

        return new(XmlConvert.ToInt32(build), ReadCards(reader));
    }

    private static IEnumerable<CardMetadata> ReadCards(XmlReader reader)
    {
        // 每次只保留当前卡牌的 XML 节点，同时读完整份文档以校验结尾。
        while (reader.Read())
        {
            if (reader.Depth != 1 || reader.NodeType != XmlNodeType.Element || reader.LocalName != "Entity" || reader.NamespaceURI.Length != 0)
                continue;
            using var entity = reader.ReadSubtree();
            yield return ReadCard(XElement.Load(entity));
        }
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
            if (id is (int)GAME_TAG.CARDTEXT or CardNameTag)
            {
                var localized = new LocalizedCardText((string?)tag.Element("enUS"), (string?)tag.Element("zhCN"));
                if (id == CardNameTag)
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
