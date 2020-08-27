using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public sealed class MakeParentDir : Task
    {
        [Required]
        public ITaskItem[] Files { get; set; }

        [Output]
        public ITaskItem[] CreatedDirectories { get; set; }

        public override bool Execute()
        {
            try
            {
                List<ITaskItem> outputs = new List<ITaskItem>();

                foreach (var file in Files)
                {
                    string path = Path.GetDirectoryName(file.GetMetadata("FullPath"));
                    bool existed = Directory.Exists(path);
                    Directory.CreateDirectory(path);
                    outputs.Add(new TaskItem(path));

                    if (!existed) Log.LogMessage("Created directory {0}.", path);
                }

                CreatedDirectories = outputs.ToArray();
                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
