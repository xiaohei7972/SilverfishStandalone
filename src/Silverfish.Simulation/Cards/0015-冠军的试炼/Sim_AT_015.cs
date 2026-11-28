using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 策反
// Convert
// 将一个敌方随从的一张复制置入你的手牌。其法力值消耗为（1）点。
// Put a copy of an enemy minion into your hand. It costs (1).
[CardSim("AT_015")]
internal static class Sim_AT_015
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CopyTargetCardEffect(Placement: CardPlacement.Hand) { SetCost = 1 },
        ],
    };
}
