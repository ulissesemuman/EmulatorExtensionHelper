using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorExtensionHelper
{
    internal static class RetroArchHelper
    {
        public class RetroArchCoreInfo
        {
            public string Name { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public List<string> SupportedExtensions { get; set; } = new List<string>();
        }

        public static List<RetroArchCoreInfo> GetRetroArchCoresForExtension(string retroArchContextName, string extension)
        {
            var retroArchPath = ConfigManager.GetEmulatorPathByName(retroArchContextName);
            var cores = new List<RetroArchCoreInfo>();
            if (retroArchPath == null) return cores;

            try
            {
                var baseDir = Path.GetDirectoryName(retroArchPath)!;
                var coresDir = Path.Combine(baseDir, "cores");
                var infoDir = Path.Combine(baseDir, "info");

                if (!Directory.Exists(coresDir) || !Directory.Exists(infoDir))
                    return cores;

                var installedCores = Directory.GetFiles(coresDir, "*.dll")
                                              .Select(f => Path.GetFileName(f).ToLower())
                                              .ToHashSet();

                foreach (var infoFile in Directory.GetFiles(infoDir, "*.info"))
                {
                    var lines = File.ReadAllLines(infoFile);

                    string? supportedExtensions = lines.FirstOrDefault(l => l.StartsWith("supported_extensions"));
                    supportedExtensions = supportedExtensions?.Split('=')[1].Trim() ?? string.Empty;
                    string? displayName = lines.FirstOrDefault(l => l.StartsWith("display_name"));
                    displayName = displayName?.Split('=')[1].Trim().Trim('"') ?? null;  

                    if (!string.IsNullOrEmpty(supportedExtensions) &&
                        supportedExtensions.Split('|').Contains(extension, StringComparer.OrdinalIgnoreCase))
                    {
                        var coreName = Path.GetFileNameWithoutExtension(infoFile);

                        var dllName = coreName.ToLower() + ".dll";

                        if (installedCores.Contains(dllName))
                        {
                            cores.Add(new RetroArchCoreInfo
                            {
                                Name = coreName,
                                DisplayName = displayName ?? coreName,
                                Path = Path.Combine(coresDir, dllName),
                                SupportedExtensions = supportedExtensions.Split('|').ToList()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar cores do RetroArch: " + ex.Message);
                MessageBox.Show($"Error reading RetroArch cores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return cores;
        }

        public static List<RetroArchCoreInfo> GetMissingRetroArchCores(string retroArchContextName, string extension)
        {
            var retroArchPath = ConfigManager.GetEmulatorPathByName(retroArchContextName);
            if (retroArchPath == null) return new();

            var baseDir = Path.GetDirectoryName(retroArchPath)!;
            var coresDir = Path.Combine(baseDir, "cores");
            var infoDir = Path.Combine(baseDir, "info");

            if (!Directory.Exists(coresDir) || !Directory.Exists(infoDir))
                return new();

            var installedCores = Directory.GetFiles(coresDir, "*.dll")
                                          .Select(f => Path.GetFileName(f).ToLower())
                                          .ToHashSet();

            var missingCores = new List<RetroArchCoreInfo>();

            foreach (var infoFile in Directory.GetFiles(infoDir, "*.info"))
            {
                var lines = File.ReadAllLines(infoFile);
                string? supportedExtensionsLine = lines.FirstOrDefault(l => l.StartsWith("supported_extensions"));
                string? coreNameLine = lines.FirstOrDefault(l => l.StartsWith("display_name"));

                if (supportedExtensionsLine == null) continue;

                var extensions = supportedExtensionsLine.Split('=')[1].Split('|');
                if (!extensions.Contains(extension, StringComparer.OrdinalIgnoreCase)) continue;

                var dllName = Path.GetFileNameWithoutExtension(infoFile).ToLower() + ".dll";

                if (!installedCores.Contains(dllName))
                {
                    var coreName = Path.GetFileNameWithoutExtension(infoFile);

                    missingCores.Add(new RetroArchCoreInfo
                    {
                        Name = coreName,
                        DisplayName = coreNameLine?.Split('=')[1].Trim().Trim('"') ?? coreName,
                        Path = Path.Combine(coresDir, dllName),
                        SupportedExtensions = extensions.ToList()
                    });
                }
            }

            return missingCores;
        }
    }
}
