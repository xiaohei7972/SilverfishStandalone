using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 古墓卫士
// Catacomb Guard
// 吸血，战吼：对一个敌方随从造成等同于本随从攻击力的伤害。
// Lifesteal Battlecry: Deal damage equal to this minion's Attack to an enemy minion.
internal static class Sim_SW_317
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
