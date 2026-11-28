using System.Text;
using System.Xml;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed class CardDataCatalogTests
{
    [Fact]
    public void ClientTagsRetainTheirNumericValuesAndProjectIntoTheSimulation()
    {
        using var xml = Utf8("""
            <CardDefs build="1">
              <Entity CardID="client-tags" ID="1">
                <Tag enumID="202" value="10" />
                <Tag enumID="49" value="1" />
                <Tag enumID="200" value="24" />
                <Tag enumID="1635" value="3" />
                <Tag enumID="2837" value="2" />
              </Entity>
            </CardDefs>
            """);
        var catalog = CardDataCatalog.ImportXml(xml);
        var card = catalog.GetCard("client-tags");

        Assert.Equal(TAG_CARDTYPE.HERO_POWER, card.ClientCardType);
        Assert.Equal(TAG_ZONE.PLAY, card.ClientZone);
        Assert.Equal(TAG_RACE.DRAGON, card.ClientRace);
        Assert.Equal(TAG_SPELL_SCHOOL.FROST, card.ClientSpellSchool);
        Assert.Equal(TAG_CARD_ALTERNATE_COST.ARMOR, card.ClientAlternateCost);
        Assert.Equal((int)TAG_CARD_ALTERNATE_COST.ARMOR, card.GetTag(GAME_TAG.CARD_ALTERNATE_COST));
        Assert.Equal(GameZone.HeroPower, ClientZoneMapping.ToGameZone(card.ClientZone, card.ClientCardType));
        Assert.Equal(GameZone.Deck, ClientZoneMapping.ToGameZone(TAG_ZONE.DECK, TAG_CARDTYPE.SPELL));
        Assert.Equal(TAG_CARD_ALTERNATE_COST.ARMOR, card.ToDefinition().AlternateCost);
        Assert.Equal(SpellSchool.Frost, card.ToDefinition().SpellSchool);
    }

    [Fact]
    public void ImportPreservesCardIdentityLocalizationAndNumericTagsAcrossJson()
    {
        using var xml = Utf8("""
            <CardDefs build="256589">
              <Entity CardID="CS2_106" ID="401">
                <Tag enumID="202" value="7" />
                <Tag enumID="45" value="2" />
                <Tag enumID="47" value="3" />
                <Tag enumID="48" value="2" />
                <Tag enumID="185"><enUS>Fiery War Axe</enUS><zhCN>炽炎战斧</zhCN></Tag>
                <Tag enumID="184"><zhCN>&lt;b&gt;牌面文本&lt;/b&gt;</zhCN></Tag>
                <Tag enumID="9999" value="-7" />
                <ReferencedTag enumID="190" value="1" />
              </Entity>
              <Entity CardID="A_ZERO" ID="2"><Tag enumID="202" value="5" /></Entity>
              <Entity CardID="OTHER" ID="3"><Tag enumID="202" value="45" /></Entity>
            </CardDefs>
            """);
        var imported = CardDataCatalog.ImportXml(xml);
        using var json = new MemoryStream();
        imported.WriteJson(json);
        json.Position = 0;
        var loaded = CardDataCatalog.LoadJson(json);

        Assert.Equal(256589, loaded.SourceBuild);
        Assert.Equal(new[] { "A_ZERO", "CS2_106", "OTHER" }, loaded.Cards.Select(card => card.CardId));
        var axe = loaded.GetCard(401);
        Assert.Same(axe, loaded.GetCard("CS2_106"));
        Assert.Equal(new LocalizedCardText("Fiery War Axe", "炽炎战斧"), axe.Name);
        Assert.Equal(new LocalizedCardText(null, "<b>牌面文本</b>"), axe.Text);
        Assert.Equal(new CardDefinition("CS2_106", EntityKind.Weapon, 2, 3, PrintedDurability: 2), axe.ToDefinition());
        Assert.Equal(-7, axe.GetTag(9999));
        Assert.Equal(0, axe.GetTag(190));
        Assert.Equal(new CardDefinition("A_ZERO", EntityKind.Spell, 0), loaded.GetCard("A_ZERO").ToDefinition());
        Assert.Equal(EntityKind.Other, loaded.GetCard("OTHER").ToDefinition().Kind);
        Assert.Throws<InvalidDataException>(() => loaded.GetCard("missing"));
    }

    [Theory]
    [InlineData(7, EntityKind.Weapon)]
    [InlineData(39, EntityKind.Location)]
    public void EquipmentTakesPrintedDurabilityOnlyFromHealthTag(int cardType, EntityKind kind)
    {
        using var xml = Utf8($"""
            <CardDefs build="256589">
              <Entity CardID="WITH_HEALTH" ID="1">
                <Tag enumID="202" value="{cardType}" />
                <Tag enumID="45" value="3" />
                <Tag enumID="187" value="8" />
              </Entity>
              <Entity CardID="WITHOUT_HEALTH" ID="2">
                <Tag enumID="202" value="{cardType}" />
                <Tag enumID="187" value="8" />
              </Entity>
            </CardDefs>
            """);
        var data = CardDataCatalog.ImportXml(xml);

        Assert.Equal(new CardDefinition("WITH_HEALTH", kind, 0, PrintedDurability: 3),
            data.GetCard("WITH_HEALTH").ToDefinition());
        Assert.Equal(new CardDefinition("WITHOUT_HEALTH", kind, 0),
            data.GetCard("WITHOUT_HEALTH").ToDefinition());
    }

    [Fact]
    public void ImportCombinesPrimaryAndAdditionalMinionTypesAndReadsSpellSchool()
    {
        using var xml = Utf8("""
            <CardDefs build="1">
              <Entity CardID="dual" ID="1">
                <Tag enumID="202" value="4" />
                <Tag enumID="200" value="24" />
                <Tag enumID="2523" value="1" />
                <Tag enumID="2534" value="1" />
                <Tag enumID="2542" value="0" />
                <ReferencedTag enumID="2539" value="1" />
              </Entity>
              <Entity CardID="universal" ID="2">
                <Tag enumID="202" value="4" />
                <Tag enumID="200" value="26" />
                <Tag enumID="2534" value="1" />
              </Entity>
              <Entity CardID="extra-only" ID="3">
                <Tag enumID="202" value="4" />
                <Tag enumID="4756" value="1" />
              </Entity>
              <Entity CardID="spell" ID="4">
                <Tag enumID="202" value="5" />
                <Tag enumID="1635" value="3" />
              </Entity>
            </CardDefs>
            """);
        var catalog = CardDataCatalog.ImportXml(xml);
        using var json = new MemoryStream();
        catalog.WriteJson(json);
        json.Position = 0;
        var loaded = CardDataCatalog.LoadJson(json);

        Assert.Equal(MinionType.Dragon | MinionType.Undead, loaded.GetCard("dual").ToDefinition().MinionTypes);
        Assert.Equal(MinionType.All, loaded.GetCard("universal").ToDefinition().MinionTypes);
        Assert.Equal(MinionType.Aberration, loaded.GetCard("extra-only").ToDefinition().MinionTypes);
        Assert.Equal(SpellSchool.Frost, loaded.GetCard("spell").ToDefinition().SpellSchool);
        Assert.Null(loaded.GetCard("spell").ToDefinition().MinionTypes);
    }

    [Fact]
    public void CastWhenDrawnReadsOnlyTheSpellsOwnTag()
    {
        using var xml = Utf8("""
            <CardDefs build="1">
              <Entity CardID="spell" ID="1"><Tag enumID="202" value="5" /><Tag enumID="1077" value="1" /></Entity>
              <Entity CardID="minion" ID="2"><Tag enumID="202" value="4" /><Tag enumID="1077" value="1" /></Entity>
              <Entity CardID="mentions" ID="3"><Tag enumID="202" value="5" /><ReferencedTag enumID="1077" value="1" /></Entity>
            </CardDefs>
            """);
        var catalog = CardDataCatalog.ImportXml(xml);
        Assert.Equal(CardKeywords.CastsWhenDrawn, SimulationCard.FromMetadata(catalog.GetCard("spell")).Keywords);
        Assert.Equal(CardKeywords.None, SimulationCard.FromMetadata(catalog.GetCard("minion")).Keywords);
        Assert.Equal(CardKeywords.None, SimulationCard.FromMetadata(catalog.GetCard("mentions")).Keywords);
    }

    [Fact]
    public void CompiledDataBindsCustomRulesWithoutTheStarterCatalog()
    {
        var rules = new CardRules { OnPlay = [new GainArmorEffect(2)] };

        var card = CompiledCardData.Bind("CS2_029", rules);

        Assert.Equal(EntityKind.Spell, card.Definition.Kind);
        Assert.Equal(4, card.Definition.PrintedCost);
        Assert.Equal(SpellSchool.Fire, card.Definition.SpellSchool);
        Assert.Equal(rules.OnPlay, card.OnPlay);
    }

    [Fact]
    public void XmlImportReadsOnlyDirectEntitiesAndValidatesTheWholeDocument()
    {
        const string input = """
            <CardDefs build="1">
              <Entity CardID="first" ID="1" />
              <Unused><Entity CardID="first" ID="1" /></Unused>
              <Entity CardID="last" ID="2" />
            </CardDefs>
            """;
        using var valid = Utf8(input);
        var catalog = CardDataCatalog.ImportXml(valid);
        Assert.Equal(new[] { "first", "last" }, catalog.Cards.Select(card => card.CardId));

        using var trailingXml = Utf8(input + "<Extra />");
        Assert.Throws<XmlException>(() => CardDataCatalog.ImportXml(trailingXml));
    }

    [Theory]
    [InlineData("""{"schemaVersion":2,"sourceBuild":256589,"cards":[]}""")]
    [InlineData("""{"schemaVersion":1,"sourceBuild":256589,"cards":[{"cardId":"A","dbfId":2,"tags":{}},{"cardId":"B","dbfId":2,"tags":{}}]}""")]
    [InlineData("""{"schemaVersion":1,"sourceBuild":256589,"cards":[{"cardId":"A","dbfId":2,"tags":{}},{"cardId":"A","dbfId":3,"tags":{}}]}""")]
    public void RejectsUnknownSchemaAndAmbiguousIds(string input)
    {
        using var json = Utf8(input);
        Assert.Throws<InvalidDataException>(() => CardDataCatalog.LoadJson(json));
    }

    private static MemoryStream Utf8(string text) => new(Encoding.UTF8.GetBytes(text));
}
