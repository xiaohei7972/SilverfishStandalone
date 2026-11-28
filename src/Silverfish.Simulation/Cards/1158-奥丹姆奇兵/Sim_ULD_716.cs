using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鱼人为王
// Tip the Scales
// 从你的牌库中召唤七个鱼人。
// Summon 7 Murlocs from your deck.
internal static class Sim_ULD_716
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
