using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 精灵咏唱者
// Elven Minstrel
// 连击：从你的牌库中抽两张随从牌。
// Combo: Draw 2 minions from your deck.
internal static class Sim_LOOT_211
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
