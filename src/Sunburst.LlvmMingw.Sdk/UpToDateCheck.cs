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
        public ITaskItem[] InputFiles { get; set; }

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
                    FileInfo info = new FileInfo(file.GetMetadata("FullPath"));
                    dateCache.Add(info.FullName, info.LastWriteTimeUtc);
                }

                var updated = new List<ITaskItem>();
                foreach (ITaskItem file in InputFiles)
                {
                    FileInfo info = new FileInfo(file.GetMetadata("FullPath"));
                    if (dateCache.Values.Any(outputDate => outputDate > info.LastWriteTimeUtc))
                        updated.Add(file);
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
