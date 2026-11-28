using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 末日镜像
// Mirror of Doom
// 用3/3的木乃伊僵尸填满你的面板。
// Fill your board with 3/3 Mummy Zombies.
[CardSim("LOEA16_5")]
internal static class Sim_LOEA16_5
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_NUM_MINION_SLOTS, 1)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new FillBoardSummonEffect("LOEA16_5t"),
        ],
    };
}
