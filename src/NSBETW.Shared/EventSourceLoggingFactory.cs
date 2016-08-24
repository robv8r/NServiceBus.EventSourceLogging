﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSourceLoggingFactory.cs" company="Rob Winningham">
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
    using NServiceBus.Logging;

    /// <summary>
    ///     Defines an Event Source Logging Factory.
    /// </summary>
    [PublicAPI]
    public class EventSourceLoggingFactory : LoggingFactoryDefinition
    {
        /// <summary>
        ///     Singleton instance of the <see cref="EventSourceLoggingFactory" />.
        /// </summary>
        private static readonly ILoggerFactory Factory = new LoggerFactory();

        /// <summary>
        ///     Constructs an instance of <see cref="ILoggerFactory" /> for use by
        ///     <see cref="M:NServiceBus.Logging.LogManager.Use``1" />
        /// </summary>
        /// <returns>
        ///     A logging factory.
        /// </returns>
        protected override ILoggerFactory GetLoggingFactory()
        {
            return Factory;
        }
    }
}