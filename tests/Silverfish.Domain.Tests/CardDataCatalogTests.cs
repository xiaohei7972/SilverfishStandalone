using System.Text;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed class CardDataCatalogTests
{
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
