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
    using System.Diagnostics.CodeAnalysis;
    using NServiceBus;
    using NServiceBus.EventSourceLogging;
    using NServiceBus.Logging;

    /// <summary>
    ///     Application host.
    /// </summary>
    [SuppressMessage(
        "ReSharper",
        "ClassNeverInstantiated.Global",
        Justification = "Entry point into application.")]
    internal class Program
    {
        /// <summary>
        ///     Entry point into application.
        /// </summary>
        private static void Main()
        {
            // Configure Logger
            var logManager = LogManager.Use<EventSourceLoggingFactory>();
            Debug.Assert(logManager != null, "logManager != null");
            logManager.WithLogger(CustomEventLogEventSource.Log);

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
                Console.WriteLine(@"Press any key to stop program");
                Console.Read();
            }
        }
    }
}