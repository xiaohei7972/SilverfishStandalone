using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰封秘典
// Glacial Mysteries
// 将每种不同的奥秘从你的牌库中置入战场。
// Put one of each Secret from your deck into the battlefield.
internal static class Sim_ICC_086
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_SECRET_ZONE_CAP_FOR_NON_SECRET)],
        []);
    // </legacy-play-requirements>

}
