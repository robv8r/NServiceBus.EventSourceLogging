// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSourceManifest.cs" company="Rob Winningham">
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
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using JetBrains.Annotations;
    using NServiceBus.EventSourceLogging.Properties;
#if USEMDT
    using Microsoft.Diagnostics.Tracing;

#else
    using System.Diagnostics.Tracing;
#endif

    /// <summary>
    ///     A strongly typed event source manifest.
    /// </summary>
    internal class EventSourceManifest
    {
        private const string EventsNamespace = "http://schemas.microsoft.com/win/2004/08/events";

        [NotNull]
        [ItemNotNull]
        private readonly Lazy<Dictionary<string, EventAttribute>> lazyEventAttributes;

        [NotNull]
        [ItemNotNull]
        private readonly Lazy<string> lazyEventProviderName;

        [NotNull]
        [ItemNotNull]
        private readonly Lazy<Dictionary<string, EventKeywords>> lazyKeywords;

        [NotNull]
        private readonly XmlElement xmlDocumentElement;

        [NotNull]
        private readonly XmlNamespaceManager xmlNamespaceManager;

        [NotNull]
        private readonly XElement xml;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourceManifest" /> class.
        /// </summary>
        /// <param name="manifestXml">The XML manifest to read.</param>
        /// <exception cref="XmlException"><paramref name="manifestXml" /> was not a valid XML string.</exception>
        public EventSourceManifest([NotNull] string manifestXml)
        {
            if (string.IsNullOrWhiteSpace(manifestXml))
            {
                throw new ArgumentNullException(nameof(manifestXml));
            }

            this.xml = XElement.Parse(manifestXml);

            var doc = new XmlDocument();
            doc.LoadXml(manifestXml);

            if (doc.NameTable == null)
            {
                throw new ArgumentException(Resources.XmlManifestDoesNotContainNameTable, nameof(manifestXml));
            }

            this.xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);
            this.xmlNamespaceManager.AddNamespace("e", EventsNamespace);

            var root = doc.DocumentElement;

            if (root == null)
            {
                throw new ArgumentException(Resources.XmlManifestDoesNotContainDocumentElement, nameof(manifestXml));
            }

            this.xmlDocumentElement = root;

            this.lazyKeywords = new Lazy<Dictionary<string, EventKeywords>>(this.RetrieveKeywords);
            this.lazyEventAttributes = new Lazy<Dictionary<string, EventAttribute>>(this.RetrieveEventAttributes);
            this.lazyEventProviderName = new Lazy<string>(this.RetrieveEventProviderName);
        }

        /// <summary>
        ///     Gets a collection of <see cref="EventAttribute" />s keyed by their symbol.
        /// </summary>
        [NotNull]
        public Dictionary<string, EventAttribute> EventAttributes => this.lazyEventAttributes.Value;

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

        private EventKeywords ParseKeywords(string keywords)
        {
            var eventKeywords = EventKeywords.None;

            if (string.IsNullOrWhiteSpace(keywords))
            {
                return eventKeywords;
            }

            var availableKeywords = this.lazyKeywords.Value;

            var keywordStrings = keywords.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));

                foreach (var keywordString in keywordStrings)
                {
                    EventKeywords keywordValue;
                    if (availableKeywords.TryGetValue(keywordString, out keywordValue))
                    {
                        eventKeywords = eventKeywords | keywordValue;
                    }
                }

            return eventKeywords;
        }

        [NotNull]
        private Dictionary<string, EventAttribute> RetrieveEventAttributes()
        {
            var query = "./e:instrumentation/e:events/e:provider[@name='" +
                        this.lazyEventProviderName.Value + "']/e:events/e:event";

            var events =
                from e in this.xml.XPathSelectElements(query, this.xmlNamespaceManager)
                where e != null
                select new
                {
                    EventId = e.Attribute("value")?.Value,
                    Symbol = e.Attribute("symbol")?.Value,
                    Level = e.Attribute("level")?.Value,
                    Channel = e.Attribute("channel")?.Value,
                    Keywords = e.Attribute("keywords")?.Value
                };

            var eventAttributeDictionary = new Dictionary<string, EventAttribute>();

            foreach (var e in events)
            {
                if (string.IsNullOrWhiteSpace(e.EventId) || string.IsNullOrWhiteSpace(e.Symbol))
                {
                    continue;
                }

                int eventId;
                if (!int.TryParse(e.EventId, out eventId))
                {
                    continue;
                }

                if (eventId == 0)
                {
                    continue;
                }

                var eventAttribute = new EventAttribute(eventId);

                if (!string.IsNullOrWhiteSpace(e.Level))
                {
                    eventAttribute.Level = GetLevel(e.Level);
                }

                byte eventChannel;
                if (byte.TryParse(e.Channel, out eventChannel))
                {
                    eventAttribute.Channel = (EventChannel)eventChannel;
                }

                var eventKeywords = this.ParseKeywords(e.Keywords);
                if (eventKeywords != EventKeywords.None)
                {
                    eventAttribute.Keywords = eventKeywords;
                }

                eventAttributeDictionary.Add(e.Symbol, eventAttribute);
            }

            return eventAttributeDictionary;
        }

        [NotNull]
        private string RetrieveEventProviderName()
        {
            var providerName = this.xmlDocumentElement.FirstChild?.SelectSingleNode("//e:provider/@name", this.xmlNamespaceManager);

            if (string.IsNullOrWhiteSpace(providerName?.Value))
            {
                throw new InvalidOperationException(Resources.XmlManifestDidNotContainTheProvidersName);
            }

            return providerName.Value;
        }

        [NotNull]
        private Dictionary<string, EventKeywords> RetrieveKeywords()
        {
            var xpath = "//e:provider[@name = '" + this.lazyEventProviderName.Value + "']/e:keywords/e:keyword";

            var keywordDictionary = new Dictionary<string, EventKeywords>();

            var keywordNodes = this.xmlDocumentElement.FirstChild?.SelectNodes(xpath, this.xmlNamespaceManager);

            if (keywordNodes == null)
            {
                return keywordDictionary;
            }

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

                    if (string.IsNullOrWhiteSpace(attribute?.Name) || string.IsNullOrWhiteSpace(attribute.Value))
                    {
                        continue;
                    }

                    if (attribute.Name == "name")
                    {
                        name = attribute.Value;
                    }
                    else if (attribute.Name == "mask" && attribute.Value.StartsWith("0x", StringComparison.CurrentCulture))
                    {
                        mask = Convert.ToInt64(attribute.Value, 16);
                    }
                }

                if (!string.IsNullOrWhiteSpace(name) && mask > 0)
                {
                    keywordDictionary.Add(name, (EventKeywords)mask);
                }
            }

            return keywordDictionary;
        }
    }
}