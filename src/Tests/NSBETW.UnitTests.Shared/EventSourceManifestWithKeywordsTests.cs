// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSourceManifestWithKeywordsTests.cs" company="Rob Winningham">
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
namespace NServiceBus.EventSourceLogging.UnitTests
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using NServiceBus.EventSourceLogging.UnitTests.EventSources;
    using Xunit;
#if USEMDT
    using Microsoft.Diagnostics.Tracing;
#else
    using System.Diagnostics.Tracing;

#endif

    /// <summary>
    ///     Tests for the <see cref="EventSourceManifest" /> type
    /// </summary>
    public class EventSourceManifestWithKeywordsTests
    {
        [NotNull]
        [ItemNotNull]
        private readonly Lazy<string> lazyManifestXml = new Lazy<string>(GenerateManifestXml);

        /// <summary>
        ///     Ensures that the event id is successfully retrieved.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for XUnit.")]
        [Fact]
        public void FirstEventHasEventId1()
        {
            // Arrange
            const int ExpectedResult = 1;

            // Act
            var manifest = new EventSourceManifest(this.lazyManifestXml.Value);
            var attr = manifest.EventAttributes[nameof(KeywordsEventSource.OneKeywordExample)];

            // Assert
            Assert.NotNull(attr);
            Assert.Equal(ExpectedResult, attr.EventId);
        }

        /// <summary>
        ///     Ensures that the event id is successfully retrieved.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for XUnit.")]
        [Fact]
        public void SecondEventHasEventId2()
        {
            // Arrange
            const int ExpectedResult = 2;

            // Act
            var manifest = new EventSourceManifest(this.lazyManifestXml.Value);
            var attr = manifest.EventAttributes[nameof(KeywordsEventSource.TwoKeywordExample)];

            // Assert
            Assert.NotNull(attr);
            Assert.Equal(ExpectedResult, attr.EventId);
        }

        /// <summary>
        ///     Ensures that the event id is successfully retrieved.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for XUnit.")]
        [Fact]
        public void ThirdEventHasEventId3()
        {
            // Arrange
            const int ExpectedResult = 3;

            // Act
            var manifest = new EventSourceManifest(this.lazyManifestXml.Value);
            var attr = manifest.EventAttributes[nameof(KeywordsEventSource.ThreeKeywordExample)];

            // Assert
            Assert.NotNull(attr);
            Assert.Equal(ExpectedResult, attr.EventId);
        }

        /// <summary>
        ///     Ensures that two keywords appear correctly in the event manifest.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for XUnit.")]
        [Fact]
        public void ThreeKeywordsYieldCorrectResults()
        {
            // Arrange
            const EventKeywords ExpectedKeywords = KeywordsEventSource.Keywords.SecondKeyword |
                                                   KeywordsEventSource.Keywords.ThirdKeyword |
                                                   KeywordsEventSource.Keywords.FourthKeyword;

            Assert.NotNull(this.lazyManifestXml);

            // Act
            var manifest = new EventSourceManifest(this.lazyManifestXml.Value);
            var attr = manifest.EventAttributes[nameof(KeywordsEventSource.ThreeKeywordExample)];

            // Assert
            Assert.NotNull(attr);
            Assert.Equal(ExpectedKeywords, attr.Keywords);
        }

        /// <summary>
        ///     Ensures that two keywords appear correctly in the event manifest.
        /// </summary>
        [Fact]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for XUnit.")]
        public void TwoKeywordsYieldCorrectResults()
        {
            // Arrange
            const EventKeywords ExpectedKeywords = KeywordsEventSource.Keywords.SecondKeyword |
                                                   KeywordsEventSource.Keywords.ThirdKeyword;

            Assert.NotNull(this.lazyManifestXml);

            // Act
            var manifest = new EventSourceManifest(this.lazyManifestXml.Value);
            var attr = manifest.EventAttributes[nameof(KeywordsEventSource.TwoKeywordExample)];

            // Assert
            Assert.NotNull(attr);
            Assert.Equal(ExpectedKeywords, attr.Keywords);
        }

        private static string GenerateManifestXml()
        {
            return EventSource.GenerateManifest(
                typeof(KeywordsEventSource),
                typeof(KeywordsEventSource).Assembly.Location,
                EventManifestOptions.AllowEventSourceOverride);
        }
    }
}