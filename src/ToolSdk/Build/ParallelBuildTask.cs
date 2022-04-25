using System.Collections.Concurrent;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Build;

internal class ParallelBuildTask : IBuildTask
{
    private readonly IEnumerable<IBuildTask> m_tasks;
    private readonly ConcurrentQueue<(string, LogLevel)> m_messageQueue = new();
    private readonly ILogger<ParallelBuildTask> m_logger;

    public int TaskCount => m_tasks.Count();
    public Action<string, LogLevel> Log { get; set; }

    public ParallelBuildTask(IEnumerable<IBuildTask> tasks, ILogger<ParallelBuildTask> logger)
    {
        Log = ConcurrentLog;
        m_tasks = tasks;
        m_logger = logger;
    }

    private void ConcurrentLog(string message, LogLevel logLevel)
    {
        m_messageQueue.Enqueue((message, logLevel));
    }

    public BuildResult Build()
    {
        List<Task<BuildResult>> taskPool = new();

        foreach (var task in m_tasks)
        {
            task.Log = ConcurrentLog;
            taskPool.Add(Task.Run(() => task.Build()));
        }

        var token = new CancellationTokenSource();
        ThreadPool.QueueUserWorkItem((obj) =>
        {
            var token = (CancellationToken)(obj ?? throw new NullReferenceException());
            while (!token.IsCancellationRequested)
            {
                while (m_messageQueue.TryDequeue(out var msg))
                {
                    m_logger.Log(msg.Item1, msg.Item2);
                }
            }
        }, token.Token);
        var result = Task.WhenAll(taskPool).GetAwaiter().GetResult();
        token.Cancel();

        return result.Any(x => x is BuildResult.Failed) ? BuildResult.Failed : BuildResult.Succeeded;
    }
}