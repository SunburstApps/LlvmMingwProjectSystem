using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public abstract class WidlTaskBase : ToolTask
    {
        [Required]
        public ITaskItem IdlFile { get; set; }

        [Output]
        public ITaskItem[] FileWrites { get; set; }

        [Required]
        public string LlvmMingwInstallRoot { get; set; }

        [Required]
        public string TargetArchitecture { get; set; }

        public ITaskItem[] IncludeDirectories { get; set; }
        public string[] PreprocessorDefinitions { get; set; }
        public bool EnableWarnings { get; set; }

        protected sealed override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();
            if (EnableWarnings) builder.AppendSwitch("-W");
            foreach (var item in IncludeDirectories) builder.AppendSwitchIfNotNull("-I ", item.ItemSpec);
            foreach (var item in PreprocessorDefinitions) builder.AppendSwitchIfNotNull("-D ", item);

            GenerateCommandLineCommandsCore(builder);

            builder.AppendFileNameIfNotNull(IdlFile);
            return builder.ToString();
        }

        protected abstract void GenerateCommandLineCommandsCore(CommandLineBuilder builder);

        protected virtual void OnExecuteSuccess() { }

        public sealed override bool Execute()
        {
            try
            {
                bool success = base.Execute();
                if (success) OnExecuteSuccess();
                return success;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        protected sealed override string ToolName => Utility.ExpandToolName(TargetArchitecture, "widl");
        protected sealed override string GenerateFullPathToTool()
        {
            string filename = Utility.ExpandToolName(TargetArchitecture, "widl");
            return Path.Combine(LlvmMingwInstallRoot, "bin", filename);
        }
    }
}
