using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蚀解软泥怪
// Dissolving Ooze
// 战吼：消灭一个友方随从，将一张骸骨置入你的手牌。骸骨可以使一个随从获得被消灭随从的攻击力和生命值。
// Battlecry: Destroy a friendly minion. Spit out the Bones of its Attack and Health into your hand.
internal static class Sim_TLC_252
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
