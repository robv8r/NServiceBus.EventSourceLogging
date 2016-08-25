// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSourceLoggerBase.cs" company="Rob Winningham">
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
    using System;
    using System.Globalization;
    using JetBrains.Annotations;
    using NServiceBus.EventSourceLogging.Properties;
#if USEMDT
    using Microsoft.Diagnostics.Tracing;
#else
    using System.Diagnostics.Tracing;
#endif

    /// <summary>
    ///     Creates ETW events for NServiceBus.
    /// </summary>
    public abstract class EventSourceLoggerBase : EventSource, IEventSourceLogger
    {
        private static readonly string InvalidFormatErrorString = Resources.InvalidFormatErrorString;

        /// <summary>
        ///     Gets a value indicating whether Debug level logging is enabled.
        /// </summary>
        public bool IsDebugEnabled
            => this.IsEnabled(EventLevel.Verbose, EventKeywords.All, EventChannel.Debug);

        /// <summary>
        ///     Gets a value indicating whether Error level logging is enabled.
        /// </summary>
        public bool IsErrorEnabled
            => this.IsEnabled(EventLevel.Error, EventKeywords.All, EventChannel.Operational);

        /// <summary>
        ///     Gets a value indicating whether Fatal level logging is enabled.
        /// </summary>
        public bool IsFatalEnabled
            => this.IsEnabled(EventLevel.Critical, EventKeywords.All, EventChannel.Operational);

        /// <summary>
        ///     Gets a value indicating whether Informational level logging is enabled.
        /// </summary>
        public bool IsInfoEnabled
            => this.IsEnabled(EventLevel.Informational, EventKeywords.All, EventChannel.Operational);

        /// <summary>
        ///     Gets a value indicating whether Warning level logging is enabled.
        /// </summary>
        public bool IsWarnEnabled
            => this.IsEnabled(EventLevel.Warning, EventKeywords.All, EventChannel.Operational);

        /// <inheritdoc />
        public abstract IEventSourceLogger Log
        {
            get;
        }

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        public virtual void Debug(string logger, string message)
        {
            if (this.IsDebugEnabled)
            {
                this.WriteEvent(EventId.Debug, logger, message);
            }
        }

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        public virtual void Debug(string logger, string message, Exception exception)
        {
            if (!this.IsDebugEnabled)
            {
                return;
            }

            if (exception == null)
            {
                this.Debug(logger, message);
            }
            else
            {
                this.DebugException(
                    logger,
                    message,
                    exception.GetType().FullName,
                    exception.Message,
                    exception.ToString());
            }
        }

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        ///     It does so by converting the value of objects to strings based on the formats specified and inserting them into
        ///     another string.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public virtual void Debug(string logger, string format, params object[] args)
        {
            if (!this.IsDebugEnabled || format == null || args == null)
            {
                return;
            }

            try
            {
                this.Debug(logger, string.Format(CultureInfo.CurrentCulture, format, args));
            }
            catch (FormatException ex)
            {
                this.Debug(logger, InvalidFormatErrorString + format, ex);
            }
        }

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        public virtual void DebugException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsDebugEnabled)
            {
                this.WriteEvent(EventId.DebugException, logger, message, exceptionType, exceptionMessage, exceptionValue);
            }
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        public virtual void Error(string logger, string message)
        {
            if (this.IsErrorEnabled)
            {
                this.WriteEvent(EventId.Error, logger, message);
            }
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        public virtual void Error(string logger, string message, Exception exception)
        {
            if (!this.IsErrorEnabled)
            {
                return;
            }

            if (exception == null)
            {
                this.Error(logger, message);
            }
            else
            {
                this.ErrorException(
                    logger,
                    message,
                    exception.GetType().FullName,
                    exception.Message,
                    exception.ToString());
            }
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public virtual void Error(string logger, string format, params object[] args)
        {
            if (!this.IsErrorEnabled || format == null || args == null)
            {
                return;
            }

            try
            {
                this.Error(logger, string.Format(CultureInfo.CurrentCulture, format, args));
            }
            catch (FormatException ex)
            {
                this.Error(logger, InvalidFormatErrorString + format, ex);
            }
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        public virtual void ErrorException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsErrorEnabled)
            {
                this.WriteEvent(EventId.ErrorException, logger, message, exceptionType, exceptionMessage, exceptionValue);
            }
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        public virtual void Fatal(string logger, string message)
        {
            if (this.IsFatalEnabled)
            {
                this.WriteEvent(EventId.Fatal, logger, message);
            }
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        public virtual void Fatal(string logger, string message, Exception exception)
        {
            if (!this.IsFatalEnabled)
            {
                return;
            }

            if (exception == null)
            {
                this.Fatal(logger, message);
            }
            else
            {
                this.FatalException(
                    logger,
                    message,
                    exception.GetType().FullName,
                    exception.Message,
                    exception.ToString());
            }
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public virtual void Fatal(string logger, string format, params object[] args)
        {
            if (!this.IsFatalEnabled || format == null || args == null)
            {
                return;
            }

            try
            {
                this.Fatal(logger, string.Format(CultureInfo.CurrentCulture, format, args));
            }
            catch (FormatException ex)
            {
                this.Fatal(logger, InvalidFormatErrorString + format, ex);
            }
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        public virtual void FatalException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsFatalEnabled)
            {
                this.WriteEvent(EventId.FatalException, logger, message, exceptionType, exceptionMessage, exceptionValue);
            }
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        public virtual void Info(string logger, string message)
        {
            if (this.IsInfoEnabled)
            {
                this.WriteEvent(EventId.Info, logger, message);
            }
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        public virtual void Info(string logger, string message, Exception exception)
        {
            if (!this.IsInfoEnabled)
            {
                return;
            }

            if (exception == null)
            {
                this.Info(logger, message);
            }
            else
            {
                this.InfoException(
                    logger,
                    message,
                    exception.GetType().FullName,
                    exception.Message,
                    exception.ToString());
            }
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public virtual void Info(string logger, string format, params object[] args)
        {
            if (!this.IsInfoEnabled || format == null || args == null)
            {
                return;
            }

            try
            {
                this.Info(logger, string.Format(CultureInfo.CurrentCulture, format, args));
            }
            catch (FormatException ex)
            {
                this.Info(logger, InvalidFormatErrorString + format, ex);
            }
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        public virtual void InfoException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsInfoEnabled)
            {
                this.WriteEvent(EventId.InfoException, logger, message, exceptionType, exceptionMessage, exceptionValue);
            }
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        public virtual void Warn(string logger, string message)
        {
            if (this.IsWarnEnabled)
            {
                this.WriteEvent(EventId.Warn, logger, message);
            }
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        public virtual void Warn(string logger, string message, Exception exception)
        {
            if (!this.IsWarnEnabled)
            {
                return;
            }

            if (exception == null)
            {
                this.Warn(logger, message);
            }
            else
            {
                this.WarnException(
                    logger,
                    message,
                    exception.GetType().FullName,
                    exception.Message,
                    exception.ToString());
            }
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public virtual void Warn(string logger, string format, params object[] args)
        {
            if (!this.IsWarnEnabled || format == null || args == null)
            {
                return;
            }

            try
            {
                this.Warn(logger, string.Format(CultureInfo.CurrentCulture, format, args));
            }
            catch (FormatException ex)
            {
                this.Warn(logger, InvalidFormatErrorString + format, ex);
            }
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exceptionType">The exception type to be logged.</param>
        /// <param name="exceptionMessage">The exception message to be logged.</param>
        /// <param name="exceptionValue">The string representation of the exception to be logged. This includes the stack trace.</param>
        public virtual void WarnException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsWarnEnabled)
            {
                this.WriteEvent(EventId.WarnException, logger, message, exceptionType, exceptionMessage, exceptionValue);
            }
        }

        /// <summary>
        ///     Override of the <see cref="WriteEvent" /> method to improve performance when writing an event with five strings.
        /// </summary>
        /// <param name="eventId">The </param>
        /// <param name="arg1">The first string.</param>
        /// <param name="arg2">The second string.</param>
        /// <param name="arg3">The third string.</param>
        /// <param name="arg4">The fourth string.</param>
        /// <param name="arg5">The fifth string.</param>
        [NonEvent]
        private unsafe void WriteEvent(
            int eventId,
            [CanBeNull] string arg1,
            [CanBeNull] string arg2,
            [CanBeNull] string arg3,
            [CanBeNull] string arg4,
            [CanBeNull] string arg5)
        {
            if (!this.IsEnabled())
            {
                return;
            }

            if (arg1 == null)
            {
                arg1 = string.Empty;
            }

            if (arg2 == null)
            {
                arg2 = string.Empty;
            }

            if (arg3 == null)
            {
                arg3 = string.Empty;
            }

            if (arg4 == null)
            {
                arg4 = string.Empty;
            }

            if (arg5 == null)
            {
                arg5 = string.Empty;
            }

            fixed (char* cp1 = arg1)
            {
                fixed (char* cp2 = arg2)
                {
                    fixed (char* cp3 = arg3)
                    {
                        fixed (char* cp4 = arg4)
                        {
                            fixed (char* cp5 = arg5)
                            {
                                EventData* data = stackalloc EventData[5];
                                data[0].DataPointer = (IntPtr)cp1;
                                data[0].Size = (arg1.Length + 1) * 2;
                                data[1].DataPointer = (IntPtr)cp2;
                                data[1].Size = (arg2.Length + 1) * 2;
                                data[2].DataPointer = (IntPtr)cp3;
                                data[2].Size = (arg3.Length + 1) * 2;
                                data[3].DataPointer = (IntPtr)cp4;
                                data[3].Size = (arg4.Length + 1) * 2;
                                data[4].DataPointer = (IntPtr)cp5;
                                data[4].Size = (arg5.Length + 1) * 2;
                                this.WriteEventCore(eventId, 5, data);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Supplies event ids.
        /// </summary>
        protected static class EventId
        {
            /// <summary>
            ///     The Debug event id.
            /// </summary>
            public const int Debug = 1;

            /// <summary>
            ///     The DebugException event id.
            /// </summary>
            public const int DebugException = 2;

            /// <summary>
            ///     The Error event id.
            /// </summary>
            public const int Error = 3;

            /// <summary>
            ///     The ErrorException event id.
            /// </summary>
            public const int ErrorException = 4;

            /// <summary>
            ///     The Fatal event id.
            /// </summary>
            public const int Fatal = 5;

            /// <summary>
            ///     The FatalException event id.
            /// </summary>
            public const int FatalException = 6;

            /// <summary>
            ///     The Info event id.
            /// </summary>
            public const int Info = 7;

            /// <summary>
            ///     The InfoException event id.
            /// </summary>
            public const int InfoException = 8;

            /// <summary>
            ///     The Warn event id.
            /// </summary>
            public const int Warn = 9;

            /// <summary>
            ///     The WarnException event id.
            /// </summary>
            public const int WarnException = 10;
        }
    }
}