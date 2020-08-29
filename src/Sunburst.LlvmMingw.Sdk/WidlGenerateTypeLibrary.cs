using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public class WidlGenerateTypeLibrary : WidlTaskBase
    {
        [Output]
        public ITaskItem TypeLibrary { get; set; }

        [Output]
        public ITaskItem[] GeneratedResources { get; set; }

        [Required]
        public ITaskItem OutputTlbFilePath { get; set; }

        public int ResourceId { get; set; } = 0;
        public ITaskItem OutputResourceFilePath { get; set; }

        protected override bool ValidateParameters()
        {
            if (ResourceId != 0 && OutputResourceFilePath == null)
            {
                Log.LogError("OututResourceFilePath must be specified if ResourceId is nonzero");
                return false;
            }

            return base.ValidateParameters();
        }

        protected override void OnExecuteSuccess()
        {
            TypeLibrary = new TaskItem(TypeLibrary);

            if (ResourceId != 0)
            {
                string rcLine = $"{ResourceId} TYPELIB \"{OutputTlbFilePath.GetMetadata("FileName")}" +
                    $"{OutputTlbFilePath.GetMetadata("Extension")}\"\r\n";
                File.WriteAllText(OutputResourceFilePath.ItemSpec, rcLine);

                // We copy the item here, instead of passing it through,
                // to erase any metadata that may be on it. Unlikely, but possible.
                TaskItem rcFile = new TaskItem(OutputResourceFilePath.ItemSpec);
                GeneratedResources = new[] { rcFile };
            }
            else
            {
                GeneratedResources = Array.Empty<ITaskItem>();
            }
        }

        protected override void GenerateCommandLineCommandsCore(CommandLineBuilder builder)
        {
            builder.AppendSwitch("-t");
            builder.AppendSwitchIfNotNull("-o ", OutputTlbFilePath);

            if (TargetArchitecture == "Win32" || TargetArchitecture == "x86" || TargetArchitecture == "ARM")
            {
                builder.AppendSwitch("--win32");
            }
            else if (TargetArchitecture == "x64" || TargetArchitecture == "ARM64")
            {
                builder.AppendSwitch("--win64");
            }
        }
    }
}
