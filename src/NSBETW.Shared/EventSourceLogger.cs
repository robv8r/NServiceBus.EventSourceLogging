// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSourceLogger.cs" company="Rob Winningham">
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
namespace NServiceBus.EventSourceLogging
{
    using JetBrains.Annotations;
#if USEMDT
    using Microsoft.Diagnostics.Tracing;
#else
    using System.Diagnostics.Tracing;
#endif

    /// <summary>
    ///     Creates ETW events for NServiceBus that write to a custom Event Log.
    /// </summary>
    [PublicAPI]
    [EventSource(Name = "NServiceBus-Logging")]
    public sealed class EventSourceLogger : EventSourceLoggerBase
    {
        private static readonly EventSourceLogger SingletonLog = new EventSourceLogger();

        private EventSourceLogger()
        {
        }

        /// <inheritdoc />
        public override IEventSourceLogger Log => SingletonLog;

        /// <inheritdoc />
        [Event(EventId.Debug, Level = EventLevel.Verbose, Message = "{0} : {1}")]
        public override void Debug(string logger, string message)
        {
            base.Debug(logger, message);
        }

        /// <inheritdoc />
        [Event(EventId.DebugException, Level = EventLevel.Verbose, Message = "{0} : {1} : {2} : {3} : {4}")]
        public override void DebugException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.DebugException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <inheritdoc />
        [Event(EventId.Error, Level = EventLevel.Error, Message = "{0} : {1}")]
        public override void Error(string logger, string message)
        {
            base.Error(logger, message);
        }

        /// <inheritdoc />
        [Event(EventId.ErrorException, Level = EventLevel.Error, Message = "{0} : {1} : {2} : {3} : {4}")]
        public override void ErrorException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.ErrorException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <inheritdoc />
        [Event(EventId.Fatal, Level = EventLevel.Critical, Message = "{0} : {1}")]
        public override void Fatal(string logger, string message)
        {
            base.Fatal(logger, message);
        }

        /// <inheritdoc />
        [Event(EventId.FatalException, Level = EventLevel.Critical, Message = "{0} : {1} : {2} : {3} : {4}")]
        public override void FatalException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.FatalException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <inheritdoc />
        [Event(EventId.Info, Level = EventLevel.Informational, Message = "{0} : {1}")]
        public override void Info(string logger, string message)
        {
            base.Info(logger, message);
        }

        /// <inheritdoc />
        [Event(EventId.InfoException, Level = EventLevel.Informational, Message = "{0} : {1} : {2} : {3} : {4}")]
        public override void InfoException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.InfoException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <inheritdoc />
        [Event(EventId.Warn, Level = EventLevel.Warning, Message = "{0} : {1}")]
        public override void Warn(string logger, string message)
        {
            base.Warn(logger, message);
        }

        /// <inheritdoc />
        [Event(EventId.WarnException, Level = EventLevel.Warning, Message = "{0} : {1} : {2} : {3} : {4}")]
        public override void WarnException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.WarnException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }
    }
}