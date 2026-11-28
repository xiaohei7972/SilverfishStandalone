using System.Collections.Immutable;
using System.Globalization;
using Silverfish.Contracts;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;

namespace Silverfish.Cli.Web;

internal sealed class CardNames(CardDataCatalog starter, CardDataCatalog? external)
{
    private readonly Dictionary<string, string> _names = starter.Cards.Concat(external?.Cards ?? [])
        .Where(card => !string.IsNullOrWhiteSpace(card.Name?.ZhCn))
        .GroupBy(card => card.CardId, StringComparer.Ordinal)
        .ToDictionary(group => group.Key, group => group.Last().Name!.ZhCn!, StringComparer.Ordinal);

    internal string? Find(string? cardId) => cardId is not null ? _names.GetValueOrDefault(cardId) : null;

    internal static string ClassName(uint mask) => mask switch
    {
        1u << ((int)TAG_CLASS.DEATHKNIGHT - 1) => "死亡骑士",
        1u << ((int)TAG_CLASS.DEMONHUNTER - 1) => "恶魔猎手",
        1u << ((int)TAG_CLASS.DRUID - 1) => "德鲁伊",
        1u << ((int)TAG_CLASS.HUNTER - 1) => "猎人",
        1u << ((int)TAG_CLASS.MAGE - 1) => "法师",
        1u << ((int)TAG_CLASS.PALADIN - 1) => "圣骑士",
        1u << ((int)TAG_CLASS.PRIEST - 1) => "牧师",
        1u << ((int)TAG_CLASS.ROGUE - 1) => "潜行者",
        1u << ((int)TAG_CLASS.SHAMAN - 1) => "萨满祭司",
        1u << ((int)TAG_CLASS.WARLOCK - 1) => "术士",
        1u << ((int)TAG_CLASS.WARRIOR - 1) => "战士",
        _ => "未收录"
    };
}

internal sealed record ConsoleHero(int EntityId, string? ClassName, int Health, int Armor, int Attack);
internal sealed record ConsoleCard(int EntityId, string CardId, string? Name, int? Cost = null,
    int? Attack = null, int? Health = null, string[]? Statuses = null);
internal sealed record ConsoleChoiceCard(int EntityId, string CardId, string Name, string? CardType,
    int Cost, int? Attack, int? Health, bool Unchoosable, bool Chosen);
internal sealed record ConsolePendingChoice(int ChoiceId, string ChoiceType, int SourceEntityId,
    int CountMin, int CountMax, bool IsSingleChoice, bool HasConfirmButton, ConsoleChoiceCard[] Cards);
internal sealed record ConsoleEntityTag(int Id, string Name, int Value);
internal sealed record ConsoleEntity(int Id, string? CardId, int ControllerId, string? Zone, int? ZonePosition,
    string? CardType, bool IsRedacted, ConsoleEntityTag[] Tags, ConsoleEntityTag[] CachedTagsForDormant);
internal sealed record ConsoleBoard(GamePhase Phase, bool IsOurTurn, int AvailableMana, int? MaximumMana,
    ConsoleHero OurHero, ConsoleHero EnemyHero, ConsoleCard[] OurBoard, ConsoleCard[] EnemyBoard,
    ConsoleCard[] OurHand, int? EnemyHandCount, int? EnemyDeckCount, int? EnemySecretCount, int? OurSecretCount,
    int? Turn = null, int? ActivePlayer = null, ConsoleEntity[]? AllEntities = null, ConsolePendingChoice? PendingChoice = null,
    bool CanAct = true, string? BlockReason = null, string? ResponseMode = null);
internal sealed record ConsoleEvent(string Sequence, SimulationEvent Event);

internal sealed record ConsolePosition(string MatchId, long Sequence, ConsoleBoard Board, GameSnapshot? Snapshot,
    GameSimulator? Simulator, GameState? Game, PlayerId Perspective, ImmutableArray<SimulationAction> Actions, ConsoleEvent[] Events)
{
    internal string SequenceText => Sequence.ToString(CultureInfo.InvariantCulture);

    internal static ConsolePosition Observable(GameSnapshot snapshot, CardNames names, ConsoleEntity[]? allEntities = null,
        ConsolePendingChoice? pendingChoice = null, bool canAct = true, string? blockReason = null, string? responseMode = null)
    {
        ConsoleCard Minion(MinionSnapshot minion) => new(minion.EntityId, minion.CardId,
            string.IsNullOrWhiteSpace(minion.CardNameZh) ? names.Find(minion.CardId) : minion.CardNameZh,
            Attack: minion.Attack, Health: minion.Health,
            Statuses: Statuses((minion.HasTaunt, "嘲讽"), (minion.HasDivineShield, "圣盾"), (minion.CanAttack, "可攻击")));
        static ConsoleHero Hero(HeroSnapshot hero) => new(hero.EntityId, hero.ClassNameZh, hero.Health, hero.Armor, hero.Attack);
        return new(snapshot.MatchId, snapshot.Sequence, new(snapshot.Phase, snapshot.IsOurTurn, snapshot.AvailableMana,
            snapshot.MaximumMana, Hero(snapshot.OurHero), Hero(snapshot.EnemyHero),
            [.. snapshot.OurBoard.Select(Minion)], [.. snapshot.EnemyBoard.Select(Minion)],
            [.. snapshot.OurHand.Select(card => new ConsoleCard(card.EntityId, card.CardId,
                string.IsNullOrWhiteSpace(card.CardNameZh) ? names.Find(card.CardId) : card.CardNameZh, card.Cost))],
            snapshot.EnemyHandCount, snapshot.EnemyDeckCount, snapshot.EnemySecretCount, snapshot.OurSecretCount,
            AllEntities: allEntities, PendingChoice: pendingChoice, CanAct: canAct, BlockReason: blockReason, ResponseMode: responseMode),
            snapshot, null, null, default, [], []);
    }

    internal static ConsolePosition Simulation(string matchId, long sequence, GameSimulator simulator, GameState game,
        PlayerId perspective, ConsoleEvent[] events, CardNames names)
    {
        var actions = simulator.LegalActions(game);
        var attackers = actions.Where(action => action.Kind == SimulationActionKind.Attack).Select(action => action.Source).ToHashSet();
        var us = game.Players[perspective];
        var enemy = game.Players[game.OpponentOf(perspective)];
        ConsoleHero Hero(PlayerState player)
        {
            var hero = game.Entities[player.Hero];
            var character = hero.Character!;
            var weapon = player.EquippedWeapon is EntityId weaponId ? game.Entities[weaponId].Weapon?.Attack : null;
            return new(hero.Id.Value, CardNames.ClassName(simulator.Cards[hero.CardId].Definition.ClassMask),
                character.Health, character.Armor, character.Attack + (weapon ?? 0));
        }

        ConsoleCard[] Board(PlayerState player) => [.. player.In(GameZone.Play).Select(id => game.Entities[id])
            .Where(entity => simulator.Cards[entity.CardId].Definition.Kind is EntityKind.Minion or EntityKind.Location)
            .Select(entity =>
            {
                var card = simulator.Cards[entity.CardId];
                return new ConsoleCard(entity.Id.Value, entity.CardId, names.Find(entity.CardId),
                    Attack: entity.Character?.Attack, Health: entity.Character?.Health ?? entity.Location?.Health,
                    Statuses: Statuses((!entity.Silenced && card.Keywords.HasFlag(CardKeywords.Taunt), "嘲讽"),
                        (entity.Character?.DivineShield == true, "圣盾"), (entity.Character?.Frozen == true, "冻结"),
                        (entity.Stealthed == true, "潜行"), (entity.Silenced, "沉默"), (attackers.Contains(entity.Id), "可攻击"),
                        (entity.Location is not null, "地标")));
            })];
        var phase = game.Outcome != MatchOutcome.InProgress ? GamePhase.GameOver :
            game.PendingChoice is not null || game.PendingPlayChoice is not null ? GamePhase.Choice : GamePhase.Main;
        return new(matchId, sequence, new(phase, game.ActivePlayer == perspective, us.Mana, us.ManaCrystals,
            Hero(us), Hero(enemy), Board(us), Board(enemy),
            [.. us.In(GameZone.Hand).Select(id => new ConsoleCard(id.Value, game.Entities[id].CardId,
                names.Find(game.Entities[id].CardId), simulator.EffectiveCost(game, id)))],
            enemy.In(GameZone.Hand).Length, enemy.In(GameZone.Deck).Length, enemy.In(GameZone.Secret).Length,
            us.In(GameZone.Secret).Length, game.Turn, game.ActivePlayer.Value), null, simulator, game, perspective, actions, events);
    }

    internal static ConsoleEvent[] AppendEvents(ConsoleEvent[] previous, long sequence, IEnumerable<SimulationEvent> events) =>
        [.. previous.Concat(events.Select(entry => new ConsoleEvent(sequence.ToString(CultureInfo.InvariantCulture), entry))).TakeLast(160)];

    private static string[] Statuses(params (bool Active, string Label)[] flags) =>
        [.. flags.Where(flag => flag.Active).Select(flag => flag.Label)];
}
