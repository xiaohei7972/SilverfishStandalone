using System.Collections.Concurrent;
using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private const int PlayChoiceOfferCount = 3;
    private static readonly uint NeutralClass = CardMetadata.ClassBit(TAG_CLASS.NEUTRAL);
    private readonly ConcurrentDictionary<(uint Classes, bool LegendaryOnly), ImmutableArray<string>> _discoverPools = new();
    private readonly ConcurrentDictionary<(uint Classes, CardFilter? Filter), ImmutableArray<string>> _discoverCardPools = new();

    private bool IsValidPlayChoice(SimulationCard card, PlayChoiceRule rule) => rule switch
    {
        DiscoverMinionChoice { SummonCopies: > 0 and <= BoardLimit } => true,
        DiscoverCardChoice discover => !discover.Options.IsDefault && IsValidCardFilter(discover.Filter) &&
            (discover.Condition is null || IsValidCondition(discover.Condition, TargetRequirement.None, false,
                card.Definition.Kind == EntityKind.Minion)) &&
            (discover.SourceDeck is not ConditionPlayer sourceDeck || Enum.IsDefined(sourceDeck)) &&
            (discover.Modifier is null || IsValidModifier(discover.Modifier) && discover.Filter?.Kind == EntityKind.Minion) &&
            (!discover.HealHeroByChosenCost || !discover.ReplaceHeroPower) &&
            IsOptionalCardIdValid(discover.EnchantmentCardId) &&
            (discover.ReplaceHeroPower ? !discover.Options.IsEmpty && discover.Filter is null &&
                discover.CostDelta == 0 && discover.Modifier is null : true) &&
            discover.Options.Distinct(StringComparer.Ordinal).Count() == discover.Options.Length &&
            discover.Options.All(id => discover.ReplaceHeroPower
                ? GetCard(id).Definition.Kind == EntityKind.HeroPower : CanCreateCard(id)) &&
            (discover.Then is null || IsValidPlayChoice(card, discover.Then)),
        AdaptChoice adapt => IsValidAdaptRecipient(card, adapt) && adapt.Times > 0 && !adapt.Options.IsDefaultOrEmpty &&
            adapt.Options.Distinct(StringComparer.Ordinal).Count() == adapt.Options.Length &&
            adapt.Options.All(id => !string.IsNullOrWhiteSpace(id) && _cards.TryGetValue(id, out var option) &&
                option.IsPlayChoiceOption && !option.EffectPending),
        _ => false
    };

    private static bool IsValidAdaptRecipient(SimulationCard card, AdaptChoice adapt) => adapt.Recipient switch
    {
        AdaptRecipient.Source => card.Definition.Kind == EntityKind.Minion && adapt.RecipientFilter is null,
        AdaptRecipient.Selected => adapt.RecipientFilter is null && (card.Choices.IsEmpty
            ? TargetsOnlyMinions(EffectCharacterTarget(card.Target, BranchRequirements(card)))
            : card.Choices.All(choice => TargetsOnlyMinions(EffectCharacterTarget(choice.Target, BranchRequirements(card, choice))))),
        AdaptRecipient.FriendlyMinions => IsValidCardFilter(adapt.RecipientFilter) &&
            adapt.RecipientFilter?.Kind is null or EntityKind.Minion,
        _ => false
    };

    private bool IsAdaptRecipient(GameState state, PlayerId player, EntityId id, AdaptChoice adapt) =>
        state.Entities.TryGetValue(id, out var entity) && entity.Character is { Health: > 0 } &&
        state.LocationOf(id).Zone == GameZone.Play && Card(entity).Definition.Kind == EntityKind.Minion &&
        (adapt.Recipient == AdaptRecipient.Selected || entity.Controller == player) &&
        (adapt.RecipientFilter is null || MatchesCardFilter(Card(entity).Definition, adapt.RecipientFilter));

    private bool IsValidChoiceContinuation(GameState state, PendingPlayChoice choice, PlayChoiceRule rule)
    {
        if (choice.SelectedTarget is EntityId target && !state.Entities.ContainsKey(target) || choice.PlayedCardCost is < 0)
            return false;
        if (rule is DiscoverCardChoice discover)
            return choice.Recipients is null && choice.RemainingSelections is null or 1 or 2 &&
                (choice.RemainingSelections is null || HasDoubleBattlecrySource(state, choice.Source)) &&
                (choice.DiscoverRound is null or 0 || choice.DiscoverRound == 1 && discover.Then is not null);
        if (rule is not AdaptChoice adapt)
            return choice.Recipients is null && choice.RemainingSelections is null;

        // Older single-round Adapt checkpoints implicitly used the source.
        if (choice.Recipients is null || choice.RemainingSelections is null)
            return choice.Recipients is null && choice.RemainingSelections is null &&
                adapt is { Recipient: AdaptRecipient.Source, Times: 1 } &&
                IsAdaptRecipient(state, state.ActivePlayer, choice.Source, adapt);

        var recipients = choice.Recipients.Value;
        return choice.RemainingSelections > 0 && choice.RemainingSelections <= adapt.Times &&
            !recipients.IsDefaultOrEmpty && recipients.Distinct().Count() == recipients.Length &&
            recipients.All(id => IsAdaptRecipient(state, state.ActivePlayer, id, adapt)) &&
            (adapt.Recipient switch
            {
                AdaptRecipient.Source => recipients.Length == 1 && recipients[0] == choice.Source,
                AdaptRecipient.Selected => recipients.Length == 1,
                _ => true
            });
    }

    private void ValidatePlayChoice(GameState state, PendingPlayChoice choice)
    {
        if (ChoiceRule(Card(state.Entities[choice.Source]), choice.PlayBranch) is not { } rule || choice.Options.Length > PlayChoiceOfferCount ||
            !IsValidChoiceContinuation(state, choice, rule))
            throw new ArgumentException("存档的待决选择缺少匹配的单卡规则。", nameof(choice));

        var eligible = PlayChoiceCandidates(state, state.ActivePlayer, choice.Source, DiscoverRound(rule, choice)).ToHashSet(StringComparer.Ordinal);
        var offered = choice.Options.Select(id => state.Entities[id].CardId).ToArray();
        if (offered.Length != Math.Min(PlayChoiceOfferCount, eligible.Count) ||
            offered.Distinct(StringComparer.Ordinal).Count() != offered.Length || offered.Any(id => !eligible.Contains(id)))
            throw new ArgumentException("存档中的候选包含重复卡牌或不符合单卡选择条件。", nameof(choice));
    }

    private bool HasDoubleBattlecrySource(GameState state, EntityId source) =>
        PlayReqTags.Has(Card(state.Entities[source]).RequirementTagMask, (int)GAME_TAG.BATTLECRY);

    private static PlayChoiceRule DiscoverRound(PlayChoiceRule rule, PendingPlayChoice choice) =>
        rule is DiscoverCardChoice { Then: { } then } && choice.DiscoverRound == 1 ? then : rule;

    private static PlayChoiceRule? ChoiceRule(SimulationCard card, string? branch) => branch switch
    {
        null => card.ChooseAfterPlay,
        "both" => card.CombinedChoice?.ChooseAfterPlay,
        _ => card.Choices.FirstOrDefault(choice => choice.Id == branch)?.ChooseAfterPlay
    };

    private ImmutableArray<string> PlayChoiceCandidates(GameState state, PlayerId player, EntityId source, PlayChoiceRule rule) => rule switch
    {
        DiscoverMinionChoice discover => DiscoverMinionCandidates(state, player, discover),
        DiscoverCardChoice { Options.IsEmpty: true } discover => DiscoverCardCandidates(state, player, discover)
            .Where(id => id != state.Entities[source].CardId).ToImmutableArray(),
        DiscoverCardChoice { ReplaceHeroPower: true } discover => discover.Options.Where(id =>
            !state.Players[player].In(GameZone.HeroPower).Any(power => state.Entities[power].CardId == id)).ToImmutableArray(),
        DiscoverCardChoice discover => discover.Options,
        AdaptChoice adapt => adapt.Options,
        _ => throw new InvalidOperationException("无法识别待决选择规则。")
    };

    private ImmutableArray<string> DiscoverCardCandidates(GameState state, PlayerId player, DiscoverCardChoice discover)
    {
        if (discover.SourceDeck is ConditionPlayer sourceDeck)
        {
            var deckPlayer = sourceDeck == ConditionPlayer.Friendly ? player : state.OpponentOf(player);
            return [.. state.Players[deckPlayer].In(GameZone.Deck)
                .Select(id => state.Entities[id].CardId)
                .Distinct(StringComparer.Ordinal)
                .Where(id =>
                {
                    var card = GetCard(id);
                    return discover.Filter is null || MatchesCardFilter(card, discover.Filter);
                })
                .Order(StringComparer.Ordinal)];
        }

        var classes = Card(state.Entities[state.Players[player].Hero]).Definition.ClassMask | NeutralClass;
        return GeneratedCardCandidates(discover.Filter, classes);
    }

    private ImmutableArray<string> GeneratedCardCandidates(CardFilter? filter, uint classes = 0) =>
        _discoverCardPools.GetOrAdd((classes, filter), pool =>
        {
            var ids = pool.Filter?.Kind == EntityKind.Minion && RandomMinionPool is { } minions
                ? minions.AsEnumerable() : CompiledCardData.CollectibleCardIds;
            return [.. ids.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).Where(id =>
            {
                if (id.StartsWith("CORE_", StringComparison.Ordinal) && CompiledCardData.CollectibleCardIds.Contains(id[5..]))
                    return false;
                var card = GetCard(id);
                return (pool.Classes == 0 || card.Definition.ClassMask == 0 || (card.Definition.ClassMask & pool.Classes) != 0) &&
                    (pool.Filter is null || MatchesCardFilter(card, pool.Filter));
            })];
        });

    private ImmutableArray<string> DiscoverMinionCandidates(GameState state, PlayerId player, DiscoverMinionChoice discover)
    {
        var classes = Card(state.Entities[state.Players[player].Hero]).Definition.ClassMask | NeutralClass;
        return _discoverPools.GetOrAdd((classes, discover.LegendaryOnly), pool =>
            [.. _randomMinionCosts.Value.Values.SelectMany(bucket => bucket).Order(StringComparer.Ordinal).Where(id =>
            {
                var card = GetCard(id);
                return (card.Definition.ClassMask == 0 || (card.Definition.ClassMask & pool.Classes) != 0) &&
                    (!pool.LegendaryOnly || PlayReqTags.Has(card.RequirementTagMask, (int)GAME_TAG.RARITY));
            })]);
    }

    private sealed partial class Resolution
    {
        private bool OfferPlayChoice(PlayerId player, EntityId source, EntityId? selected, SimulationCard card,
            ImmutableArray<Listener> listeners, string? branch = null, int repeats = 1, int? playedCost = null)
        {
            if (ChoiceRule(card, branch) is not { } rule || !CanOfferChoice(player, source, card) ||
                rule is DiscoverCardChoice { Condition: { } condition } && !MeetsCondition(new(source, player, selected), condition))
                return false;

            var choice = new PendingPlayChoice(source, [], AfterPlayListeners(card, listeners))
            {
                PlayBranch = branch, SelectedTarget = selected, PlayedCardCost = playedCost
            };
            if (rule is DiscoverCardChoice && repeats > 1)
                choice = choice with { RemainingSelections = repeats };
            if (rule is AdaptChoice adapt)
            {
                IEnumerable<EntityId> candidates = adapt.Recipient switch
                {
                    AdaptRecipient.Source => [source],
                    AdaptRecipient.Selected => selected is EntityId target ? [target] : [],
                    _ => _state.Players[player].In(GameZone.Play)
                };
                var recipients = candidates.Where(id => _rules.IsAdaptRecipient(_state, player, id, adapt)).ToImmutableArray();
                if (recipients.IsEmpty)
                    return false;
                choice = choice with { Recipients = recipients, RemainingSelections = adapt.Times };
            }
            return OfferOptions(player, choice, rule);
        }

        private bool CanOfferChoice(PlayerId player, EntityId source, SimulationCard card) =>
            _state.LocationOf(source) == new EntityLocation(player, GameZone.Play) &&
            _state.Entities[source].CardId == card.Definition.CardId &&
            _state.Players.Values.All(side => IsLivingCharacter(side.Hero));

        private bool OfferOptions(PlayerId player, PendingPlayChoice choice, PlayChoiceRule rule)
        {
            var pool = _rules.PlayChoiceCandidates(_state, player, choice.Source, rule).ToList();
            if (pool.Count == 0)
                return false;

            var offered = ImmutableArray.CreateBuilder<EntityId>();
            for (var number = 0; number < PlayChoiceOfferCount && pool.Count > 0; number++)
            {
                var index = ChooseIndex(pool.Count);
                var id = new EntityId(_state.NextEntity);
                _state = _state.AddToSetAside(new(id, pool[index], player, player));
                offered.Add(id);
                pool.RemoveAt(index);
                _events.Add(new(SimulationEventKind.PlayChoiceOffered, player, choice.Source, id));
            }

            _state = _state.WithPendingPlayChoice(choice with { Options = offered.ToImmutable() });
            return true;
        }

        private ImmutableArray<PlayChoiceListener> AfterPlayListeners(SimulationCard card, ImmutableArray<Listener> listeners)
        {
            var after = card.Definition.Kind == EntityKind.Minion ? TriggerTiming.AfterMinionPlayed : TriggerTiming.AfterSpellResolved;
            return listeners.Where(listener =>
                (listener.Trigger.Timing == after ||
                    listener.Trigger.Timing == TriggerTiming.AfterCardPlayed ||
                    after == TriggerTiming.AfterMinionPlayed && listener.Trigger.Timing == TriggerTiming.AfterMinionSummoned) &&
                IsCurrent(listener)).Select(ChoiceListener).Distinct().ToImmutableArray();
        }

        private void ChoosePlayOption(EntityId selected)
        {
            var choice = _state.PendingPlayChoice!;
            var player = _state.ActivePlayer;
            var card = _rules.Card(_state.Entities[choice.Source]);
            var selectedCard = _state.Entities[selected].CardId;
            var snapshot = choice.AfterPlayListeners.ToHashSet();
            var listeners = CaptureListeners().Where(listener => snapshot.Contains(ChoiceListener(listener))).ToImmutableArray();

            _events.Add(new(SimulationEventKind.PlayChoiceSelected, player, choice.Source, selected));
            _state = _state.WithPendingPlayChoice(null);
            foreach (var option in choice.Options)
                _state = _state.Move(option, new(player, GameZone.Removed));

            var rootRule = ChoiceRule(card, choice.PlayBranch)!;
            var rule = DiscoverRound(rootRule, choice);
            if (rule is DiscoverMinionChoice discover)
            {
                var effects = Enumerable.Repeat<CardEffect>(new SummonEffect(selectedCard), discover.SummonCopies).ToImmutableArray();
                ResolveEffects(new(choice.Source, player), effects);
            }
            else if (rule is DiscoverCardChoice discoverCard)
            {
                ResolveDiscoveredCard(player, choice.Source, selectedCard, discoverCard);
                var more = discoverCard.Then is not null && choice.DiscoverRound is null;
                var repeat = choice.RemainingSelections is > 1;
                if ((more || repeat) && CanOfferChoice(player, choice.Source, card) &&
                    OfferOptions(player, choice with
                    {
                        DiscoverRound = more ? 1 : null,
                        RemainingSelections = more ? choice.RemainingSelections : choice.RemainingSelections - 1
                    }, more ? discoverCard.Then! : rootRule))
                    return;
            }
            else if (rule is AdaptChoice adapt && ResolveAdaptChoice(player, choice, card, adapt, selectedCard, listeners))
                return;
            CompletePlayedCard(player, choice.Source, card, listeners, choice.SelectedTarget, choice.PlayBranch, choice.PlayedCardCost);
        }

        private void ResolveDiscoveredCard(PlayerId player, EntityId source, string cardId, DiscoverCardChoice rule)
        {
            if (rule.ReplaceHeroPower)
            {
                ReplaceHeroPower(player, source, cardId);
                return;
            }

            var id = new EntityId(_state.NextEntity);
            if (TryCreateCard(cardId, player, source, CardPlacement.Hand))
            {
                var added = _state.Entities[id];
                if (rule.CostDelta != 0)
                    added = added with { CostModifiers = [CreateCostModifier(source, rule.CostDelta, rule.EnchantmentCardId, EnchantmentDuration.Permanent)] };
                if (rule.Modifier is { } stats)
                    added = added with { Enchantments = [new(source, stats) { CardId = rule.EnchantmentCardId }] };
                _state = _state.UpdateEntity(added);
            }
            if (rule.HealHeroByChosenCost && _rules.GetCard(cardId).Definition.PrintedCost is > 0 and var heal)
                ResolveEffects(new(source, player), [new HealFriendlyHeroEffect(heal)]);
        }

        private bool ResolveAdaptChoice(PlayerId player, PendingPlayChoice choice, SimulationCard card,
            AdaptChoice adapt, string selectedCard, ImmutableArray<Listener> listeners)
        {
            var recipients = choice.Recipients ?? [choice.Source];
            var effects = _rules.GetCard(selectedCard).OnPlay;
            foreach (var recipient in recipients)
                if (_rules.IsAdaptRecipient(_state, player, recipient, adapt))
                    ResolveEffects(new(choice.Source, player, Selected: recipient), effects);

            if (choice.RemainingSelections is not > 1)
                return false;

            ResolveDeaths();
            var surviving = recipients.Where(id => _rules.IsAdaptRecipient(_state, player, id, adapt)).ToImmutableArray();
            return !surviving.IsEmpty && CanOfferChoice(player, choice.Source, card) &&
                OfferOptions(player, choice with
                {
                    Recipients = surviving,
                    RemainingSelections = choice.RemainingSelections - 1,
                    AfterPlayListeners = AfterPlayListeners(card, listeners)
                }, adapt);
        }

        private static PlayChoiceListener ChoiceListener(Listener listener) =>
            new(listener.Id.Entity, listener.CardId, listener.Controller, listener.PlayOrder, listener.Zone);
    }
}
