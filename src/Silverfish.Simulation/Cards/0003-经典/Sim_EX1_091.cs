using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 秘教暗影祭司
// Cabal Shadow Priest
// 战吼：夺取一个攻击力小于或等于2的敌方随从的控制权。
// Battlecry: Take control of an enemy minion that has 2 or less Attack.
[CardSim("EX1_091")]
internal static class Sim_EX1_091
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_TARGET_MAX_ATTACK, 2), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new TakeControlEffect()]
    };
}
