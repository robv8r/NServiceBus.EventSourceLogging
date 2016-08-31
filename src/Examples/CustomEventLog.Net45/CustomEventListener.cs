// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomEventListener.cs" company="Rob Winningham">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.Diagnostics.Tracing;
    using NServiceBus.EventSourceLogging.Samples.CustomEventLog.Properties;

    /// <summary>
    /// Example event source listener.  Typically, you wouldn't consume events in the same application as you publish them.  This is for example purposes only.
    /// </summary>
    internal class CustomEventListener : EventListener
    {
        private static readonly EventKeywords[] AvailableKeywords =
                {
                    EventSourceLogger.Keywords.Warning,
                    EventSourceLogger.Keywords.Critical,
                    EventSourceLogger.Keywords.Debug,
                    EventSourceLogger.Keywords.Error,
                    EventSourceLogger.Keywords.ExceptionData,
                    EventSourceLogger.Keywords.Informational
                };

        /// <summary>Called whenever an event has been written by an event source for which the event listener has enabled events.</summary>
        /// <param name="eventData">The event arguments that describe the event.</param>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            if (eventData.Message == null)
            {
                return;
            }

            var keywordStrings = GetKeywordStrings(eventData.Keywords);

            var keywords = eventData.Keywords.ToString().Split(' ').Concat(keywordStrings);

            var kj = string.Join(" ", keywords);

            var message = string.Format(CultureInfo.CurrentUICulture, eventData.Message, eventData.Payload?.ToArray() ?? new object[0]);

            Console.WriteLine(
                string.Format(
                    CultureInfo.CurrentUICulture,
                    Resources.EventTraceOutput,
                    eventData.EventId,
                    eventData.Channel,
                    kj,
                    eventData.EventName,
                    eventData.Level,
                    message));
        }

        [NotNull]
        private static IEnumerable<string> GetKeywordStrings(EventKeywords keyword)
        {
            foreach (var kw in AvailableKeywords)
            {
                if ((keyword & kw) != kw)
                {
                    continue;
                }

                switch (kw)
                {
                    case EventSourceLogger.Keywords.Warning:
                        yield return nameof(EventSourceLogger.Keywords.Warning);
                        break;
                    case EventSourceLogger.Keywords.Critical:
                        yield return nameof(EventSourceLogger.Keywords.Critical);
                        break;
                    case EventSourceLogger.Keywords.Debug:
                        yield return nameof(EventSourceLogger.Keywords.Debug);
                        break;
                    case EventSourceLogger.Keywords.Error:
                        yield return nameof(EventSourceLogger.Keywords.Error);
                        break;
                    case EventSourceLogger.Keywords.ExceptionData:
                        yield return nameof(EventSourceLogger.Keywords.ExceptionData);
                        break;
                    case EventSourceLogger.Keywords.Informational:
                        yield return nameof(EventSourceLogger.Keywords.Informational);
                        break;
                }
            }
        }
    }
}