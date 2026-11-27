namespace Silverfish.Simulation.Cards;

// 群体驱散
// Mass Dispel
// 沉默所有敌方随从，抽一张牌。
// Silence all enemy minions. Draw a card.
[CardSim("EX1_626")]
internal static class Sim_EX1_626
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SilenceEffect(AllEnemyMinions: true), new DrawEffect(1)]
    };
}
