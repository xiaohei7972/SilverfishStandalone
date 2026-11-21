using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private void PayMana(PlayerId id, int cost)
        {
            var player = _state.Players[id];
            var temporary = Math.Min(cost, player.TemporaryMana);
            _state = _state.UpdateResources(player with
            {
                TemporaryMana = player.TemporaryMana - temporary,
                ManaSpent = checked(player.ManaSpent!.Value + (cost - temporary))
            });
        }

        private void AddOverload(PlayerId player, EntityId source, int amount)
        {
            if (amount == 0)
                return;

            var resources = _state.Players[player];
            _state = _state.UpdateResources(resources with { OverloadOwed = checked(resources.OverloadOwed + amount) });
            _events.Add(new(SimulationEventKind.OverloadScheduled, player, source, Amount: amount));
        }

        private void UnlockOverload(EffectContext context)
        {
            var before = _state.Players[context.Controller];
            if (before is { OverloadLocked: 0, OverloadOwed: 0 })
                return;

            var unlocked = before with { OverloadLocked = 0, OverloadOwed = 0 };
            _state = _state.UpdateResources(unlocked);
            _events.Add(new(SimulationEventKind.OverloadUnlocked, context.Controller, context.Source, Amount: unlocked.Mana - before.Mana));
        }

        private void ChangeMana(EffectContext context, ChangeManaEffect effect)
        {
            ChangeMana(context.Source, context.Controller, effect);
            if (effect.Recipient == ManaRecipient.BothPlayers)
                ChangeMana(context.Source, _state.OpponentOf(context.Controller), effect);
        }

        private void ChangeMana(EntityId source, PlayerId id, ChangeManaEffect effect)
        {
            var player = _state.Players[id];
            switch (effect.Operation)
            {
                case ManaOperation.GainTemporary:
                    AddTemporaryMana(player, source, effect.Amount, SimulationEventKind.TemporaryManaGained);
                    break;
                case ManaOperation.GainEmpty:
                case ManaOperation.GainFilled:
                    GainManaCrystals(player, source, effect);
                    break;
                case ManaOperation.Refresh:
                    AddTemporaryMana(player, source, Math.Min(effect.Amount, EmptyUnlockedCrystals(player)), SimulationEventKind.ManaRefreshed);
                    break;
                case ManaOperation.Destroy:
                    DestroyManaCrystals(player, source, effect.Amount);
                    break;
            }
        }

        private void GainManaCrystals(PlayerState player, EntityId source, ChangeManaEffect effect)
        {
            var count = Math.Min(effect.Amount, player.MaximumManaCrystals - player.ManaCrystals);
            if (count == 0 && effect.OverflowCardId is string overflow)
            {
                TryCreateCard(overflow, player.Id, source, CardPlacement.Hand);
                return;
            }

            var updated = player with
            {
                ManaCrystals = player.ManaCrystals + count,
                ManaSpent = checked(player.ManaSpent!.Value + (effect.Operation == ManaOperation.GainEmpty ? count : 0))
            };
            if (count > 0)
            {
                _state = _state.UpdateResources(updated);
                _events.Add(new(SimulationEventKind.ManaCrystalsGained, player.Id, source, Amount: count));
            }

            if (effect.Operation == ManaOperation.GainFilled)
                AddTemporaryMana(updated, source, Math.Min(effect.Amount - count, EmptyUnlockedCrystals(updated)), SimulationEventKind.ManaRefreshed);
        }

        private void DestroyManaCrystals(PlayerState player, EntityId source, int amount)
        {
            var count = Math.Min(amount, player.ManaCrystals);
            if (count == 0)
                return;

            _state = _state.UpdateResources(player with
            {
                ManaCrystals = player.ManaCrystals - count,
                ManaSpent = Math.Max(0, player.ManaSpent!.Value - count)
            });
            _events.Add(new(SimulationEventKind.ManaCrystalsDestroyed, player.Id, source, Amount: count));
        }

        private static int EmptyUnlockedCrystals(PlayerState player)
        {
            var unlocked = Math.Max(0, player.ManaCrystals - player.OverloadLocked);
            return Math.Max(0, unlocked - player.Mana);
        }

        private void AddTemporaryMana(PlayerState player, EntityId source, int amount, SimulationEventKind kind)
        {
            if (amount == 0)
                return;

            _state = _state.UpdateResources(player with { TemporaryMana = checked(player.TemporaryMana + amount) });
            _events.Add(new(kind, player.Id, source, Amount: amount));
        }
    }
}
