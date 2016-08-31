// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Rob Winningham">
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if SIGNED
[assembly: InternalsVisibleTo("NServiceBus.EventSourceLogging.UnitTests.Net46, PublicKey=002400000480000094000000060200000024000052534131000400000100010097263141eeb008a5dc8706847214edf3c7dc3ead55456de03c58c219fbd720d375ebb49b849ae028fc3ca7ff0bff189cd9c6007d2897c2d3bec1c954f9ba518bbe55aabce39cc5395f4421a89e9d8b5efe307c1ed00f3c8af8f135dbced9e19b1384aedc6b3dfcbf81e2d8ee18f8c60ecf489fde4eae9ebc439f0fcd2a7d4dbd")]
#else
[assembly: InternalsVisibleTo("NServiceBus.EventSourceLogging.UnitTests.Net46")]
#endif
[assembly: Guid("8C10CA7E-D871-4C88-8577-8E2DB6D3DA31")]