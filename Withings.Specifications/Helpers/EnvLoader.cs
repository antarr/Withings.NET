using System;
using System.IO;

namespace Withings.Specifications.Helpers
{
    public static class EnvLoader
    {
        public static void Load()
        {
            var root = FindSolutionRoot();
            if (root == null) return;

            var envPath = Path.Combine(root, ".env");
            if (!File.Exists(envPath)) return;

            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var separatorIndex = trimmed.IndexOf('=');
                if (separatorIndex < 0)
                    continue;

                var key = trimmed.Substring(0, separatorIndex).Trim();
                var value = trimmed.Substring(separatorIndex + 1).Trim();

                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                    Environment.SetEnvironmentVariable(key, value);
            }
        }

        private static string FindSolutionRoot()
        {
            var dir = AppContext.BaseDirectory;
            while (dir != null)
            {
                if (Directory.GetFiles(dir, "*.sln").Length > 0)
                    return dir;
                dir = Directory.GetParent(dir)?.FullName;
            }
            return null;
        }
    }
}
