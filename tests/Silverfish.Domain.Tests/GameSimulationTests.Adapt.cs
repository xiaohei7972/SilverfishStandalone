using System.Text.Json;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static readonly EntityId Hatchling = new(11);
    private static readonly EntityId AdaptDefender = new(21);

    [Fact]
    public void HatchlingOffersThreeDistinctAdaptationsAndResumesTheSameChoice()
    {
        var (simulator, pending) = PlayHatchling();
        var choice = Assert.IsType<PendingPlayChoice>(pending.PendingPlayChoice);
        var rule = Assert.IsType<AdaptChoice>(simulator.Cards["UNG_001"].ChooseAfterPlay);
        Assert.Equal(10, rule.Options.Length);
        Assert.Equal(3, choice.Options.Select(id => pending.Entities[id].CardId).Distinct().Count());
        Assert.All(choice.Options, id =>
        {
            Assert.Equal(GameZone.SetAside, pending.LocationOf(id).Zone);
            Assert.Contains(pending.Entities[id].CardId, rule.Options);
        });
        var select = simulator.LegalActions(pending)[0];
        Assert.Equal(SimulationActionKind.ChooseOption, select.Kind);
        var state = JsonSerializer.Deserialize<GameContinuation>(JsonSerializer.Serialize(
            new GameContinuation(pending.NextEntity, pending.NextPlayOrder, pending.Outcome, pending.Winner)
            {
                PendingPlayChoice = choice
            }))!;
        var restored = simulator.RestoreGame(pending.Players.Values, pending.Entities.Values, pending.ActivePlayer,
            pending.Turn, pending.MinionDeaths, pending.RandomState, state);
        var resumed = simulator.Apply(restored, select);

        Assert.Equal<SimulationEvent>(simulator.Apply(pending, select).Events, resumed.Events);
        Assert.Equal(pending.Players[First].Mana, resumed.State.Players[First].Mana);
        Assert.Equal(pending.Entities[select.Target!.Value].CardId + "e", Assert.Single(resumed.State.Entities[Hatchling].Enchantments).CardId);
        Assert.DoesNotContain(resumed.Events, entry => entry.Kind is SimulationEventKind.CardPlayed or SimulationEventKind.Summoned);
        Assert.NotNull(simulator.CheckAction(resumed.State, select));
    }

    [Fact]
    public void AdaptCanResumeOnTheOriginallySelectedMinion()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("unit", 1, 3)]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12, 13]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            Card(11, "UNG_961", First), InPlay(12, "unit", First, 3, 1, 3), InPlay(13, "unit", First, 4, 1, 3));
        var selected = new EntityId(13);
        var pending = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, Hatchling, selected)).State;
        Assert.Equal(selected, Assert.Single(pending.PendingPlayChoice!.Recipients!.Value));
        var restored = RestoreAdapt(simulator, pending);
        var result = simulator.Apply(restored, simulator.LegalActions(restored)[0]);
        Assert.Empty(result.State.Entities[new(12)].Enchantments);
        Assert.Single(result.State.Entities[selected].Enchantments);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(Hatchling).Zone);
    }

    [Fact]
    public void AdaptGroupUsesTheSameSelectionForAllEligibleFriendlyMinions()
    {
        var murloc = new SimulationCard(new CardDefinition("murloc", EntityKind.Minion, 0, 1, 3)
        {
            MinionTypes = MinionType.Murloc
        });
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, murloc, Minion("unit", 1, 3)]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 10, ManaSpent = 0, Zones = Zones((GameZone.Play, [10, 12, 13, 14]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "UNG_089", First), InPlay(12, "murloc", First, 3, 1, 3), InPlay(13, "unit", First, 4, 1, 3),
            InPlay(14, "murloc", First, 5, 1, 3), InPlay(21, "murloc", Second, 6, 1, 3));
        var pending = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, Hatchling, Position: 0)).State;
        Assert.Equal(new[] { new EntityId(12), new EntityId(14) }, pending.PendingPlayChoice!.Recipients!.Value);
        var resolved = simulator.Apply(pending, simulator.LegalActions(pending)[0]).State;
        Assert.Equal(Assert.Single(resolved.Entities[new(12)].Enchantments).CardId,
            Assert.Single(resolved.Entities[new(14)].Enchantments).CardId);
        Assert.Empty(resolved.Entities[new(13)].Enchantments);
        Assert.Empty(resolved.Entities[new(21)].Enchantments);
    }

    [Fact]
    public void RepeatedAdaptKeepsItsNextRoundAcrossSaveAndResume()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 10, ManaSpent = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero), Card(11, "UNG_002", First));
        var pending = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, Hatchling, Position: 0)).State;
        Assert.Equal(2, pending.PendingPlayChoice!.RemainingSelections);
        var choose = simulator.LegalActions(pending)[0];
        var next = simulator.Apply(pending, choose).State;
        Assert.Equal(1, next.PendingPlayChoice!.RemainingSelections);
        Assert.Equal(3, next.PendingPlayChoice.Options.Length);
        Assert.Empty(pending.PendingPlayChoice.Options.Intersect(next.PendingPlayChoice.Options));
        Assert.NotNull(simulator.CheckAction(next, choose));

        var restored = RestoreAdapt(simulator, next);
        var second = simulator.LegalActions(next)[0];
        var result = simulator.Apply(restored, second);
        Assert.Equal<SimulationEvent>(simulator.Apply(next, second).Events, result.Events);
        Assert.Equal(pending.Players[First].Mana, result.State.Players[First].Mana);
        Assert.Null(result.State.PendingPlayChoice);
        Assert.DoesNotContain(result.Events, entry => entry.Kind == SimulationEventKind.CardPlayed);
    }

    private static GameState RestoreAdapt(GameSimulator simulator, GameState pending)
    {
        var checkpoint = new GameContinuation(pending.NextEntity, pending.NextPlayOrder, pending.Outcome, pending.Winner)
        {
            PendingPlayChoice = pending.PendingPlayChoice
        };
        var restored = JsonSerializer.Deserialize<GameContinuation>(JsonSerializer.Serialize(checkpoint))!;
        return simulator.RestoreGame(pending.Players.Values, pending.Entities.Values, pending.ActivePlayer,
            pending.Turn, pending.MinionDeaths, pending.RandomState, restored);
    }

    [Theory]
    [InlineData("UNG_999t2", 0, 0, GAME_TAG.DEATHRATTLE)]
    [InlineData("UNG_999t3", 3, 0, null)]
    [InlineData("UNG_999t4", 0, 3, null)]
    [InlineData("UNG_999t5", 0, 0, GAME_TAG.ELUSIVE)]
    [InlineData("UNG_999t6", 0, 0, GAME_TAG.TAUNT)]
    [InlineData("UNG_999t7", 0, 0, GAME_TAG.WINDFURY)]
    [InlineData("UNG_999t8", 0, 0, GAME_TAG.DIVINE_SHIELD)]
    [InlineData("UNG_999t10", 0, 0, GAME_TAG.STEALTH)]
    [InlineData("UNG_999t13", 0, 0, GAME_TAG.POISONOUS)]
    [InlineData("UNG_999t14", 1, 1, null)]
    public void EachAdaptationUsesItsOwnEnchantment(string cardId, int attack, int health, GAME_TAG? tag)
    {
        var (simulator, pending) = PlayHatchling(cardId);
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State.Entities[Hatchling];
        Assert.Equal(pending.Entities[Hatchling].Character!.Attack + attack, adapted.Character!.Attack);
        Assert.Equal(pending.Entities[Hatchling].Character!.MaximumHealth + health, adapted.Character.MaximumHealth);
        var enchantment = Assert.Single(adapted.Enchantments);
        Assert.Equal(cardId + "e", enchantment.CardId);
        Assert.Equal(tag, enchantment.GrantedTag);
        Assert.Equal(tag == GAME_TAG.DIVINE_SHIELD, adapted.Character.DivineShield);
    }

    [Theory]
    [InlineData(false, 2)]
    [InlineData(true, 0)]
    public void LivingSporesTriggersOnDeathAndSilenceRemovesIt(bool silence, int plants)
    {
        var (simulator, pending) = PlayHatchling("UNG_999t2");
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        if (silence)
            enemy = simulator.Apply(enemy, new(SimulationActionKind.PlayCard, Second, new EntityId(24), Hatchling)).State;
        var killed = simulator.Apply(enemy, new(SimulationActionKind.PlayCard, Second, new EntityId(23), Hatchling));
        Assert.Equal(GameZone.Graveyard, killed.State.LocationOf(Hatchling).Zone);
        Assert.Equal(plants, killed.State.Players[First].In(GameZone.Play)
            .Count(id => killed.State.Entities[id].CardId == "UNG_999t2t1"));
        Assert.Equal(silence ? 0 : 1, killed.Events.Count(entry => entry.Kind == SimulationEventKind.Deathrattle));
    }

    [Fact]
    public void ShroudingMistBlocksEnemyTargetsAndExpiresBeforeTheNextOwnerTurn()
    {
        var (simulator, pending) = PlayHatchling("UNG_999t10");
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        var spell = new SimulationAction(SimulationActionKind.PlayCard, Second, new EntityId(25), Hatchling);
        var attack = new SimulationAction(SimulationActionKind.Attack, Second, AdaptDefender, Hatchling);
        Assert.Equal(ErrorType.REQ_NONSTEALTH_ENEMY_TARGET, simulator.CheckAction(enemy, spell)?.Type);
        Assert.DoesNotContain(attack, simulator.LegalActions(enemy));

        var back = simulator.Apply(enemy, new(SimulationActionKind.EndTurn, Second));
        Assert.Empty(back.State.Entities[Hatchling].Enchantments);
        Assert.Contains(back.Events, entry => entry.Kind == SimulationEventKind.EnchantmentExpired && entry.Target == Hatchling);
        var enemyAgain = simulator.Apply(back.State, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Contains(attack, simulator.LegalActions(enemyAgain));
        Assert.Null(simulator.CheckAction(enemyAgain, spell));
    }

    [Fact]
    public void AttackingBreaksShroudingMistBeforeItsScheduledExpiry()
    {
        var (simulator, pending) = PlayHatchling("UNG_999t10", charge: true);
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var attack = simulator.Apply(adapted, new(SimulationActionKind.Attack, First, Hatchling, SecondHero));
        Assert.Empty(attack.State.Entities[Hatchling].Enchantments);
        var enemy = simulator.Apply(attack.State, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Contains(new SimulationAction(SimulationActionKind.Attack, Second, AdaptDefender, Hatchling), simulator.LegalActions(enemy));
    }

    [Fact]
    public void LiquidMembraneBlocksASpellAndAllowsABattlecry()
    {
        var (simulator, pending) = PlayHatchling("UNG_999t5");
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(ErrorType.REQ_CAN_BE_TARGETED_BY_SPELLS, simulator.CheckAction(enemy,
            new(SimulationActionKind.PlayCard, Second, new EntityId(25), Hatchling))?.Type);
        Assert.Null(simulator.CheckAction(enemy, new(SimulationActionKind.PlayCard, Second, new EntityId(26), Hatchling, Position: 0)));
    }

    [Fact]
    public void MassiveForcesAttacksOntoTheAdaptedMinion()
    {
        var (simulator, pending) = PlayHatchling("UNG_999t6");
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        var attack = Assert.Single(simulator.LegalActions(enemy), action => action.Kind == SimulationActionKind.Attack);
        Assert.Equal(Hatchling, attack.Target);
    }

    [Fact]
    public void LightningSpeedAllowsExactlyTwoAttacks()
    {
        var (simulator, pending) = PlayHatchling("UNG_999t7");
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        var ready = simulator.Apply(enemy, new(SimulationActionKind.EndTurn, Second)).State;
        var attack = new SimulationAction(SimulationActionKind.Attack, First, Hatchling, SecondHero);
        var once = simulator.Apply(ready, attack).State;
        Assert.Contains(attack, simulator.LegalActions(once));
        var twice = simulator.Apply(once, attack).State;
        Assert.DoesNotContain(attack, simulator.LegalActions(twice));
    }

    [Fact]
    public void CracklingShieldIsConsumedByDamage()
    {
        var (simulator, pending) = PlayHatchling("UNG_999t8");
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        var hit = simulator.Apply(enemy, new(SimulationActionKind.PlayCard, Second, new EntityId(25), Hatchling));
        var target = hit.State.Entities[Hatchling];
        Assert.False(target.Character!.DivineShield);
        Assert.Equal(0, target.Character.Damage);
        Assert.Empty(target.Enchantments);
        Assert.Contains(hit.Events, entry => entry.Kind == SimulationEventKind.ShieldBroken);
    }

    [Theory]
    [InlineData(false, GameZone.Graveyard)]
    [InlineData(true, GameZone.Play)]
    public void PoisonSpitDestroysOnlyAMinionThatTookDamage(bool shielded, GameZone zone)
    {
        var (simulator, pending) = PlayHatchling("UNG_999t13", shielded);
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        var ready = simulator.Apply(enemy, new(SimulationActionKind.EndTurn, Second)).State;
        var attack = simulator.Apply(ready, new(SimulationActionKind.Attack, First, Hatchling, AdaptDefender));
        Assert.Equal(zone, attack.State.LocationOf(AdaptDefender).Zone);
    }

    [Fact]
    public void PoisonSpitIsRecordedBeforeAFollowUpSilence()
    {
        var (simulator, pending) = PlayHatchling("UNG_999t13", silenceOnDamage: true);
        var adapted = simulator.Apply(pending, Assert.Single(simulator.LegalActions(pending))).State;
        var enemy = simulator.Apply(adapted, new(SimulationActionKind.EndTurn, First)).State;
        var ready = simulator.Apply(enemy, new(SimulationActionKind.EndTurn, Second)).State;
        var attack = simulator.Apply(ready, new(SimulationActionKind.Attack, First, Hatchling, AdaptDefender));
        Assert.True(attack.State.Entities[Hatchling].Silenced);
        Assert.Equal(GameZone.Graveyard, attack.State.LocationOf(AdaptDefender).Zone);
        Assert.Equal(
            new[] { SimulationEventKind.Silenced, SimulationEventKind.Died },
            attack.Events.Where(entry => entry.Kind is SimulationEventKind.Silenced or SimulationEventKind.Died).Select(entry => entry.Kind));
    }

    private static (GameSimulator Simulator, GameState State) PlayHatchling(string? onlyOption = null, bool shielded = false,
        bool charge = false, bool silenceOnDamage = false)
    {
        var starter = onlyOption is null ? StarterCardCatalog.Cards : StarterCardCatalog.Cards
            .Select(card => card.Definition.CardId == "UNG_001" ? card with
            {
                ChooseAfterPlay = new AdaptChoice([onlyOption]),
                Keywords = charge ? card.Keywords | CardKeywords.Charge : card.Keywords
            } : card);
        var defender = Minion("adapt-defender", 1, 8) with
        {
            Triggers = silenceOnDamage
                ? [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new DrawEffect(1)])
                    { EntityMatch = TriggerEntityMatch.Self, RequireLivingEntity = true }]
                : []
        };
        var simulator = new GameSimulator([
            .. starter, Hero, defender,
            Spell("adapt-kill") with { Target = TargetRequirement.Minion, OnPlay = [new DestroyMinionEffect()] },
            Spell("adapt-ping") with { Target = TargetRequirement.Minion, OnPlay = [new DamageEffect(DamageTarget.Selected, 1)] },
            Spell("adapt-drawn-silence") with
            {
                Keywords = CardKeywords.CastsWhenDrawn, Target = TargetRequirement.EnemyMinion, OnPlay = [new SilenceEffect()]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 10, ManaSpent = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [23, 24, 25, 26]), (GameZone.Deck, silenceOnDamage ? [27, 28] : []))
            },
            [
                Card(11, "UNG_001", First),
                InPlay(21, "adapt-defender", Second, 3, 1, 8) with { Character = new(1, 8, DivineShield: shielded) },
                Card(23, "adapt-kill", Second), Card(24, "EX1_332", Second),
                Card(25, "adapt-ping", Second), Card(26, "DAL_614", Second),
                .. silenceOnDamage ? new[] { Card(27, "adapt-ping", Second), Card(28, "adapt-drawn-silence", Second) } : []
            ]);
        return (simulator, simulator.Apply(root, new(SimulationActionKind.PlayCard, First, Hatchling, Position: 0)).State);
    }
}
