// 
// Copyright (c) 2004-2009 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System.Collections.Generic;
using NLog.Config;
using NLog.Contexts;
using NLog.LayoutRenderers;
using NLog.Layouts;

namespace NLog.Targets
{
    /// <summary>
    /// Sends logging messages to the remote instance of NLog Viewer. 
    /// </summary>
    /// <example>
    /// <p>
    /// To set up the target in the <a href="config.html">configuration file</a>, 
    /// use the following syntax:
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/NLogViewer/NLog.config" />
    /// <p>
    /// This assumes just one target and a single rule. More configuration
    /// options are described <a href="config.html">here</a>.
    /// </p>
    /// <p>
    /// To set up the log target programmatically use code like this:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/NLogViewer/Simple/Example.cs" />
    /// <p>
    /// NOTE: If your receiver application is ever likely to be off-line, don't use TCP protocol
    /// or you'll get TCP timeouts and your application will crawl. 
    /// Either switch to UDP transport or use <a href="target.AsyncWrapper.html">AsyncWrapper</a> target
    /// so that your application threads will not be blocked by the timing-out connection attempts.
    /// </p>
    /// </example>
    [Target("NLogViewer")]
    public class NLogViewerTarget : NetworkTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NLogViewerTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public NLogViewerTarget()
        {
            this.Parameters = new List<NLogViewerParameterInfo>();
            this.Layout = new Log4JXmlEventLayout();
            this.Renderer.Parameters = this.Parameters;
            NewLine = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include NLog-specific extensions to log4j schema.
        /// </summary>
        public bool IncludeNLogData
        {
            get { return this.Renderer.IncludeNLogData; }
            set { this.Renderer.IncludeNLogData = value; }
        }

        /// <summary>
        /// Gets or sets the AppInfo field. By default it's the friendly name of the current AppDomain.
        /// </summary>
        public string AppInfo
        {
            get { return this.Renderer.AppInfo; }
            set { this.Renderer.AppInfo = value; }
        }

#if !NET_CF
        /// <summary>
        /// Gets or sets a value indicating whether to include call site (class and method name) in the information sent over the network.
        /// </summary>
        public bool IncludeCallSite
        {
            get { return this.Renderer.IncludeCallSite; }
            set { this.Renderer.IncludeCallSite = value; }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether to include source info (file name and line number) in the information sent over the network.
        /// </summary>
        public bool IncludeSourceInfo
        {
            get { return this.Renderer.IncludeSourceInfo; }
            set { this.Renderer.IncludeSourceInfo = value; }
        }
#endif

#endif

        /// <summary>
        /// Gets or sets a value indicating whether to include <see cref="MappedDiagnosticsContext"/> dictionary contents.
        /// </summary>
        public bool IncludeMdc
        {
            get { return this.Renderer.IncludeMDC; }
            set { this.Renderer.IncludeMDC = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include <see cref="NestedDiagnosticsContext"/> stack contents.
        /// </summary>
        public bool IncludeNdc
        {
            get { return this.Renderer.IncludeNDC; }
            set { this.Renderer.IncludeNDC = value; }
        }

        /// <summary>
        /// Gets the collection of parameters. Each parameter contains a mapping
        /// between NLog layout and a named parameter.
        /// </summary>
        [ArrayParameter(typeof(NLogViewerParameterInfo), "parameter")]
        public ICollection<NLogViewerParameterInfo> Parameters { get; private set; }

        /// <summary>
        /// Gets or sets the instance of <see cref="Log4JXmlEventLayout"/> that is used to format log messages.
        /// </summary>
        protected new Log4JXmlEventLayout Layout
        {
            get { return base.Layout as Log4JXmlEventLayout; }
            set { base.Layout = value; }
        }

        private Log4JXmlEventLayoutRenderer Renderer
        {
            get { return this.Layout.Renderer; }
        }

#if !NET_CF
        /// <summary>
        /// Returns the value indicating whether call site and/or source information should be gathered.
        /// </summary>
        /// <returns>2 - when IncludeSourceInfo is set, 1 when IncludeCallSite is set, 0 otherwise.</returns>
        protected internal override StackTraceUsage GetStackTraceUsage()
        {
#if !SILVERLIGHT
            if (this.IncludeSourceInfo)
            {
                return StackTraceUsage.WithSource;
            }
#endif

            if (this.IncludeCallSite)
            {
                return StackTraceUsage.WithoutSource;
            }

            return base.GetStackTraceUsage();
        }
#endif
    }
}
