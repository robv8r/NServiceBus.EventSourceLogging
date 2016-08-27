// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerFactory.cs" company="Rob Winningham">
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
    using JetBrains.Annotations;
    using NServiceBus.Logging;

    /// <summary>
    ///     Redirects Logging to ETW.
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        [NotNull]
        private readonly IEventSourceLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFactory"/> class with the given <paramref name="logger"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public LoggerFactory([NotNull] IEventSourceLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.logger = logger;
        }

        /// <summary>
        ///     Gets a <see cref="ILog" /> for a specific <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to get the <see cref="ILog" /> for.</param>
        /// <returns>An instance of a <see cref="ILog" /> specifically for <paramref name="type" />.</returns>
        public ILog GetLogger(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return new Logger(this.logger, type.FullName);
        }

        /// <summary>
        ///     Gets a <see cref="ILog" /> for a specific <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the usage to get the <see cref="ILog" /> for.</param>
        /// <returns>An instance of a <see cref="ILog" /> specifically for <paramref name="name" />.</returns>
        public ILog GetLogger(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Logger(this.logger, name);
        }
    }
}