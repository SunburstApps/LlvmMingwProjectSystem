using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public class WidlGenerateHeaders : ToolTask
    {
        [Required]
        public ITaskItem SourceFile { get; set; }

        [Output]
        public ITaskItem[] Headers { get; set; }

        [Output]
        public ITaskItem[] FileWrites { get; set; }

        [Required]
        public string LlvmMingwRoot { get; set; }

        [Required]
        public string TargetArchitecture { get; set; }

        [Required]
        public string OutputDirectory { get; set; }

        public ITaskItem[] IncludeDirectories { get; set; }
        public string[] PreprocessorDefinitions { get; set; }
        public string OutputHeaderFileName { get; set; }
        public bool EnableWarnings { get; set; }

        private TaskItem GeneratedHeader = null;

        protected override string ToolName => Utility.ExpandToolName(TargetArchitecture, "widl");
        protected override string GenerateFullPathToTool()
        {
            string filename = Utility.ExpandToolName(TargetArchitecture, "widl");
            return Path.Combine(LlvmMingwRoot, "bin", filename);
        }

        public override bool Execute()
        {
            try
            {
                GeneratedHeader = new TaskItem(Path.Combine(OutputDirectory, OutputHeaderFileName));
                GeneratedHeader.SetMetadata("UsesPCH", "False");

                bool success = base.Execute();
                if (success)
                {

                    Headers = new[] { GeneratedHeader };
                    FileWrites = new[] { GeneratedHeader };
                }

                return success;
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();

            builder.AppendSwitch("-h");
            builder.AppendSwitchIfNotNull("-o ", GeneratedHeader);
            if (EnableWarnings) builder.AppendSwitch("-W");

            foreach (ITaskItem path in IncludeDirectories)
            {
                builder.AppendSwitchIfNotNull("-I ", path.ItemSpec);
            }
            foreach (string define in PreprocessorDefinitions)
            {
                builder.AppendSwitchIfNotNull("-D ", define);
            }

            if (TargetArchitecture == "Win32" || TargetArchitecture == "x86" || TargetArchitecture == "ARM")
            {
                builder.AppendSwitch("--win32");
            }
            else if (TargetArchitecture == "x64" || TargetArchitecture == "ARM64")
            {
                builder.AppendSwitch("--win64");
            }

            builder.AppendFileNameIfNotNull(SourceFile);
            return builder.ToString();
        }

        protected override string GetWorkingDirectory() => OutputDirectory;
    }
}
