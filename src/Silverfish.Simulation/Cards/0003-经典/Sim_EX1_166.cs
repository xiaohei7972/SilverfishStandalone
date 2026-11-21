using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 丛林守护者
// Keeper of the Grove
// 抉择：造成2点伤害；或者沉默一个随从。
// Choose One - Deal_2_damage; or Silence a minion.
internal static class Sim_EX1_166
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
