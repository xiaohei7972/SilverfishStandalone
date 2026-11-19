using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

/// <summary>Runs the rules supported by the registered card definitions over immutable match states.</summary>
public sealed partial class GameSimulator
{
    private const int BoardLimit = 7;
    private const int HandLimit = 10;
    private const int SecretLimit = 5;
    private readonly ImmutableDictionary<string, SimulationCard> _cards;
    private readonly int _resolutionLimit;

    public GameSimulator(IEnumerable<SimulationCard> cards, int resolutionLimit = 4096)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentOutOfRangeException.ThrowIfLessThan(resolutionLimit, 1);
        _resolutionLimit = resolutionLimit;
        var catalog = ImmutableDictionary.CreateBuilder<string, SimulationCard>(StringComparer.Ordinal);
        foreach (var card in cards)
        {
            if (card?.Definition is not { } definition || string.IsNullOrWhiteSpace(definition.CardId))
                throw new ArgumentException("A simulation card needs a definition and card ID.", nameof(cards));
            catalog.Add(definition.CardId, card);
        }
        _cards = catalog.ToImmutable();
        foreach (var card in _cards.Values)
        {
            var definition = card.Definition;
            if (definition.Kind is not (EntityKind.Hero or EntityKind.Minion or EntityKind.Spell or EntityKind.Weapon or EntityKind.HeroPower) ||
                definition.PrintedCost < 0 || definition.PrintedAttack < 0 ||
                (IsCharacter(definition.Kind) && definition.PrintedHealth < 1) ||
                (definition.Kind == EntityKind.Weapon && definition.PrintedDurability <= 0) ||
                !Enum.IsDefined(card.Target) ||
                (card.Requirements & ~(PlayRequirements.BoardSpace | PlayRequirements.FriendlyMinionDeath)) != 0 ||
                card.MinimumEnemyMinions is < 0 or > BoardLimit || card.MinimumTotalMinions is < 0 or > BoardLimit * 2 ||
                (card.Keywords & ~(CardKeywords.Taunt | CardKeywords.Charge | CardKeywords.DivineShield |
                    CardKeywords.Rush | CardKeywords.Windfury)) != 0 ||
                card.Overload < 0 || (card.Overload > 0 && definition.Kind is EntityKind.Hero or EntityKind.HeroPower) ||
                card.Triggers.IsDefault || card.Auras.IsDefault ||
                (definition.Kind is not (EntityKind.Minion or EntityKind.Weapon) && !card.Deathrattle.IsEmpty) ||
                (definition.Kind != EntityKind.Minion && !card.Auras.IsEmpty) ||
                (!IsCharacter(definition.Kind) && definition.Kind != EntityKind.Weapon && !card.IsSecret && !card.Triggers.IsEmpty) ||
                (card.IsSecret && (definition.Kind != EntityKind.Spell || card.Target != TargetRequirement.None ||
                    !card.OnPlay.IsDefaultOrEmpty || card.Triggers.Length != 1)))
                throw new ArgumentException($"Invalid simulation card '{definition.CardId}'.", nameof(cards));

            ValidateEffects(card, card.OnPlay, maySelect: true);
            ValidateEffects(card, card.Deathrattle, maySelect: false);
            ValidateEffects(card, card.WhenDrawn, maySelect: false);
            foreach (var aura in card.Auras)
            {
                if (aura is null || !Enum.IsDefined(aura.Recipients) ||
                    aura.Modifier?.Mode != StatModifierMode.Add || !IsValidModifier(aura.Modifier))
                    throw new ArgumentException($"Invalid stat aura on '{definition.CardId}'.", nameof(cards));
            }
            foreach (var trigger in card.Triggers)
            {
                if (trigger is null || !Enum.IsDefined(trigger.Timing) || !Enum.IsDefined(trigger.Relation) ||
                    !Enum.IsDefined(trigger.Focus) || !Enum.IsDefined(trigger.EntityMatch) ||
                    (trigger.EntityKind is EntityKind kind && (kind == EntityKind.Unknown || !Enum.IsDefined(kind))) ||
                    (trigger.Focus == TriggerFocus.DamageSource && trigger.Timing != TriggerTiming.Damage) ||
                    (trigger.Focus == TriggerFocus.AttackDefender && trigger.Timing != TriggerTiming.AttackProposed) ||
                    (trigger.Timing is TriggerTiming.SpellCounterWindow or TriggerTiming.AttackProposed && !card.IsSecret) ||
                    (trigger.Timing is TriggerTiming.TurnStarted or TriggerTiming.TurnEnded &&
                        (trigger.EntityMatch != TriggerEntityMatch.Any || trigger.EntityKind is not null || trigger.RequireLivingEntity)) ||
                    trigger.Effects.IsDefaultOrEmpty)
                    throw new ArgumentException($"Invalid trigger on '{definition.CardId}'.", nameof(cards));
                ValidateEffects(card, trigger.Effects, maySelect: false, timing: trigger.Timing,
                    hasInPlaySource: IsCharacter(definition.Kind));
            }
        }
    }

    public GameState CreateGame(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities,
        PlayerId activePlayer, int turn = 1, IEnumerable<MinionDeath>? minionDeaths = null, ulong randomState = 0)
    {
        var state = GameState.Create(players, entities, activePlayer, turn, minionDeaths, randomState);
        foreach (var death in state.MinionDeaths)
        {
            if (GetCard(death.CardId).Definition.Kind != EntityKind.Minion ||
                Card(state.Entities[death.Entity]).Definition.Kind != EntityKind.Minion)
                throw new ArgumentException("Death history must reference minions.", nameof(minionDeaths));
        }
        foreach (var player in state.Players.Values)
        {
            if (player.In(GameZone.Hand).Length > HandLimit || MinionCount(state, player.Id) > BoardLimit)
                throw new ArgumentException("An initial zone exceeds its capacity.");
            var secrets = player.In(GameZone.Secret);
            if (secrets.Length > SecretLimit || secrets.Select(id => state.Entities[id].CardId).Distinct(StringComparer.Ordinal).Count() != secrets.Length)
                throw new ArgumentException("An initial secret zone exceeds its capacity or contains a duplicate.");
        }

        foreach (var entity in state.Entities.Values)
        {
            var kind = Card(entity).Definition.Kind;
            var location = state.LocationOf(entity.Id);
            var player = state.Players[entity.Controller];
            var isHeroSlot = player.Hero == entity.Id;
            var isEquippedSlot = player.EquippedWeapon == entity.Id;
            if (isHeroSlot != (kind == EntityKind.Hero) ||
                (location.Zone == GameZone.Play && (kind == EntityKind.Weapon ? !isEquippedSlot :
                    !IsCharacter(kind) || entity.Character is not { Health: > 0 })) ||
                ((location.Zone == GameZone.HeroPower) != (kind == EntityKind.HeroPower)) ||
                (location.Zone == GameZone.Secret && !Card(entity).IsSecret) ||
                (entity.Character is not null && !IsCharacter(kind)) ||
                (entity.Weapon is not null && (kind != EntityKind.Weapon || location.Zone is not (GameZone.Play or GameZone.Graveyard))) ||
                (location.Zone != GameZone.Hand && !entity.CostModifiers.IsEmpty) ||
                ((location.Zone != GameZone.Play || entity.Character is null) &&
                    (!entity.Enchantments.IsEmpty || !entity.AuraGrants.IsEmpty)))
                throw new ArgumentException($"Entity {entity.Id} is incompatible with its card definition or zone.");
            if (entity.CostModifiers.Any(cost => cost is null || !state.Entities.ContainsKey(cost.Source) || cost.Delta == 0))
                throw new ArgumentException($"Entity {entity.Id} has an invalid mana cost modifier.");
            ValidateStatLayers(state, entity);
        }
        return RefreshAuras(state);
    }

    public ImmutableArray<SimulationAction> LegalActions(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (state.Outcome != MatchOutcome.InProgress)
            return [];

        var actions = ImmutableArray.CreateBuilder<SimulationAction>();
        var actor = state.ActivePlayer;
        var player = state.Players[actor];
        var enemy = state.OpponentOf(actor);
        var enemyBoard = state.Players[enemy].In(GameZone.Play).Select(id => state.Entities[id])
            .Where(entity => entity.Character is { Health: > 0 }).ToArray();
        var taunts = enemyBoard.Where(IsTaunt).ToArray();

        foreach (var id in player.In(GameZone.Play))
        {
            var attacker = state.Entities[id];
            if (attacker.Character is not { Frozen: false } || EffectiveAttack(state, attacker) <= 0)
                continue;
            var reach = GetAttackReach(state, attacker);
            if (reach == AttackReach.None)
                continue;
            foreach (var defender in taunts.Length == 0 ? enemyBoard : taunts)
                if (reach == AttackReach.AnyCharacter || defender.Id != state.Players[enemy].Hero)
                    actions.Add(new(SimulationActionKind.Attack, actor, id, defender.Id));
        }

        var minions = MinionCount(state, actor);
        foreach (var id in player.In(GameZone.Hand))
        {
            var entity = state.Entities[id];
            var card = Card(entity);
            var createsMinion = card.Definition.Kind == EntityKind.Minion;
            if (EffectiveCost(entity) > player.Mana ||
                (minions >= BoardLimit && (createsMinion || card.OnPlay.Any(effect => effect is TakeControlEffect))) ||
                (card.IsSecret && !CanArmSecret(state, player, entity.CardId)) ||
                !MeetsPlayRequirements(state, card, minions))
                continue;

            var targets = PlayTargets(state, card.Target).ToArray();
            if (card.Definition.Kind is EntityKind.Minion or EntityKind.Weapon && targets.Length == 0)
                targets = [null];

            foreach (var target in targets)
            {
                if (createsMinion)
                {
                    for (var position = 0; position <= minions; position++)
                        actions.Add(new(SimulationActionKind.PlayCard, actor, id, target, position));
                }
                else
                {
                    actions.Add(new(SimulationActionKind.PlayCard, actor, id, target));
                }
            }
        }

        if (player.HeroPowerUsesThisTurn == 0 && player.In(GameZone.HeroPower) is [var powerId])
        {
            var power = Card(state.Entities[powerId]);
            if (power.Definition.PrintedCost <= player.Mana && MeetsPlayRequirements(state, power, minions))
                foreach (var target in PlayTargets(state, power.Target))
                    actions.Add(new(SimulationActionKind.UseHeroPower, actor, powerId, target));
        }

        actions.Add(new(SimulationActionKind.EndTurn, actor));
        return actions.ToImmutable();
    }

    public SimulationResult Apply(GameState state, SimulationAction action)
    {
        EnsureLegal(state, action);
        return new Resolution(this, state).Run(action);
    }

    private void EnsureLegal(GameState state, SimulationAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (!LegalActions(state).Contains(action))
            throw new InvalidOperationException($"Illegal action {action.Kind} for player {action.Actor.Value}.");
    }

    private IEnumerable<EntityId?> PlayTargets(GameState state, TargetRequirement requirement)
    {
        if (requirement == TargetRequirement.None)
        {
            yield return null;
            yield break;
        }

        foreach (var player in state.Players.Values.OrderBy(player => player.Id.Value))
        {
            if (requirement == TargetRequirement.FriendlyMinion && player.Id != state.ActivePlayer)
                continue;
            if ((requirement is TargetRequirement.EnemyCharacter or TargetRequirement.EnemyMinion) &&
                player.Id == state.ActivePlayer)
                continue;
            foreach (var id in player.In(GameZone.Play))
                if (state.Entities[id].Character is { Health: > 0 } &&
                    (!TargetsOnlyMinions(requirement) || Card(state.Entities[id]).Definition.Kind == EntityKind.Minion))
                    yield return id;
        }
    }

    private bool IsTaunt(GameEntity entity) =>
        Card(entity).Definition.Kind == EntityKind.Minion && HasKeyword(entity, CardKeywords.Taunt);

    private bool HasKeyword(GameEntity entity, CardKeywords keyword) =>
        !entity.Silenced && Card(entity).Keywords.HasFlag(keyword);

    private static bool TargetsOnlyMinions(TargetRequirement requirement) =>
        requirement is TargetRequirement.Minion or TargetRequirement.FriendlyMinion or TargetRequirement.EnemyMinion;

    private static bool IsCharacter(EntityKind kind) => kind is EntityKind.Hero or EntityKind.Minion;

    private static bool CanArmSecret(GameState state, PlayerState player, string cardId) =>
        player.In(GameZone.Secret).Length < SecretLimit && !player.In(GameZone.Secret).Any(id => state.Entities[id].CardId == cardId);

    private bool MeetsPlayRequirements(GameState state, SimulationCard card, int minions) =>
        (!card.Requirements.HasFlag(PlayRequirements.BoardSpace) || minions < BoardLimit) &&
        (!card.Requirements.HasFlag(PlayRequirements.FriendlyMinionDeath) ||
            state.MinionDeaths.Any(death => death.Controller == state.ActivePlayer)) &&
        ((card.MinimumEnemyMinions == 0 && card.MinimumTotalMinions == 0) ||
            HasRequiredMinions(state, card, minions));

    private bool HasRequiredMinions(GameState state, SimulationCard card, int friendlyMinions)
    {
        var enemies = MinionCount(state, state.OpponentOf(state.ActivePlayer));
        return enemies >= card.MinimumEnemyMinions && friendlyMinions + enemies >= card.MinimumTotalMinions;
    }

    private int EffectiveCost(GameEntity entity)
    {
        long cost = Card(entity).Definition.PrintedCost;
        foreach (var modifier in entity.CostModifiers)
            cost += modifier.Delta;
        return (int)Math.Clamp(cost, 0, int.MaxValue);
    }

    private SimulationCard Card(GameEntity entity) => GetCard(entity.CardId);
    private SimulationCard GetCard(string cardId) => _cards.TryGetValue(cardId, out var card)
        ? card
        : throw new ArgumentException($"Card '{cardId}' has no implementation in this rules catalog.");

    private int MinionCount(GameState state, PlayerId player) => BoardMinions(state, player).Count();

    private IEnumerable<EntityId> BoardMinions(GameState state, PlayerId player) =>
        state.Players[player].In(GameZone.Play)
            .Where(id => Card(state.Entities[id]).Definition.Kind == EntityKind.Minion);

    private int BoardIndex(GameState state, PlayerId player, int position)
    {
        var zone = state.Players[player].In(GameZone.Play);
        foreach (var minion in BoardMinions(state, player))
        {
            if (position-- == 0)
                return zone.IndexOf(minion);
        }
        return zone.Length;
    }

    private void ValidateEffects(SimulationCard card, ImmutableArray<CardEffect> effects, bool maySelect,
        TriggerTiming? timing = null, bool hasInPlaySource = false)
    {
        if (effects.IsDefault)
            throw new ArgumentException("Effect arrays must be initialized.");
        var hasCharacterSubject = timing is TriggerTiming.WhenMinionSummoned or TriggerTiming.AfterMinionSummoned or
            TriggerTiming.AfterMinionPlayed or TriggerTiming.Damage or TriggerTiming.HeroAttacking or TriggerTiming.AttackProposed;
        foreach (var effect in effects)
        {
            var valid = effect switch
            {
                DamageEffect damage => damage.Amount > 0 && (damage.Target switch
                {
                    DamageTarget.Selected => maySelect && card.Target != TargetRequirement.None,
                    DamageTarget.Source => hasInPlaySource || (maySelect && IsCharacter(card.Definition.Kind)),
                    DamageTarget.TriggerSubject => hasCharacterSubject,
                    DamageTarget.EnemyHero or DamageTarget.FriendlyHero or DamageTarget.AllMinions or
                        DamageTarget.EnemyMinions or DamageTarget.AllEnemies => true,
                    _ => false
                }),
                DrawEffect draw => draw.Count > 0,
                GainArmorEffect armor => armor.Amount > 0,
                HealFriendlyHeroEffect heal => heal.Amount > 0,
                EquipWeaponEffect equip => GetCard(equip.CardId).Definition.Kind == EntityKind.Weapon,
                DestroyEnemyWeaponEffect => true,
                EnchantStatsEffect enchant => Enum.IsDefined(enchant.Target) && Enum.IsDefined(enchant.Duration) &&
                    IsValidModifier(enchant.Modifier) &&
                    (enchant.Target != StatTarget.Selected || (maySelect && card.Target != TargetRequirement.None)) &&
                    (enchant.Target != StatTarget.Source || hasInPlaySource || (maySelect && IsCharacter(card.Definition.Kind))),
                SilenceEffect or DestroyMinionEffect => maySelect && TargetsOnlyMinions(card.Target),
                ReturnMinionEffect returned => returned.Target switch
                {
                    EffectTarget.Selected => maySelect && TargetsOnlyMinions(card.Target),
                    EffectTarget.TriggerSubject => hasCharacterSubject,
                    _ => false
                },
                TransformMinionEffect transform => maySelect && TargetsOnlyMinions(card.Target) &&
                    GetCard(transform.CardId).Definition.Kind == EntityKind.Minion,
                ResurrectFriendlyMinionEffect => true,
                FreezeEffect freeze => freeze.Target switch
                {
                    FreezeTarget.Selected => maySelect && card.Target != TargetRequirement.None,
                    FreezeTarget.TriggerSubject => hasCharacterSubject,
                    FreezeTarget.EnemyMinions => true,
                    _ => false
                },
                UnlockOverloadEffect => true,
                CounterSpellEffect => card.IsSecret && timing == TriggerTiming.SpellCounterWindow,
                SummonAttackDefenderEffect redirect => card.IsSecret && timing == TriggerTiming.AttackProposed &&
                    GetCard(redirect.CardId).Definition.Kind == EntityKind.Minion,
                RandomDamageEffect random => random.Amount > 0 && random.Count > 0 && Enum.IsDefined(random.Pool) && Enum.IsDefined(random.Mode),
                DestroyRandomEnemyMinionEffect or DestroyAllButOneMinionEffect => true,
                SummonRandomMinionEffect summon => !summon.CardIds.IsDefaultOrEmpty &&
                    summon.CardIds.All(id => GetCard(id).Definition.Kind == EntityKind.Minion),
                SummonEffect summon => Enum.IsDefined(summon.Placement) && GetCard(summon.CardId).Definition.Kind == EntityKind.Minion,
                TakeControlEffect => maySelect && card.Definition.Kind == EntityKind.Spell &&
                    card.Target == TargetRequirement.EnemyMinion,
                _ => false
            };
            if (!valid)
                throw new ArgumentException($"Invalid effect on '{card.Definition.CardId}'.");
        }
    }
}
