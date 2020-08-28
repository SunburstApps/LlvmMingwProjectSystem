using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public class WidlGenerateHeaders : WidlTaskBase
    {
        [Output]
        public ITaskItem[] Headers { get; set; }

        [Required]
        public string OutputHeaderFilePath { get; set; }

        protected override void OnExecuteSuccess()
        {
            var generatedHeader = new TaskItem(OutputHeaderFilePath);
            generatedHeader.SetMetadata("UsesPCH", "False");

            Headers = new[] { generatedHeader };
            FileWrites = new[] { generatedHeader };
        }

        protected override void GenerateCommandLineCommandsCore(CommandLineBuilder builder)
        {
            builder.AppendSwitch("-h");
            builder.AppendSwitchIfNotNull("-o ", OutputHeaderFilePath);

            if (TargetArchitecture == "Win32" || TargetArchitecture == "x86" || TargetArchitecture == "ARM")
            {
                builder.AppendSwitch("--win32");
            }
            else if (TargetArchitecture == "x64" || TargetArchitecture == "ARM64")
            {
                builder.AppendSwitch("--win64");
            }
        }

        protected override string GetWorkingDirectory() => Path.GetDirectoryName(OutputHeaderFilePath);
    }
}
