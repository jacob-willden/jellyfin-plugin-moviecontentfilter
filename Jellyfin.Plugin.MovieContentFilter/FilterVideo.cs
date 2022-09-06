using System;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Session;

namespace Jellyfin.Plugin.Test;

/// <summary>
/// The heart of the plugin.
/// </summary>
public class TestClass : IServerEntryPoint
{
    private ILogger<TestClass> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClass"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public TestClass(ILogger<TestClass> logger) {
        _logger = logger;
    }

    /// <summary>
    /// Runs at Jellyfin start.
    /// </summary>
    /// <returns>Task.</returns>
    public Task RunAsync()
    {
        _logger.LogDebug("Hello World!");
        return Task.CompletedTask;
    }
    /// <summary>
    /// Dispose.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }
    }
}

// Remember to Credit Intro Skipper by ConfusedPolarBear on Github