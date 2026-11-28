using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 背叛
// Betrayal
// 使一个敌方随从对其相邻的随从 造成等同于其攻击力的伤害。
// Force an enemy minion to deal its damage to the minions next to it.
[CardSim("EX1_126")]
internal static class Sim_EX1_126
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new AdjacentAttackDamageEffect()]
    };
}
