using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void DescribesAndAppliesLightningBoltsEffectivePlayRequirements()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var source = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { ManaCrystals = 3, ManaSpent = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "EX1_238", First));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source, FirstHero);

        var declared = PlayReqCatalog.Cards["EX1_238"];
        Assert.True(declared.EffectImplemented);
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, Assert.Single(declared.Requirements.Play).Type);
        var branch = Assert.Single(simulator.ExplainSource(root, source).Branches);
        Assert.Equal(PlayTargetMode.Required, branch.TargetMode);
        Assert.Equal(new EntityId?[] { FirstHero, SecondHero }, branch.Targets);
        Assert.Contains(play, simulator.LegalActions(root));
        Assert.Null(simulator.CheckAction(root, play));
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(root, play with { Target = null })?.Type);
        Assert.Equal(27, simulator.Apply(root, play).State.Entities[FirstHero].Character!.Health);
    }

    [Fact]
    public void ComboTargetDependsOnPlaysBeforeTheAgentEntersPlay()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var agent = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { ManaCrystals = 3, ManaSpent = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero), Card(11, "GAME_005", First), Card(12, "EX1_134", First));

        var withoutCombo = Assert.Single(simulator.LegalActions(root), action => action.Source == agent);
        Assert.Null(withoutCombo.Target);
        Assert.DoesNotContain(simulator.Apply(root, withoutCombo).Events, entry => entry.Kind == SimulationEventKind.Damage);

        var primed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        var withCombo = Assert.Single(simulator.LegalActions(primed), action => action.Source == agent && action.Target == SecondHero);
        Assert.NotNull(simulator.CheckAction(primed, withoutCombo));
        Assert.Equal(27, simulator.Apply(primed, withCombo).State.Entities[SecondHero].Character!.Health);
    }

    [Fact]
    public void KoboldLackeyTargetsEitherHeroAndRespectsEnemyStealth()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("unit", 1, 3)]);
        var source = new EntityId(11);
        var stealthed = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "DAL_614", First), InPlay(21, "unit", Second, 3, 1, 3) with { Stealthed = true });
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source, SecondHero, Position: 0);

        var explanation = simulator.ExplainSource(root, source);
        Assert.True(explanation.EffectImplemented);
        Assert.Equal(ErrorType.REQ_TARGET_IF_AVAILABLE, Assert.Single(PlayReqCatalog.Cards["DAL_614"].Requirements.Play).Type);
        var branch = Assert.Single(explanation.Branches);
        Assert.Equal(PlayTargetMode.IfAvailable, branch.TargetMode);
        Assert.Equal(new EntityId?[] { FirstHero, SecondHero }, branch.Targets);
        Assert.Equal(new[] { play with { Target = FirstHero }, play }, simulator.LegalActions(root).Where(action => action.Source == source));
        Assert.Equal(ErrorType.REQ_NONSTEALTH_ENEMY_TARGET, simulator.CheckAction(root, play with { Target = stealthed })?.Type);
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(root, play with { Target = null })?.Type);

        var result = simulator.Apply(root, play);
        Assert.Equal(GameZone.Play, result.State.LocationOf(source).Zone);
        Assert.Equal(28, result.State.Entities[SecondHero].Character!.Health);
        var damage = Assert.Single(result.Events, entry => entry.Kind == SimulationEventKind.Damage);
        Assert.Equal(source, damage.Source);
        Assert.Equal(SecondHero, damage.Target);
        Assert.Equal(2, damage.Amount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PendingDragonConditionControlsGenericOptionalTargeting(bool hasDragon)
    {
        var dragon = new SimulationCard(new CardDefinition("dragon", EntityKind.Minion, 0, 1, 1)
        {
            MinionTypes = MinionType.Dragon
        });
        var simulator = new GameSimulator([Hero, dragon]);
        var source = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, hasDragon ? [11, 12] : [11])) },
            Player(Second, SecondHero), hasDragon ? [Card(11, "DRG_081", First), Card(12, "dragon", First)] : [Card(11, "DRG_081", First)]);

        var explanation = simulator.ExplainSource(root, source);
        Assert.False(explanation.EffectImplemented);
        var branch = Assert.Single(explanation.Branches);
        Assert.Equal(hasDragon ? PlayTargetMode.IfAvailable : PlayTargetMode.None, branch.TargetMode);
        Assert.Equal(hasDragon ? new EntityId?[] { FirstHero, SecondHero } : new EntityId?[] { null }, branch.Targets);
        Assert.DoesNotContain(simulator.LegalActions(root), action => action.Source == source);
    }

    [Fact]
    public void OptionalBattlecryAllowsNoTargetWhenLegacyDataAlsoDeclaresATarget()
    {
        var simulator = new GameSimulator([Hero]);
        var source = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "CS2_188", First));

        var branch = Assert.Single(simulator.ExplainSource(root, source).Branches);
        Assert.Equal(PlayTargetMode.IfAvailable, branch.TargetMode);
        Assert.Null(Assert.Single(branch.Targets));
        Assert.Null(branch.Failure);
    }

    [Fact]
    public void CopiedOptionalTargetRulesUseTheSameDecisionForEnumerationReplayAndForecasts()
    {
        var copied = Spell("copy") with
        {
            PlayReqs = new([PlayReq.Copied], []),
            OnPlay = [new GainArmorEffect(2)]
        };
        var simulator = new GameSimulator([Hero, copied, Minion("unit", 1, 3)]);
        var source = new EntityId(11);
        var first = Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) };
        var card = Card(11, "copy", First) with { CopiedSpellCardId = "CS2_188" };
        var root = Create(simulator, First, first, Player(Second, SecondHero), card);
        var actions = simulator.GetLegalActionSet(root).Filter(action => action.Source == source);
        var play = Assert.Single(actions.Actions);

        Assert.Null(play.Target);
        Assert.Null(simulator.CheckAction(root, play));
        Assert.Equal(PlayTargetMode.IfAvailable, Assert.Single(simulator.ExplainSource(root, source).Branches).TargetMode);
        Assert.DoesNotContain(simulator.Apply(root, play).Events, entry => entry.Kind == SimulationEventKind.ArmorGained);
        Assert.DoesNotContain(Assert.Single(simulator.ApplyDistribution(root, play).Outcomes).Result.Events,
            entry => entry.Kind == SimulationEventKind.ArmorGained);
        Assert.DoesNotContain(Assert.Single(actions.ApplyDistribution(0).Outcomes).Result.Events,
            entry => entry.Kind == SimulationEventKind.ArmorGained);

        var second = Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) };
        var withTarget = Create(simulator, First, first, second, card, InPlay(21, "unit", Second, 3, 1, 3));
        var targetedPlay = Assert.Single(simulator.LegalActions(withTarget), action => action.Source == source);
        Assert.Equal(new EntityId(21), targetedPlay.Target);
        Assert.NotNull(simulator.CheckAction(withTarget, play));
        Assert.Equal(2, simulator.Apply(withTarget, targetedPlay).State.Entities[FirstHero].Character!.Armor);
    }

    [Fact]
    public void ExecuteDestroysOnlyDamagedEnemyMinions()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("unit", 2, 4)]);
        var source = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 2, ManaSpent = 0, Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "CS2_108", First),
            InPlay(12, "unit", First, 3, 2, 4) with { Character = new(2, 4, Damage: 1) },
            InPlay(21, "unit", Second, 4, 2, 4) with { Character = new(2, 4, Damage: 1) },
            InPlay(22, "unit", Second, 5, 2, 4));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source, new(21));

        Assert.Equal(play, Assert.Single(simulator.LegalActions(root), action => action.Source == source));
        Assert.Equal(ErrorType.REQ_ENEMY_TARGET, simulator.CheckAction(root, play with { Target = new EntityId(12) })?.Type);
        var undamaged = Assert.IsType<PlayFailure>(simulator.CheckAction(root, play with { Target = new EntityId(22) }));
        Assert.Equal(ErrorType.REQ_DAMAGED_TARGET, undamaged.Type);
        Assert.Equal(0, undamaged.Actual);
        var result = simulator.Apply(root, play);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new(21)).Zone);
        Assert.Equal(new EntityId(21), Assert.Single(result.State.MinionDeaths).Entity);
    }

    [Theory]
    [InlineData("CS2_234", 3, ErrorType.REQ_TARGET_MAX_ATTACK)]
    [InlineData("EX1_622", 5, ErrorType.REQ_TARGET_MIN_ATTACK)]
    public void ShadowWordsUseCurrentAttackAndIncludeTheBoundary(string cardId, int limit, ErrorType requirement)
    {
        var adjustment = Spell("adjustment") with
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, Attack: limit - 4))]
        };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, adjustment, Minion("unit", 4, 4)]);
        var source = new EntityId(11);
        var target = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 2, ManaSpent = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, cardId, First), Card(12, "adjustment", First), InPlay(21, "unit", Second, 3, 4, 4));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source, target);

        Assert.DoesNotContain(simulator.LegalActions(root), action => action.Source == source);
        var failure = Assert.IsType<PlayFailure>(simulator.CheckAction(root, play));
        Assert.Equal(requirement, failure.Type);
        Assert.Equal(limit, failure.Parameter);
        Assert.Equal(4, failure.Actual);

        var adjusted = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(12), target)).State;
        Assert.Equal(limit, adjusted.Entities[target].Character!.Attack);
        Assert.Equal(play, Assert.Single(simulator.LegalActions(adjusted), action => action.Source == source));
        Assert.Equal(GameZone.Graveyard, simulator.Apply(adjusted, play).State.LocationOf(target).Zone);
    }

    [Fact]
    public void FreshScentAndItsCopyTargetDualTypeBeastsAndShareOneEnchantment()
    {
        var beast = new SimulationCard(new CardDefinition("beast", EntityKind.Minion, 0, 2, 3)
        {
            MinionTypes = MinionType.Beast | MinionType.Dragon
        });
        var dragon = new SimulationCard(new CardDefinition("dragon", EntityKind.Minion, 0, 2, 3)
        {
            MinionTypes = MinionType.Dragon
        });
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, beast, dragon]);
        var source = new EntityId(11);
        var target = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 10, ManaSpent = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "YOD_005", First), Card(12, "EX1_332", First),
            InPlay(21, "beast", Second, 3, 2, 3), InPlay(22, "dragon", Second, 4, 2, 3));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source, target);

        Assert.Equal(play, Assert.Single(simulator.LegalActions(root), action => action.Source == source));
        Assert.Equal(ErrorType.REQ_TARGET_WITH_RACE, simulator.CheckAction(root, play with { Target = new EntityId(22) })?.Type);
        var buffed = simulator.Apply(root, play).State;
        Assert.Equal("YOD_005e", Assert.Single(buffed.Entities[target].Enchantments).CardId);
        var copy = Assert.Single(buffed.Players[First].In(GameZone.Hand), id => buffed.Entities[id].CardId == "YOD_005ts");
        var replay = play with { Source = copy };
        Assert.Equal(replay, Assert.Single(simulator.LegalActions(buffed), action => action.Source == copy));
        var doubled = simulator.Apply(buffed, replay).State;
        Assert.Equal(6, doubled.Entities[target].Character!.Attack);
        Assert.Equal(7, doubled.Entities[target].Character!.MaximumHealth);
        Assert.All(doubled.Entities[target].Enchantments, enchantment => Assert.Equal("YOD_005e", enchantment.CardId));
        Assert.Equal(new EntityId(12), Assert.Single(doubled.Players[First].In(GameZone.Hand)));

        var silenced = simulator.Apply(doubled, play with { Source = new EntityId(12) }).State.Entities[target];
        Assert.Empty(silenced.Enchantments);
        Assert.Equal(2, silenced.Character!.Attack);
        Assert.Equal(3, silenced.Character!.MaximumHealth);
    }
}
