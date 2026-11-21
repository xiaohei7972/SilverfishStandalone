using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星系投影
// The Galaxy's Lens
// 法术迸发：吸收法术的能量！@施放{0}。
// Spellburst: Absorb the spell's power!@Cast {0}.
internal static class Sim_GDB_136t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [PlayReq.Copied]);
    // </legacy-play-requirements>

}
