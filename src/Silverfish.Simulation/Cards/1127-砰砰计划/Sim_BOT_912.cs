using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 坎格尔的无尽大军
// Kangor's Endless Army
// 复活三个友方机械，它们会保留所有磁力升级。
// Resurrect 3 friendly Mechs. They keep any Magnetic upgrades.
internal static class Sim_BOT_912
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
