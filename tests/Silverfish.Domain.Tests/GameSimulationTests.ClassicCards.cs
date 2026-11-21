using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Theory]
    [InlineData(false, 16, 8, 4, 4)]
    [InlineData(true, 8, 4, 2, 2)]
    public void VelenMultipliesAfterSpellDamageAndIgnoresSilencedAndOpposingAuras(bool silenced,
        int spellDamage, int spellHealing, int powerDamage, int powerHealing)
    {
        var spell = Spell("classic-test-spell") with
        {
            OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 3), new HealFriendlyHeroEffect(2)]
        };
        var power = new SimulationCard(new CardDefinition("classic-test-power", EntityKind.HeroPower, 0))
        {
            OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 1), new HealFriendlyHeroEffect(1)]
        };
        var spellDamageMinion = Minion("classic-spell-damage", 2, 2) with { SpellDamage = 1 };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, spell, power, spellDamageMinion]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 11, 12, 13]), (GameZone.Hand, [14]), (GameZone.HeroPower, [15]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 12) },
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "EX1_350", First, 3, 7, 7), InPlay(12, "EX1_350", First, 4, 7, 7) with { Silenced = silenced },
            InPlay(13, spellDamageMinion.Definition.CardId, First, 5, 2, 2), InPlay(21, "EX1_350", Second, 6, 7, 7),
            Card(14, spell.Definition.CardId, First), Card(15, power.Definition.CardId, First)
        ], First);

        var cast = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(14)));
        Assert.Equal(spellDamage, Assert.Single(cast.Events, e => e.Kind == SimulationEventKind.Damage).Amount);
        Assert.Equal(spellHealing, Assert.Single(cast.Events, e => e.Kind == SimulationEventKind.Healed).Amount);
        var used = simulator.Apply(cast.State, new(SimulationActionKind.UseHeroPower, First, new(15)));
        Assert.Equal(powerDamage, Assert.Single(used.Events, e => e.Kind == SimulationEventKind.Damage).Amount);
        Assert.Equal(powerHealing, Assert.Single(used.Events, e => e.Kind == SimulationEventKind.Healed).Amount);
        var attack = simulator.Apply(used.State, new(SimulationActionKind.Attack, First, new(13), SecondHero));
        Assert.Equal(2, Assert.Single(attack.Events, e => e.Kind == SimulationEventKind.Damage).Amount);
    }

    [Fact]
    public void BatchDamageKeepsItsMultiplierWhenTheSameDamageKillsTheAuraSource()
    {
        var sweep = Spell("classic-sweep") with { OnPlay = [new DamageEffect(DamageTarget.AllMinions, 1)] };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, sweep, Minion("classic-target", 1, 3)]);
        var velen = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "EX1_350", First, 3, 7, 7) with { Character = new(7, 7, Damage: 6) },
            Card(12, sweep.Definition.CardId, First), Card(13, "EX1_277", First),
            InPlay(21, "classic-target", Second, 4, 1, 3));
        var cast = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12)));
        Assert.Equal(new int?[] { 2, 2 }, cast.Events.Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Amount));
        Assert.Equal(GameZone.Graveyard, cast.State.LocationOf(velen).Zone);
        var missiles = simulator.Apply(cast.State, new(SimulationActionKind.PlayCard, First, new(13)));
        var hits = missiles.Events.Where(e => e.Kind == SimulationEventKind.Damage).ToArray();
        Assert.Equal(3, hits.Length);
        Assert.All(hits, hit => Assert.Equal(1, hit.Amount));
    }

    [Fact]
    public void ManaAddictStacksSuccessfulPlayerCastsAndLosesTheLayersAtTurnEnd()
    {
        var spell = Spell("classic-drawn-spell") with { Keywords = CardKeywords.CastsWhenDrawn };
        var draw = Spell("classic-draw") with { OnPlay = [new DrawEffect(1)] };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, spell, draw]);
        var addict = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14]), (GameZone.Deck, [15, 16]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Secret, [22])) },
            InPlay(11, "EX1_055", First, 3, 1, 3), InPlay(21, "EX1_055", Second, 4, 1, 3),
            Armed(22, "EX1_287", Second, 5), Card(12, "GAME_005", First), Card(13, draw.Definition.CardId, First),
            Card(14, "GAME_005", First), Card(15, spell.Definition.CardId, First), Card(16, "EX1_055", First));
        var countered = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12)));
        Assert.Contains(countered.Events, e => e.Kind == SimulationEventKind.SpellCountered);
        Assert.Empty(countered.State.Entities[addict].Enchantments);

        var drawn = simulator.Apply(countered.State, new(SimulationActionKind.PlayCard, First, new(13)));
        Assert.Contains(drawn.Events, e => e.Kind == SimulationEventKind.SpellCastWhenDrawn);
        Assert.Equal(3, drawn.State.Entities[addict].Character!.Attack);
        Assert.Equal(new EntityId(13), Assert.Single(drawn.Events, e => e.Kind == SimulationEventKind.Triggered && e.Source == addict).Target);
        var cast = simulator.Apply(drawn.State, new(SimulationActionKind.PlayCard, First, new(14)));
        Assert.Equal(5, cast.State.Entities[addict].Character!.Attack);
        Assert.All(cast.State.Entities[addict].Enchantments, layer => Assert.Equal("EX1_055o", layer.CardId));
        Assert.Empty(cast.State.Entities[new(21)].Enchantments);

        var passed = simulator.Apply(cast.State, new(SimulationActionKind.EndTurn, First));
        Assert.Equal(2, passed.Events.Count(e => e.Kind == SimulationEventKind.EnchantmentExpired && e.Target == addict));
        Assert.Equal(1, passed.State.Entities[addict].Character!.Attack);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InnerRageAddsPermanentAttackBeforeDamageTriggersEvenWhenShieldBlocksDamage(bool shield)
    {
        var minion = Minion("classic-rage-target", 2, 4) with
        {
            Keywords = shield ? CardKeywords.DivineShield : CardKeywords.None,
            Triggers =
            [
                new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
                {
                    EntityMatch = TriggerEntityMatch.Self,
                    Condition = new CharacterCondition(ConditionCharacter.Source, CharacterMetric.Attack, NumberComparison.Equal, 4)
                }
            ]
        };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, minion]);
        var source = new EntityId(12);
        var target = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero),
            InPlay(11, minion.Definition.CardId, First, 3, 2, 4) with { Character = new(2, 4, DivineShield: shield) },
            Card(12, "EX1_607", First));
        var play = Assert.Single(simulator.LegalActions(root), action => action.Source == source);
        Assert.Equal(target, play.Target);
        Assert.Equal(ErrorType.REQ_MINION_TARGET, simulator.CheckAction(root, play with { Target = FirstHero })?.Type);
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(root, play with { Target = null })?.Type);

        var result = simulator.Apply(root, play);
        Assert.Contains(result.Events, e => e.Kind == (shield ? SimulationEventKind.ShieldBroken : SimulationEventKind.Damage));
        Assert.Equal(shield ? 0 : 1, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(shield ? 4 : 3, result.State.Entities[target].Character!.Health);
        Assert.Equal("EX1_607e", Assert.Single(result.State.Entities[target].Enchantments).CardId);
        var next = simulator.Apply(result.State, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(4, next.Entities[target].Character!.Attack);
    }
}
