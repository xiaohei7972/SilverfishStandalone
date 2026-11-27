using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using static Silverfish.Domain.Client.ErrorType;

namespace Silverfish.Simulation;

public enum PlayTargetMode { None, Required, IfAvailable }
public sealed record PlayBranch(string? Choice, PlayTargetMode TargetMode, ImmutableArray<EntityId?> Targets, PlayFailure? Failure)
{
    internal bool ResolvesWithoutTarget { get; init; }
    internal bool ResolvesEffects(EntityId? target) => target.HasValue || ResolvesWithoutTarget;
}

public sealed partial class GameSimulator
{
    private enum TargetDirective { None, Required, IfAvailable, Conditional }

    private static TargetDirective TargetDirectiveFor(ErrorType? type) => type switch
    {
        REQ_TARGET_TO_PLAY or REQ_STEADY_SHOT => TargetDirective.Required,
        REQ_TARGET_IF_AVAILABLE => TargetDirective.IfAvailable,
        REQ_TARGET_FOR_COMBO or REQ_TARGET_FOR_NO_COMBO or REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND or
            REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN or REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN or
            REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK or REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS or
            REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS => TargetDirective.Conditional,
        _ => TargetDirective.None
    };

    private static ImmutableArray<PlayReq> BranchRequirements(SimulationCard card, CardPlayChoice? choice = null, bool activation = false)
    {
        var requirements = (activation ? card.PlayReqs?.Ability : card.PlayReqs?.Play).GetValueOrDefault();
        if (requirements.IsDefault)
            requirements = [];
        return choice is null || choice.PlayReqs.IsDefaultOrEmpty ? requirements : requirements.AddRange(choice.PlayReqs);
    }

    private static bool DeclaresTarget(ImmutableArray<PlayReq> requirements) =>
        requirements.Any(req => TargetDirectiveFor(req.Type) != TargetDirective.None);

    private static TargetRequirement EffectCharacterTarget(TargetRequirement native, ImmutableArray<PlayReq> requirements)
    {
        if (requirements.Any(req => req.Type == REQ_LOCATION_TARGET))
            return TargetRequirement.None;
        if (native == TargetRequirement.None && !DeclaresTarget(requirements))
            return native;

        var minion = TargetsOnlyMinions(native) || requirements.Any(req => req.Type is REQ_MINION_TARGET or REQ_TARGET_WITH_RACE);
        if (native is TargetRequirement.EnemyMinion or TargetRequirement.EnemyCharacter || requirements.Any(req => req.Type == REQ_ENEMY_TARGET))
            return minion ? TargetRequirement.EnemyMinion : TargetRequirement.EnemyCharacter;
        if (native is TargetRequirement.FriendlyMinion or TargetRequirement.FriendlyCharacter || requirements.Any(req => req.Type == REQ_FRIENDLY_TARGET))
            return minion ? TargetRequirement.FriendlyMinion : TargetRequirement.FriendlyCharacter;
        return minion ? TargetRequirement.Minion : TargetRequirement.Character;
    }

    private PlayBranch EvaluateBranch(GameState state, GameEntity source, SimulationCard card,
        CardPlayChoice? choice = null, LocationAbility? ability = null, EntityId? requested = null)
    {
        var player = state.Players[source.Controller];
        var native = ability?.Target ?? choice?.Target ?? card.Target;
        var filter = ability?.TargetFilter ?? choice?.TargetFilter ?? card.TargetFilter;
        var requirements = BranchRequirements(card, choice, activation: ability is not null);
        var mode = native == TargetRequirement.None ? PlayTargetMode.None :
            ability is null && card.Definition.Kind is EntityKind.Minion or EntityKind.Weapon
                ? PlayTargetMode.IfAvailable : PlayTargetMode.Required;
        PlayBranch Fail(PlayFailure error) => new(choice?.Id, mode, [], error with { Source = source.Id, Target = requested });

        if (requirements.Contains(PlayReq.Copied))
        {
            if (source.CopiedSpellCardId is not string copied || !PlayReqCatalog.Cards.TryGetValue(copied, out var info))
                return Fail(new("缺少吸收的法术条件", null));
            requirements = requirements.Remove(PlayReq.Copied).AddRange(info.Requirements.Play);
        }

        var flags = ability is null ? card.Requirements | (choice?.Requirements ?? PlayRequirements.None) : PlayRequirements.None;
        var effects = ability?.Effects ?? choice?.Effects ?? card.OnPlay;
        if ((flags.HasFlag(PlayRequirements.BoardSpace) || effects.Any(effect => effect is TakeControlEffect)) &&
            BoardSlotCount(state, player.Id) >= BoardLimit)
            return Fail(new("战场空位不足", REQ_NUM_MINION_SLOTS, 1, 0));
        if (flags.HasFlag(PlayRequirements.FriendlyMinionDeath) && !state.MinionDeaths.Any(death => death.Controller == player.Id))
            return Fail(new("本局没有友方随从死亡", REQ_FRIENDLY_MINION_DIED_THIS_GAME));
        if (ability is null && card.MinimumEnemyMinions > 0)
            requirements = requirements.Add(new(REQ_MINIMUM_ENEMY_MINIONS, card.MinimumEnemyMinions));
        if (ability is null && card.MinimumTotalMinions > 0)
            requirements = requirements.Add(new(REQ_MINIMUM_TOTAL_MINIONS, card.MinimumTotalMinions));

        var requiredTarget = false;
        var optionalTarget = false;
        var inactiveCondition = false;
        foreach (var req in requirements)
        {
            if (GateFailure(state, player, req) is { } gate)
                return Fail(gate);

            switch (TargetDirectiveFor(req.Type))
            {
                case TargetDirective.Required:
                    requiredTarget = true;
                    break;
                case TargetDirective.IfAvailable:
                    optionalTarget = true;
                    break;
                case TargetDirective.Conditional:
                    var active = TargetCondition(state, source, req);
                    if (active is null)
                        return Fail(new("缺少此条件需要的历史状态", req.Type, req.Parameter));
                    inactiveCondition |= !active.Value;
                    optionalTarget = true;
                    break;
            }
        }
        // 旧 Sim 可能同时声明必选和有目标时才选，例如叫嚣的中士。
        if (inactiveCondition)
            mode = PlayTargetMode.None;
        else if (optionalTarget)
            mode = PlayTargetMode.IfAvailable;
        else if (requiredTarget)
            mode = PlayTargetMode.Required;

        if (mode == PlayTargetMode.None)
            return new PlayBranch(choice?.Id, mode, [null], requested is null ? null : new("此分支无需指定目标", Source: source.Id, Target: requested))
            {
                ResolvesWithoutTarget = native == TargetRequirement.None && !DeclaresTarget(requirements)
            };
        var sourceKind = card.Definition.Kind;
        if (requested is EntityId targetId)
        {
            if (!state.Entities.TryGetValue(targetId, out var target) || state.LocationOf(targetId).Zone != GameZone.Play || !IsPlayTargetCandidate(target))
                return Fail(new("目标不在可选区域内"));
            var failure = TargetFailure(state, source, sourceKind, target, native, filter, requirements);
            return failure is null ? new(choice?.Id, mode, [targetId], null) : Fail(failure);
        }

        var targets = ImmutableArray.CreateBuilder<EntityId?>();
        foreach (var side in state.Players.Values.OrderBy(p => p.Id.Value))
        foreach (var id in side.In(GameZone.Play))
        {
            var entity = state.Entities[id];
            if (IsPlayTargetCandidate(entity) && TargetFailure(state, source, sourceKind, entity, native, filter, requirements) is null)
                targets.Add(id);
        }

        if (targets.Count == 0 && mode == PlayTargetMode.IfAvailable)
            targets.Add(null);
        return targets.Count > 0 ? new(choice?.Id, mode, targets.ToImmutable(), null) : Fail(new("没有合法目标", REQ_TARGET_TO_PLAY));
    }

    private static bool IsPlayTargetCandidate(GameEntity entity) => entity.Character is { Health: > 0 } || entity.Location is { Health: > 0 };

    private bool? TargetCondition(GameState state, GameEntity source, PlayReq req)
    {
        var player = state.Players[source.Controller];
        return req.Type switch
        {
            REQ_TARGET_FOR_COMBO => player.PlayedCardThisTurn,
            REQ_TARGET_FOR_NO_COMBO => !player.PlayedCardThisTurn,
            REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND => player.In(GameZone.Hand).Any(id =>
                id != source.Id && MatchesMinionType(Card(state.Entities[id]).Definition, MinionType.Dragon)),
            REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN => player.PlayedElementalLastTurn,
            REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN => player.HeroHealthChangedThisTurn,
            REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK => EffectiveAttack(state, state.Entities[player.Hero]) > 0,
            REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS => req.Parameter is int minions ? MinionCount(state, player.Id) >= minions : null,
            REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS => req.Parameter is int secrets ? player.In(GameZone.Secret).Length >= secrets : null,
            _ => null
        };
    }

    private PlayFailure? GateFailure(GameState state, PlayerState player, PlayReq req)
    {
        var minimum = req.Parameter ?? 1;
        int? actual = req.Type switch
        {
            REQ_NUM_MINION_SLOTS or REQ_MINION_CAP => BoardLimit - BoardSlotCount(state, player.Id),
            REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT => Math.Max(BoardLimit - BoardSlotCount(state, player.Id), player.MaximumManaCrystals - player.ManaCrystals),
            REQ_MINIMUM_ENEMY_MINIONS => MinionCount(state, state.OpponentOf(player.Id)),
            REQ_MINIMUM_TOTAL_MINIONS => MinionCount(state, state.OpponentOf(player.Id)) + MinionCount(state, player.Id),
            REQ_MINIMUM_CORPSES => player.AvailableCorpses,
            REQ_HAND_NOT_FULL => HandLimit - player.In(GameZone.Hand).Length,
            REQ_SECRET_ZONE_CAP_FOR_NON_SECRET => SecretLimit - player.In(GameZone.Secret).Length,
            REQ_WEAPON_EQUIPPED => player.EquippedWeapon.HasValue ? 1 : 0,
            REQ_ENEMY_WEAPON_EQUIPPED => state.Players[state.OpponentOf(player.Id)].EquippedWeapon.HasValue ? 1 : 0,
            REQ_FRIENDLY_MINION_DIED_THIS_GAME => state.MinionDeaths.Count(d => d.Controller == player.Id),
            REQ_FRIENDLY_MINION_DIED_THIS_TURN => state.MinionDeaths.Count(d => d.Controller == player.Id && d.Turn == state.Turn),
            REQ_ALL_BASIC_TOTEMS_NOT_IN_PLAY => MissingBasicTotems(state, player),
            REQ_MUST_PLAY_OTHER_CARD_FIRST => 0, // Shadow Reflection is replaced after another card is played.
            _ => null
        };
        return actual < minimum ? new("不满足出牌条件", req.Type, req.Parameter, actual) : null;
    }

    private static int MissingBasicTotems(GameState state, PlayerState player)
    {
        string[] pool = ["CS2_050", "CS2_051", "CS2_052", "NEW1_009"];
        return pool.Count(totem => !player.In(GameZone.Play).Any(id => state.Entities[id].CardId == totem));
    }

    private PlayFailure? TargetFailure(GameState state, GameEntity source, EntityKind sourceKind, GameEntity target,
        TargetRequirement native, TargetFilter? filter, ImmutableArray<PlayReq> requirements)
    {
        var card = Card(target);
        var friendly = target.Controller == source.Controller;
        var kind = card.Definition.Kind;
        var ch = target.Character;
        bool Printed(GAME_TAG tag) => PlayReqTags.Has(card.RequirementTagMask, (int)tag);
        var stealth = IsStealthed(target);
        if (!friendly && (stealth || IsEntityImmune(state, target) || (!target.Silenced && Printed(GAME_TAG.CANT_BE_TARGETED_BY_OPPONENTS))))
            return new("敌方目标不可指定", stealth ? REQ_NONSTEALTH_ENEMY_TARGET : REQ_CAN_BE_TARGETED_BY_OPPONENTS);
        if (sourceKind == EntityKind.Spell && (HasEffectiveTag(target, GAME_TAG.ELUSIVE) || HasEffectiveTag(target, GAME_TAG.CANT_BE_TARGETED_BY_SPELLS)) ||
            sourceKind == EntityKind.HeroPower && (HasEffectiveTag(target, GAME_TAG.ELUSIVE) || HasEffectiveTag(target, GAME_TAG.CANT_BE_TARGETED_BY_HERO_POWERS)))
            return new("目标免疫此类指向效果", sourceKind == EntityKind.Spell ? REQ_CAN_BE_TARGETED_BY_SPELLS : REQ_CAN_BE_TARGETED_BY_HERO_POWERS);

        foreach (var req in requirements)
        {
            int? observed = null;
            bool valid;
            switch (req.Type)
            {
                case REQ_TARGET_MAX_ATTACK:
                case REQ_TARGET_MIN_ATTACK:
                case REQ_TARGET_EXACT_ATTACK:
                    observed = ch is null ? null : EffectiveAttack(state, target);
                    valid = observed is int attack && req.Parameter is int limit && (req.Type switch
                    {
                        REQ_TARGET_MAX_ATTACK => attack <= limit,
                        REQ_TARGET_MIN_ATTACK => attack >= limit,
                        _ => attack == limit
                    });
                    break;
                case REQ_DAMAGED_TARGET:
                case REQ_UNDAMAGED_TARGET:
                    observed = ch?.Damage;
                    valid = observed is int damage && (req.Type == REQ_DAMAGED_TARGET ? damage > 0 : damage == 0);
                    break;
                case REQ_TARGET_EXACT_COST:
                    observed = card.Definition.PrintedCost;
                    valid = observed == req.Parameter;
                    break;
                case REQ_DAMAGED_TARGET_UNLESS_COMBO:
                    if (ch is { Damage: > 0 }) continue;
                    if (state.Players[source.Controller].PlayedCardThisTurn is not bool combo)
                        return new("缺少本回合出牌历史", req.Type);
                    valid = combo;
                    break;
                case REQ_TARGET_MUST_HAVE_TAG:
                    // Replicator-inator style alternatives refer to any matching tag.
                    valid = requirements.Any(r => r.Type == REQ_TARGET_MUST_HAVE_TAG && r.Parameter is int tag &&
                        (tag == (int)GAME_TAG.CARDRACE ? card.Definition.MinionTypes.GetValueOrDefault() != MinionType.None :
                            PlayReqTags.Has(card.RequirementTagMask, tag)));
                    break;
                default:
                    valid = req.Type switch
                    {
                        REQ_MINION_TARGET => kind == EntityKind.Minion,
                        REQ_HERO_TARGET => kind == EntityKind.Hero,
                        REQ_HERO_OR_MINION_TARGET => IsCharacter(kind),
                        REQ_LOCATION_TARGET => kind == EntityKind.Location,
                        REQ_FRIENDLY_TARGET => friendly,
                        REQ_ENEMY_TARGET => !friendly,
                        REQ_NONSELF_TARGET => source.Id != target.Id,
                        REQ_MINION_OR_ENEMY_HERO => kind == EntityKind.Minion || (!friendly && kind == EntityKind.Hero),
                        REQ_STEADY_SHOT => !friendly && kind == EntityKind.Hero ||
                            (kind == EntityKind.Minion && (state.Players[source.Controller].SteadyShotCanTargetMinions ||
                                BoardMinions(state, source.Controller).Any(id =>
                                {
                                    var ally = state.Entities[id];
                                    return !ally.Silenced && ally.Character is { Health: > 0 } && Card(ally).AllowHeroPowerTargetMinions;
                                }))),
                        REQ_FROZEN_TARGET => ch?.Frozen == true,
                        REQ_TARGET_WITH_RACE => req.Parameter is int race && MatchesMinionType(card.Definition, CardTraitReader.RaceType((TAG_RACE)race)),
                        REQ_TARGET_WITH_DEATHRATTLE => (!target.Silenced && !card.Deathrattle.IsEmpty) ||
                            HasEffectiveTag(target, GAME_TAG.DEATHRATTLE),
                        REQ_LEGENDARY_TARGET => Printed(GAME_TAG.RARITY),
                        REQ_NOT_LEGENDARY_TARGET => !Printed(GAME_TAG.RARITY),
                        REQ_MUST_TARGET_TAUNTER => IsTaunt(target),
                        REQ_STEALTHED_TARGET => stealth,
                        REQ_NONSTEALTH_ENEMY_TARGET => friendly || !stealth,
                        REQ_TARGET_IS_NON_TITAN => !Printed(GAME_TAG.TITAN),
                        REQ_TARGET_SILVER_HAND_RECRUIT => PlayReqTags.Has(card.RequirementTagMask, 3444),
                        _ => true
                    };
                    break;
            }
            if (!valid)
                return new("目标不满足此条件", req.Type, req.Parameter, observed);
        }
        if (ch is null && (native != TargetRequirement.None || !requirements.Any(r => r.Type == REQ_LOCATION_TARGET)))
            return new("需要角色目标", REQ_HERO_OR_MINION_TARGET);
        if (TargetsOnlyMinions(native) && kind != EntityKind.Minion)
            return new("需要随从目标", REQ_MINION_TARGET);
        if (!friendly && native is TargetRequirement.FriendlyCharacter or TargetRequirement.FriendlyMinion)
            return new("需要友方目标", REQ_FRIENDLY_TARGET);
        if (friendly && native is TargetRequirement.EnemyCharacter or TargetRequirement.EnemyMinion)
            return new("需要敌方目标", REQ_ENEMY_TARGET);
        if (filter is not null && (ch is null || !MatchesTargetFilter(state, target, filter)))
            return new("目标未通过此分支的目标过滤条件");
        return null;
    }
}
