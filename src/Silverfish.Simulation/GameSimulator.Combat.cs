using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private enum AttackReach { None, Minions, AnyCharacter }

    private AttackReach GetAttackReach(GameState state, GameEntity attacker)
    {
        var card = Card(attacker);
        var lacksPermission = !attacker.Silenced && card.AllowAttackEnchantmentCardId is { } permission &&
            !attacker.Enchantments.Any(layer => layer.CardId == permission);
        if (attacker.Character is not { Health: > 0 } character ||
            (!attacker.Silenced && (card.CannotAttack ||
                card.RequiresHeroAttacked && state.Entities[state.Players[attacker.Controller].Hero].Character?.AttacksThisTurn == 0 ||
                card.AttacksOnlyIfLoneMinion && state.Players.Keys.Sum(player => MinionCount(state, player)) != 1)) ||
            (card.AllowAttackEnchantmentCardId is null && HasEffectiveTag(attacker, Silverfish.Domain.Client.GAME_TAG.CANT_ATTACK)) ||
            lacksPermission ||
            character.AttacksThisTurn >= AttackLimit(state, attacker))
            return AttackReach.None;
        if (!character.Exhausted || HasKeyword(attacker, CardKeywords.Charge))
            return (!attacker.Silenced && card.CannotAttackHeroes) || ActiveAttackWeapon(state, attacker)?.CannotAttackHeroes == true
                ? AttackReach.Minions : AttackReach.AnyCharacter;
        return HasKeyword(attacker, CardKeywords.Rush) ? AttackReach.Minions : AttackReach.None;
    }

    private int AttackLimit(GameState state, GameEntity attacker)
    {
        if (ActiveAttackWeapon(state, attacker)?.UnlimitedHeroAttacks == true)
            return int.MaxValue;
        if (!attacker.Silenced && Card(attacker).MegaWindfury)
            return 4;
        if (HasKeyword(attacker, CardKeywords.Windfury))
            return 2;

        var player = state.Players[attacker.Controller];
        return player.Hero == attacker.Id && player.EquippedWeapon is EntityId weapon &&
            HasKeyword(state.Entities[weapon], CardKeywords.Windfury) ? 2 : 1;
    }

    private SimulationCard? ActiveAttackWeapon(GameState state, GameEntity attacker)
    {
        var player = state.Players[attacker.Controller];
        return player.Hero == attacker.Id && player.EquippedWeapon is EntityId weapon &&
            state.Entities[weapon] is { Silenced: false, Weapon.Durability: > 0 } entity ? Card(entity) : null;
    }

    private sealed partial class Resolution
    {
        private void Freeze(EffectContext context, FreezeEffect effect)
        {
            if (effect.Target == FreezeTarget.RandomUnfrozenEnemy)
            {
                var candidates = DamageCandidates(context, RandomCharacterPool.EnemyCharacters)
                    .Where(id => _state.Entities[id].Character is { Frozen: false }).ToArray();
                if (candidates.Length > 0)
                    FreezeCharacter(context.Source, candidates[ChooseIndex(candidates.Length)]);
                return;
            }
            if (effect.Target == FreezeTarget.EnemyMinions)
            {
                var targets = _rules.BoardMinions(_state, _state.OpponentOf(context.Controller))
                    .OrderBy(id => _state.Entities[id].PlayOrder).ToArray();
                foreach (var target in targets)
                    FreezeCharacter(context.Source, target);
                return;
            }
            if (effect.Target == FreezeTarget.SelectedAndAdjacentMinions)
            {
                foreach (var target in TargetAndAdjacentMinions(context.Selected!.Value))
                    FreezeCharacter(context.Source, target);
                return;
            }

            var id = effect.Target == FreezeTarget.Selected ? context.Selected!.Value : context.Subject!.Value;
            FreezeCharacter(context.Source, id);
        }

        private void FreezeCharacter(EntityId source, EntityId target)
        {
            var entity = _state.Entities[target];
            if (entity.Character is not { Frozen: false } character || !IsLivingCharacter(target))
                return;

            _state = _state.UpdateCharacter(target, character with { Frozen = true });
            _events.Add(new(SimulationEventKind.Frozen, entity.Controller, source, target));
        }

        private void ThawAtTurnEnd(PlayerId player)
        {
            var opponentHasMinion = _rules.BoardMinions(_state, _state.OpponentOf(player)).Any(id => IsLivingCharacter(id));
            foreach (var id in _state.Players[player].In(GameZone.Play))
            {
                var entity = _state.Entities[id];
                if (entity.Character is not { Frozen: true } character)
                    continue;

                // 错过一次可用的攻击机会即可解冻，即使角色的攻击力为零。
                var reach = _rules.GetAttackReach(_state, entity);
                if (reach == AttackReach.None || (reach == AttackReach.Minions && !opponentHasMinion))
                    continue;

                _state = _state.UpdateCharacter(id, character with { Frozen = false });
                _events.Add(new(SimulationEventKind.Thawed, player, Target: id));
            }
        }
    }
}
