using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 遮天雨云
// Cumulo-Maximus
// 战吼：如果你有过载的法力水晶，造成5点伤害。
// Battlecry: If you have Overloaded Mana Crystals, deal 5 damage.
internal static class Sim_DRG_223
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
