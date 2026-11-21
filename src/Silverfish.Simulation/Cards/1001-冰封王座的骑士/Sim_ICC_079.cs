using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 铁齿铜牙
// Gnash
// 使你的英雄获得3点护甲值，并在本回合中获得 +3攻击力。
// Give your hero +3_Attack this turn. Gain 3 Armor.
internal static class Sim_ICC_079
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
