using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    /// <summary>Enumerates random paths within the work limit and samples independently of the replay seed when necessary.</summary>
    /// <remarks>Forecast states retain the input replay seed. Use Apply to advance the seeded game.</remarks>
    public SimulationDistribution ApplyDistribution(GameState state, SimulationAction action, int workLimit = 64)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(workLimit, 1);
        EnsureLegal(state, action);
        var paths = new Stack<RandomPath>();
        paths.Push(new([], 1));
        var outcomes = ImmutableArray.CreateBuilder<SimulationOutcome>();
        var resolutions = 0;
        const int sampleLimit = 12;
        var sampleReserve = Math.Min(sampleLimit, Math.Max(1, workLimit / 4));
        var exactBudget = workLimit - sampleReserve;
        while (paths.Count > 0 && resolutions < exactBudget)
        {
            var path = paths.Pop();
            var next = 0;
            resolutions++;
            try
            {
                var result = new Resolution(this, state, Select).Run(action);
                outcomes.Add(new(result, path.Probability));
            }
            catch (UnresolvedRandomChoice choice)
            {
                if (choice.Count > exactBudget - resolutions - paths.Count)
                    break;

                var probability = path.Probability / choice.Count;
                for (var index = choice.Count - 1; index >= 0; index--)
                    paths.Push(new(path.Choices.Add(index), probability));
                continue;
            }

            if (paths.Count == 0)
                return new(outcomes.ToImmutable(), resolutions, IsExact: true);

            int Select(int count) => next < path.Choices.Length ? path.Choices[next++] : throw new UnresolvedRandomChoice(count);
        }

        var samples = Math.Min(sampleLimit, workLimit - resolutions);
        var random = new Random(0);
        outcomes.Clear();
        for (var index = 0; index < samples; index++)
        {
            var randomized = false;
            var result = new Resolution(this, state, Sample).Run(action);
            resolutions++;
            if (!randomized)
                return new([new(result, 1)], resolutions, IsExact: true);

            outcomes.Add(new(result, 1.0 / samples));

            int Sample(int count)
            {
                randomized = true;
                return random.Next(count);
            }
        }

        return new(outcomes.ToImmutable(), resolutions, IsExact: false);
    }

    private readonly record struct RandomPath(ImmutableArray<int> Choices, double Probability);

    private sealed class UnresolvedRandomChoice(int count) : Exception
    {
        public int Count { get; } = count;
    }

    private sealed partial class Resolution
    {
        private void ApplyRandomDamage(Stack<Work> work, EffectContext context, RandomDamageEffect effect)
        {
            var candidates = DamageCandidates(context, effect.Pool).ToList();
            if (candidates.Count == 0)
                return;

            if (effect.Mode == RandomDamageMode.Sequential)
            {
                if (effect.Count > 1)
                    work.Push(new ApplyEffect(context, effect with { Count = effect.Count - 1 }));
                PushOccurrence(work, Damage(context.Source, candidates[ChooseIndex(candidates.Count)], effect.Amount));
                return;
            }

            var hits = new List<Occurrence?>();
            var count = Math.Min(effect.Count, candidates.Count);
            for (var hit = 0; hit < count; hit++)
            {
                var index = ChooseIndex(candidates.Count);
                hits.Add(Damage(context.Source, candidates[index], effect.Amount));
                candidates.RemoveAt(index);
            }
            for (var index = hits.Count - 1; index >= 0; index--)
                PushOccurrence(work, hits[index]);
        }

        private IEnumerable<EntityId> DamageCandidates(EffectContext context, RandomCharacterPool pool)
        {
            var enemy = _state.OpponentOf(context.Controller);
            var candidates = pool switch
            {
                RandomCharacterPool.EnemyMinions => _rules.BoardMinions(_state, enemy),
                RandomCharacterPool.EnemyCharacters => _state.Players[enemy].In(GameZone.Play),
                RandomCharacterPool.OtherCharacters => _state.Players.Values.SelectMany(player => player.In(GameZone.Play))
                    .Where(id => id != context.Source),
                _ => throw new InvalidOperationException("Unknown random damage pool.")
            };
            return candidates.Where(id => IsLivingCharacter(id)).OrderBy(id => _state.Entities[id].PlayOrder);
        }

        private void DestroyRandomEnemyMinion(EffectContext context)
        {
            var candidates = DamageCandidates(context, RandomCharacterPool.EnemyMinions).ToArray();
            if (candidates.Length > 0)
                DestroyMinion(candidates[ChooseIndex(candidates.Length)]);
        }

        private void DestroyAllButOneMinion()
        {
            var candidates = _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player))
                .Where(id => IsLivingCharacter(id)).OrderBy(id => _state.Entities[id].PlayOrder).ToArray();
            if (candidates.Length <= 1)
                return;

            var survivor = candidates[ChooseIndex(candidates.Length)];
            foreach (var id in candidates)
                if (id != survivor)
                    DestroyMinion(id);
        }

        private void SummonRandomMinion(Stack<Work> work, PlayerId player, SummonRandomMinionEffect effect)
        {
            if (_rules.MinionCount(_state, player) >= BoardLimit)
                return;

            var cardId = effect.CardIds[ChooseIndex(effect.CardIds.Length)];
            if (Summon(player, cardId, position: null) is EntityId summoned)
                PushSummonTriggers(work, player, summoned);
        }

        private int ChooseIndex(int count)
        {
            if (count == 1)
                return 0;
            if (_randomChoice is not null)
                return _randomChoice(count);

            // SplitMix64 keeps a branch-local replay stream. Reject the short tail before taking a modulus.
            var randomState = _state.RandomState;
            var bound = (ulong)count;
            var threshold = unchecked(0UL - bound) % bound;
            ulong value;
            do
            {
                randomState = unchecked(randomState + 0x9E3779B97F4A7C15UL);
                value = unchecked((randomState ^ (randomState >> 30)) * 0xBF58476D1CE4E5B9UL);
                value = unchecked((value ^ (value >> 27)) * 0x94D049BB133111EBUL);
                value ^= value >> 31;
            } while (value < threshold);

            _state = _state.SetRandomState(randomState);
            return (int)(value % bound);
        }
    }
}
