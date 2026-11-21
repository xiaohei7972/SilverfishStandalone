using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 心灵尖啸
// Psychic Scream
// 将所有随从洗入你对手的牌库。
// Shuffle all minions into your opponent's deck.
internal static class Sim_LOOT_008
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
