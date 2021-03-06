// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Rob Winningham">
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
using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage(
        "Microsoft.Naming",
        "CA1716:IdentifiersShouldNotMatchKeywords",
        MessageId = "Error",
        Scope = "member",
        Target = "NServiceBus.EventSourceLogging.IEventSourceLogger.#Error(System.String,System.String)",
        Justification = "Name is required due to the NServiceBus interfaces.")]
[assembly:
    SuppressMessage(
        "Microsoft.Naming",
        "CA1716:IdentifiersShouldNotMatchKeywords",
        MessageId = "Error",
        Scope = "member",
        Target = "NServiceBus.EventSourceLogging.IEventSourceLogger.#Error(System.String,System.String,System.Exception)",
        Justification = "Name is required due to the NServiceBus interfaces.")]
[assembly:
    SuppressMessage(
        "Microsoft.Naming",
        "CA1716:IdentifiersShouldNotMatchKeywords",
        MessageId = "Error",
        Scope = "member",
        Target = "NServiceBus.EventSourceLogging.IEventSourceLogger.#Error(System.String,System.String,System.Object[])",
        Justification = "Name is required due to the NServiceBus interfaces.")]