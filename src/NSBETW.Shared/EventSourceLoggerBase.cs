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
    using System.Xml;
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
        private readonly EventChannel debugChannel = EventChannel.None;
        private readonly int debugEventId = 1009;
        private readonly EventChannel debugExceptionChannel = EventChannel.None;
        private readonly int debugExceptionEventId = 1010;
        private readonly EventKeywords debugExceptionKeywords = EventKeywords.None;
        private readonly EventLevel debugExceptionLevel = EventLevel.Verbose;
        private readonly EventKeywords debugKeywords = EventKeywords.None;
        private readonly EventLevel debugLevel = EventLevel.Verbose;
        private readonly EventChannel errorChannel = EventChannel.None;
        private readonly int errorEventId = 1003;
        private readonly EventChannel errorExceptionChannel = EventChannel.None;
        private readonly int errorExceptionEventId = 1004;
        private readonly EventKeywords errorExceptionKeywords = EventKeywords.None;
        private readonly EventLevel errorExceptionLevel = EventLevel.Error;
        private readonly EventKeywords errorKeywords = EventKeywords.None;
        private readonly EventLevel errorLevel = EventLevel.Error;
        private readonly EventChannel fatalChannel = EventChannel.None;
        private readonly int fatalEventId = 1001;
        private readonly EventChannel fatalExceptionChannel = EventChannel.None;
        private readonly int fatalExceptionEventId = 1002;
        private readonly EventKeywords fatalExceptionKeywords = EventKeywords.None;
        private readonly EventLevel fatalExceptionLevel = EventLevel.Critical;
        private readonly EventKeywords fatalKeywords = EventKeywords.None;
        private readonly EventLevel fatalLevel = EventLevel.Critical;
        private readonly EventChannel infoChannel = EventChannel.None;
        private readonly int infoEventId = 1007;
        private readonly EventChannel infoExceptionChannel = EventChannel.None;
        private readonly int infoExceptionEventId = 1008;
        private readonly EventKeywords infoExceptionKeywords = EventKeywords.None;
        private readonly EventLevel infoExceptionLevel = EventLevel.Informational;
        private readonly EventKeywords infoKeywords = EventKeywords.None;
        private readonly EventLevel infoLevel = EventLevel.Informational;
        private readonly EventChannel warnChannel = EventChannel.None;
        private readonly int warnEventId = 1005;
        private readonly EventChannel warnExceptionChannel = EventChannel.None;
        private readonly int warnExceptionEventId = 1006;
        private readonly EventKeywords warnExceptionKeywords = EventKeywords.None;
        private readonly EventLevel warnExceptionLevel = EventLevel.Warning;
        private readonly EventKeywords warnKeywords = EventKeywords.None;
        private readonly EventLevel warnLevel = EventLevel.Warning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourceLoggerBase" /> class.
        /// </summary>
        protected EventSourceLoggerBase()
        {
            // First, get the Event Source manifest as XML.
            var manifest = GenerateManifest(
                this.GetType(),
                this.GetType().Assembly.Location,
                EventManifestOptions.AllowEventSourceOverride | EventManifestOptions.AllCultures);

            if (manifest == null)
            {
                return;
            }

            // Load the manifest into an XmlDocument so we can query it.
            var doc = new XmlDocument();
            doc.LoadXml(manifest);
            var root = doc.DocumentElement;

            if (root == null)
            {
                return;
            }

            if (doc.NameTable == null)
            {
                return;
            }

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("e", "http://schemas.microsoft.com/win/2004/08/events");

            var keywordDictionary = new Dictionary<string, EventKeywords>();

            var keywordNodes =
                root.FirstChild?.SelectNodes("//e:provider[@name = '" + this.Name + "']/e:keywords/e:keyword", nsmgr);

            if (keywordNodes != null)
            {
                foreach (var nodeObject in keywordNodes)
                {
                    var node = nodeObject as XmlNode;

                    if (node?.Attributes == null)
                    {
                        continue;
                    }

                    var mask = 0L;
                    string name = null;

                    foreach (var attributeObject in node.Attributes)
                    {
                        var attribute = attributeObject as XmlAttribute;

                        if (attribute?.Name == null || attribute.Value == null)
                        {
                            continue;
                        }

                        switch (attribute.Name)
                        {
                            case "mask":
                                if (!string.IsNullOrWhiteSpace(attribute.Value) && attribute.Value.StartsWith("0x", StringComparison.CurrentCulture))
                                {
                                    mask = Convert.ToInt64(attribute.Value, 16);
                                }

                                break;
                            case "name":
                                name = attribute.Value;
                                break;
                            default:
                                break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(name) && mask > 0)
                    {
                        keywordDictionary.Add(name, (EventKeywords)mask);
                    }
                }
            }

            var nodes = root.FirstChild?.SelectNodes(
                "//e:provider[@name = '" + this.Name + "']/e:events/e:event",
                nsmgr);

            if (nodes == null)
            {
                return;
            }

            foreach (var nodeObject in nodes)
            {
                var node = nodeObject as XmlNode;

                if (node?.Attributes == null)
                {
                    continue;
                }

                var level = EventLevel.Informational;
                var channel = EventChannel.None;
                var eventId = -1;
                var keywords = EventKeywords.None;
                string symbol = null;

                foreach (var attributeObject in node.Attributes)
                {
                    var attribute = attributeObject as XmlAttribute;

                    if (attribute?.Name == null || attribute.Value == null)
                    {
                        continue;
                    }

                    switch (attribute.Name)
                    {
                        case "value":
                            if (!int.TryParse(attribute.Value, out eventId))
                            {
                            }

                            break;
                        case "level":
                            level = GetLevel(attribute.Value);
                            break;
                        case "channel":
                            if (!Enum.TryParse(attribute.Value, out channel))
                            {
                            }

                            break;
                        case "keywords":
                            if (!string.IsNullOrWhiteSpace(attribute.Value))
                            {
                                var keywordStrings = attribute.Value.Split(' ');

                                foreach (var keywordString in keywordStrings)
                                {
                                    if (keywordDictionary.ContainsKey(keywordString))
                                    {
                                        keywords = keywords | keywordDictionary[keywordString];
                                    }
                                }
                            }

                            break;
                        case "symbol":
                            symbol = attribute.Value;
                            break;
                        default:
                            break;
                    }
                }

                if (eventId > 0 && string.IsNullOrWhiteSpace(symbol))
                {
                    continue;
                }

                switch (symbol)
                {
                    case nameof(this.Debug):
                        this.debugChannel = channel;
                        this.debugEventId = eventId;
                        this.debugLevel = level;
                        this.debugKeywords = keywords;
                        break;
                    case nameof(this.DebugException):
                        this.debugExceptionChannel = channel;
                        this.debugExceptionEventId = eventId;
                        this.debugExceptionLevel = level;
                        this.debugExceptionKeywords = keywords;
                        break;
                    case nameof(this.Info):
                        this.infoChannel = channel;
                        this.infoEventId = eventId;
                        this.infoLevel = level;
                        this.infoKeywords = keywords;
                        break;
                    case nameof(this.InfoException):
                        this.infoExceptionChannel = channel;
                        this.infoExceptionEventId = eventId;
                        this.infoExceptionLevel = level;
                        this.infoExceptionKeywords = keywords;
                        break;
                    case nameof(this.Error):
                        this.errorChannel = channel;
                        this.errorEventId = eventId;
                        this.errorLevel = level;
                        this.errorKeywords = keywords;
                        break;
                    case nameof(this.ErrorException):
                        this.errorExceptionChannel = channel;
                        this.errorExceptionEventId = eventId;
                        this.errorExceptionLevel = level;
                        this.errorExceptionKeywords = keywords;
                        break;
                    case nameof(this.Fatal):
                        this.fatalChannel = channel;
                        this.fatalEventId = eventId;
                        this.fatalLevel = level;
                        this.fatalKeywords = keywords;
                        break;
                    case nameof(this.FatalException):
                        this.fatalExceptionChannel = channel;
                        this.fatalExceptionEventId = eventId;
                        this.fatalExceptionLevel = level;
                        this.fatalExceptionKeywords = keywords;
                        break;
                    case nameof(this.Warn):
                        this.warnChannel = channel;
                        this.warnEventId = eventId;
                        this.warnLevel = level;
                        this.warnKeywords = keywords;
                        break;
                    case nameof(this.WarnException):
                        this.warnExceptionChannel = channel;
                        this.warnExceptionEventId = eventId;
                        this.warnExceptionLevel = level;
                        this.warnExceptionKeywords = keywords;
                        break;
                    default:
                        break;
                }
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
        public bool IsFatalEnabled => this.IsFatalEventEnabled || this.IsErrorExceptionEventEnabled;

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
        protected bool IsDebugEventEnabled => this.IsEnabled(this.debugLevel, this.debugKeywords, this.debugChannel);

        /// <summary>
        ///     Gets a value indicating whether the DebugException event is enabled.
        /// </summary>
        protected bool IsDebugExceptionEventEnabled =>
            this.IsEnabled(this.debugExceptionLevel, this.debugExceptionKeywords, this.debugExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Error event is enabled.
        /// </summary>
        protected bool IsErrorEventEnabled =>
            this.IsEnabled(this.errorLevel, this.errorKeywords, this.errorChannel);

        /// <summary>
        ///     Gets a value indicating whether the ErrorException event is enabled.
        /// </summary>
        protected bool IsErrorExceptionEventEnabled =>
            this.IsEnabled(this.errorExceptionLevel, this.errorExceptionKeywords, this.errorExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Fatal event is enabled.
        /// </summary>
        protected bool IsFatalEventEnabled =>
            this.IsEnabled(this.fatalLevel, this.fatalKeywords, this.fatalChannel);

        /// <summary>
        ///     Gets a value indicating whether the FatalException event is enabled.
        /// </summary>
        protected bool IsFatalExceptionEventEnabled =>
            this.IsEnabled(this.fatalExceptionLevel, this.fatalExceptionKeywords, this.fatalExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Info event is enabled.
        /// </summary>
        protected bool IsInfoEventEnabled =>
            this.IsEnabled(this.infoLevel, this.infoKeywords, this.infoChannel);

        /// <summary>
        ///     Gets a value indicating whether the InfoException event is enabled.
        /// </summary>
        protected bool IsInfoExceptionEventEnabled =>
            this.IsEnabled(this.infoExceptionLevel, this.infoExceptionKeywords, this.infoExceptionChannel);

        /// <summary>
        ///     Gets a value indicating whether the Warn event is enabled.
        /// </summary>
        protected bool IsWarnEventEnabled =>
            this.IsEnabled(this.warnLevel, this.warnKeywords, this.warnChannel);

        /// <summary>
        ///     Gets a value indicating whether the WarnException event is enabled.
        /// </summary>
        protected bool IsWarnExceptionEventEnabled =>
            this.IsEnabled(this.warnExceptionLevel, this.warnExceptionKeywords, this.warnExceptionChannel);

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

        private static EventLevel GetLevel(string manifestLevel)
        {
            switch (manifestLevel)
            {
                case "win:LogAlways":
                    return EventLevel.LogAlways;
                case "win:Warning":
                    return EventLevel.Warning;
                case "win:Critical":
                    return EventLevel.Critical;
                case "win:Error":
                    return EventLevel.Error;
                case "win:Informational":
                    return EventLevel.Informational;
                case "win:Verbose":
                    return EventLevel.Verbose;
                default:
                    return EventLevel.Informational;
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
    }
}