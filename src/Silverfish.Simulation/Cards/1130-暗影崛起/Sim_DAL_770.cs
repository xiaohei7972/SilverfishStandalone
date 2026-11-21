using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 欧米茄毁灭者
// Omega Devastator
// 战吼：如果你有十个法力水晶，对一个随从造成10点伤害。
// Battlecry: If you have 10 Mana Crystals, deal 10 damage to a minion.
internal static class Sim_DAL_770
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
