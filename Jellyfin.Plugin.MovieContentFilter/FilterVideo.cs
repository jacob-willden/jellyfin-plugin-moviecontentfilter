/*
This file is part of the Movie Content Filter Jellyfin Plugin Project.

Movie Content Filter Jellyfin Plugin Project Copyright (C) 2022 Jacob Willden
(Released under the GNU General Public License (GNU GPL) Version 3.0)
Source Code Link: https://github.com/jacob-willden/jellyfin-plugin-moviecontentfilter

Intro Skipper Jellyfin Plugin Copyright (C) 2022 ConfusedPolarBear
(Released under the GNU General Public License (GNU GPL) Version 3.0)
Source Code Link: https://github.com/ConfusedPolarBear/intro-skipper

VideoSkip Browser Extension Copyright (C) 2020, 2021, 2022 Francisco Ruiz
(Released under the GNU General Public License (GNU GPL) Version 3.0 or later)
Source Code Link: https://github.com/fruiz500/VideoSkip-extension/

Much of the code below was derived and modified from the "AutoSkip.cs"
source code file in the Intro Skipper repository (source link above). 
Except where stated otherwise, the FilterVideo class below and its 
properties and methods are derived and modified from the AutoSkip 
class in the "AutoSkip.cs" file from Intro Skipper. Other lines of 
code below were derived and modified from several source code files 
in the VideoSkip browser extension repository (source link above), 
including "content1.js" and "content2.js", and is explicitly 
labeled as so.

Afformentioned source code derived and modified by Jacob Willden
Start Date of Derivation/Modification: November 20, 2020
Most Recent Date of Derivation/Modification: September 5, 2022

"Movie Content Filter" Website Copyright (C) delight.im
Website Link: https://www.moviecontentfilter.com/

The Movie Content Filter Jellyfin Plugin Project is free software: 
you can redistribute it and/or modify it under the terms of the GNU
General Public License (GNU GPL) as published by the Free Software
Foundation, either version 3 of the License, or (at your option)
any later version. The project is distributed WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the GNU GPL for more details.

As additional permission under GNU GPL version 3 section 7, you
may distribute non-source (e.g., minimized or compacted) forms of
the code without the copy of the GNU GPL normally required by
section 4, provided you include this license notice and a URL
through which recipients can access the Corresponding Source.

You should have recieved a copy of the GNU General Public License
along with this project. Otherwise, see: https://www.gnu.org/licenses/
*/

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

namespace Jellyfin.Plugin.MovieContentFilter;

/// <summary>
/// The heart of the plugin.
/// </summary>
public class FilterVideo : IServerEntryPoint
{
    private ISessionManager _sessionManager;
    private System.Timers.Timer _playbackTimer = new(1000);
    private ILogger<FilterVideo> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterVideo"/> class.
    /// </summary>
    /// <param name="sessionManager">Session manager.</param>
    /// <param name="logger">Logger.</param>
    public FilterVideo(ISessionManager sessionManager, ILogger<FilterVideo> logger) {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    /// <summary>
    /// Runs at Jellyfin start.
    /// </summary>
    /// <returns>Task.</returns>
    public Task RunAsync()
    {
        _logger.LogDebug("Hello World!");
        _playbackTimer.Elapsed += DoTheFiltering;
        _playbackTimer.Enabled = true;
        //_logger.LogDebug("Session count: " + _sessionManager.Sessions.ToList().Count()); // returns 0
        return Task.CompletedTask;
    }

    // Modifed from the PlaybackTimer_Elapsed method in the AutoSkip class (Intro Skipper)
    private void DoTheFiltering(object? sender, EventArgs e)
    {
        // _logger.LogDebug("Start DoTheFiltering method");
        foreach (var session in _sessionManager.Sessions)
        {
            // var sessionString = session.ToString();
            // _logger.LogDebug("sessionString: {0}", sessionString);

            var deviceId = session.DeviceId;
            var itemId = session.NowPlayingItem.Id;
            var position = session.PlayState.PositionTicks / TimeSpan.TicksPerSecond; // in seconds
            // _logger.LogDebug("Playback position is " + position.ToString());

            int tagStart = 2;
            int tagEnd = 4;

            if (position < tagStart || position > tagEnd)
            {
                continue;
            }
            else {
                _logger.LogDebug("Playback position is " + position.ToString());

                _sessionManager.SendMessageCommand(
                session.Id,
                session.Id,
                new MessageCommand()
                {
                    Header = string.Empty,      // some clients require header to be a string instead of null
                    Text = "Notice: The performance of the motion picture is altered from the performance intended by the director or copyright holder of the motion picture.",
                    TimeoutMs = 2000,
                },
                CancellationToken.None);
                

                _sessionManager.SendPlaystateCommand(
                session.Id,
                session.Id,
                new PlaystateRequest()
                {
                    Command = PlaystateCommand.Seek,
                    ControllingUserId = session.UserId.ToString("N"),
                    SeekPositionTicks = tagEnd * TimeSpan.TicksPerSecond,
                },
                CancellationToken.None);
            }
        }
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

        _playbackTimer.Stop();
        _playbackTimer.Dispose();
    }
}