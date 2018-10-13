﻿// Python Tools for Visual Studio
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Threading.Tasks;
using Microsoft.Python.LanguageServer.Implementation;
using Microsoft.PythonTools.Analysis;
using Microsoft.PythonTools.Intellisense;
using Microsoft.PythonTools.Interpreter;

namespace AnalysisTests {
    public class ServerBasedTest {
        private Server _server;

        protected async Task<Server> CreateServerAsync(InterpreterConfiguration configuration = null, Uri rootUri = null) {
            configuration = configuration ?? PythonVersions.LatestAvailable2X ?? PythonVersions.LatestAvailable3X;
            configuration.AssertInstalled();

            _server = await new Server().InitializeAsync(configuration, rootUri);
            _server.Analyzer.EnableDiagnostics = true;
            _server.Analyzer.Limits = GetLimits();

            return _server;
        }

        protected virtual AnalysisLimits GetLimits() => AnalysisLimits.GetDefaultLimits();

        protected IDisposable FileLoading() => new AnalysisQueueControl(_server.AnalysisQueue);

        class AnalysisQueueControl: IDisposable {
            private readonly AnalysisQueue _queue;

            public AnalysisQueueControl(AnalysisQueue queue) {
                _queue = queue;
                _queue.Stop();
            }

            public void Dispose() => _queue.Start();
        }
    }
}
