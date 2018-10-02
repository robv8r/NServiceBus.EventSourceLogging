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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
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

        private bool isDebugConfigured;
        private bool isDebugExceptionConfigured;
        private bool isErrorConfigured;
        private bool isErrorExceptionConfigured;
        private bool isFatalConfigured;
        private bool isFatalExceptionConfigured;
        private bool isInfoConfigured;
        private bool isInfoExceptionConfigured;
        private bool isWarnConfigured;
        private bool isWarnExceptionConfigured;

        private EventChannel debugChannel = EventChannel.None;
        private int debugEventId = NServiceBusEtwConstants.DebugEventId;
        private EventChannel debugExceptionChannel = EventChannel.None;
        private int debugExceptionEventId = NServiceBusEtwConstants.DebugExceptionEventId;
        private EventKeywords debugExceptionKeywords = EventKeywords.None;
        private EventLevel debugExceptionLevel = EventLevel.Verbose;
        private EventKeywords debugKeywords = EventKeywords.None;
        private EventLevel debugLevel = EventLevel.Verbose;
        private EventChannel errorChannel = EventChannel.None;
        private int errorEventId = NServiceBusEtwConstants.ErrorEventId;
        private EventChannel errorExceptionChannel = EventChannel.None;
        private int errorExceptionEventId = NServiceBusEtwConstants.ErrorExceptionEventId;
        private EventKeywords errorExceptionKeywords = EventKeywords.None;
        private EventLevel errorExceptionLevel = EventLevel.Error;
        private EventKeywords errorKeywords = EventKeywords.None;
        private EventLevel errorLevel = EventLevel.Error;
        private EventChannel fatalChannel = EventChannel.None;
        private int fatalEventId = NServiceBusEtwConstants.FatalEventId;
        private EventChannel fatalExceptionChannel = EventChannel.None;
        private int fatalExceptionEventId = NServiceBusEtwConstants.FatalExceptionEventId;
        private EventKeywords fatalExceptionKeywords = EventKeywords.None;
        private EventLevel fatalExceptionLevel = EventLevel.Critical;
        private EventKeywords fatalKeywords = EventKeywords.None;
        private EventLevel fatalLevel = EventLevel.Critical;
        private EventChannel infoChannel = EventChannel.None;
        private int infoEventId = NServiceBusEtwConstants.InfoEventId;
        private EventChannel infoExceptionChannel = EventChannel.None;
        private int infoExceptionEventId = NServiceBusEtwConstants.InfoExceptionEventId;
        private EventKeywords infoExceptionKeywords = EventKeywords.None;
        private EventLevel infoExceptionLevel = EventLevel.Informational;
        private EventKeywords infoKeywords = EventKeywords.None;
        private EventLevel infoLevel = EventLevel.Informational;
        private EventChannel warnChannel = EventChannel.None;
        private int warnEventId = NServiceBusEtwConstants.WarnEventId;
        private EventChannel warnExceptionChannel = EventChannel.None;
        private int warnExceptionEventId = NServiceBusEtwConstants.WarnExceptionEventId;
        private EventKeywords warnExceptionKeywords = EventKeywords.None;
        private EventLevel warnExceptionLevel = EventLevel.Warning;
        private EventKeywords warnKeywords = EventKeywords.None;
        private EventLevel warnLevel = EventLevel.Warning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourceLoggerBase" /> class.
        /// </summary>
        protected EventSourceLoggerBase()
        {
            var manifestXml = GenerateManifest(
                this.GetType(),
                this.GetType().Assembly.Location,
                EventManifestOptions.AllowEventSourceOverride);

            if (manifestXml == null)
            {
                return;
            }

            var eventSourceManifest = new EventSourceManifest(manifestXml);
            var attributes = eventSourceManifest.EventAttributes;

            foreach (var attribute in attributes)
            {
                this.SetEventDefinition(attribute);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Debug level logging is enabled.
        /// </summary>
        public bool IsDebugEnabled => this.IsDebugEventEnabled || this.IsDebugExceptionEventEnabled;

        /// <summary>
        ///     Gets a value indicating whether Error level logging is enabled.
        /// </summary>
        public bool IsErrorEnabled => this.IsErrorEventEnabled || this.IsErrorExceptionEventEnabled;

        /// <summary>
        ///     Gets a value indicating whether Fatal level logging is enabled.
        /// </summary>
        public bool IsFatalEnabled => this.IsFatalEventEnabled || this.IsFatalExceptionEventEnabled;

        /// <summary>
        ///     Gets a value indicating whether Informational level logging is enabled.
        /// </summary>
        public bool IsInfoEnabled => this.IsInfoEventEnabled || this.IsInfoExceptionEventEnabled;

        /// <summary>
        ///     Gets a value indicating whether Warning level logging is enabled.
        /// </summary>
        public bool IsWarnEnabled => this.IsWarnEventEnabled || this.IsWarnExceptionEventEnabled;

        /// <summary>
        ///     Gets a value indicating whether the Debug event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsDebugEventEnabled => this.isDebugConfigured && this.IsEnabled(this.debugLevel, this.debugKeywords, this.debugChannel);

        /// <summary>
        ///     Gets a value indicating whether the DebugException event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsDebugExceptionEventEnabled =>
            this.isDebugExceptionConfigured && this.IsEnabled(this.debugExceptionLevel, this.debugExceptionKeywords, this.debugExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Error event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsErrorEventEnabled =>
            this.isErrorConfigured && this.IsEnabled(this.errorLevel, this.errorKeywords, this.errorChannel);

        /// <summary>
        ///     Gets a value indicating whether the ErrorException event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsErrorExceptionEventEnabled =>
            this.isErrorExceptionConfigured && this.IsEnabled(this.errorExceptionLevel, this.errorExceptionKeywords, this.errorExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Fatal event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsFatalEventEnabled =>
            this.isFatalConfigured && this.IsEnabled(this.fatalLevel, this.fatalKeywords, this.fatalChannel);

        /// <summary>
        ///     Gets a value indicating whether the FatalException event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsFatalExceptionEventEnabled =>
            this.isFatalExceptionConfigured && this.IsEnabled(this.fatalExceptionLevel, this.fatalExceptionKeywords, this.fatalExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Info event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsInfoEventEnabled =>
            this.isInfoConfigured && this.IsEnabled(this.infoLevel, this.infoKeywords, this.infoChannel);

        /// <summary>
        ///     Gets a value indicating whether the InfoException event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsInfoExceptionEventEnabled =>
            this.isInfoExceptionConfigured && this.IsEnabled(this.infoExceptionLevel, this.infoExceptionKeywords, this.infoExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Warn event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsWarnEventEnabled =>
            this.isWarnConfigured && this.IsEnabled(this.warnLevel, this.warnKeywords, this.warnChannel);

        /// <summary>
        ///     Gets a value indicating whether the WarnException event is enabled.
        /// </summary>
        [PublicAPI]
        protected bool IsWarnExceptionEventEnabled =>
            this.isWarnExceptionConfigured && this.IsEnabled(this.warnExceptionLevel, this.warnExceptionKeywords, this.warnExceptionChannel);

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Debug(string logger, string message)
        {
            if (this.IsDebugEnabled)
            {
                this.WriteEvent(this.debugEventId, logger, message);
            }
        }

        /// <summary>
        ///     If Debug level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Debug(string logger, string format, params object[] args)
        {
            if (!this.IsDebugEnabled || format == null)
            {
                return;
            }

            if (args == null)
            {
                this.Debug(logger, format);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void DebugException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsDebugEnabled)
            {
                this.WriteEvent(
                    this.debugExceptionEventId,
                    logger,
                    message,
                    exceptionType,
                    exceptionMessage,
                    exceptionValue);
            }
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Error(string logger, string message)
        {
            if (this.IsErrorEnabled)
            {
                this.WriteEvent(this.errorEventId, logger, message);
            }
        }

        /// <summary>
        ///     If Error level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Error(string logger, string format, params object[] args)
        {
            if (!this.IsErrorEnabled || format == null)
            {
                return;
            }

            if (args == null)
            {
                this.Error(logger, format);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void ErrorException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsErrorEnabled)
            {
                this.WriteEvent(
                    this.errorExceptionEventId,
                    logger,
                    message,
                    exceptionType,
                    exceptionMessage,
                    exceptionValue);
            }
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Fatal(string logger, string message)
        {
            if (this.IsFatalEnabled)
            {
                this.WriteEvent(this.fatalEventId, logger, message);
            }
        }

        /// <summary>
        ///     If Fatal level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Fatal(string logger, string format, params object[] args)
        {
            if (!this.IsFatalEnabled || format == null)
            {
                return;
            }

            if (args == null)
            {
                this.Fatal(logger, format);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void FatalException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsFatalEnabled)
            {
                this.WriteEvent(
                    this.fatalExceptionEventId,
                    logger,
                    message,
                    exceptionType,
                    exceptionMessage,
                    exceptionValue);
            }
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Info(string logger, string message)
        {
            if (this.IsInfoEnabled)
            {
                this.WriteEvent(this.infoEventId, logger, message);
            }
        }

        /// <summary>
        ///     If Informational level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Info(string logger, string format, params object[] args)
        {
            if (!this.IsInfoEnabled || format == null)
            {
                return;
            }

            if (args == null)
            {
                this.Info(logger, format);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void InfoException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsInfoEnabled)
            {
                this.WriteEvent(
                    this.infoExceptionEventId,
                    logger,
                    message,
                    exceptionType,
                    exceptionMessage,
                    exceptionValue);
            }
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Warn(string logger, string message)
        {
            if (this.IsWarnEnabled)
            {
                this.WriteEvent(this.warnEventId, logger, message);
            }
        }

        /// <summary>
        ///     If Warning level logging is enabled, writes an event with the given parameters.
        /// </summary>
        /// <param name="logger">The name of the logger performing the logging.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception to be logged.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Warn(string logger, string format, params object[] args)
        {
            if (!this.IsWarnEnabled || format == null)
            {
                return;
            }

            if (args == null)
            {
                this.Warn(logger, format);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void WarnException(
            string logger,
            string message,
            string exceptionType,
            string exceptionMessage,
            string exceptionValue)
        {
            if (this.IsWarnEnabled)
            {
                this.WriteEvent(
                    this.warnExceptionEventId,
                    logger,
                    message,
                    exceptionType,
                    exceptionMessage,
                    exceptionValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetEventDefinition(KeyValuePair<string, EventAttribute> attribute)
        {
            var eventAttribute = attribute.Value;
            System.Diagnostics.Debug.Assert(eventAttribute != null, "eventAttribute != null");

            switch (attribute.Key)
            {
                case nameof(this.Debug):
                    this.debugChannel = eventAttribute.Channel;
                    this.debugEventId = eventAttribute.EventId;
                    this.debugLevel = eventAttribute.Level;
                    this.debugKeywords = eventAttribute.Keywords;
                    this.isDebugConfigured = true;
                    break;
                case nameof(this.DebugException):
                    this.debugExceptionChannel = eventAttribute.Channel;
                    this.debugExceptionEventId = eventAttribute.EventId;
                    this.debugExceptionLevel = eventAttribute.Level;
                    this.debugExceptionKeywords = eventAttribute.Keywords;
                    this.isDebugExceptionConfigured = true;
                    break;
                case nameof(this.Info):
                    this.infoChannel = eventAttribute.Channel;
                    this.infoEventId = eventAttribute.EventId;
                    this.infoLevel = eventAttribute.Level;
                    this.infoKeywords = eventAttribute.Keywords;
                    this.isInfoConfigured = true;
                    break;
                case nameof(this.InfoException):
                    this.infoExceptionChannel = eventAttribute.Channel;
                    this.infoExceptionEventId = eventAttribute.EventId;
                    this.infoExceptionLevel = eventAttribute.Level;
                    this.infoExceptionKeywords = eventAttribute.Keywords;
                    this.isInfoExceptionConfigured = true;
                    break;
                case nameof(this.Error):
                    this.errorChannel = eventAttribute.Channel;
                    this.errorEventId = eventAttribute.EventId;
                    this.errorLevel = eventAttribute.Level;
                    this.errorKeywords = eventAttribute.Keywords;
                    this.isErrorConfigured = true;
                    break;
                case nameof(this.ErrorException):
                    this.errorExceptionChannel = eventAttribute.Channel;
                    this.errorExceptionEventId = eventAttribute.EventId;
                    this.errorExceptionLevel = eventAttribute.Level;
                    this.errorExceptionKeywords = eventAttribute.Keywords;
                    this.isErrorExceptionConfigured = true;
                    break;
                case nameof(this.Fatal):
                    this.fatalChannel = eventAttribute.Channel;
                    this.fatalEventId = eventAttribute.EventId;
                    this.fatalLevel = eventAttribute.Level;
                    this.fatalKeywords = eventAttribute.Keywords;
                    this.isFatalConfigured = true;
                    break;
                case nameof(this.FatalException):
                    this.fatalExceptionChannel = eventAttribute.Channel;
                    this.fatalExceptionEventId = eventAttribute.EventId;
                    this.fatalExceptionLevel = eventAttribute.Level;
                    this.fatalExceptionKeywords = eventAttribute.Keywords;
                    this.isFatalExceptionConfigured = true;
                    break;
                case nameof(this.Warn):
                    this.warnChannel = eventAttribute.Channel;
                    this.warnEventId = eventAttribute.EventId;
                    this.warnLevel = eventAttribute.Level;
                    this.warnKeywords = eventAttribute.Keywords;
                    this.isWarnConfigured = true;
                    break;
                case nameof(this.WarnException):
                    this.warnExceptionChannel = eventAttribute.Channel;
                    this.warnExceptionEventId = eventAttribute.EventId;
                    this.warnExceptionLevel = eventAttribute.Level;
                    this.warnExceptionKeywords = eventAttribute.Keywords;
                    this.isWarnExceptionConfigured = true;
                    break;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Declares ids for those events broadcast by the NServiceBus libraries
        /// </summary>
        protected class NServiceBusEtwConstants
        {
            /// <summary>  Fatal level  </summary>
            public const int FatalEventId = 1001;

            /// <summary>  Fatal level with exception info  </summary>
            public const int FatalExceptionEventId = 1002;

            /// <summary>  Error level  </summary>
            public const int ErrorEventId = 1003;

            /// <summary>  Error level with exception info  </summary>
            public const int ErrorExceptionEventId = 1004;

            /// <summary>  Warning level  </summary>
            public const int WarnEventId = 1005;

            /// <summary>  Warning level with exception info  </summary>
            public const int WarnExceptionEventId = 1006;

            /// <summary>  Informational level  </summary>
            public const int InfoEventId = 1007;

            /// <summary>  Informational level with exception info  </summary>
            public const int InfoExceptionEventId = 1008;

            /// <summary>  Debug level  </summary>
            public const int DebugEventId = 1009;

            /// <summary>  Debug level with exception info  </summary>
            public const int DebugExceptionEventId = 1010;

            /// <summary>  Message format for Fatal level  </summary>
            public const string FatalMessage = "{0} : {1}";

            /// <summary>  Message format for Fatal level with exception info  </summary>
            public const string FatalExceptionMessage = "{0} : {1} : {2} : {3} : {4}";

            /// <summary>  Message format for Error level  </summary>
            public const string ErrorMessage = "{0} : {1}";

            /// <summary>  Message format for Error level with exception info  </summary>
            public const string ErrorExceptionMessage = "{0} : {1} : {2} : {3} : {4}";

            /// <summary>  Message format for Warning level  </summary>
            public const string WarnMessage = "{0} : {1}";

            /// <summary>  Message format for Warning level with exception info  </summary>
            public const string WarnExceptionMessage = "{0} : {1} : {2} : {3} : {4}";

            /// <summary>  Message format for Informational level  </summary>
            public const string InfoMessage = "{0} : {1}";

            /// <summary>  Message format for Informational level with exception info  </summary>
            public const string InfoExceptionMessage = "{0} : {1} : {2} : {3} : {4}";

            /// <summary>  Message format for Debug level  </summary>
            public const string DebugMessage = "{0} : {1}";

            /// <summary>  Message format for Debug level with exception info  </summary>
            public const string DebugExceptionMessage = "{0} : {1} : {2} : {3} : {4}";
        }
    }
}