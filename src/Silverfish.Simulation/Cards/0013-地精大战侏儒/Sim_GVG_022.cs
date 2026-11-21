using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 修补匠的磨刀油
// Tinker's Sharpsword Oil
// 使你的武器获得+3攻击力。连击：随机使一个友方随从获得+3攻击力。
// Give your weapon +3 Attack. Combo: Give a random friendly minion +3 Attack.
internal static class Sim_GVG_022
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
