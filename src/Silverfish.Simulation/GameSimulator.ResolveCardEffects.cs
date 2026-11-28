using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private void SummonJadeGolem(Stack<Work> work, EffectContext context, SummonJadeGolemEffect effect)
        {
            var player = context.Controller;
            if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                return;

            var count = checked(_state.Players[player].JadeGolemsSummoned + 1);
            if (Summon(player, "CFM_712_t01", ResolveSummonPosition(context, SummonPlacement.Default)) is not EntityId summoned)
                return;

            _state = _state.UpdateResources(_state.Players[player] with { JadeGolemsSummoned = count });
            var entity = _state.Entities[summoned];
            var character = entity.Character! with
            {
                Attack = count,
                MaximumHealth = count,
                BaseAttack = count,
                BaseMaximumHealth = count
            };
            var enchantments = effect.Taunt
                ? entity.Enchantments.Add(new StatEnchantment(context.Source, null) { GrantedTag = GAME_TAG.TAUNT })
                : entity.Enchantments;
            var updated = RecalculateStats(entity with { Character = character, Enchantments = enchantments },
                entity.AuraGrants, !entity.Silenced && _rules.Card(entity).AttackEqualsHealth);
            _state = _state.UpdateEntity(updated);
            if (effect.Taunt)
                _events.Add(new(SimulationEventKind.EnchantmentApplied, player, context.Source, summoned));
            PushSummonTriggers(work, player, summoned);
        }

        private void Joust(Stack<Work> work, EffectContext context, JoustEffect effect)
        {
            var opponent = _state.OpponentOf(context.Controller);
            var friendly = ChooseDeckCard(_state.Players[context.Controller].In(GameZone.Deck), null, EntityKind.Minion);
            var enemy = ChooseDeckCard(_state.Players[opponent].In(GameZone.Deck), null, EntityKind.Minion);
            var friendlyCost = Reveal(friendly, context.Controller);
            var enemyCost = Reveal(enemy, opponent);
            var won = friendlyCost > enemyCost;
            var draw = won && effect.DrawRevealedMinion;
            if (friendly is not null && !draw)
                ShuffleDeck(context.Controller, context.Source);
            if (enemy is not null)
                ShuffleDeck(opponent, context.Source);
            if (draw)
                work.Push(new DrawCards(context.Controller, 1, SpecificCard: friendly));
            PushEffects(work, context, won ? effect.OnWin : effect.Otherwise);

            int Reveal(EntityId? revealed, PlayerId player)
            {
                if (revealed is not EntityId id)
                    return -1;
                var cost = _rules.Card(_state.Entities[id]).Definition.PrintedCost;
                _events.Add(new(SimulationEventKind.CardRevealed, player, context.Source, id, cost));
                return cost;
            }
        }

        private void ReturnSourceCard(EffectContext context)
        {
            var source = _state.Entities[context.Source];
            if (_state.LocationOf(source.Id).Zone != GameZone.Graveyard ||
                _state.Players[context.Controller].In(GameZone.Hand).Length >= HandLimit)
                return;

            _state = _state.Move(source.Id, new(context.Controller, GameZone.Hand));
            _state = _state.UpdateEntity(_state.Entities[source.Id] with { Character = null, Weapon = null, Silenced = false });
            _events.Add(new(SimulationEventKind.ReturnedToHand, context.Controller, context.Source, source.Id));
        }

        private void SummonMatchingCost(Stack<Work> work, EffectContext context, SummonMatchingCostEffect effect)
        {
            var subject = effect.UseSource ? context.Source : context.Subject!.Value;
            var cost = effect.UsePlayedCost && context.EventAmount is int playedCost
                ? playedCost : _rules.Card(_state.Entities[subject]).Definition.PrintedCost;
            var afterSummon = effect.DurabilityLossOnSummon == 0 ? null :
                new ModifyWeaponEffect(DurabilityDelta: -effect.DurabilityLossOnSummon) { UseSource = true };
            SummonMatchingMinion(work, context, new(new CardFilter
            {
                MinimumPrintedCost = cost, MaximumPrintedCost = cost
            }), afterSummon);
        }

        private void ShufflePlayedCard(EffectContext context, ShufflePlayedCardEffect effect)
        {
            var recipient = ResolveRecipient(effect.Recipient, context.Controller);
            _state = _state.Move(context.Source, new(recipient, GameZone.Deck));
            ShuffleDeck(recipient, context.Source);
        }

        private IEnumerable<Activate> RepeatedDeathrattles(GameEntity minion)
        {
            var player = minion.Controller;
            var times = _rules.BoardMinions(_state, player).Any(id =>
                !_state.Entities[id].Silenced && IsLivingCharacter(id) && _rules.Card(_state.Entities[id]).DoubleFriendlyDeathrattles) ? 2 : 1;
            var position = _rules.BoardSlots(_state, player).TakeWhile(slot => slot != minion.Id).Count();
            return DeathrattleActivations(minion, position, times);
        }

        private IEnumerable<Activate> DeathrattleActivations(GameEntity entity, int? position, int times)
        {
            if (entity.Silenced)
                yield break;
            foreach (var rule in DeathrattleReferences(entity, null))
            {
                var effects = rule.IsEnchantment ? _rules._enchantmentRemovalEffects.GetValueOrDefault((rule.CardId, EnchantmentRemovalReason.Died)) :
                    _rules.GetCard(rule.CardId).Deathrattle;
                if (!effects.IsDefaultOrEmpty)
                    for (var pass = 0; pass < times; pass++)
                        yield return new(new(entity.Id, entity.Controller, Subject: rule.IsEnchantment ? entity.Id : null,
                            DeathPosition: position, CapturedMinion: rule.CapturedMinion), entity.PlayOrder!.Value, effects);
            }
        }

        private IEnumerable<DeathrattleReference> DeathrattleReferences(GameEntity entity, string? attachment)
        {
            if (entity.Silenced)
                yield break;
            if (!_rules.Card(entity).Deathrattle.IsEmpty)
                yield return new(entity.CardId, false, attachment ?? entity.CardId);
            foreach (var layer in entity.Enchantments)
                if (layer.GrantedTag == GAME_TAG.DEATHRATTLE && layer.CardId is { } enchantment)
                    yield return new(enchantment, true, attachment ?? enchantment) { CapturedMinion = layer.CapturedMinion };
            foreach (var copied in entity.CopiedDeathrattles ?? [])
                yield return attachment is null ? copied : copied with { AttachmentCardId = attachment };
        }

        private void CopyDeathrattle(EffectContext context, CopyDeathrattleEffect effect)
        {
            if (!IsLivingCharacter(context.Source) || context.Selected is not EntityId selected || !IsLivingCharacter(selected))
                return;
            var copied = DeathrattleReferences(_state.Entities[selected], effect.EnchantmentCardId).ToImmutableArray();
            if (copied.IsEmpty)
                return;
            var source = _state.Entities[context.Source];
            _state = _state.UpdateEntity(source with { CopiedDeathrattles = [.. source.CopiedDeathrattles ?? [], .. copied] });
            _events.Add(new(SimulationEventKind.EnchantmentApplied, source.Controller, source.Id, source.Id)
            {
                EnchantmentCardId = effect.EnchantmentCardId
            });
        }

        private void CopySpellOntoSelf(EffectContext context)
        {
            if (!IsLivingCharacter(context.Source) || context.Subject is not EntityId spellId)
                return;
            var spell = _rules.Card(_state.Entities[spellId]);
            var copy = new EntityId(_state.NextEntity);
            _state = _state.AddToSetAside(new(copy, spell.Definition.CardId, context.Controller, context.Controller));
            _state = _state.Move(copy, new(context.Controller, GameZone.Play));
            _events.Add(new(SimulationEventKind.SpellCastAutomatically, context.Controller, copy, context.Source));
            var effects = context.PlayedBranch is { } choice ? PlayChoiceAtResolution(spell, choice).Effects : spell.OnPlay;
            ResolveEffects(new(copy, context.Controller, context.Source), effects);
            if (_state.LocationOf(copy).Zone == GameZone.Play)
                _state = _state.Move(copy, new(context.Controller, GameZone.Graveyard));
        }

        private void TransformHandCard(EffectContext context, TransformHandCardEffect effect)
        {
            if (_state.LocationOf(context.Source).Zone != GameZone.Hand)
                return;
            var pool = _rules.GeneratedCardCandidates(effect.CardFilter);
            if (pool.IsEmpty)
                return;
            var entity = _state.Entities[context.Source];
            _state = _state.UpdateEntity(entity with
            {
                HandTransformRuleCardId = entity.HandTransformRuleCardId ?? entity.CardId,
                CardId = pool[ChooseIndex(pool.Length)], Enchantments = [], CostModifiers = [], CostTypeModifiers = null
            });
            _events.Add(new(SimulationEventKind.Transformed, entity.Controller, entity.Id, entity.Id)
            {
                EnchantmentCardId = effect.EnchantmentCardId
            });
        }

        private void ReplaceClassCards(EffectContext context, ReplaceClassCardsEffect effect)
        {
            var choice = effect.Options[ChooseIndex(effect.Options.Length)];
            var filter = new CardFilter { CardClass = choice.CardClass };
            var pool = _rules.GeneratedCardCandidates(filter);
            if (pool.IsEmpty)
                return;
            var original = new CardFilter { CardClass = effect.OriginalClass };
            var player = _state.Players[context.Controller];
            ReplaceHeroPower(player.Id, context.Source, choice.HeroPowerCardId);
            foreach (var id in player.In(GameZone.Hand).Concat(player.In(GameZone.Deck)))
            {
                var entity = _state.Entities[id];
                if (!MatchesCardFilter(_rules.Card(entity), original))
                    continue;
                _state = _state.UpdateEntity(entity with
                {
                    CardId = pool[ChooseIndex(pool.Length)], Enchantments = [], HandTransformRuleCardId = null,
                    CostModifiers = [CreateCostModifier(context.Source, effect.CostDelta, effect.EnchantmentCardId, EnchantmentDuration.Permanent)],
                    CostTypeModifiers = null
                });
                _events.Add(new(SimulationEventKind.Transformed, player.Id, context.Source, id));
            }
        }

        private void ResurrectMatchingMinions(Stack<Work> work, EffectContext context, ResurrectMatchingMinionsEffect effect)
        {
            var player = context.Controller;
            var candidates = _state.MinionDeaths.Where(death => (effect.IncludeOpponent || death.Controller == player) &&
                MatchesCardFilter(_rules.GetCard(death.CardId), effect.CardFilter)).ToList();
            var summoned = new List<EntityId>();
            while (summoned.Count < effect.Count && candidates.Count > 0 && _rules.BoardSlotCount(_state, player) < BoardLimit)
            {
                var index = ChooseIndex(candidates.Count);
                var death = candidates[index];
                candidates.RemoveAt(index);
                if (Summon(player, death.CardId, position: null) is EntityId id)
                {
                    summoned.Add(id);
                    _events.Add(new(SimulationEventKind.Resurrected, player, death.Entity, id));
                }
            }
            for (var index = summoned.Count - 1; index >= 0; index--)
                PushSummonTriggers(work, player, summoned[index]);
        }

        private int PermanentCardAttack(PlayerId player, string cardId) =>
            checked(_rules.GetCard(cardId).Definition.PrintedAttack + (_state.Players[player].PermanentCardGrowth ?? [])
                .Where(growth => growth.CardId == cardId).Sum(growth => growth.Enchantment.Modifier?.Attack ?? 0));

        private void EmpowerCard(EffectContext context, EmpowerCardEffect effect)
        {
            var player = _state.Players[context.Controller];
            var modifier = effect.Modifier;
            if (effect.FromSelectedStats)
            {
                if (context.Selected is not EntityId target || !IsLivingCharacter(target))
                    return;
                var minion = _state.Entities[target];
                modifier = new(StatModifierMode.Add, _rules.EffectiveAttack(_state, minion), minion.Character!.Health);
                _pendingDestruction.Add(target);
            }

            var enchantment = new StatEnchantment(context.Source, modifier)
            {
                CardId = effect.EnchantmentCardId, GrantedTag = effect.GrantedTag
            };
            _state = _state.UpdateResources(player with
            {
                PermanentCardGrowth = [.. player.PermanentCardGrowth ?? [], new(effect.CardId, enchantment)]
            });
            foreach (var id in _rules.BoardMinions(_state, player.Id).Where(id =>
                _state.Entities[id].CardId == effect.CardId && IsLivingCharacter(id)).ToArray())
                EnchantStats(context with { Selected = id }, new(StatTarget.Selected, modifier)
                {
                    EnchantmentCardId = enchantment.CardId, GrantedTag = enchantment.GrantedTag
                });

            if (effect.ReshuffleIfDead && player.In(GameZone.Graveyard)
                .Where(id => _state.Entities[id].CardId == effect.CardId).Select(id => (EntityId?)id).LastOrDefault() is EntityId dead)
            {
                _state = _state.Move(dead, new(player.Id, GameZone.Deck));
                _state = _state.UpdateEntity(_state.Entities[dead] with { Character = null, Silenced = false });
                _events.Add(new(SimulationEventKind.CardAddedToDeck, player.Id, context.Source, dead));
                ShuffleDeck(player.Id, context.Source);
            }
        }

        private void ApplyPermanentCardGrowth(PlayerId player, EntityId id)
        {
            if (_state.Players[player].PermanentCardGrowth is not { IsEmpty: false } growth)
                return;

            var entity = _state.Entities[id];
            var additions = growth.Where(layer => layer.CardId == entity.CardId)
                .Select(layer => layer.Enchantment).ToImmutableArray();
            if (!additions.IsEmpty)
                _state = _state.UpdateEntity(RecalculateStats(entity with
                {
                    Enchantments = entity.Enchantments.AddRange(additions)
                }, entity.AuraGrants));
        }

        private static bool HasPrintedBattlecry(SimulationCard card) =>
            PlayReqTags.Has(card.RequirementTagMask, (int)GAME_TAG.BATTLECRY);

        private bool ActiveMinionRule(PlayerId player, Func<CardRules, bool> predicate, EntityId? exclude = null) =>
            _rules.BoardMinions(_state, player).Any(id => id != exclude && IsLivingCharacter(id) &&
                !_state.Entities[id].Silenced && predicate(_rules.Card(_state.Entities[id])));

        private void CountSpellCast(PlayerId player)
        {
            var resources = _state.Players[player];
            _state = _state.UpdateResources(resources with { SpellsCastThisGame = checked(resources.SpellsCastThisGame + 1) });
        }

        private void ResolveSpellCastRewards(PlayerId player, EntityId source)
        {
            foreach (var reward in _state.Players[player].SpellCastRewardsThisTurn ?? [])
                if (reward.Source != source)
                    CreateRandomMatchingCard(new(reward.Source, player), new(new CardFilter { CardClass = reward.CardClass }));
        }

        private void RecoverDiscardedCards(EffectContext context, ReturnDiscardedCardsEffect effect)
        {
            var player = _state.Players[context.Controller];
            foreach (var cardId in player.DiscardedCardsThisGame ?? [])
            {
                var id = new EntityId(_state.NextEntity);
                if (!TryCreateCard(cardId, player.Id, context.Source, CardPlacement.Hand))
                    break;
                var entity = _state.Entities[id];
                _state = _state.UpdateEntity(entity with
                {
                    CostTypeModifiers = [new(context.Source, TAG_CARD_ALTERNATE_COST.HEALTH) { CardId = effect.EnchantmentCardId }]
                });
            }
        }

        private void ScheduleTurnEndSummon(EffectContext context, SummonAtTurnEndEffect effect)
        {
            var player = _state.Players[context.Controller];
            _state = _state.UpdateResources(player with { EndOfTurnSummons = [.. player.EndOfTurnSummons ?? [], effect.CardId] });
            _events.Add(new(SimulationEventKind.EnchantmentApplied, player.Id, context.Source, player.Hero)
            {
                EnchantmentCardId = effect.EnchantmentCardId
            });
        }

        private void ResolveEndOfTurnSummons(PlayerId player)
        {
            if (_state.Players[player].EndOfTurnSummons is not { IsEmpty: false } summons)
                return;
            _state = _state.UpdateResources(_state.Players[player] with { EndOfTurnSummons = null });
            ResolveEffects(new(_state.Players[player].Hero, player), [.. summons.Select(id => new SummonEffect(id))]);
        }

        private void SpendRemainingMana(Stack<Work> work, EffectContext context, SpendRemainingManaEffect effect)
        {
            var spent = _state.Players[context.Controller].Mana;
            if (spent == 0 && effect.Reward is not (ManaSpendReward.SummonMatchingCost or ManaSpendReward.DamageSelected))
                return;
            PayMana(context.Controller, spent);
            switch (effect.Reward)
            {
                case ManaSpendReward.BuffSource:
                    EnchantStats(context, new(StatTarget.Source, new(StatModifierMode.Add, spent, spent))
                        { EnchantmentCardId = effect.EnchantmentCardId });
                    break;
                case ManaSpendReward.DamageSelected:
                    ApplyDamageTarget(work, context, DamageTarget.Selected, ModifiedEffectAmount(context, spent, damage: true));
                    break;
                case ManaSpendReward.HealSelected:
                    HealCharacters(work, context, new(HealTarget.Selected,
                        ModifiedEffectAmount(context, checked(2 * spent), damage: false)));
                    break;
                case ManaSpendReward.SummonMatchingCost:
                    SummonMatchingMinion(work, context, new(new CardFilter { MinimumPrintedCost = spent, MaximumPrintedCost = spent }));
                    break;
                case ManaSpendReward.SummonTokens:
                    PushEffects(work, context, [.. Enumerable.Repeat<CardEffect>(new SummonEffect(effect.TokenCardId!),
                        Math.Min(spent, BoardLimit))]);
                    break;
            }
        }

        private void CopyOrSwapMinionStats(EffectContext context, CopyOrSwapMinionStatsEffect effect)
        {
            if (!IsLivingCharacter(context.Source) || context.Selected is not EntityId target || !IsLivingCharacter(target))
                return;
            var source = _state.Entities[context.Source].Character!;
            var selected = _state.Entities[target].Character!;
            EnchantStats(context, new(StatTarget.Source, new(StatModifierMode.Set, selected.Attack, selected.Health))
                { EnchantmentCardId = effect.EnchantmentCardId });
            if (effect.Swap)
                EnchantStats(context, new(StatTarget.Selected, new(StatModifierMode.Set, source.Attack, source.Health))
                    { EnchantmentCardId = effect.EnchantmentCardId });
        }

        private void ReplaceHandAndDeck(EffectContext context, CardFilter filter)
        {
            var pool = _rules.GeneratedCardCandidates(filter);
            if (pool.IsEmpty)
                return;
            var player = _state.Players[context.Controller];
            foreach (var id in player.In(GameZone.Hand).Concat(player.In(GameZone.Deck)))
            {
                var entity = _state.Entities[id];
                _state = _state.UpdateEntity(entity with
                {
                    CardId = pool[ChooseIndex(pool.Length)], Enchantments = [], CostModifiers = [], CostTypeModifiers = null
                });
                _events.Add(new(SimulationEventKind.Transformed, player.Id, context.Source, id));
            }
        }

        private void MergeMatchingMinions(Stack<Work> work, EffectContext context, MergeMatchingMinionsEffect effect)
        {
            var cardId = _state.Entities[context.Source].CardId;
            var matches = _rules.BoardMinions(_state, context.Controller)
                .Where(id => IsLivingCharacter(id) && _state.Entities[id].CardId == cardId).Take(effect.Count).ToArray();
            if (matches.Length != effect.Count)
                return;
            var position = _rules.BoardSlots(_state, context.Controller).TakeWhile(id => id != matches[0]).Count();
            foreach (var id in matches)
                _state = _state.Move(id, new(context.Controller, GameZone.Removed));
            if (Summon(context.Controller, effect.ResultCardId, position) is EntityId summoned)
                PushSummonTriggers(work, context.Controller, summoned);
        }
    }
}
