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

    [Fact]
    public void BaneOfDoomTargetsCharactersAndSummonsEligibleDemonOnlyOnLethalWithBoardSpace()
    {
        var demon = new SimulationCard(new("classic-demon", EntityKind.Minion, 4, 4, 5) { MinionTypes = MinionType.Demon })
        {
            OnPlay = [new GainArmorEffect(5)]
        };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("tough", 2, 4), demon],
            randomMinionPool: ["CS2_231", "EX1_301", demon.Definition.CardId]);
        var bane = new EntityId(11);
        var enemyThreeHp = new EntityId(21);
        var enemyFourHp = new EntityId(22);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "EX1_320", First), InPlay(21, "EX1_055", Second, 3, 1, 3), InPlay(22, "tough", Second, 4, 2, 4));

        var targets = simulator.LegalActions(root).Where(a => a.Source == bane).Select(a => a.Target).ToArray();
        Assert.Equal(new EntityId?[] { FirstHero, SecondHero, enemyThreeHp, enemyFourHp }, targets);
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY,
            simulator.CheckAction(root, new(SimulationActionKind.PlayCard, First, bane))?.Type);

        var nonLethal = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, bane, enemyFourHp));
        Assert.Equal(1, nonLethal.State.Entities[enemyFourHp].Character!.Health);
        Assert.DoesNotContain(nonLethal.Events, e => e.Kind == SimulationEventKind.Summoned);

        var lethalDist = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, bane, enemyThreeHp));
        Assert.True(lethalDist.IsExact);
        Assert.Equal(["EX1_301", demon.Definition.CardId],
            lethalDist.Outcomes.Select(o => o.Result.State.Entities[Assert.Single(o.Result.State.Players[First].In(GameZone.Play), id => id != FirstHero)].CardId));
        Assert.All(lethalDist.Outcomes, o =>
        {
            Assert.Equal(GameZone.Graveyard, o.Result.State.LocationOf(enemyThreeHp).Zone);
            Assert.Equal(0, o.Result.State.Entities[FirstHero].Character!.Armor);
        });

        var fullIds = Enumerable.Range(31, 7).ToArray();
        var fullBoard = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5, Zones = Zones((GameZone.Play, [10, .. fullIds]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            [Card(11, "EX1_320", First), .. fullIds.Select((id, index) => InPlay(id, "CS2_231", First, 3 + index, 1, 1))]);
        var selfKill = simulator.Apply(fullBoard, new(SimulationActionKind.PlayCard, First, bane, new(31)));
        Assert.DoesNotContain(selfKill.Events, e => e.Kind == SimulationEventKind.Summoned);
        Assert.Equal(6, selfKill.State.Players[First].In(GameZone.Play).Count(id => id != FirstHero));

        var defaultPoolSim = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var defaultState = Create(defaultPoolSim, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "EX1_320", First), InPlay(21, "CS2_231", Second, 3, 1, 1));
        var summonedId = Assert.Single(defaultPoolSim.Apply(defaultState, new(SimulationActionKind.PlayCard, First, bane, enemyThreeHp))
            .State.Players[First].In(GameZone.Play), id => id != FirstHero);
        var summonedTypes = CompiledCardData.Bind(defaultPoolSim.Apply(defaultState, new(SimulationActionKind.PlayCard, First, bane, enemyThreeHp))
            .State.Entities[summonedId].CardId, new()).Definition.MinionTypes.GetValueOrDefault();
        Assert.True(summonedTypes.HasFlag(MinionType.Demon) || summonedTypes.HasFlag(MinionType.All));
    }

    [Fact]
    public void ThoughtstealCopiesUpToTwoDistinctOpponentDeckCardsWithoutAlteringOpponentDeck()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21, 22, 23])) },
            Card(11, "EX1_339", First), Card(21, "CS2_231", Second), Card(22, "EX1_350", Second), Card(23, "EX1_607", Second));

        var dist = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, new(11)));
        Assert.True(dist.IsExact);
        Assert.Equal(6, dist.Outcomes.Length);
        Assert.All(dist.Outcomes, outcome =>
        {
            var generated = outcome.Result.State.Players[First].In(GameZone.Hand).Select(id => outcome.Result.State.Entities[id]).ToArray();
            Assert.Equal(2, generated.Length);
            Assert.Equal(2, generated.Select(card => card.CardId).Distinct().Count());
            Assert.All(generated, card => Assert.Equal(First, card.Owner));
            Assert.Equal(new[] { 21, 22, 23 }, outcome.Result.State.Players[Second].In(GameZone.Deck).Select(id => id.Value));
        });

        var emptyDeck = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "EX1_339", First));
        var castOnEmpty = simulator.Apply(emptyDeck, new(SimulationActionKind.PlayCard, First, new(11)));
        Assert.Empty(castOnEmpty.State.Players[First].In(GameZone.Hand));
        Assert.DoesNotContain(castOnEmpty.Events, e => e.Kind == SimulationEventKind.Fatigue);
    }

    [Fact]
    public void AlAkirAndSilvermoonGuardianApplyTheirPrintedCombatKeywordsAndElementalTracking()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var alAkir = new EntityId(11);
        var guardian = new EntityId(12);
        var blocker = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 10, ManaCrystals = 10, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "NEW1_010", First), Card(12, "EX1_023", First), InPlay(21, "CS2_231", Second, 3, 2, 2));

        var playedAlAkir = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, alAkir, Position: 0)).State;
        Assert.True(playedAlAkir.Players[First].PlayedElementalThisTurn);
        Assert.True(playedAlAkir.Entities[alAkir].Character!.DivineShield);

        var firstSwing = simulator.Apply(playedAlAkir, new(SimulationActionKind.Attack, First, alAkir, blocker));
        Assert.Contains(firstSwing.Events, e => e.Kind == SimulationEventKind.ShieldBroken && e.Target == alAkir);
        Assert.Equal(6, firstSwing.State.Entities[alAkir].Character!.Health);
        Assert.Equal(GameZone.Graveyard, firstSwing.State.LocationOf(blocker).Zone);

        var secondSwing = simulator.Apply(firstSwing.State, new(SimulationActionKind.Attack, First, alAkir, SecondHero));
        Assert.Equal(27, secondSwing.State.Entities[SecondHero].Character!.Health);
        Assert.DoesNotContain(simulator.LegalActions(secondSwing.State), a => a.Kind == SimulationActionKind.Attack && a.Source == alAkir);

        var nextTurn = simulator.Apply(
            simulator.Apply(secondSwing.State, new(SimulationActionKind.EndTurn, First)).State,
            new(SimulationActionKind.EndTurn, Second)).State;
        var playedGuardian = simulator.Apply(nextTurn, new(SimulationActionKind.PlayCard, First, guardian, Position: 1)).State;
        Assert.True(playedGuardian.Entities[guardian].Character!.DivineShield);
        Assert.DoesNotContain(simulator.LegalActions(playedGuardian), a => a.Kind == SimulationActionKind.Attack && a.Source == guardian);
    }

    [Fact]
    public void CenariusBuffsOnlyOtherFriendlyMinionsOrSummonsTauntTreantsOnEitherSide()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var leftWisp = new EntityId(11);
        var rightWisp = new EntityId(12);
        var cenarius = new EntityId(13);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 8, ManaCrystals = 8, Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13])) },
            Player(Second, SecondHero),
            InPlay(11, "CS2_231", First, 3, 1, 1), InPlay(12, "CS2_231", First, 4, 1, 1), Card(13, "EX1_573", First));

        var buffAction = new SimulationAction(SimulationActionKind.PlayCard, First, cenarius, Position: 1) { Choice = "buffMinions" };
        var summonAction = new SimulationAction(SimulationActionKind.PlayCard, First, cenarius, Position: 1) { Choice = "summonTreants" };
        Assert.Contains(buffAction, simulator.LegalActions(root));
        Assert.Contains(summonAction, simulator.LegalActions(root));

        var buffed = simulator.Apply(root, buffAction).State;
        Assert.Equal(3, buffed.Entities[leftWisp].Character!.Attack);
        Assert.Equal(3, buffed.Entities[leftWisp].Character!.Health);
        Assert.Equal("EX1_573ae", Assert.Single(buffed.Entities[rightWisp].Enchantments).CardId);
        Assert.Equal(5, buffed.Entities[cenarius].Character!.Attack);
        Assert.Equal(8, buffed.Entities[cenarius].Character!.Health);
        Assert.Empty(buffed.Entities[cenarius].Enchantments);

        var summoned = simulator.Apply(root, summonAction).State;
        var board = summoned.Players[First].In(GameZone.Play).Where(id => id != FirstHero).ToArray();
        Assert.Equal(5, board.Length);
        Assert.Equal(leftWisp, board[0]);
        Assert.Equal(cenarius, board[2]);
        Assert.Equal(rightWisp, board[4]);
        Assert.All(new[] { board[1], board[3] }, treantId =>
        {
            Assert.Equal("EX1_573t", summoned.Entities[treantId].CardId);
            Assert.Equal(2, summoned.Entities[treantId].Character!.Attack);
            Assert.Equal(2, summoned.Entities[treantId].Character!.Health);
            Assert.True(simulator.Cards["EX1_573t"].Keywords.HasFlag(CardKeywords.Taunt));
        });
        Assert.True(summoned.Entities[board[3]].PlayOrder < summoned.Entities[board[1]].PlayOrder);

        var fiveIds = Enumerable.Range(31, 5).ToArray();
        var oneSlotLeft = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 8, ManaCrystals = 8, Zones = Zones((GameZone.Play, [10, .. fiveIds]), (GameZone.Hand, [13])) },
            Player(Second, SecondHero),
            [Card(13, "EX1_573", First), .. fiveIds.Select((id, index) => InPlay(id, "CS2_231", First, 3 + index, 1, 1))]);
        var oneTreant = simulator.Apply(oneSlotLeft, new(SimulationActionKind.PlayCard, First, cenarius, Position: 2) { Choice = "summonTreants" }).State;
        var fullSlots = oneTreant.Players[First].In(GameZone.Play).Where(id => id != FirstHero).ToArray();
        Assert.Equal(7, fullSlots.Length);
        Assert.Equal(cenarius, fullSlots[2]);
        Assert.Equal("EX1_573t", oneTreant.Entities[fullSlots[3]].CardId);
    }
}
