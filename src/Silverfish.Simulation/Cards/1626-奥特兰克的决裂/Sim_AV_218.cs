using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 群体变形
// Mass Polymorph
// 使所有随从变形成为1/1的绵羊。
// Transform all minions into 1/1 Sheep.
internal static class Sim_AV_218
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
