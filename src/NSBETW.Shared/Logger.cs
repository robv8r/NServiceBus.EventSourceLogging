// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Rob Winningham">
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
    using NServiceBus.Logging;

    /// <summary>
    ///     Provides logging methods and utility functions.
    /// </summary>
    public class Logger : ILog
    {
        private const string DefaultLoggerName = "Default";

        private static readonly EventLogEventSource EventSourceLogger = EventLogEventSource.Log;

        private readonly string loggerName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Logger" /> class with the given <paramref name="loggerName" />.
        /// </summary>
        /// <param name="loggerName">The name of the <see cref="Logger" /> to create.</param>
        public Logger(string loggerName)
        {
            this.loggerName = string.IsNullOrWhiteSpace(loggerName) ? DefaultLoggerName : loggerName;
        }

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <see cref="F:NServiceBus.Logging.LogLevel.Debug" />
        ///     level.
        /// </summary>
        public bool IsDebugEnabled => EventSourceLogger.IsDebugEnabled;

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <see cref="F:NServiceBus.Logging.LogLevel.Error" />
        ///     level.
        /// </summary>
        public bool IsErrorEnabled => EventSourceLogger.IsErrorEnabled;

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <see cref="F:NServiceBus.Logging.LogLevel.Fatal" />
        ///     level.
        /// </summary>
        public bool IsFatalEnabled => EventSourceLogger.IsFatalEnabled;

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <see cref="F:NServiceBus.Logging.LogLevel.Info" />
        ///     level.
        /// </summary>
        public bool IsInfoEnabled => EventSourceLogger.IsInfoEnabled;

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <see cref="F:NServiceBus.Logging.LogLevel.Warn" />
        ///     level.
        /// </summary>
        public bool IsWarnEnabled => EventSourceLogger.IsWarnEnabled;

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Debug" /> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Debug(string message)
        {
            EventSourceLogger.Debug(this.loggerName, message);
        }

        /// <summary>
        ///     Writes the message and exception at the <see cref="F:NServiceBus.Logging.LogLevel.Debug" /> level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        public void Debug(string message, Exception exception)
        {
            EventSourceLogger.Debug(this.loggerName, message, exception);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Debug" /> level using the specified
        ///     <paramref name="format" /> provider and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        public void DebugFormat(string format, params object[] args)
        {
            EventSourceLogger.Debug(this.loggerName, format, args);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Error" /> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Error(string message)
        {
            EventSourceLogger.Error(this.loggerName, message);
        }

        /// <summary>
        ///     Writes the message and exception at the <see cref="F:NServiceBus.Logging.LogLevel.Error" /> level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        public void Error(string message, Exception exception)
        {
            EventSourceLogger.Error(this.loggerName, message, exception);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Error" /> level using the specified
        ///     <paramref name="format" /> provider and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            EventSourceLogger.Error(this.loggerName, format, args);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Fatal" /> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Fatal(string message)
        {
            EventSourceLogger.Fatal(this.loggerName, message);
        }

        /// <summary>
        ///     Writes the message and exception at the <see cref="F:NServiceBus.Logging.LogLevel.Fatal" /> level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        public void Fatal(string message, Exception exception)
        {
            EventSourceLogger.Fatal(this.loggerName, message, exception);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Fatal" /> level using the specified
        ///     <paramref name="format" /> provider and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        public void FatalFormat(string format, params object[] args)
        {
            EventSourceLogger.Fatal(this.loggerName, format, args);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Info" /> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Info(string message)
        {
            EventSourceLogger.Info(this.loggerName, message);
        }

        /// <summary>
        ///     Writes the message and exception at the <see cref="F:NServiceBus.Logging.LogLevel.Info" /> level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        public void Info(string message, Exception exception)
        {
            EventSourceLogger.Info(this.loggerName, message, exception);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Info" /> level using the specified
        ///     <paramref name="format" /> provider and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        public void InfoFormat(string format, params object[] args)
        {
            EventSourceLogger.Info(this.loggerName, format, args);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Warn" /> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Warn(string message)
        {
            EventSourceLogger.Warn(this.loggerName, message);
        }

        /// <summary>
        ///     Writes the message and exception at the <see cref="F:NServiceBus.Logging.LogLevel.Warn" /> level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        public void Warn(string message, Exception exception)
        {
            EventSourceLogger.Warn(this.loggerName, message, exception);
        }

        /// <summary>
        ///     Writes the message at the <see cref="F:NServiceBus.Logging.LogLevel.Warn" /> level using the specified
        ///     <paramref name="format" /> provider and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        public void WarnFormat(string format, params object[] args)
        {
            EventSourceLogger.Warn(this.loggerName, format, args);
        }
    }
}