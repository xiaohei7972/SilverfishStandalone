using System.Collections.Concurrent;
using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private readonly ConcurrentDictionary<CardFilter, ImmutableArray<string>> _matchingMinionPools = new();

    private ImmutableArray<string> MatchingMinionCandidates(CardFilter filter) =>
        _matchingMinionPools.GetOrAdd(filter, key =>
            [.. _randomMinionCosts.Value.Values.SelectMany(bucket => bucket).Order(StringComparer.Ordinal)
                .Where(id => MatchesCardFilter(GetCard(id).Definition, key))]);
    /// <summary>在预算内枚举随机分支，必要时使用独立于回放种子的采样。</summary>
    /// <remarks>预测保留输入的回放种子。实际推进对局时调用 Apply。</remarks>
    public SimulationDistribution ApplyDistribution(GameState state, SimulationAction action, int workLimit = 64)
    {
        ValidateWorkLimit(workLimit);
        if (action is { Kind: SimulationActionKind.ChooseOption } && state.PendingChoice is not null)
            return new([new(Apply(state, action), 1.0)], 1, IsExact: true);
        var resolvesEffects = EnsureLegal(state, action);
        return RunDistribution(state, action, workLimit, resolvesEffects);
    }

    private static void ValidateWorkLimit(int workLimit)
    {
        if (workLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(workLimit), "随机结算预算必须大于零。");
    }

    private SimulationDistribution RunDistribution(GameState state, SimulationAction action, int workLimit, bool resolvesEffects) =>
        RunDistributionCore(randomChoice => new Resolution(this, state, randomChoice).Run(action, resolvesEffects), workLimit);

    /// <summary>Shared exact/random-sampling engine for gameplay actions and offline choice previews.</summary>
    private static SimulationDistribution RunDistributionCore(Func<Func<int, int>, SimulationResult> resolve, int workLimit)
    {
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
                var result = resolve(Select);
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
            var result = resolve(Sample);
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

    public LegalActionSet GetLegalActionSet(GameState state) => new(this, state);

    /// <summary>绑定到一个不可变局面的合法动作，供多个预测共用。</summary>
    public sealed class LegalActionSet
    {
        private readonly GameSimulator _simulator;

        internal LegalActionSet(GameSimulator simulator, GameState state)
            : this(simulator, state, simulator.LegalActions(state))
        {
        }

        private LegalActionSet(GameSimulator simulator, GameState state, ImmutableArray<SimulationAction> actions)
        {
            _simulator = simulator;
            State = state;
            Actions = actions;
        }

        public GameState State { get; }
        public ImmutableArray<SimulationAction> Actions { get; }

        public LegalActionSet Filter(Func<SimulationAction, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate), "必须提供合法动作的筛选条件。");
            return new(_simulator, State, [.. Actions.Where(predicate)]);
        }

        public SimulationDistribution ApplyDistribution(int index, int workLimit = 64)
        {
            ValidateWorkLimit(workLimit);
            if ((uint)index >= (uint)Actions.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "合法动作序号超出范围。");
            var action = Actions[index];
            // The action was already enumerated as legal. Only a targetless play needs the branch's effect decision.
            var resolvesEffects = action.Target.HasValue || ((action.Kind is SimulationActionKind.PlayCard or
                SimulationActionKind.UseHeroPower or SimulationActionKind.ActivateLocation) && _simulator.EnsureLegal(State, action));
            return _simulator.RunDistribution(State, action, workLimit, resolvesEffects);
        }
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

            var amount = ModifiedEffectAmount(context, effect.Amount, damage: true);
            if (effect.Mode == RandomDamageMode.Sequential)
            {
                if (effect.Count > 1)
                    work.Push(new ApplyEffect(context, effect with { Count = effect.Count - 1 }));
                PushOccurrence(work, Damage(context.Source, candidates[ChooseIndex(candidates.Count)], amount));
                return;
            }

            var hits = new List<Occurrence?>();
            var count = Math.Min(effect.Count, candidates.Count);
            for (var hit = 0; hit < count; hit++)
            {
                var index = ChooseIndex(candidates.Count);
                hits.Add(Damage(context.Source, candidates[index], amount));
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
                _ => throw new InvalidOperationException("无法识别随机伤害的目标范围。")
            };
            return candidates.Where(id => IsLivingCharacter(id)).OrderBy(id => _state.Entities[id].PlayOrder);
        }

        private void DestroyRandomEnemyMinion(EffectContext context, DestroyRandomEnemyMinionEffect effect)
        {
            var candidates = DamageCandidates(context, RandomCharacterPool.EnemyMinions)
                .Where(id => effect.MaximumAttack is not int maxAtk || _rules.EffectiveAttack(_state, _state.Entities[id]) <= maxAtk)
                .ToArray();
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
            if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                return;

            var cardId = effect.CardIds[ChooseIndex(effect.CardIds.Length)];
            if (Summon(player, cardId, position: null) is EntityId summoned)
                PushSummonTriggers(work, player, summoned);
        }

        private void SummonMatchingMinion(Stack<Work> work, EffectContext context, SummonMatchingMinionEffect effect)
        {
            var player = context.Controller;
            if (_rules.BoardSlotCount(_state, player) >= BoardLimit)
                return;

            var candidates = _rules.MatchingMinionCandidates(effect.CardFilter);
            if (candidates.IsEmpty)
                return;

            if (effect.Count > 1)
                work.Push(new ApplyEffect(context, effect with { Count = effect.Count - 1 }));
            var cardId = candidates[ChooseIndex(candidates.Length)];
            if (Summon(player, cardId, position: null) is EntityId summoned)
                PushSummonTriggers(work, player, summoned);
        }

        private int ChooseIndex(int count)
        {
            if (count == 1)
                return 0;
            if (_randomChoice is not null)
                return _randomChoice(count);

            // 各分支使用独立的 SplitMix64 随机序列，取模前丢弃末端不足一组的值。
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
