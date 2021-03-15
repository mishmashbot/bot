using System;

namespace Ollio.Common.Models
{
    public class RuntimeInfo
    {
        public string AppCommit { get; set; }
        public string AppCopyright { get; set; }
        public Version AppVersion { get; set; }
        public string Hostname { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
        public DateTime TimeStarted { get; set; }
    }
}