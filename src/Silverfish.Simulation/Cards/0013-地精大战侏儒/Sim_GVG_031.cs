using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 回收
// Recycle
// 将一个敌方随从洗入你对手的 牌库。
// Shuffle an enemy minion into your opponent's deck.
[CardSim("GVG_031")]
internal static class Sim_GVG_031
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ShuffleMinionIntoDeckEffect()]
    };
}
