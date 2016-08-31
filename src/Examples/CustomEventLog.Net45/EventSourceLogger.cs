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
namespace NServiceBus.EventSourceLogging.Samples.CustomEventLog
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Diagnostics.Tracing;
    using NServiceBus.EventSourceLogging;

    /// <summary>
    ///     Creates ETW events for NServiceBus that write to a custom Event Log.
    /// </summary>
    [PublicAPI]
    [EventSource(
        Name = "NServiceBus-Samples-EventSourceLoggerLoggingNet46",
        LocalizationResources = "NServiceBus.EventSourceLogging.Samples.CustomEventLog.Properties.Resources")]
    public sealed class EventSourceLogger : EventSourceLoggerBase
    {
        private EventSourceLogger()
        {
        }

        /// <summary>
        ///     Gets an instance of an <see cref="EventSourceLogger" />.
        /// </summary>
        public static EventSourceLogger Log { get; } = new EventSourceLogger();

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [Event(9, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Keywords = Keywords.Debug)]
        public override void Debug(string logger, string message)
        {
            base.Debug(logger, message);
        }

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        [Event(10, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Keywords = Keywords.Debug | Keywords.ExceptionData)]
        public override void DebugException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.DebugException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [Event(3, Level = EventLevel.Error, Channel = EventChannel.Operational, Keywords = Keywords.Error)]
        public override void Error(string logger, string message)
        {
            base.Error(logger, message);
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        [Event(4, Level = EventLevel.Error, Channel = EventChannel.Operational, Keywords = Keywords.Error | Keywords.ExceptionData)]
        public override void ErrorException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.ErrorException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [Event(11, Level = EventLevel.Critical, Channel = EventChannel.Operational, Keywords = Keywords.Critical)]
        public override void Fatal(string logger, string message)
        {
            base.Fatal(logger, message);
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        [Event(2, Level = EventLevel.Critical, Channel = EventChannel.Operational, Keywords = Keywords.Critical | Keywords.ExceptionData)]
        public override void FatalException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.FatalException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [Event(7, Level = EventLevel.Informational, Channel = EventChannel.Operational, Keywords = Keywords.Informational)]
        public override void Info(string logger, string message)
        {
            base.Info(logger, message);
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        [Event(8, Level = EventLevel.Informational, Channel = EventChannel.Operational, Keywords = Keywords.Informational | Keywords.ExceptionData)]
        public override void InfoException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.InfoException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [Event(5, Level = EventLevel.Warning, Channel = EventChannel.Operational, Keywords = Keywords.Warning)]
        public override void Warn(string logger, string message)
        {
            base.Warn(logger, message);
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        [Event(6, Level = EventLevel.Warning, Channel = EventChannel.Operational, Keywords = Keywords.Warning | Keywords.ExceptionData)]
        public override void WarnException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            base.WarnException(logger, message, exceptionType, exceptionMessage, exceptionValue);
        }

        /// <summary>
        ///     Contains constants that can be used to create a bit-vector that represent <see cref="EventKeywords"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for EventSource API.")]
        [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Required for EventSource API.")]
        public class Keywords
        {
            /// <summary>
            ///     Indicates whether an event contains information about an <see cref="System.Exception"/>.
            /// </summary>
            public const EventKeywords ExceptionData = (EventKeywords)0x0001;

            /// <summary>
            ///     Indicates whether an event contains Debug information.
            /// </summary>
            public const EventKeywords Debug = (EventKeywords)0x0002;

            /// <summary>
            ///     Indicates whether an event contains Warning information.
            /// </summary>
            public const EventKeywords Warning = (EventKeywords)0x0004;

            /// <summary>
            ///     Indicates whether an event contains Informational information.
            /// </summary>
            public const EventKeywords Informational = (EventKeywords)0x0008;

            /// <summary>
            ///     Indicates whether an event contains Critical information.
            /// </summary>
            public const EventKeywords Critical = (EventKeywords)0x0010;

            /// <summary>
            ///     Indicates whether an event contains Error information.
            /// </summary>
            public const EventKeywords Error = (EventKeywords)0x0020;
        }
    }
}