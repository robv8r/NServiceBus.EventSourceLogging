// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Rob Winningham">
//   MIT License
//
//   Copyright (c) 2016 Rob Winningham
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//   SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace NServiceBus.EventSourceLogging.Samples.CustomEventLog
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Tracing;
    using JetBrains.Annotations;
    using NServiceBus.EventSourceLogging.Samples.CustomEventLog.Properties;
    using NServiceBus.Logging;

    /// <summary>
    ///     Application host.
    /// </summary>
    [UsedImplicitly]
    internal class Program
    {
        /// <summary>
        ///     Entry point into application.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This example uses an EventListener to output ETW events to the
        ///         <see cref="Console" />.
        ///     </para>
        ///     <para>
        ///         Normally, you would NOT do this.
        ///     </para>
        ///     <para>
        ///         Instead, you'd consume the events using an existing tool such
        ///         as PerfView, Microsoft Message Analyzer, Event Log Viewer,
        ///         or logmon.
        ///     </para>
        /// </remarks>
        private static void Main()
        {
            using (var listener = new CustomEventSourceListener())
            {
                listener.EnableEvents(CustomEventLogEventSource.Log, EventLevel.Informational);

                // Configure Logger
                var logManager = LogManager.Use<EventSourceLoggingFactory>();

                if (logManager == null)
                {
                    throw new InvalidOperationException("Logger is null.");
                }

                logManager.WithLogger(CustomEventLogEventSource.Log);

                var logger = LogManager.GetLogger("Example");

                if (logger == null)
                {
                    throw new InvalidOperationException("Logger is null.");
                }

                logger.DebugFormat("{0}", "My Debug Message");
                logger.InfoFormat("{0}", "My Info Message");
                logger.WarnFormat("{0}", "My Warn Message");
                logger.ErrorFormat("{0}", "My Error Message");
                logger.FatalFormat("{0}", "My Fatal Message");

                // Start using NServiceBus
                var busConfig = new BusConfiguration();
                busConfig.EndpointName("EventSourceSample");
                busConfig.UseSerialization<JsonSerializer>();
                busConfig.EnableInstallers();
                busConfig.UsePersistence<InMemoryPersistence>();
                using (var bus = Bus.Create(busConfig))
                {
                    Debug.Assert(bus != null, "bus != null");
                    bus.Start();
                    Console.WriteLine(Resources.PressAnyKeyToStopProgram);
                    Console.Read();
                }
            }
        }
    }
}