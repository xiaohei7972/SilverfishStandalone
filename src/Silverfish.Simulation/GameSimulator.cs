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
    private readonly Dictionary<string, ImmutableArray<CardEffect>> _enchantmentAttackEffects = new(StringComparer.Ordinal);
    private readonly int _resolutionLimit;
    private readonly bool _hasCostAuras;
    private readonly bool _hasWeaponAuras;
    private readonly bool _hasHealingDealsDamage;

    public GameSimulator(IEnumerable<SimulationCard> cards, int resolutionLimit = 4096,
        IEnumerable<string>? randomMinionPool = null)
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
        RandomMinionPool = randomMinionPool?.ToImmutableArray();
        _randomMinionCosts = new(BuildRandomMinionCosts);
        if (RandomMinionPool.HasValue)
            _ = _randomMinionCosts.Value;
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
                    CardKeywords.Rush | CardKeywords.Windfury | CardKeywords.CastsWhenDrawn |
                    CardKeywords.Poisonous | CardKeywords.Lifesteal | CardKeywords.Overheal)) != 0 ||
                (card.Keywords.HasFlag(CardKeywords.Overheal) && definition.Kind != EntityKind.Minion) ||
                (card.Keywords.HasFlag(CardKeywords.CastsWhenDrawn) && definition.Kind != EntityKind.Spell) ||
                card.SpellDamage < 0 || card.SpellAndHeroPowerMultiplier is < 2 ||
                (definition.Kind != EntityKind.Minion && (card.SpellDamage != 0 || card.SpellAndHeroPowerMultiplier is not null)) ||
                card.Overload < 0 || (card.Overload > 0 && definition.Kind is EntityKind.Hero or EntityKind.HeroPower) ||
                card.Triggers.IsDefault || card.Auras.IsDefault || card.WeaponAuras.IsDefault || card.CostAuras.IsDefault || card.Choices.IsDefault ||
                !Enum.IsDefined(card.SelfCostReduction) || (card.SelfCostReduction != SelfCostReduction.None && definition.Kind == EntityKind.Hero) ||
                (definition.Kind is not (EntityKind.Minion or EntityKind.Weapon) && !card.Deathrattle.IsEmpty) ||
                (definition.Kind != EntityKind.Minion && (!card.Auras.IsEmpty || !card.WeaponAuras.IsEmpty || card.LeavesNoCorpse || card.HealingDealsDamage || card.CannotAttack || card.AttackEqualsHealth)) ||
                (definition.Kind != EntityKind.Weapon && (card.HeroImmuneWhileAttacking || card.MinionAttackRule is not null)) ||
                (card.MinionAttackRule is { } minionAtkRule && (minionAtkRule.AttackLoss <= 0 || !IsOptionalCardIdValid(minionAtkRule.EnchantmentCardId))) ||
                (!IsCharacter(definition.Kind) && definition.Kind != EntityKind.Weapon && !card.CostAuras.IsEmpty) ||
                (!IsCharacter(definition.Kind) && definition.Kind != EntityKind.Weapon && !card.IsSecret &&
                    card.Triggers.Any(trigger => trigger.Timing is not (TriggerTiming.Overkill or TriggerTiming.HonorableKill))) ||
                (!card.Choices.IsEmpty && (card.Choices.Length < 2 || card.IsSecret || card.Target != TargetRequirement.None ||
                    !card.OnPlay.IsDefaultOrEmpty || definition.Kind is not (EntityKind.Minion or EntityKind.Spell))) ||
                (card.IsSecret && (definition.Kind != EntityKind.Spell || card.Target != TargetRequirement.None ||
                    !card.OnPlay.IsDefaultOrEmpty || card.Triggers.Length != 1)))
                throw new ArgumentException($"模拟卡牌「{definition.CardId}」的定义无效。", nameof(cards));

            ValidateEffects(card, card.OnPlay, EffectCharacterTarget(card.Target, BranchRequirements(card)),
                hasInPlaySource: IsCharacter(definition.Kind),
                canUseCombo: !card.IsPlayChoiceOption && definition.Kind is EntityKind.Minion or EntityKind.Spell or EntityKind.Weapon or EntityKind.Hero);
            if (card.ChooseAfterPlay is { } choiceRule &&
                (definition.Kind is not (EntityKind.Minion or EntityKind.Spell) || card.IsSecret ||
                    card.Keywords.HasFlag(CardKeywords.CastsWhenDrawn) ||
                    !IsValidPlayChoice(card, choiceRule)))
                throw new ArgumentException($"卡牌「{definition.CardId}」的出牌后选择定义无效。", nameof(cards));
            if (card.IsPlayChoiceOption && (definition.Kind != EntityKind.Spell || card.OnPlay.IsEmpty || card.IsSecret ||
                card.ChooseAfterPlay is not null || !card.Choices.IsEmpty))
                throw new ArgumentException($"卡牌「{definition.CardId}」的待决选项定义无效。", nameof(cards));
            ValidateEffects(card, card.Deathrattle);
            ValidateEffects(card, card.WhenDrawn, isDrawnCard: true);
            if (card.WhenDiscarded is { } whenDiscarded)
                ValidateEffects(card, whenDiscarded, isDiscardedCard: true);
            if (card.Activation is { } activation)
            {
                if (!IsValidTarget(activation.Target, activation.TargetFilter) || activation.Effects.IsDefaultOrEmpty)
                    throw new ArgumentException($"卡牌「{definition.CardId}」的地标激活规则无效。", nameof(cards));
                ValidateEffects(card, activation.Effects, EffectCharacterTarget(activation.Target, BranchRequirements(card, activation: true)));
            }
            var choiceIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var choice in card.Choices)
            {
                if (choice is null || string.IsNullOrWhiteSpace(choice.Id) || !choiceIds.Add(choice.Id) ||
                    !IsValidTarget(choice.Target, choice.TargetFilter) || (choice.Requirements & ~SupportedPlayRequirements) != 0 ||
                    choice.Effects.IsDefaultOrEmpty)
                    throw new ArgumentException($"卡牌「{definition.CardId}」的抉择选项无效。", nameof(cards));
                ValidateEffects(card, choice.Effects, EffectCharacterTarget(choice.Target, BranchRequirements(card, choice)),
                    hasInPlaySource: definition.Kind == EntityKind.Minion,
                    canUseCombo: definition.Kind is EntityKind.Minion or EntityKind.Spell);
            }
            foreach (var aura in card.Auras)
            {
                if (aura is null || !Enum.IsDefined(aura.Recipients) || !Enum.IsDefined(aura.Condition) ||
                    (aura.Modifier is null && aura.GrantedTag is null) ||
                    (aura.Modifier is not null && (aura.Modifier.Mode != StatModifierMode.Add || !IsValidModifier(aura.Modifier))) ||
                    (aura.GrantedTag is GAME_TAG auraTag && !IsSupportedAuraTag(auraTag)) ||
                    (aura.Recipients == AuraRecipients.Self && aura.CardFilter is not null) ||
                    !IsOptionalCardIdValid(aura.EnchantmentCardId) || !IsValidCardFilter(aura.CardFilter))
                    throw new ArgumentException($"卡牌「{definition.CardId}」的属性光环无效。", nameof(cards));
            }
            foreach (var aura in card.WeaponAuras)
            {
                if (aura is null || aura.Attack <= 0 || !Enum.IsDefined(aura.Condition) ||
                    !IsOptionalCardIdValid(aura.EnchantmentCardId))
                    throw new ArgumentException($"卡牌「{definition.CardId}」的武器攻击光环无效。", nameof(cards));
            }
            foreach (var aura in card.CostAuras)
                if (aura is null || !Enum.IsDefined(aura.Recipient) || !Enum.IsDefined(aura.Target) || aura.Delta == 0 ||
                    (aura.FirstMinionEachTurn && aura.Target != CostAuraTarget.Minion) ||
                    (aura.MinimumCost is int minCost && (minCost < 0 || aura.Delta >= 0)) ||
                    !IsValidCardFilter(aura.CardFilter))
                    throw new ArgumentException($"卡牌「{definition.CardId}」的费用光环无效。", nameof(cards));

            foreach (var trigger in card.Triggers)
            {
                if (trigger is null || !Enum.IsDefined(trigger.Timing) || !Enum.IsDefined(trigger.Relation) ||
                    !Enum.IsDefined(trigger.Focus) || !Enum.IsDefined(trigger.EntityMatch) ||
                    !IsValidCardFilter(trigger.CardFilter) ||
                    (trigger.SecretOnly && trigger.Timing is not (TriggerTiming.BeforeSpellResolved or TriggerTiming.AfterSpellResolved or TriggerTiming.SpellCounterWindow)) ||
                    (trigger.OverloadOnly && trigger.Timing is not (TriggerTiming.WhenCardPlayed or TriggerTiming.AfterCardPlayed)) ||
                    (trigger.EntityKind is EntityKind kind && (kind == EntityKind.Unknown || !Enum.IsDefined(kind))) ||
                    (trigger.AttackingKind is EntityKind atkKind && (trigger.Timing != TriggerTiming.AttackProposed || atkKind == EntityKind.Unknown || !Enum.IsDefined(atkKind))) ||
                    (trigger.Focus == TriggerFocus.DamageSource &&
                    trigger.Timing is not (TriggerTiming.Damage or TriggerTiming.Overkill or TriggerTiming.HonorableKill)) ||
                (trigger.Timing is TriggerTiming.Overkill or TriggerTiming.HonorableKill &&
                    (trigger.Focus != TriggerFocus.DamageSource || trigger.EntityMatch != TriggerEntityMatch.Self)) ||
                (trigger.Timing == TriggerTiming.Overheal &&
                    (definition.Kind != EntityKind.Minion || !card.Keywords.HasFlag(CardKeywords.Overheal) ||
                        trigger.Focus != TriggerFocus.EventTarget || trigger.EntityMatch != TriggerEntityMatch.Self)) ||
                    (trigger.Focus == TriggerFocus.AttackDefender && trigger.Timing != TriggerTiming.AttackProposed) ||
                    (trigger.Timing is TriggerTiming.SpellCounterWindow or TriggerTiming.SpellTargetWindow or TriggerTiming.AttackProposed or TriggerTiming.FatalDamageWindow && !card.IsSecret) ||
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
        _hasWeaponAuras = _cards.Values.Any(card => !card.WeaponAuras.IsEmpty);
        _hasHealingDealsDamage = _cards.Values.Any(card => card.HealingDealsDamage);
    }

    public GameState CreateGame(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities,
        PlayerId activePlayer, int turn = 1, IEnumerable<MinionDeath>? minionDeaths = null, ulong randomState = 0, PendingEntityChoice? pendingChoice = null) =>
        ValidateState(GameState.Create(turn == 1 ? players.Select(p => p with
        {
            PlayedCardThisTurn = p.PlayedCardThisTurn ?? (p.CardsPlayedThisTurn > 0),
            PlayedMinionThisTurn = p.PlayedMinionThisTurn ?? false,
            PlayedElementalThisTurn = p.PlayedElementalThisTurn ?? false,
            PlayedElementalLastTurn = p.PlayedElementalLastTurn ?? false, HeroHealthChangedThisTurn = p.HeroHealthChangedThisTurn ?? false
        }) : players, entities, activePlayer, turn, minionDeaths, randomState, pendingChoice: pendingChoice));

    public IReadOnlyDictionary<string, SimulationCard> Cards => _cards;

    public GameState RestoreGame(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities, PlayerId activePlayer,
        int turn, IEnumerable<MinionDeath> minionDeaths, ulong randomState, GameContinuation continuation, PendingEntityChoice? pendingChoice = null)
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
            if (player.TemporaryCostAuras.Any(aura => aura is null || !state.Entities.ContainsKey(aura.Source) ||
                !Enum.IsDefined(aura.TargetKind) || aura.TargetKind == EntityKind.Unknown || !Enum.IsDefined(aura.Mode) ||
                (aura.Mode == CostModifierMode.Set ? aura.Amount < 0 : aura.Amount == 0) ||
                aura.ActiveOnTurn < state.Turn || !IsOptionalCardIdValid(aura.CardId)))
                throw new ArgumentException($"玩家 {player.Id.Value} 的临时费用光环无效。");
            if (player.EndOfTurnHandAdditions.Any(cardId => !CanCreateCard(cardId)))
                throw new ArgumentException($"玩家 {player.Id.Value} 的回合结束待生成卡牌无效。");
        }

        foreach (var entity in state.Entities.Values)
        {
            var kind = Card(entity).Definition.Kind;
            var location = state.LocationOf(entity.Id);
            var player = state.Players[entity.Controller];
            var isHeroSlot = player.Hero == entity.Id;
            var isEquippedSlot = player.EquippedWeapon == entity.Id;
            var isPendingSpell = kind == EntityKind.Spell && state.PendingPlayChoice?.Source == entity.Id;
            if ((isHeroSlot && kind != EntityKind.Hero) ||
                (location.Zone == GameZone.Play && kind == EntityKind.Hero && !isHeroSlot) ||
                (location.Zone == GameZone.Play && kind != EntityKind.Location && !isPendingSpell && (kind == EntityKind.Weapon ? !isEquippedSlot :
                    !IsCharacter(kind) || entity.Character is not { Health: > 0 })) ||
                (location.Zone == GameZone.HeroPower ? kind != EntityKind.HeroPower : (kind == EntityKind.HeroPower && location.Zone != GameZone.Removed)) ||
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
        if (state.PendingPlayChoice is { } choice)
            ValidatePlayChoice(state, choice);
        return RefreshAuras(state);
    }

    public ImmutableArray<SimulationAction> LegalActions(GameState state)
    {
        if (state is null)
            throw new ArgumentNullException(nameof(state), "必须提供对局状态。");
        if (state.Outcome != MatchOutcome.InProgress || state.PendingChoice is not null)
            return [];
        if (state.PendingPlayChoice is { } choice)
            return [.. choice.Options.Select(option => new SimulationAction(SimulationActionKind.ChooseOption,
                state.ActivePlayer, choice.Source, option))];

        var actions = ImmutableArray.CreateBuilder<SimulationAction>();
        var actor = state.ActivePlayer;
        var player = state.Players[actor];
        var enemy = state.OpponentOf(actor);
        var enemyBoard = state.Players[enemy].In(GameZone.Play).Select(id => state.Entities[id])
            .Where(entity => entity.Character is { Health: > 0 } && !IsStealthed(entity) && !HasEffectiveTag(entity, GAME_TAG.IMMUNE)).ToArray();
        var taunts = enemyBoard.Where(IsTaunt).ToArray();

        var costAuras = ActiveCostAuras(state, actor);
        var occupiedSlots = BoardSlotCount(state, actor);
        foreach (var id in player.In(GameZone.Play))
        {
            var attacker = state.Entities[id];
            if (attacker.Location is not null)
            {
                AddCardActions(attacker, GameZone.Play, SimulationActionKind.ActivateLocation);
                continue;
            }
            if (attacker.Character is not { } character || Card(attacker).EffectPending)
                continue;
            if (character.Frozen || EffectiveAttack(state, attacker) <= 0)
                continue;
            var reach = GetAttackReach(state, attacker);
            if (reach == AttackReach.None)
                continue;
            foreach (var defender in taunts.Length == 0 ? enemyBoard : taunts)
                if (reach == AttackReach.AnyCharacter || defender.Id != state.Players[enemy].Hero)
                    actions.Add(new(SimulationActionKind.Attack, actor, id, defender.Id));
        }

        foreach (var id in player.In(GameZone.Hand))
            AddCardActions(state.Entities[id], GameZone.Hand, SimulationActionKind.PlayCard);
        foreach (var id in player.In(GameZone.HeroPower))
            AddCardActions(state.Entities[id], GameZone.HeroPower, SimulationActionKind.UseHeroPower);

        void AddCardActions(GameEntity entity, GameZone zone, SimulationActionKind kind)
        {
            var card = Card(entity);
            if (SourceFailure(state, entity, card, zone, costAuras) is not null)
                return;
            foreach (var branch in Branches(state, entity, card, kind == SimulationActionKind.ActivateLocation))
            foreach (var target in branch.Targets)
            {
                if (kind == SimulationActionKind.PlayCard && UsesBoardSlot(card.Definition.Kind))
                {
                    for (var position = 0; position <= occupiedSlots; position++)
                        actions.Add(new(kind, actor, entity.Id, target, position) { Choice = branch.Choice });
                }
                else
                    actions.Add(new(kind, actor, entity.Id, target) { Choice = branch.Choice });
            }
        }

        actions.Add(new(SimulationActionKind.EndTurn, actor));
        return actions.ToImmutable();
    }

    public SimulationResult Apply(GameState state, SimulationAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);
        if (action.Kind == SimulationActionKind.ChooseOption && state.PendingChoice is not null)
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
        var resolvesEffects = EnsureLegal(state, action);
        return new Resolution(this, state).Run(action, resolvesEffects);
    }

    private bool EnsureLegal(GameState state, SimulationAction action)
    {
        if (CheckAction(state, action, out var resolvesEffects) is { } failure)
            throw new InvalidOperationException($"{failure.Name} {failure.Reason}".Trim());
        return resolvesEffects;
    }

    private bool IsTaunt(GameEntity entity) =>
        Card(entity).Definition.Kind == EntityKind.Minion && HasKeyword(entity, CardKeywords.Taunt) &&
        !HasEffectiveTag(entity, GAME_TAG.IMMUNE);

    private bool HasKeyword(GameEntity entity, CardKeywords keyword) =>
        (!entity.Silenced && Card(entity).Keywords.HasFlag(keyword)) || (keyword switch
        {
            CardKeywords.Taunt => HasGrantedTag(entity, GAME_TAG.TAUNT),
            CardKeywords.Charge => HasGrantedTag(entity, GAME_TAG.CHARGE),
            CardKeywords.Rush => HasGrantedTag(entity, GAME_TAG.RUSH),
            CardKeywords.Windfury => HasGrantedTag(entity, GAME_TAG.WINDFURY),
            CardKeywords.Poisonous => HasGrantedTag(entity, GAME_TAG.POISONOUS),
            CardKeywords.Lifesteal => HasGrantedTag(entity, GAME_TAG.LIFESTEAL),
            _ => false
        });

    private static bool HasGrantedTag(GameEntity entity, GAME_TAG tag) =>
        entity.Enchantments.Any(layer => layer.GrantedTag == tag) ||
        entity.AuraGrants.Any(aura => aura.GrantedTag == tag);

    private bool HasEffectiveTag(GameEntity entity, GAME_TAG tag) =>
        (!entity.Silenced && PlayReqTags.Has(Card(entity).RequirementTagMask, (int)tag)) || HasGrantedTag(entity, tag);

    private bool IsStealthed(GameEntity entity) =>
        (entity.Stealthed ?? (!entity.Silenced && PlayReqTags.Has(Card(entity).RequirementTagMask, (int)GAME_TAG.STEALTH))) ||
        HasGrantedTag(entity, GAME_TAG.STEALTH);

    private static bool IsCharacter(EntityKind kind) => kind is EntityKind.Hero or EntityKind.Minion;

    private bool CanCreateCard(string? cardId) => !string.IsNullOrWhiteSpace(cardId) &&
        GetCard(cardId).Definition.Kind is EntityKind.Minion or EntityKind.Spell or EntityKind.Weapon or EntityKind.Location or EntityKind.Hero;

    private static bool CanArmSecret(GameState state, PlayerState player, string cardId) =>
        player.In(GameZone.Secret).Length < SecretLimit && !player.In(GameZone.Secret).Any(id => state.Entities[id].CardId == cardId);

    private SimulationCard Card(GameEntity entity) => GetCard(entity.CardId);
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, SimulationCard> _pending = new(StringComparer.Ordinal);
    private SimulationCard GetCard(string cardId)
    {
        if (_cards.TryGetValue(cardId, out var card))
            return card;
        if (_pending.TryGetValue(cardId, out var pending))
            return pending;
        if (PlayReqCatalog.Cards.TryGetValue(cardId, out var info) && !info.EffectImplemented)
            return _pending.GetOrAdd(cardId, id => CompiledCardData.Bind(id, new CardRules { PlayReqs = info.Requirements }) with { EffectPending = true });
        if (CompiledCardData.CollectibleMinionCosts.ContainsKey(cardId))
            return _pending.GetOrAdd(cardId, id => CompiledCardData.Bind(id,
                info is not null ? new CardRules { PlayReqs = info.Requirements } : new CardRules()) with { EffectPending = true });
        throw new ArgumentException($"此规则目录缺少卡牌「{cardId}」的模拟实现。");
    }

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
        bool hasCharacterSubject = false, bool canUseCombo = false)
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
                IsValidCondition(conditional.Condition, target, hasCharacterSubject, hasInPlaySource, canUseCombo) &&
                !conditional.Then.IsDefault && !conditional.Otherwise.IsDefault &&
                (!conditional.Then.IsEmpty || !conditional.Otherwise.IsEmpty))
            {
                foreach (var otherwise in conditional.Otherwise)
                    pending.Push(otherwise);
                foreach (var then in conditional.Then)
                    pending.Push(then);
                continue;
            }
            if (effect is RandomChanceEffect chance &&
                chance.Numerator > 0 && chance.Numerator < chance.Denominator &&
                !chance.Then.IsDefaultOrEmpty)
            {
                foreach (var then in chance.Then)
                    pending.Push(then);
                continue;
            }

            var valid = effect switch
            {
                DamageEffect damage => damage.Amount > 0 &&
                    (damage.Enchantment is null || (damage.Target == DamageTarget.Selected && damage.Enchantment.Target == StatTarget.Selected)) &&
                    IsValidDamageTarget(damage.Target),
                EventAmountDamageEffect eventDamage => timing == TriggerTiming.Damage && IsValidDamageTarget(eventDamage.Target),
                SplashDamageEffect splash => splash.CenterAmount > 0 && splash.AdjacentAmount > 0 && TargetsOnlyMinions(target),
                ScaledDamageEffect scaled => IsValidConditionCharacter(scaled.ValueSource, target, hasCharacterSubject, hasInPlaySource) &&
                    Enum.IsDefined(scaled.Metric) &&
                    (!scaled.DestroyValueSource || (scaled.ValueSource == ConditionCharacter.Selected && TargetsOnlyMinions(target))) &&
                    IsValidDamageTarget(scaled.Target),
                AdjacentAttackDamageEffect adjacent => adjacent.Target switch
                {
                    EffectTarget.Selected => TargetsOnlyMinions(target),
                    EffectTarget.TriggerSubject => hasCharacterSubject,
                    _ => false
                },
                DrawEffect draw => draw.Count > 0 && Enum.IsDefined(draw.Recipient) &&
                    (draw.CostDelta != 0 || draw.EnchantmentCardId is null) &&
                    IsOptionalCardIdValid(draw.EnchantmentCardId),
                DynamicDrawEffect draw => Enum.IsDefined(draw.Mode),
                DrawAndDamageByCostEffect draw => IsValidDamageTarget(draw.Target),
                DrawMatchingEffect draw => draw.Count > 0 && draw.CardFilter is not null && IsValidCardFilter(draw.CardFilter),
                ChangeHandCostEffect change => change.Delta != 0 && IsHandCostDuration(change.Duration) &&
                    IsOptionalCardIdValid(change.EnchantmentCardId) && HasHandCostTarget(change.Target, change.CardFilter),
                AddTemporaryCostAuraEffect tempCost => Enum.IsDefined(tempCost.TargetKind) && tempCost.TargetKind != EntityKind.Unknown &&
                    Enum.IsDefined(tempCost.Mode) && Enum.IsDefined(tempCost.Recipient) &&
                    (tempCost.Mode == CostModifierMode.Set ? tempCost.Amount >= 0 : tempCost.Amount != 0) &&
                    (!tempCost.SecretOnly || tempCost.TargetKind == EntityKind.Spell) &&
                    IsOptionalCardIdValid(tempCost.EnchantmentCardId),
                ChangeCostTypeEffect change => SupportsCostType(change.Type) && IsHandCostDuration(change.Duration) &&
                    IsOptionalCardIdValid(change.EnchantmentCardId) && HasHandCostTarget(change.Target, change.CardFilter),
                RecruitEffect recruit => recruit.Count > 0 && IsValidCardFilter(recruit.CardFilter) &&
                    recruit.CardFilter?.Kind is null or EntityKind.Minion,
                CreateCardEffect create => create.Count > 0 && Enum.IsDefined(create.Placement) &&
                    Enum.IsDefined(create.Recipient) && CanCreateCard(create.CardId),
                CreateRandomCardEffect createRandom => !createRandom.CardIds.IsDefaultOrEmpty && createRandom.Count > 0 &&
                    Enum.IsDefined(createRandom.Recipient) && createRandom.CardIds.All(CanCreateCard),
                CreateCardAtTurnEndEffect delayedCreate => CanCreateCard(delayedCreate.CardId),
                CopyDeckCardsEffect copy => copy.Count > 0 && Enum.IsDefined(copy.SourceDeck) && IsValidCardFilter(copy.CardFilter),
                SummonDeckCopyEffect copy => copy.Count > 0 && Enum.IsDefined(copy.SourceDeck) &&
                    IsValidCardFilter(copy.CardFilter) && copy.CardFilter?.Kind is null or EntityKind.Minion &&
                    (copy.FallbackCardId is null || GetCard(copy.FallbackCardId).Definition.Kind == EntityKind.Minion),
                DiscardEffect discard => discard.Count > 0 && Enum.IsDefined(discard.Selection) && Enum.IsDefined(discard.Recipient),
                DiscardHandEffect discard => Enum.IsDefined(discard.Recipient),
                SummonDiscardedMinionEffect => isDiscardedCard && card.Definition.Kind == EntityKind.Minion,
                GainArmorEffect armor => armor.Amount > 0,
                HealFriendlyHeroEffect heal => heal.Amount > 0,
                HealEffect heal => heal.Amount > 0 && (heal.Target switch
                {
                    HealTarget.Selected => target != TargetRequirement.None,
                    HealTarget.Source => hasInPlaySource,
                    HealTarget.TriggerSubject => hasCharacterSubject,
                    HealTarget.FriendlyHero or HealTarget.EnemyHero or HealTarget.FriendlyMinions or
                        HealTarget.AllMinions or HealTarget.AllFriendlyCharacters or HealTarget.RandomDamagedFriendlyCharacter => true,
                    _ => false
                }),
                SetHeroRemainingHealthEffect setHeroHealth => setHeroHealth.Health > 0 && (setHeroHealth.Target switch
                {
                    EffectTarget.Selected => target != TargetRequirement.None && !TargetsOnlyMinions(target),
                    EffectTarget.TriggerSubject => hasCharacterSubject,
                    _ => false
                }),
                ReplaceHeroPowerEffect replaceHeroPower => GetCard(replaceHeroPower.CardId).Definition.Kind == EntityKind.HeroPower,
                EquipWeaponEffect equip => GetCard(equip.CardId).Definition.Kind == EntityKind.Weapon,
                ModifyWeaponEffect modifyWeapon => (modifyWeapon.AttackDelta != 0 || modifyWeapon.DurabilityDelta != 0) &&
                    Enum.IsDefined(modifyWeapon.TargetWeapon) && IsOptionalCardIdValid(modifyWeapon.EnchantmentCardId),
                DestroyEnemyWeaponEffect => true,
                DestroyFriendlyWeaponDamageEffect flurry => IsValidDamageTarget(flurry.Target),
                ProtectFriendlyMinionsMinimumHealthEffect => true,
                CopyTriggerSpellToOtherPlayerEffect => timing is TriggerTiming.BeforeSpellResolved or TriggerTiming.AfterSpellResolved,
                EnchantStatsEffect enchant => Enum.IsDefined(enchant.Target) && Enum.IsDefined(enchant.Duration) &&
                    IsValidEnchantment(enchant.Modifier, enchant.GrantedTag, !enchant.OnAttack.IsDefaultOrEmpty) &&
                    IsOptionalCardIdValid(enchant.EnchantmentCardId) &&
                    IsValidCardFilter(enchant.CardFilter) &&
                    ((enchant.GrantedTag == GAME_TAG.DEATHRATTLE) ==
                        (!enchant.OnRemoved.IsDefaultOrEmpty && enchant.OnRemoved.Any(rule => rule?.Reason == EnchantmentRemovalReason.Died))) &&
                    (enchant.Target != StatTarget.Selected || target != TargetRequirement.None) &&
                    (enchant.Target != StatTarget.TriggerSubject || hasCharacterSubject) &&
                    (enchant.Target is not (StatTarget.Source or StatTarget.OtherFriendlyMinions or StatTarget.AdjacentFriendlyMinions or StatTarget.RandomOtherFriendlyMinion) || hasInPlaySource),
                ScaledEnchantStatsEffect scaled => Enum.IsDefined(scaled.Target) && Enum.IsDefined(scaled.Scale) &&
                    (scaled.AttackPerUnit > 0 || scaled.HealthPerUnit > 0) && scaled.AttackPerUnit >= 0 && scaled.HealthPerUnit >= 0 &&
                    IsOptionalCardIdValid(scaled.EnchantmentCardId) &&
                    (scaled.Scale != StatScaleSource.CardsPlayedBeforeThis || canUseCombo) &&
                    (scaled.Target != StatTarget.Selected || target != TargetRequirement.None) &&
                    (scaled.Target != StatTarget.Source || hasInPlaySource),
                DynamicStatChangeEffect dynamicStat => Enum.IsDefined(dynamicStat.Mode) &&
                    IsOptionalCardIdValid(dynamicStat.EnchantmentCardId) && (dynamicStat.Target switch
                    {
                        StatTarget.Selected => target != TargetRequirement.None,
                        StatTarget.Source => hasInPlaySource,
                        StatTarget.TriggerSubject => hasCharacterSubject,
                        _ => false
                    }),
                DrainDivineShieldsBuffEffect drain => hasInPlaySource &&
                    (drain.AttackPerShield > 0 || drain.HealthPerShield > 0) &&
                    drain.AttackPerShield >= 0 && drain.HealthPerShield >= 0 &&
                    IsOptionalCardIdValid(drain.EnchantmentCardId),
                ConsumeAdjacentMinionsEffect consume => hasInPlaySource && IsOptionalCardIdValid(consume.EnchantmentCardId),
                SilenceEffect silence => silence.AllEnemyMinions || TargetsOnlyMinions(target),
                RemoveAllStealthEffect => true,
                DestroySecretsEffect destroySecrets => Enum.IsDefined(destroySecrets.TargetPlayer),
                DestroyMinionEffect destroy => destroy.Target switch
                {
                    EffectTarget.Selected => TargetsOnlyMinions(target),
                    EffectTarget.TriggerSubject => hasCharacterSubject,
                    _ => false
                },
                DestroyAllMinionsEffect destroy => !destroy.ExcludeSource || hasInPlaySource,
                ReturnMinionEffect returned => IsHandCostDuration(returned.CostDuration) && IsOptionalCardIdValid(returned.EnchantmentCardId) &&
                    (returned.CostDelta != 0 || (returned.EnchantmentCardId is null && returned.CostDuration == EnchantmentDuration.Permanent)) &&
                    (returned.Target switch
                    {
                        EffectTarget.Selected => TargetsOnlyMinions(target),
                        EffectTarget.TriggerSubject => hasCharacterSubject,
                        _ => false
                    }),
                TransformMinionEffect transform => (transform.Self ? hasInPlaySource && card.Definition.Kind == EntityKind.Minion : TargetsOnlyMinions(target)) &&
                    GetCard(transform.CardId).Definition.Kind == EntityKind.Minion,
                RandomTransformOtherMinionEffect randomTransform => hasInPlaySource && !randomTransform.CardIds.IsDefaultOrEmpty &&
                    randomTransform.CardIds.All(id => GetCard(id).Definition.Kind == EntityKind.Minion),
                TransformMinionByCostEffect transform => TargetsOnlyMinions(target) && transform.Increase > 0,
                ResurrectFriendlyMinionEffect => true,
                ResummonMinionEffect resummon => (resummon.RemainingHealth is null or > 0) && (resummon.Target switch
                {
                    EffectTarget.Selected => TargetsOnlyMinions(target),
                    EffectTarget.TriggerSubject => timing == TriggerTiming.MinionDied || hasCharacterSubject,
                    _ => false
                }),
                FreezeEffect freeze => freeze.Target switch
                {
                    FreezeTarget.Selected => target != TargetRequirement.None,
                    FreezeTarget.SelectedAndAdjacentMinions => TargetsOnlyMinions(target),
                    FreezeTarget.TriggerSubject => hasCharacterSubject,
                    FreezeTarget.EnemyMinions => true,
                    _ => false
                },
                UnlockOverloadEffect => true,
                CounterSpellEffect => card.IsSecret && timing == TriggerTiming.SpellCounterWindow,
                RedirectSpellTargetEffect redirect => card.IsSecret && timing == TriggerTiming.SpellTargetWindow &&
                    GetCard(redirect.CardId).Definition.Kind == EntityKind.Minion,
                PreventDamageEffect => card.IsSecret && timing == TriggerTiming.FatalDamageWindow,
                SummonAttackDefenderEffect redirect => card.IsSecret && timing == TriggerTiming.AttackProposed &&
                    GetCard(redirect.CardId).Definition.Kind == EntityKind.Minion,
                RedirectAttackToRandomOtherCharacterEffect => card.IsSecret && timing == TriggerTiming.AttackProposed,
                RandomDamageEffect random => random.Amount > 0 && random.Count > 0 && Enum.IsDefined(random.Pool) && Enum.IsDefined(random.Mode),
                DestroyRandomEnemyMinionEffect destroyRandom => destroyRandom.MaximumAttack is null or >= 0,
                DestroyAllButOneMinionEffect => true,
                SummonRandomMinionEffect summon => !summon.CardIds.IsDefaultOrEmpty &&
                    summon.CardIds.All(id => GetCard(id).Definition.Kind == EntityKind.Minion),
                SummonMatchingMinionEffect summon => summon.Count > 0 && summon.CardFilter is not null &&
                    IsValidCardFilter(summon.CardFilter) && summon.CardFilter.Kind is null or EntityKind.Minion,
                ChangeManaEffect mana => mana.Amount > 0 && Enum.IsDefined(mana.Operation) && Enum.IsDefined(mana.Recipient) &&
                    (mana.OverflowCardId is null || (mana.Operation is ManaOperation.GainEmpty or ManaOperation.GainFilled &&
                        CanCreateCard(mana.OverflowCardId))),
                SummonEffect summon => Enum.IsDefined(summon.Placement) && Enum.IsDefined(summon.Recipient) &&
                    (summon.Recipient == CardRecipient.Controller || summon.Placement is SummonPlacement.Default or SummonPlacement.Rightmost) &&
                    (summon.Placement is not (SummonPlacement.LeftOfSource or SummonPlacement.RightOfSource) || hasInPlaySource) &&
                    GetCard(summon.CardId).Definition.Kind == EntityKind.Minion,
                SummonPerEnemyMinionEffect perEnemy => GetCard(perEnemy.CardId).Definition.Kind == EntityKind.Minion,
                SwapSelfWithRandomHandMinionEffect => hasInPlaySource && card.Definition.Kind == EntityKind.Minion,
                FillBoardSummonEffect fill => (!fill.AlternateSides || hasInPlaySource) &&
                    GetCard(fill.CardId).Definition.Kind == EntityKind.Minion,
                SummonCopyEffect copy => copy.Target switch
                {
                    EffectTarget.Selected => TargetsOnlyMinions(target),
                    EffectTarget.TriggerSubject => hasCharacterSubject,
                    _ => false
                },
                CopyMinionToSelfEffect => hasInPlaySource && card.Definition.Kind == EntityKind.Minion && TargetsOnlyMinions(target),
                TakeControlEffect takeControl => takeControl.Target switch
                {
                    EffectTarget.Selected => !takeControl.ReturnToSourceOpponent &&
                        card.Definition.Kind is EntityKind.Spell or EntityKind.Minion && target == TargetRequirement.EnemyMinion,
                    EffectTarget.TriggerSubject => hasCharacterSubject && !takeControl.ReadyToAttack,
                    _ => false
                },
                TakeControlRandomEnemyMinionEffect => true,
                _ => false
            };
            if (!valid)
                throw new ArgumentException($"卡牌「{card.Definition.CardId}」的效果定义无效。");
            if (effect is DamageEffect { Enchantment: { } simultaneous })
                pending.Push(simultaneous);
            if (effect is EnchantStatsEffect { OnRemoved.IsDefaultOrEmpty: false } enchantment)
                RegisterEnchantmentRemovalEffects(card, enchantment);
            if (effect is EnchantStatsEffect { OnAttack.IsDefaultOrEmpty: false } attackEnchantment)
                RegisterEnchantmentAttackEffects(card, attackEnchantment);
        }

        bool IsValidDamageTarget(DamageTarget damageTarget) => damageTarget switch
        {
            DamageTarget.Selected => target != TargetRequirement.None,
            DamageTarget.Source or DamageTarget.OtherCharacters => hasInPlaySource,
            DamageTarget.TriggerSubject => hasCharacterSubject,
            DamageTarget.EnemyHero or DamageTarget.FriendlyHero or DamageTarget.AllMinions or
                DamageTarget.EnemyMinions or DamageTarget.AllEnemies or DamageTarget.AllCharacters => true,
            _ => false
        };

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
            TriggerTiming.AfterMinionPlayed or TriggerTiming.Damage or TriggerTiming.Overkill or
            TriggerTiming.HonorableKill or TriggerTiming.Healed or TriggerTiming.Overheal or TriggerTiming.HeroAttacking or
            TriggerTiming.AttackProposed or TriggerTiming.FatalDamageWindow or TriggerTiming.SpellTargetWindow;

    private static bool IsOptionalCardIdValid(string? cardId) => cardId is null || !string.IsNullOrWhiteSpace(cardId);

    private static bool IsHandCostDuration(EnchantmentDuration duration) =>
        duration is EnchantmentDuration.Permanent or EnchantmentDuration.CurrentTurn;
}
