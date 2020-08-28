﻿using System;
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

        protected override void GenerateCommandLineCommandsCore(CommandLineBuilder builder)
        {
            builder.AppendSwitch("-c");
            builder.AppendSwitchIfNotNull("-o ", OutputFilePath);
        }

        protected override void OnExecuteSuccess()
        {
            var item = new TaskItem(OutputFilePath);
            GeneratedSources = new[] { item };
            FileWrites = new[] { item };
        }
    }
}