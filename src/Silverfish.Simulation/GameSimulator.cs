using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

/// <summary>在不可变的对局状态上执行已注册的卡牌规则。</summary>
public sealed partial class GameSimulator
{
    private const int BoardLimit = 7;
    private const int HandLimit = 10;
    private const int SecretLimit = 5;
    private const PlayRequirements SupportedPlayRequirements = PlayRequirements.BoardSpace | PlayRequirements.FriendlyMinionDeath;
    private readonly ImmutableDictionary<string, SimulationCard> _cards;
    private readonly Dictionary<(string CardId, EnchantmentRemovalReason Reason), ImmutableArray<CardEffect>> _enchantmentRemovalEffects = new();
    private readonly int _resolutionLimit;
    private readonly bool _hasCostAuras;

    public GameSimulator(IEnumerable<SimulationCard> cards, int resolutionLimit = 4096)
    {
        if (cards is null)
            throw new ArgumentNullException(nameof(cards), "必须提供模拟卡牌目录。");
        if (resolutionLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(resolutionLimit), "结算预算必须大于零。");
        _resolutionLimit = resolutionLimit;
        var catalog = ImmutableDictionary.CreateBuilder<string, SimulationCard>(StringComparer.Ordinal);
        foreach (var card in cards)
        {
            if (card?.Definition is not { } definition || string.IsNullOrWhiteSpace(definition.CardId))
                throw new ArgumentException("模拟卡牌必须提供定义和卡牌 ID。", nameof(cards));
            if (!catalog.TryAdd(definition.CardId, card))
                throw new ArgumentException($"模拟卡牌 ID「{definition.CardId}」重复。", nameof(cards));
        }
        _cards = catalog.ToImmutable();
        foreach (var card in _cards.Values)
        {
            var definition = card.Definition;
            if (definition.Kind is not (EntityKind.Hero or EntityKind.Minion or EntityKind.Spell or EntityKind.Weapon or EntityKind.HeroPower or EntityKind.Location) ||
                definition.PrintedCost < 0 || definition.PrintedAttack < 0 || !HasValidCardTraits(definition) ||
                (IsCharacter(definition.Kind) && definition.PrintedHealth < 1) ||
                (definition.Kind is EntityKind.Weapon or EntityKind.Location && definition.PrintedDurability <= 0) ||
                (definition.Kind == EntityKind.Location) != (card.Activation is not null) ||
                !IsValidTarget(card.Target, card.TargetFilter) ||
                (card.Requirements & ~SupportedPlayRequirements) != 0 ||
                card.MinimumEnemyMinions is < 0 or > BoardLimit || card.MinimumTotalMinions is < 0 or > BoardLimit * 2 ||
                (card.Keywords & ~(CardKeywords.Taunt | CardKeywords.Charge | CardKeywords.DivineShield |
                    CardKeywords.Rush | CardKeywords.Windfury | CardKeywords.CastsWhenDrawn)) != 0 ||
                (card.Keywords.HasFlag(CardKeywords.CastsWhenDrawn) && definition.Kind != EntityKind.Spell) ||
                card.Overload < 0 || (card.Overload > 0 && definition.Kind is EntityKind.Hero or EntityKind.HeroPower) ||
                card.Triggers.IsDefault || card.Auras.IsDefault || card.CostAuras.IsDefault || card.Choices.IsDefault ||
                (definition.Kind is not (EntityKind.Minion or EntityKind.Weapon) && !card.Deathrattle.IsEmpty) ||
                (definition.Kind != EntityKind.Minion && (!card.Auras.IsEmpty || card.LeavesNoCorpse)) ||
                (!IsCharacter(definition.Kind) && definition.Kind != EntityKind.Weapon && !card.CostAuras.IsEmpty) ||
                (!IsCharacter(definition.Kind) && definition.Kind != EntityKind.Weapon && !card.IsSecret && !card.Triggers.IsEmpty) ||
                (!card.Choices.IsEmpty && (card.Choices.Length < 2 || card.IsSecret || card.Target != TargetRequirement.None ||
                    !card.OnPlay.IsDefaultOrEmpty || definition.Kind is not (EntityKind.Minion or EntityKind.Spell))) ||
                (card.IsSecret && (definition.Kind != EntityKind.Spell || card.Target != TargetRequirement.None ||
                    !card.OnPlay.IsDefaultOrEmpty || card.Triggers.Length != 1)))
                throw new ArgumentException($"模拟卡牌「{definition.CardId}」的定义无效。", nameof(cards));

            ValidateEffects(card, card.OnPlay, card.Target, hasInPlaySource: IsCharacter(definition.Kind));
            ValidateEffects(card, card.Deathrattle);
            ValidateEffects(card, card.WhenDrawn, isDrawnCard: true);
            if (card.WhenDiscarded is { } whenDiscarded)
                ValidateEffects(card, whenDiscarded, isDiscardedCard: true);
            if (card.Activation is { } activation)
            {
                if (!IsValidTarget(activation.Target, activation.TargetFilter) || activation.Effects.IsDefaultOrEmpty)
                    throw new ArgumentException($"卡牌「{definition.CardId}」的地标激活规则无效。", nameof(cards));
                ValidateEffects(card, activation.Effects, activation.Target);
            }
            var choiceIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var choice in card.Choices)
            {
                if (choice is null || string.IsNullOrWhiteSpace(choice.Id) || !choiceIds.Add(choice.Id) ||
                    !IsValidTarget(choice.Target, choice.TargetFilter) || (choice.Requirements & ~SupportedPlayRequirements) != 0 ||
                    choice.Effects.IsDefaultOrEmpty)
                    throw new ArgumentException($"卡牌「{definition.CardId}」的抉择选项无效。", nameof(cards));
                ValidateEffects(card, choice.Effects, choice.Target, hasInPlaySource: definition.Kind == EntityKind.Minion);
            }
            foreach (var aura in card.Auras)
            {
                if (aura is null || !Enum.IsDefined(aura.Recipients) ||
                    aura.Modifier?.Mode != StatModifierMode.Add || !IsValidModifier(aura.Modifier) ||
                    !IsOptionalCardIdValid(aura.EnchantmentCardId) || !IsValidCardFilter(aura.CardFilter))
                    throw new ArgumentException($"卡牌「{definition.CardId}」的属性光环无效。", nameof(cards));
            }
            foreach (var aura in card.CostAuras)
                if (aura is null || !Enum.IsDefined(aura.Recipient) || !Enum.IsDefined(aura.Target) || aura.Delta == 0 ||
                    !IsValidCardFilter(aura.CardFilter))
                    throw new ArgumentException($"卡牌「{definition.CardId}」的费用光环无效。", nameof(cards));

            foreach (var trigger in card.Triggers)
            {
                if (trigger is null || !Enum.IsDefined(trigger.Timing) || !Enum.IsDefined(trigger.Relation) ||
                    !Enum.IsDefined(trigger.Focus) || !Enum.IsDefined(trigger.EntityMatch) ||
                    !IsValidCardFilter(trigger.CardFilter) ||
                    (trigger.EntityKind is EntityKind kind && (kind == EntityKind.Unknown || !Enum.IsDefined(kind))) ||
                    (trigger.Focus == TriggerFocus.DamageSource && trigger.Timing != TriggerTiming.Damage) ||
                    (trigger.Focus == TriggerFocus.AttackDefender && trigger.Timing != TriggerTiming.AttackProposed) ||
                    (trigger.Timing is TriggerTiming.SpellCounterWindow or TriggerTiming.AttackProposed && !card.IsSecret) ||
                    (trigger.Timing is TriggerTiming.TurnStarted or TriggerTiming.TurnEnded &&
                        (trigger.EntityMatch != TriggerEntityMatch.Any || trigger.EntityKind is not null ||
                            trigger.CardFilter is not null || trigger.RequireLivingEntity)) ||
                    (trigger.Condition is not null && !IsValidCondition(trigger.Condition, TargetRequirement.None,
                        HasCharacterSubject(trigger.Timing), IsCharacter(definition.Kind))) ||
                    trigger.Effects.IsDefaultOrEmpty)
                    throw new ArgumentException($"卡牌「{definition.CardId}」的触发规则无效。", nameof(cards));
                ValidateEffects(card, trigger.Effects, timing: trigger.Timing,
                    hasInPlaySource: IsCharacter(definition.Kind));
            }
        }
        _hasCostAuras = _cards.Values.Any(card => !card.CostAuras.IsEmpty);
    }

    public GameState CreateGame(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities,
        PlayerId activePlayer, int turn = 1, IEnumerable<MinionDeath>? minionDeaths = null, ulong randomState = 0,
        PendingEntityChoice? pendingChoice = null) =>
        ValidateState(GameState.Create(players, entities, activePlayer, turn, minionDeaths, randomState,
            pendingChoice: pendingChoice));

    public IReadOnlyDictionary<string, SimulationCard> Cards => _cards;

    public GameState RestoreGame(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities, PlayerId activePlayer,
        int turn, IEnumerable<MinionDeath> minionDeaths, ulong randomState, GameContinuation continuation,
        PendingEntityChoice? pendingChoice = null)
    {
        if (continuation is null)
            throw new ArgumentNullException(nameof(continuation), "必须提供存档的结算计数和对局结果。");
        var saved = GameState.Create(players, entities, activePlayer, turn, minionDeaths, randomState, continuation, pendingChoice);
        var validated = ValidateState(saved);
        // 存档保存的是结算后的状态，载入时核对属性与光环是否一致。
        if (validated.Entities != saved.Entities)
            throw new ArgumentException("存档中的场上属性或光环与卡牌规则不一致。", nameof(entities));
        return validated;
    }

    private GameState ValidateState(GameState state)
    {
        var corpsesFromHistory = new Dictionary<PlayerId, int>();
        foreach (var death in state.MinionDeaths)
        {
            var card = GetCard(death.CardId);
            if (card.Definition.Kind != EntityKind.Minion ||
                Card(state.Entities[death.Entity]).Definition.Kind != EntityKind.Minion)
                throw new ArgumentException("死亡记录必须引用随从。");
            // 旧存档只有死亡记录，按记录恢复尚未写入存档的尸体累计值。
            if (!card.LeavesNoCorpse && state.Players[death.Controller].Corpses is null)
                corpsesFromHistory[death.Controller] = corpsesFromHistory.GetValueOrDefault(death.Controller) + 1;
        }
        foreach (var player in state.Players.Values)
        {
            if (corpsesFromHistory.TryGetValue(player.Id, out var corpses))
                state = state.UpdateResources(player with { Corpses = corpses });
            if (player.In(GameZone.Hand).Length > HandLimit || BoardSlotCount(state, player.Id) > BoardLimit)
                throw new ArgumentException($"玩家 {player.Id.Value} 的初始手牌或战场占位超过上限。");
            var secrets = player.In(GameZone.Secret);
            if (secrets.Length > SecretLimit || secrets.Select(id => state.Entities[id].CardId).Distinct(StringComparer.Ordinal).Count() != secrets.Length)
                throw new ArgumentException($"玩家 {player.Id.Value} 的初始奥秘数量超过上限，或存在重复的奥秘。");
        }

        foreach (var entity in state.Entities.Values)
        {
            var kind = Card(entity).Definition.Kind;
            var location = state.LocationOf(entity.Id);
            var player = state.Players[entity.Controller];
            var isHeroSlot = player.Hero == entity.Id;
            var isEquippedSlot = player.EquippedWeapon == entity.Id;
            if (isHeroSlot != (kind == EntityKind.Hero) ||
                (location.Zone == GameZone.Play && kind != EntityKind.Location && (kind == EntityKind.Weapon ? !isEquippedSlot :
                    !IsCharacter(kind) || entity.Character is not { Health: > 0 })) ||
                ((location.Zone == GameZone.HeroPower) != (kind == EntityKind.HeroPower)) ||
                (location.Zone == GameZone.Secret && !Card(entity).IsSecret) ||
                (entity.Character is not null && !IsCharacter(kind)) ||
                (entity.Weapon is not null && (kind != EntityKind.Weapon || location.Zone is not (GameZone.Play or GameZone.Graveyard))) ||
                !IsValidLocationState(kind, location.Zone, entity.Location) ||
                (location.Zone != GameZone.Hand && !entity.CostModifiers.IsEmpty) ||
                ((location.Zone != GameZone.Play || entity.Character is null) &&
                    (!entity.Enchantments.IsEmpty || !entity.AuraGrants.IsEmpty)))
                throw new ArgumentException($"实体 {entity.Id} 的状态与卡牌定义或所在区域不符。");
            if (entity.CostModifiers.Any(cost => cost is null || !state.Entities.ContainsKey(cost.Source) || cost.Delta == 0 ||
                !IsOptionalCardIdValid(cost.CardId) || cost.ExpiresAfterTurn < state.Turn))
                throw new ArgumentException($"实体 {entity.Id} 的费用修正无效。");
            if (entity.CostTypeModifiers is { } costTypes && (location.Zone != GameZone.Hand || costTypes.IsDefaultOrEmpty ||
                costTypes.Any(cost => cost is null || !state.Entities.ContainsKey(cost.Source) || !SupportsCostType(cost.Type) ||
                    !IsOptionalCardIdValid(cost.CardId) || cost.ExpiresAfterTurn < state.Turn)))
                throw new ArgumentException($"实体 {entity.Id} 的支付方式修正无效。");
            ValidateStatLayers(state, entity);
        }
        return RefreshAuras(state);
    }

    public ImmutableArray<SimulationAction> LegalActions(GameState state)
    {
        if (state is null)
            throw new ArgumentNullException(nameof(state), "必须提供对局状态。");
        if (state.Outcome != MatchOutcome.InProgress || state.PendingChoice is not null)
            return []; // Unknown card effects: no regular moves or fabricated search predictions during CHOICE.

        var actions = ImmutableArray.CreateBuilder<SimulationAction>();
        var actor = state.ActivePlayer;
        var player = state.Players[actor];
        var enemy = state.OpponentOf(actor);
        var enemyBoard = state.Players[enemy].In(GameZone.Play).Select(id => state.Entities[id])
            .Where(entity => entity.Character is { Health: > 0 }).ToArray();
        var taunts = enemyBoard.Where(IsTaunt).ToArray();

        var minions = 0;
        var occupiedSlots = 0;
        foreach (var id in player.In(GameZone.Play))
        {
            var attacker = state.Entities[id];
            if (attacker.Location is { } location)
            {
                occupiedSlots++;
                if (location.TurnsUntilReady == 0 && Card(attacker).Activation is { } ability)
                    foreach (var target in PlayTargets(state, actor, ability.Target, ability.TargetFilter))
                        actions.Add(new(SimulationActionKind.ActivateLocation, actor, id, target));
                continue;
            }
            if (attacker.Character is not { } character)
                continue;
            if (id != player.Hero)
            {
                minions++;
                occupiedSlots++;
            }
            if (character.Frozen || EffectiveAttack(state, attacker) <= 0)
                continue;
            var reach = GetAttackReach(state, attacker);
            if (reach == AttackReach.None)
                continue;
            foreach (var defender in taunts.Length == 0 ? enemyBoard : taunts)
                if (reach == AttackReach.AnyCharacter || defender.Id != state.Players[enemy].Hero)
                    actions.Add(new(SimulationActionKind.Attack, actor, id, defender.Id));
        }

        var costAuras = ActiveCostAuras(state, actor);
        foreach (var id in player.In(GameZone.Hand))
        {
            var entity = state.Entities[id];
            var card = Card(entity);
            var usesBoardSlot = UsesBoardSlot(card.Definition.Kind);
            if (!CanPayCardCost(state, player, EffectiveCostType(entity, card), EffectiveCost(entity, card, costAuras)) ||
                (occupiedSlots >= BoardLimit && usesBoardSlot) ||
                (card.IsSecret && !CanArmSecret(state, player, entity.CardId)) ||
                !MeetsPlayRequirements(state, actor, card, occupiedSlots, minions))
                continue;

            if (card.Choices.IsEmpty)
                AddPlayActions(card.Target, card.TargetFilter, card.OnPlay, choiceId: null);
            else
                foreach (var choice in card.Choices)
                    if (MeetsPlayRequirements(state, actor, choice.Requirements, occupiedSlots))
                        AddPlayActions(choice.Target, choice.TargetFilter, choice.Effects, choice.Id);

            void AddPlayActions(TargetRequirement requirement, TargetFilter? filter, ImmutableArray<CardEffect> effects, string? choiceId)
            {
                if (occupiedSlots >= BoardLimit && effects.Any(effect => effect is TakeControlEffect))
                    return;

                var targets = PlayTargets(state, actor, requirement, filter);
                if (card.Definition.Kind is EntityKind.Minion or EntityKind.Weapon)
                    targets = targets.DefaultIfEmpty();

                foreach (var target in targets)
                {
                    if (usesBoardSlot)
                    {
                        for (var position = 0; position <= occupiedSlots; position++)
                            actions.Add(new(SimulationActionKind.PlayCard, actor, id, target, position) { Choice = choiceId });
                    }
                    else
                    {
                        actions.Add(new(SimulationActionKind.PlayCard, actor, id, target) { Choice = choiceId });
                    }
                }
            }
        }

        if (player.HeroPowerUsesThisTurn == 0 && player.In(GameZone.HeroPower) is [var powerId])
        {
            var entity = state.Entities[powerId];
            var power = Card(entity);
            if (CanPayCardCost(state, player, EffectiveCostType(entity, power), EffectiveCost(entity, power, costAuras)) &&
                MeetsPlayRequirements(state, actor, power, occupiedSlots, minions))
                foreach (var target in PlayTargets(state, actor, power.Target, power.TargetFilter))
                    actions.Add(new(SimulationActionKind.UseHeroPower, actor, powerId, target));
        }

        actions.Add(new(SimulationActionKind.EndTurn, actor));
        return actions.ToImmutable();
    }

    public SimulationResult Apply(GameState state, SimulationAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);
        if (action.Kind == SimulationActionKind.ChooseOption)
        {
            // Local selection intent only: no simulated OS input or game network/API submission occurs here.
            var packet = state.PendingChoice?.Packet ??
                throw new InvalidOperationException("没有等待处理的选择包。");
            if (state.Outcome != MatchOutcome.InProgress || action.Actor != packet.Player ||
                action.ChoicePacketId != packet.ChoiceId || action.Source is not null ||
                action.Target is not null || action.Position is not null || action.Choice is not null)
                throw new InvalidOperationException("选择动作的玩家、包 ID 或额外字段与当前选择包不匹配。");
            var submitted = state.SubmitChoice(packet.ChoiceId, action.OrderedChoiceTargets);
            return new(submitted, [new(SimulationEventKind.ChoiceSubmitted, action.Actor, packet.Source)
            {
                ChoiceId = packet.ChoiceId, OrderedChoiceTargets = action.OrderedChoiceTargets
            }]);
        }
        EnsureLegal(state, action);
        return new Resolution(this, state).Run(action);
    }

    private void EnsureLegal(GameState state, SimulationAction action)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action), "必须提供要结算的动作。");
        if (LegalActions(state).Contains(action))
            return;
        if (action.Kind is SimulationActionKind.PlayCard or SimulationActionKind.UseHeroPower &&
            action.Source is EntityId source && state.Entities.TryGetValue(source, out var entity) &&
            entity.Controller == action.Actor)
        {
            var type = EffectiveCostType(entity, Card(entity));
            if (!SupportsCostType(type))
                throw new InvalidOperationException($"卡牌「{entity.CardId}」的替代费用类型为 {type}，尚未接入该支付方式。");
        }
        throw new InvalidOperationException($"玩家 {action.Actor.Value} 当前不能执行动作「{action.Kind}」。");
    }

    private bool IsTaunt(GameEntity entity) =>
        Card(entity).Definition.Kind == EntityKind.Minion && HasKeyword(entity, CardKeywords.Taunt);

    private bool HasKeyword(GameEntity entity, CardKeywords keyword) =>
        !entity.Silenced && Card(entity).Keywords.HasFlag(keyword);

    private static bool IsCharacter(EntityKind kind) => kind is EntityKind.Hero or EntityKind.Minion;

    private bool CanCreateCard(string? cardId) => !string.IsNullOrWhiteSpace(cardId) &&
        GetCard(cardId).Definition.Kind is EntityKind.Minion or EntityKind.Spell or EntityKind.Weapon or EntityKind.Location;

    private static bool CanArmSecret(GameState state, PlayerState player, string cardId) =>
        player.In(GameZone.Secret).Length < SecretLimit && !player.In(GameZone.Secret).Any(id => state.Entities[id].CardId == cardId);

    private bool MeetsPlayRequirements(GameState state, PlayerId player, SimulationCard card, int occupiedSlots, int minions) =>
        MeetsPlayRequirements(state, player, card.Requirements, occupiedSlots) &&
        ((card.MinimumEnemyMinions == 0 && card.MinimumTotalMinions == 0) ||
            HasRequiredMinions(state, player, card, minions));

    private static bool MeetsPlayRequirements(GameState state, PlayerId player, PlayRequirements requirements, int occupiedSlots) =>
        (!requirements.HasFlag(PlayRequirements.BoardSpace) || occupiedSlots < BoardLimit) &&
        (!requirements.HasFlag(PlayRequirements.FriendlyMinionDeath) ||
            state.MinionDeaths.Any(death => death.Controller == player));

    private bool HasRequiredMinions(GameState state, PlayerId player, SimulationCard card, int friendlyMinions)
    {
        var enemies = MinionCount(state, state.OpponentOf(player));
        return enemies >= card.MinimumEnemyMinions && friendlyMinions + enemies >= card.MinimumTotalMinions;
    }

    private SimulationCard Card(GameEntity entity) => GetCard(entity.CardId);
    private SimulationCard GetCard(string cardId) => _cards.TryGetValue(cardId, out var card)
        ? card
        : throw new ArgumentException($"此规则目录缺少卡牌「{cardId}」的模拟实现。");

    private int MinionCount(GameState state, PlayerId player) => BoardMinions(state, player).Count();

    private IEnumerable<EntityId> BoardMinions(GameState state, PlayerId player) =>
        state.Players[player].In(GameZone.Play)
            .Where(id => Card(state.Entities[id]).Definition.Kind == EntityKind.Minion);

    private static bool UsesBoardSlot(EntityKind kind) => kind is EntityKind.Minion or EntityKind.Location;

    private int BoardSlotCount(GameState state, PlayerId player) => BoardSlots(state, player).Count();

    private IEnumerable<EntityId> BoardSlots(GameState state, PlayerId player) =>
        state.Players[player].In(GameZone.Play).Where(id => UsesBoardSlot(Card(state.Entities[id]).Definition.Kind));

    private int BoardIndex(GameState state, PlayerId player, int position)
    {
        var zone = state.Players[player].In(GameZone.Play);
        foreach (var id in BoardSlots(state, player))
        {
            if (position-- == 0)
                return zone.IndexOf(id);
        }
        return zone.Length;
    }

    private void ValidateEffects(SimulationCard card, ImmutableArray<CardEffect> effects, TargetRequirement target = TargetRequirement.None,
        TriggerTiming? timing = null, bool hasInPlaySource = false, bool isDiscardedCard = false, bool isDrawnCard = false,
        bool hasCharacterSubject = false)
    {
        if (effects.IsDefault)
            throw new ArgumentException($"卡牌「{card.Definition.CardId}」的效果列表必须初始化。");
        if (effects.IsEmpty)
            return;
        hasCharacterSubject |= HasCharacterSubject(timing);
        var pending = new Stack<CardEffect>(effects);
        while (pending.TryPop(out var effect))
        {
            if (effect is ConditionalEffect conditional &&
                IsValidCondition(conditional.Condition, target, hasCharacterSubject, hasInPlaySource) &&
                !conditional.Then.IsDefault && !conditional.Otherwise.IsDefault &&
                (!conditional.Then.IsEmpty || !conditional.Otherwise.IsEmpty))
            {
                foreach (var otherwise in conditional.Otherwise)
                    pending.Push(otherwise);
                foreach (var then in conditional.Then)
                    pending.Push(then);
                continue;
            }

            var valid = effect switch
            {
                DamageEffect damage => damage.Amount > 0 && (damage.Target switch
                {
                    DamageTarget.Selected => target != TargetRequirement.None,
                    DamageTarget.Source => hasInPlaySource,
                    DamageTarget.TriggerSubject => hasCharacterSubject,
                    DamageTarget.EnemyHero or DamageTarget.FriendlyHero or DamageTarget.AllMinions or
                        DamageTarget.EnemyMinions or DamageTarget.AllEnemies => true,
                    _ => false
                }),
                DrawEffect draw => draw.Count > 0,
                DrawMatchingEffect draw => draw.Count > 0 && draw.CardFilter is not null && IsValidCardFilter(draw.CardFilter),
                ChangeHandCostEffect change => change.Delta != 0 && Enum.IsDefined(change.Duration) &&
                    IsOptionalCardIdValid(change.EnchantmentCardId) && HasHandCostTarget(change.Target, change.CardFilter),
                ChangeCostTypeEffect change => SupportsCostType(change.Type) && Enum.IsDefined(change.Duration) &&
                    IsOptionalCardIdValid(change.EnchantmentCardId) && HasHandCostTarget(change.Target, change.CardFilter),
                RecruitEffect recruit => recruit.Count > 0 && IsValidCardFilter(recruit.CardFilter) &&
                    recruit.CardFilter?.Kind is null or EntityKind.Minion,
                CreateCardEffect create => create.Count > 0 && Enum.IsDefined(create.Placement) &&
                    Enum.IsDefined(create.Recipient) && CanCreateCard(create.CardId),
                DiscardEffect discard => discard.Count > 0 && Enum.IsDefined(discard.Selection) && Enum.IsDefined(discard.Recipient),
                DiscardHandEffect discard => Enum.IsDefined(discard.Recipient),
                SummonDiscardedMinionEffect => isDiscardedCard && card.Definition.Kind == EntityKind.Minion,
                GainArmorEffect armor => armor.Amount > 0,
                HealFriendlyHeroEffect heal => heal.Amount > 0,
                EquipWeaponEffect equip => GetCard(equip.CardId).Definition.Kind == EntityKind.Weapon,
                DestroyEnemyWeaponEffect => true,
                EnchantStatsEffect enchant => Enum.IsDefined(enchant.Target) && Enum.IsDefined(enchant.Duration) &&
                    IsValidModifier(enchant.Modifier) && IsOptionalCardIdValid(enchant.EnchantmentCardId) &&
                    (enchant.Target != StatTarget.Selected || target != TargetRequirement.None) &&
                    (enchant.Target != StatTarget.Source || hasInPlaySource),
                SilenceEffect or DestroyMinionEffect => TargetsOnlyMinions(target),
                ReturnMinionEffect returned => Enum.IsDefined(returned.CostDuration) && IsOptionalCardIdValid(returned.EnchantmentCardId) &&
                    (returned.CostDelta != 0 || (returned.EnchantmentCardId is null && returned.CostDuration == EnchantmentDuration.Permanent)) &&
                    (returned.Target switch
                    {
                        EffectTarget.Selected => TargetsOnlyMinions(target),
                        EffectTarget.TriggerSubject => hasCharacterSubject,
                        _ => false
                    }),
                TransformMinionEffect transform => TargetsOnlyMinions(target) &&
                    GetCard(transform.CardId).Definition.Kind == EntityKind.Minion,
                ResurrectFriendlyMinionEffect => true,
                FreezeEffect freeze => freeze.Target switch
                {
                    FreezeTarget.Selected => target != TargetRequirement.None,
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
                ChangeManaEffect mana => mana.Amount > 0 && Enum.IsDefined(mana.Operation) && Enum.IsDefined(mana.Recipient) &&
                    (mana.OverflowCardId is null || (mana.Operation is ManaOperation.GainEmpty or ManaOperation.GainFilled &&
                        CanCreateCard(mana.OverflowCardId))),
                SummonEffect summon => Enum.IsDefined(summon.Placement) && GetCard(summon.CardId).Definition.Kind == EntityKind.Minion,
                TakeControlEffect => card.Definition.Kind == EntityKind.Spell && target == TargetRequirement.EnemyMinion,
                _ => false
            };
            if (!valid)
                throw new ArgumentException($"卡牌「{card.Definition.CardId}」的效果定义无效。");
            if (effect is EnchantStatsEffect { OnRemoved.IsDefaultOrEmpty: false } enchantment)
                RegisterEnchantmentRemovalEffects(card, enchantment);
        }

        bool HasHandCostTarget(HandCostTarget recipient, CardFilter? filter) => IsValidCardFilter(filter) && (recipient switch
        {
            HandCostTarget.Source => isDrawnCard,
            HandCostTarget.DrawnCard => timing == TriggerTiming.CardDrawn,
            HandCostTarget.FriendlyHand => true,
            _ => false
        });
    }

    private static bool HasCharacterSubject(TriggerTiming? timing) =>
        timing is TriggerTiming.WhenMinionSummoned or TriggerTiming.AfterMinionSummoned or
            TriggerTiming.AfterMinionPlayed or TriggerTiming.Damage or TriggerTiming.HeroAttacking or TriggerTiming.AttackProposed;

    private static bool IsOptionalCardIdValid(string? cardId) => cardId is null || !string.IsNullOrWhiteSpace(cardId);
}
