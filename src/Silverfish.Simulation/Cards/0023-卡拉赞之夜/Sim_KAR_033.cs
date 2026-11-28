using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 书卷之龙
// Book Wyrm
// 战吼：如果你的手牌中有龙牌，则消灭一个攻击力小于或等于3的敌方随从。
// Battlecry: If you're holding a Dragon, destroy an enemy minion with 3 or less Attack.
[CardSim("KAR_033")]
internal static class Sim_KAR_033
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_MAX_ATTACK, 3), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyMinionEffect()],
    };
}
