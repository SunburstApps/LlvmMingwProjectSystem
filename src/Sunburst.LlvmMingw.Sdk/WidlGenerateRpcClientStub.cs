using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public class WidlGenerateRpcClientStub : WidlTaskBase
    {
        [Required]
        public ITaskItem OutputFilePath { get; set; }

        [Output]
        public ITaskItem[] GeneratedSources { get; set; }

        public string SymbolPrefix { get; set; }
        public string StubType { get; set; }

        protected override bool ValidateParameters()
        {
            if (!string.IsNullOrEmpty(StubType) && !(StubType == "Os" || StubType == "Oi" || StubType == "Oif"))
            {
                Log.LogError("Invailid StubType (expected \"Os\", \"Oi\", or \"Oif\")");
                return false;
            }

            return base.ValidateParameters();
        }

        protected override void GenerateCommandLineCommandsCore(CommandLineBuilder builder)
        {
            builder.AppendSwitch("-c");
            builder.AppendSwitchIfNotNull("-o ", OutputFilePath);
            builder.AppendSwitchIfNotNull("--prefix-client=", SymbolPrefix);

            if (StubType == "Os") builder.AppendSwitch("-Os");
            else if (StubType == "Oi") builder.AppendSwitch("-Oi");
            else if (StubType == "Oif") builder.AppendSwitch("-Oif");
        }

        protected override void OnExecuteSuccess()
        {
            var item = new TaskItem(OutputFilePath);
            GeneratedSources = new[] { item };
            FileWrites = new[] { item };
        }
    }
}
