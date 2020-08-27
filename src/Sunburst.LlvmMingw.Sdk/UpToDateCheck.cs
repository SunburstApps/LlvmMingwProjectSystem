using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.LlvmMingw.Sdk
{
    public sealed class UpToDateCheck : Task
    {
        [Required]
        public ITaskItem InputFile { get; set; }

        [Required]
        public ITaskItem[] OutputFiles { get; set; }

        [Output]
        public ITaskItem[] UpdatedFiles { get; set; }

        public override bool Execute()
        {
            try
            {
                Dictionary<string, DateTime> dateCache = new Dictionary<string, DateTime>();

                foreach (ITaskItem file in OutputFiles)
                {
                    FileInfo outputInfo = new FileInfo(file.GetMetadata("FullPath"));
                    if (!outputInfo.Exists)
                    {
                        Log.LogMessage(MessageImportance.Normal, $"File {outputInfo.Name} does not exist");
                        dateCache.Add(outputInfo.FullName, new DateTime(0));
                    }
                    else
                    {
                        Log.LogMessage(MessageImportance.Normal, $"File {outputInfo.Name}, last modified time {outputInfo.LastWriteTimeUtc}");
                        dateCache.Add(outputInfo.FullName, outputInfo.LastWriteTimeUtc);
                    }
                }

                var updated = new List<ITaskItem>();
                FileInfo inputInfo = new FileInfo(InputFile.GetMetadata("FullPath"));
                if (!dateCache.Values.Any(outputDate => outputDate > inputInfo.LastWriteTimeUtc))
                {
                    Log.LogMessage(MessageImportance.Normal, $"File {inputInfo.Name} is newer than its outputs");
                    updated.Add(InputFile);
                }
                else
                {
                    Log.LogMessage(MessageImportance.Normal, $"File {inputInfo.Name} up-to-date.");
                }

                UpdatedFiles = updated.ToArray();
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }
    }
}
