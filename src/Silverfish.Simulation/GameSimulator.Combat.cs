using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private enum AttackReach { None, Minions, AnyCharacter }

    private AttackReach GetAttackReach(GameState state, GameEntity attacker)
    {
        if (attacker.Character is not { Health: > 0 } character ||
            (!attacker.Silenced && Card(attacker).CannotAttack) ||
            HasEffectiveTag(attacker, Silverfish.Domain.Client.GAME_TAG.CANT_ATTACK) ||
            character.AttacksThisTurn >= AttackLimit(state, attacker))
            return AttackReach.None;
        if (!character.Exhausted || HasKeyword(attacker, CardKeywords.Charge))
            return AttackReach.AnyCharacter;
        return HasKeyword(attacker, CardKeywords.Rush) ? AttackReach.Minions : AttackReach.None;
    }

    private int AttackLimit(GameState state, GameEntity attacker)
    {
        if (HasKeyword(attacker, CardKeywords.Windfury))
            return 2;

        var player = state.Players[attacker.Controller];
        return player.Hero == attacker.Id && player.EquippedWeapon is EntityId weapon &&
            HasKeyword(state.Entities[weapon], CardKeywords.Windfury) ? 2 : 1;
    }

    private sealed partial class Resolution
    {
        private void Freeze(EffectContext context, FreezeEffect effect)
        {
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
