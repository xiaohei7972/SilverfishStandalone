using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 传播瘟疫
// Spreading Plague
// 召唤一只1/5并具有嘲讽的甲虫。如果你的对手拥有的随从更多，则再次施放该法术。
// Summon a 1/5 Scarab with Taunt. If your_opponent has more minions, cast this again.
internal static class Sim_ICC_054
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
