using Silverfish.Contracts;
using Silverfish.Core;
using ClassicAction = HREngine.Bots.Action;

namespace Silverfish.ClassicBridge;

/// <summary>
/// Map a completed *original* Silverfish search result onto trusted snapshot
/// candidate IDs. Never manufactures clickable game instructions.
/// </summary>
public static class ClassicDecisionAdapter
{
    public static BotDecision Match(
        GameSnapshot snapshot,
        ClassicAction? bestMove,
        double bestMoveValue)
    {
        SnapshotValidator.Validate(snapshot);
        if (!snapshot.IsOurTurn || snapshot.Phase != GamePhase.Main)
            return Unavailable(snapshot, "No main-turn decision is allowed.");

        if (bestMove is null)
            return Unavailable(snapshot, "Classic AI has no bestmove for this snapshot.");

        // Use names to stay compatible with historical versions whose enum
        // may not include later additions (location, forge, etc.).
        var name = bestMove.actionType.ToString();
        var kind = name switch
        {
            "endturn" => GameActionKind.EndTurn,
            "playcard" => GameActionKind.PlayCard,
            "attackWithHero" => GameActionKind.Attack,
            "attackWithMinion" => GameActionKind.Attack,
            "useHeroPower" => GameActionKind.HeroPower,
            _ => (GameActionKind?)null
        };
        if (kind is null)
            return Unavailable(snapshot, $"Classic action '{name}' is not mapped by contract v1.");

        int? source = name switch
        {
            "playcard" => FindHandEntityId(bestMove),
            "attackWithMinion" => bestMove.own?.entitiyID,
            "attackWithHero" => snapshot.OurHero.EntityId,
            _ => null
        };
        int? target = bestMove.target?.entitiyID;

        // If original AI references a missing source, fail closed.
        if ((name is "playcard" or "attackWithMinion") && source is null)
            return Unavailable(snapshot, "Classic action is missing its source entity.");

        var compatible = snapshot.LegalActions
            .Where(a => a.Kind == kind && a.SourceEntityId == source
                        && a.TargetEntityId == target)
            .ToArray();
        if (compatible.Length != 1)
        {
            return Unavailable(snapshot, compatible.Length == 0
                ? "Classic bestmove did not match a legal candidate in this snapshot."
                : "Classic bestmove matches multiple legal candidates; placement/choice details are required.");
        }

        return new BotDecision
        {
            MatchId = snapshot.MatchId,
            SnapshotSequence = snapshot.Sequence,
            Engine = "classic-silverfish",
            ChosenAction = compatible[0],
            Score = bestMoveValue,
            Reason = "Original classic AI bestmove, matched to a unique legal candidate."
        };
    }

    private static int? FindHandEntityId(ClassicAction move)
    {
        // Old classic action.cs used a public field named 'card'; the
        // 2026-11-05 source renamed this field to 'hc'. Resolve only
        // this small compatibility edge at runtime.
        var field = typeof(ClassicAction).GetField("hc")
                    ?? typeof(ClassicAction).GetField("card");
        return (field?.GetValue(move) as HREngine.Bots.Handmanager.Handcard)?.entity;
    }

    private static BotDecision Unavailable(GameSnapshot snapshot, string reason) => new()
    {
        MatchId = snapshot.MatchId,
        SnapshotSequence = snapshot.Sequence,
        Engine = "classic-silverfish",
        Reason = reason
    };
}
