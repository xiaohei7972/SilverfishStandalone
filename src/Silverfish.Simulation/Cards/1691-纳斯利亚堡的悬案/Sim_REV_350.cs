using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狂暴利齿
// Frenzied Fangs
// 召唤两只2/1的蝙蝠。注能（@）：使其获得+1/+2。
// Summon two 2/1 Bats. Infuse (@): Give them +1/+2.
internal static class Sim_REV_350
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
