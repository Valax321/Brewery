using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Class that handles the dispatching of build command tasks.
/// </summary>
public class BuildCommandDispatcher
{
    private readonly Queue<IBuildTask> m_tasks = new();
    private readonly IServiceProvider m_services;

    /// <summary>
    /// The number of tasks queued to execute.
    /// Includes parallel tasks.
    /// </summary>
    public int TaskCount
    {
        get
        {
            var tasks = 0;
            foreach (var task in m_tasks)
            {
                if (task is ParallelBuildTask pt)
                    tasks += pt.TaskCount;
                else tasks++;
            }

            return tasks;
        }
    }

    /// <summary>
    /// Creates a new <see cref="BuildCommandDispatcher"/> instance.
    /// </summary>
    /// <param name="services"></param>
    public BuildCommandDispatcher(IServiceProvider services)
    {
        m_services = services;
    }

    /// <summary>
    /// Runs the given task in sequence.
    /// </summary>
    /// <param name="task">The task to run.</param>
    /// <returns>Self to allow chaining.</returns>
    public BuildCommandDispatcher RunTask(IBuildTask task)
    {
        var logger = m_services.GetRequiredService<ILogger<IBuildTask>>();
        task.Log = (msg, level) => logger.Log(msg, level);
        m_tasks.Enqueue(task);
        return this;
    }

    /// <summary>
    /// Run the specified tasks in parallel on the thread pool.
    /// </summary>
    /// <param name="tasks">The tasks to run.</param>
    /// <returns>Self to allow chaining.</returns>
    public BuildCommandDispatcher RunParallel(IEnumerable<IBuildTask> tasks)
    {
        m_tasks.Enqueue(new ParallelBuildTask(tasks, m_services.GetRequiredService<ILogger<ParallelBuildTask>>()));
        return this;
    }

    /// <summary>
    /// Execute and dequeue the tasks.
    /// </summary>
    public BuildResult ExecuteTasks()
    {
        while (m_tasks.TryDequeue(out var currentTask))
        {
            var result = currentTask.Build();
            if (result == BuildResult.Failed)
                return BuildResult.Failed;
        }

        return BuildResult.Succeeded;
    }
}