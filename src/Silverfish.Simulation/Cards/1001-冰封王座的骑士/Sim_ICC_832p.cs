using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 瘟疫领主
// Plague Lord
// 抉择：在本回合中获得+$a3攻击力；或者获得+$d3点护甲值。
// Choose One - +$a3 Attack this turn; or Gain $d3 Armor.
internal static class Sim_ICC_832p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
