using System;

namespace Ollio.Common.Models
{
    public class RuntimeInfo
    {
        public string AppCommit { get; set; }
        public string AppCopyright { get; set; }
        public Version AppVersion { get; set; }
        public DateTime DateStarted { get; set; }
        public string Hostname { get; set; }
        public string IPAddress { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }

        public decimal GetMemoryUsage()
        {
            return Convert.ToDecimal(System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1000000);
        }

        public TimeSpan GetUptime()
        {
            return DateTime.Now.ToUniversalTime().Subtract(DateStarted.ToUniversalTime());
        }

        public string GetVersion(
            bool includeBuild = true,
            bool includeCommit = true,
            bool includeRelease = false
        )
        {
            var version = $"{AppVersion.Major}.{AppVersion.Minor}";

            if(includeBuild)
                version += $".{AppVersion.Build}";

            if(AppCommit != null && includeCommit)
                version += $"+{AppCommit}";

            if(includeRelease)
                version += $" \"{GetVersionRelease()}\"";

            return version;
        }

        public string GetVersionRelease()
        {
            string release;

            switch($"{AppVersion.Major}.{AppVersion.Minor}")
            {
                case "0.3":
                    release = "Catherham";
                    break;
                case "0.2":
                    release = "Bentley";
                    break;
                case "0.1":
                    release = "Ariel";
                    break;
                default:
                    release = "Leyland";
                    break;
            }

            return release;
        }
    }
}