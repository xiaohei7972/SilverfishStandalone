using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狂奔怒吼
// Stampeding Roar
// 随机从你的手牌中召唤一个野兽，并使其获得突袭。
// Summon a random Beast from your hand and give it Rush.
internal static class Sim_TRL_255
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
