using System;
using System.Text;

namespace Sunburst.LlvmMingw.Sdk
{
    internal static class Utility
    {
        internal static string ExpandToolName(string arch, string toolName, bool uwp = false)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(arch switch
            {
                "Win32" => "i686",
                "x86" => "i686",
                "x64" => "x86_64",
                "ARM" => "armv7",
                "ARM64" => "aarch64",
                _ => throw new ArgumentException("Unrecognized Target Architecture value. Expected one of: Win32 x86 x64 ARM ARM64")
            });

            builder.Append("-w64-mingw32");
            if (uwp) builder.Append("uwp");
            builder.Append('-');
            builder.Append(toolName);

            return builder.ToString();
        }
    }
}
