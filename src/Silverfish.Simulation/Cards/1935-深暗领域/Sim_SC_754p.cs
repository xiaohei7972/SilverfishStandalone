using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 双刃
// Twin Blades
// 使一个友方随从和你的英雄获得圣盾以及本回合中的+$a1攻击力。
// Give a friendly minion and your hero +$a1 Attack this turn and Divine Shield.
internal static class Sim_SC_754p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
