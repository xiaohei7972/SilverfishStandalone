using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void AnAttackSettingEnchantmentUsesBaseAndEarlierBuffsBeforeAuras()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var minion = new EntityId(11);
        var champion = new EntityId(12);
        var kings = new EntityId(13);
        var humility = new EntityId(14);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14])) },
            Player(Second, SecondHero),
            InPlay(11, "CS2_231", First, 3, 1, 1), InPlay(12, "CS2_222", First, 4, 7, 7),
            Card(13, "CS2_092", First), Card(14, "EX1_360", First));

        var buffed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, kings, minion)).State;
        var reset = simulator.Apply(buffed, new(SimulationActionKind.PlayCard, First, humility, minion)).State;
        var rebuild = simulator.CreateGame(reset.Players.Values, reset.Entities.Values, reset.ActivePlayer, reset.Turn);

        Assert.Equal(6, buffed.Entities[minion].Character!.Attack);
        Assert.Equal(2, reset.Entities[minion].Character!.Attack);
        Assert.Equal(6, reset.Entities[minion].Character!.MaximumHealth);
        Assert.Equal(1, reset.Entities[minion].Character!.BaseAttack);
        Assert.Equal(new[] { kings, humility }, reset.Entities[minion].Enchantments.Select(layer => layer.Source));
        Assert.Equal(champion, Assert.Single(reset.Entities[minion].AuraGrants).Source);
        Assert.Equal(reset.Entities[minion].Character, rebuild.Entities[minion].Character);
        Assert.Empty(root.Entities[minion].Enchantments);
    }

    [Fact]
    public void BloodlustExpiresAfterCombatAndLeavesTheEntryAuraInPlay()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var minion = new EntityId(11);
        var leader = new EntityId(12);
        var bloodlust = new EntityId(13);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 8, ManaCrystals = 8,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13])) },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21]))
            },
            InPlay(11, "CS2_231", First, 3, 1, 1), Card(12, "CS2_122", First), Card(13, "CS2_046", First),
            Card(21, "CS2_231", Second));

        var withLeader = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, leader, Position: 1)).State;
        var withBloodlust = simulator.Apply(withLeader, new(SimulationActionKind.PlayCard, First, bloodlust)).State;
        var attacked = simulator.Apply(withBloodlust, new(SimulationActionKind.Attack, First, minion, SecondHero)).State;
        var ended = simulator.Apply(attacked, new(SimulationActionKind.EndTurn, First));

        Assert.Equal(2, withLeader.Entities[minion].Character!.Attack);
        Assert.Equal(5, withBloodlust.Entities[minion].Character!.Attack);
        Assert.Equal(25, ended.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(2, ended.State.Entities[minion].Character!.Attack);
        Assert.Equal(leader, Assert.Single(ended.State.Entities[minion].AuraGrants).Source);
        Assert.Empty(ended.State.Entities[minion].Enchantments);
        Assert.Equal(new[] { SimulationEventKind.TurnEnded, SimulationEventKind.EnchantmentExpired,
            SimulationEventKind.EnchantmentExpired, SimulationEventKind.TurnStarted, SimulationEventKind.CardDrawn },
            ended.Events.Select(e => e.Kind));
    }

    [Fact]
    public void SilencingADamagedRecipientKeepsTheExternalAuraAndClampsCurrentHealth()
    {
        var ping = Spell("ping") with
        {
            Target = TargetRequirement.Minion, OnPlay = [new DamageEffect(DamageTarget.Selected, 4)]
        };
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero).Add(ping));
        var minion = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 4, ManaCrystals = 4,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15])) },
            Player(Second, SecondHero),
            InPlay(11, "CS2_172", First, 3, 3, 2) with { Character = new(3, 2, Damage: 1) },
            InPlay(12, "CS2_222", First, 4, 7, 7), Card(13, "CS2_092", First), Card(14, "ping", First),
            Card(15, "EX1_332", First));

        var buffed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(13), minion)).State;
        var damaged = simulator.Apply(buffed, new(SimulationActionKind.PlayCard, First, new EntityId(14), minion)).State;
        var silenced = simulator.Apply(damaged, new(SimulationActionKind.PlayCard, First, new EntityId(15), minion));
        var target = silenced.State.Entities[minion];

        Assert.True(target.Silenced);
        Assert.Equal(4, target.Character!.Attack);
        Assert.Equal(2, target.Character.Health);
        Assert.Equal(3, target.Character.MaximumHealth);
        Assert.Empty(target.Enchantments);
        Assert.Single(target.AuraGrants);
        Assert.DoesNotContain(silenced.Events, e => e.Kind is SimulationEventKind.Damage or SimulationEventKind.Died);
    }

    [Fact]
    public void ADirectBattlecryBuffPersistsAfterItsSourceLeavesPlay()
    {
        var remove = Spell("remove") with
        {
            Target = TargetRequirement.Minion, OnPlay = [new DamageEffect(DamageTarget.Selected, 5)]
        };
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero).Add(remove));
        var recipient = new EntityId(11);
        var cleric = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 3, ManaCrystals = 3,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13])) },
            Player(Second, SecondHero),
            InPlay(11, "CS2_231", First, 3, 1, 1), Card(12, "EX1_019", First), Card(13, "remove", First));

        var buffed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, cleric, recipient, Position: 0)).State;
        var removed = simulator.Apply(buffed, new(SimulationActionKind.PlayCard, First, new EntityId(13), cleric)).State;

        Assert.Equal(2, removed.Entities[recipient].Character!.Attack);
        Assert.Equal(2, removed.Entities[recipient].Character!.Health);
        Assert.Equal(cleric, Assert.Single(removed.Entities[recipient].Enchantments).Source);
        Assert.Equal(GameZone.Graveyard, removed.LocationOf(cleric).Zone);
    }

    [Fact]
    public void AuraTargetsFollowControlAtThePhaseBoundary()
    {
        var steal = Spell("steal") with { Target = TargetRequirement.EnemyMinion, OnPlay = [new TakeControlEffect()] };
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero).Add(steal));
        var friendly = new EntityId(11);
        var enemy = new EntityId(21);
        var champion = new EntityId(22);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            InPlay(11, "CS2_231", First, 3, 1, 1), Card(12, "steal", First),
            InPlay(21, "CS2_231", Second, 4, 1, 1), InPlay(22, "CS2_222", Second, 5, 7, 7));

        var changed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(12), champion)).State;

        Assert.Equal(champion, Assert.Single(changed.Entities[friendly].AuraGrants).Source);
        Assert.Equal(2, changed.Entities[friendly].Character!.Health);
        Assert.Empty(changed.Entities[enemy].AuraGrants);
        Assert.Equal(1, changed.Entities[enemy].Character!.Health);
        Assert.Equal(2, root.Entities[enemy].Character!.Health);
        Assert.Equal(Second, changed.Entities[champion].Owner);
    }

    [Fact]
    public void AdjacencyMovesAfterDeathsAndEndsWhenTheSourceIsSilenced()
    {
        var remove = Spell("remove") with
        {
            Target = TargetRequirement.Minion, OnPlay = [new DamageEffect(DamageTarget.Selected, 5)]
        };
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero).Add(remove));
        var left = new EntityId(11);
        var wolf = new EntityId(12);
        var center = new EntityId(13);
        var right = new EntityId(14);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14]), (GameZone.Hand, [15, 16])) },
            Player(Second, SecondHero),
            InPlay(11, "CS2_231", First, 3, 1, 1), InPlay(12, "EX1_162", First, 4, 2, 2),
            InPlay(13, "CS2_231", First, 5, 1, 1), InPlay(14, "CS2_231", First, 6, 1, 1),
            Card(15, "remove", First), Card(16, "EX1_332", First));

        var moved = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(15), center)).State;
        var silenced = simulator.Apply(moved, new(SimulationActionKind.PlayCard, First, new EntityId(16), wolf)).State;

        Assert.Equal(1, root.Entities[right].Character!.Attack);
        Assert.Equal(2, moved.Entities[right].Character!.Attack);
        Assert.Equal(2, moved.Entities[left].Character!.Attack);
        Assert.Equal(1, silenced.Entities[right].Character!.Attack);
        Assert.Equal(1, silenced.Entities[left].Character!.Attack);
    }

    [Fact]
    public void SilenceRemovesChargeAndEarthShockKillsAfterRemovingTheShieldAndDeathrattle()
    {
        var swift = Minion("swift", 1, 1) with
        {
            Keywords = CardKeywords.Charge | CardKeywords.DivineShield,
            Deathrattle = [new DamageEffect(DamageTarget.EnemyHero, 5)]
        };
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero).Add(swift));
        var minion = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13])) },
            Player(Second, SecondHero),
            Card(11, "swift", First), Card(12, "EX1_332", First), Card(13, "EX1_245", First));

        var placed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, minion, Position: 0)).State;
        var silenced = simulator.Apply(placed, new(SimulationActionKind.PlayCard, First, new EntityId(12), minion)).State;
        var killed = simulator.Apply(placed, new(SimulationActionKind.PlayCard, First, new EntityId(13), minion));

        Assert.Contains(simulator.LegalActions(placed), action => action.Kind == SimulationActionKind.Attack && action.Source == minion);
        Assert.DoesNotContain(simulator.LegalActions(silenced), action => action.Kind == SimulationActionKind.Attack && action.Source == minion);
        Assert.Equal(GameZone.Graveyard, killed.State.LocationOf(minion).Zone);
        Assert.DoesNotContain(killed.Events, e => e.Kind is SimulationEventKind.ShieldBroken or SimulationEventKind.Deathrattle);
        Assert.Equal(30, killed.State.Entities[SecondHero].Character!.Health);
    }
}
