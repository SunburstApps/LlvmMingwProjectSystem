using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public sealed class Widl : ToolTask
    {
        private enum WidlOutputMode
        {
            None,
            Headers,
            RpcClientStub,
            RpcServerStub,
            TypeLibrary,
            IidHeaderFile,
            ProxySource
        }

        private WidlOutputMode OutputMode = WidlOutputMode.None;

        [Required]
        public string LlvmMingwRoot { get; set; }

        [Required]
        public string TargetArchitecture { get; set; }

        public ITaskItem AcfFile { get; set; }
        public ITaskItem RpcClientFileName { get; set; }
        public ITaskItem RpcServerFileName { get; set; }
        public ITaskItem HeaderFileName { get; set; }
        public string IncludeDirectories { get; set; }
        public string PreprocessorDirectives { get; set; }

        protected override string ToolName => "widl";

        protected override string GenerateFullPathToTool()
        {
            string LogArchError(string param)
            {
                Log.LogWarning("WIDL", "100", null, null, 0, 0, 0, 0, "Unrecognize TargetArchitecture value, defaulting to x86");
                return "i686";
            }

            string shortArch = TargetArchitecture.ToLowerInvariant() switch
            {
                "x86" => "i686",
                "x64" => "x86_64",
                "arm" => "armv7",
                "arm64" => "aarch64",
                _ => LogArchError(TargetArchitecture)
            };

            return Path.Combine(LlvmMingwRoot, "bin", $"{shortArch}-w64-mingw32-midl");
        }

        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();

            builder.AppendSwitchIfNotNull("-acf=", AcfFile);

            switch (OutputMode)
            {
                case WidlOutputMode.None:
                    throw new ArgumentException("OutputMode must be set");
                case WidlOutputMode.Headers:
                    builder.AppendSwitch("-h");
                    break;
                case WidlOutputMode.RpcClientStub:
                    builder.AppendSwitch("-c");
                    break;
                case WidlOutputMode.RpcServerStub:
                    builder.AppendSwitch("-t");
                    break;
                case WidlOutputMode.IidHeaderFile:
                    builder.AppendSwitch("-u");
                    break;
            }

            return builder.ToString();
        }
    }
}
