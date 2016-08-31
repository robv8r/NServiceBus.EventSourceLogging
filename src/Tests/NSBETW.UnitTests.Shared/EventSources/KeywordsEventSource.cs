// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordsEventSource.cs" company="Rob Winningham">
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
namespace NServiceBus.EventSourceLogging.UnitTests.EventSources
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
#if USEMDT
    using Microsoft.Diagnostics.Tracing;
#else
    using System.Diagnostics.Tracing;
#endif

    /// <summary>
    ///     Event Source with multiple keywords on a given event.
    /// </summary>
    [EventSource]
    public class KeywordsEventSource : EventSource
    {
        private KeywordsEventSource()
        {
        }

        /// <summary>
        ///     Gets an instance of the MultipleKeywordEventSource class.
        /// </summary>
        public static KeywordsEventSource Log { get; } = new KeywordsEventSource();

        /// <summary>
        ///     Writes an event with one keyword.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Event(1, Keywords = Keywords.FirstKeyword)]
        public void OneKeywordExample(string message)
        {
            this.WriteEvent(1, message);
        }

        /// <summary>
        ///     Writes an event with two keywords.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Event(3, Keywords = Keywords.SecondKeyword | Keywords.ThirdKeyword | Keywords.FourthKeyword)]
        public void ThreeKeywordExample(string message)
        {
            this.WriteEvent(3, message);
        }

        /// <summary>
        ///     Writes an event with two keywords.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Event(2, Keywords = Keywords.SecondKeyword | Keywords.ThirdKeyword)]
        public void TwoKeywordExample(string message)
        {
            this.WriteEvent(2, message);
        }

        /// <summary>
        ///     A container for a bit-vector of <see cref="EventKeywords" /> for this <see cref="EventSource" />.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for ETW.")]
        [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Required for ETW.")]
        [UsedImplicitly]
        public class Keywords
        {
            /// <summary>
            ///     The first keyword.
            /// </summary>
            public const EventKeywords FirstKeyword = (EventKeywords)0x1;

            /// <summary>
            ///     The third keyword.
            /// </summary>
            public const EventKeywords FourthKeyword = (EventKeywords)0x8;

            /// <summary>
            ///     The second keyword.
            /// </summary>
            public const EventKeywords SecondKeyword = (EventKeywords)0x2;

            /// <summary>
            ///     The third keyword.
            /// </summary>
            public const EventKeywords ThirdKeyword = (EventKeywords)0x4;
        }
    }
}