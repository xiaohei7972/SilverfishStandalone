using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灼烧
// Scorch
// 对一个随从造成$4点伤害。如果你在上个回合使用过元素牌，则法力值消耗变为（1）点。
// Deal $4 damage to a minion. Costs (1) if you played an Elemental last turn.
internal static class Sim_TRL_313
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
