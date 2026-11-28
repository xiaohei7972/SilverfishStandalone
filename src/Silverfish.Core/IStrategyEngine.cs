using Silverfish.Contracts;

namespace Silverfish.Core;

/// <summary>根据完整快照返回动作建议。读取进程和执行游戏动作由调用方负责。</summary>
public interface IStrategyEngine
{
    string Name { get; }
    Task<BotDecision> DecideAsync(GameSnapshot snapshot, CancellationToken cancellationToken = default);
}
