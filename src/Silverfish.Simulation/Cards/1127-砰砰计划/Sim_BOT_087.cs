using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 学术剽窃
// Academic Espionage
// 将十张你对手职业的卡牌洗入你的牌库，其法力值消耗为（1）点。
// Shuffle 10 cards from your opponent's class into your deck. They_cost (1).
internal static class Sim_BOT_087
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
