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
namespace $rootnamespace$
{
    using Microsoft.Diagnostics.Tracing;

    /// <summary>
    ///     Creates ETW events for NServiceBus.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         TODO: Replace the EventSource Name "NServiceBus" with something specific to your application
    ///     </para>
    ///     <para>
    ///         See <see href="https://github.com/Microsoft/dotnetsamples/blob/master/Microsoft.Diagnostics.Tracing/EventSource/docs/EventSource.md"/>
    ///	        for more information about naming Event Sources.
    ///     </para>
    /// </remarks>
    [EventSource(
        Name = "NServiceBus",
        LocalizationResources = "NServiceBus.EventSourceLogging.Properties.Resources")]
    public sealed class EventSourceLogger : EventSourceLoggerBase
    {
        private static readonly EventSourceLogger SingletonLog = new EventSourceLogger();

        private EventSourceLogger()
        {
        }

        /// <summary>
        ///     Gets an instance of an <see cref="IEventSourceLogger" />.
        /// </summary>
        public static IEventSourceLogger Log => SingletonLog;

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [Event(1, Level = EventLevel.Verbose)]
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
        [Event(2, Level = EventLevel.Verbose)]
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
        [Event(3, Level = EventLevel.Error)]
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
        [Event(4, Level = EventLevel.Error)]
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
        [Event(5, Level = EventLevel.Critical)]
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
        [Event(6, Level = EventLevel.Critical)]
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
        [Event(7, Level = EventLevel.Informational)]
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
        [Event(8, Level = EventLevel.Informational)]
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
        [Event(9, Level = EventLevel.Warning)]
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
        [Event(10, Level = EventLevel.Warning)]
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