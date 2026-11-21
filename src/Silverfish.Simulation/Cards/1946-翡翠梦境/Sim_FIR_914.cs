using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 焚火之力
// Smoldering Strength
// 使一个友方随从获得+{0}/+{0}。（每回合都会升级，但本牌会在{1}回合后弃掉！）@使一个友方随从获得+{0}/+{0}。（本牌会在本回合弃掉！）
// Give a friendly minion +{0}/+{0}. (Upgrades each turn, but discards after {1}!)@Give a friendly minion +{0}/+{0}. (Discards this turn!)
internal static class Sim_FIR_914
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
