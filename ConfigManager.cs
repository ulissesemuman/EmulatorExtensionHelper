using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmulatorExtensionHelper
{

    public class ConfigModel
    {
        [JsonPropertyName("language")]
        public string Language { get; set; } = "en-us";

        [JsonPropertyName("emulators")]
        public Dictionary<string, EmulatorConfig> Emulators { get; set; } = new();

        [JsonPropertyName("files")]
        public Dictionary<string, FileEntry> Files { get; set; } = new();
    }

    public class EmulatorConfig
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("extensions")]
        public HashSet<string> Extensions { get; set; } = new();
    }

    public class FileEntry
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("emulators")]
        public HashSet<string> Emulators { get; set; } = new();
    }

    internal static class ConfigManager
    {
        private static LanguageManager lang = new LanguageManager("language", "config.json");

        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        private static ConfigModel _config = new ConfigModel();

        public static ConfigModel Config { get => _config; set => _config = value; }

        static ConfigManager()
        {
            LoadConfig();
        }

        public static void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                Config = new ConfigModel();
                return;
            }

            try
            {
                string json = File.ReadAllText(ConfigPath);
                // Validação simples do JSON antes de deserializar
                using var doc = JsonDocument.Parse(json); // Se falhar, lança exceção

                Config = JsonSerializer.Deserialize<ConfigModel>(json)!;
            }
            catch (JsonException ex)
            {
                MessageBox.Show(
                                lang.T("ConfigManager.InvalidJson"),
                                lang.T("Common.Error"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                Console.Error.WriteLine($"Detalhes: {ex.Message}");
                Config = new ConfigModel(); // Continua com configuração padrão
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                                 lang.T("ConfigManager.UnexpectedError"),
                                lang.T("Common.Error"),
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error
 );
                Console.Error.WriteLine($"Detalhes: {ex.Message}");
                Config = new ConfigModel(); // Continua com configuração padrão
            }
        }

        public static void SaveConfig()
        {
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static ConfigModel GetConfig() => Config;

        public static void AddOrUpdateEmulator(string friendlyName, string path, string extension)
        {
            if (!Config.Emulators.ContainsKey(friendlyName))
            {
                Config.Emulators[friendlyName] = new EmulatorConfig
                {
                    Path = path,
                    Extensions = new HashSet<string>()
                };
            }

            Config.Emulators[friendlyName].Extensions.Add(extension);
        }

        public static void AddOrUpdateFileEntry(string hash, string filePath, string emulatorName)
        {
            if (!Config.Files.TryGetValue(hash, out var fileEntry))
            {
                fileEntry = new FileEntry
                {
                    Path = filePath,
                    Emulators = new HashSet<string>()
                };
                Config.Files[hash] = fileEntry;
            }

            fileEntry.Emulators.Add(emulatorName);
        }

        public static string FindFileHash(string hash, string filePath)
        {
            foreach (var kvp in Config.Files)
            {
                if (kvp.Key == hash || string.Equals(kvp.Value.Path, filePath, StringComparison.OrdinalIgnoreCase))
                    return kvp.Key;
            }
            return null;
        }

        public static void AssociateFileWithEmulator(string filePath, string emulatorName, string emulatorPath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            string hash = FileHashHelper.CalculateMD5(filePath);
            //emulatorPath = GetEmulatorPathByName(emulatorName);

            ConfigManager.AddOrUpdateEmulator(emulatorName, emulatorPath, extension);

            string existingHash = FindFileHash(hash, filePath);
            if (existingHash == null)
                ConfigManager.AddOrUpdateFileEntry(hash, filePath, emulatorName);
            else
                ConfigManager.AddOrUpdateFileEntry(existingHash, filePath, emulatorName);

            ConfigManager.SaveConfig();

            FileAssociationHelper.AssociateExtensionToLauncher(Path.GetExtension(filePath));
        }

        public static void AssociateExtensionWithEmulator(string filePath, string emulatorName, string emulatorPath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            //emulatorPath = GetEmulatorPathByName(emulatorName);

            // Atualiza ou adiciona o emulador com a nova extensão
            ConfigManager.AddOrUpdateEmulator(emulatorName, emulatorPath, extension);

            // Nada é feito com a seção "files" neste método, pois estamos associando por extensão apenas
            ConfigManager.SaveConfig();

            FileAssociationHelper.AssociateExtensionToLauncher(Path.GetExtension(filePath));
        }

        public static void DisassociateFileWithEmulator(string filePath, string emulatorName)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Arquivo não encontrado: {filePath}");
                return;
            }

            string fileHash = FileHashHelper.CalculateMD5(filePath);

            // Tenta encontrar uma entrada que tenha o mesmo hash OU o mesmo caminho
            string matchingKey = ConfigManager.Config.Files
                .FirstOrDefault(kvp =>
                    kvp.Key == fileHash ||
                    string.Equals(kvp.Value.Path, filePath, StringComparison.OrdinalIgnoreCase)).Key;

            if (matchingKey == null)
            {
                Console.WriteLine("Arquivo não encontrado na configuração.");
                return;
            }

            var fileEntry = ConfigManager.Config.Files[matchingKey];

            if (!fileEntry.Emulators.Contains(emulatorName))
            {
                Console.WriteLine($"O arquivo não está associado ao emulador '{emulatorName}'.");
                return;
            }

            fileEntry.Emulators.Remove(emulatorName);

            // Se a lista de emuladores ficar vazia, opcional: remover o arquivo inteiro
            if (fileEntry.Emulators.Count == 0)
            {
                Console.WriteLine("Nenhum emulador restante. Entrada de arquivo será mantida, mas vazia.");
                // Ou: ConfigManager.Config.Files.Remove(matchingKey);
            }

            ConfigManager.SaveConfig();

            Console.WriteLine($"Emulador '{emulatorName}' desassociado do arquivo '{filePath}'.");
        }

        public static void DisassociateExtensionWithEmulator(string filePath, string emulatorName)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Arquivo não encontrado: {filePath}");
                return;
            }

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            if (!ConfigManager.Config.Emulators.TryGetValue(emulatorName, out var emulatorEntry))
            {
                Console.WriteLine($"Emulador '{emulatorName}' não encontrado na configuração.");
                return;
            }

            if (!emulatorEntry.Extensions.Contains(extension))
            {
                Console.WriteLine($"Extensão '{extension}' não está associada ao emulador '{emulatorName}'.");
                return;
            }

            emulatorEntry.Extensions.Remove(extension);
            ConfigManager.SaveConfig();

            Console.WriteLine($"Extensão '{extension}' desassociada do emulador '{emulatorName}'.");
        }

        public static List<string> ListAssociatedEmulators()
        {
            return ConfigManager.Config.Emulators.Keys.ToList();
        }

        public static List<string> ListAssociatedEmulators(string filePath, bool isExtension, bool onlyEmulatorsNotAssociated)
        {
            if (isExtension)
            {
                if (onlyEmulatorsNotAssociated) 
                {
                    return ConfigManager.Config.Emulators
                        .Where(pair => !pair.Value.Extensions
                            .Any(ext => string.Equals(ext, Path.GetExtension(filePath.ToLowerInvariant()), StringComparison.OrdinalIgnoreCase)))
                        .Select(pair => pair.Key)
                        .ToList();
                }
                else
                {
                    var extension = Path.GetExtension(filePath.ToLowerInvariant());

                    return ConfigManager.Config.Emulators
                        .Where(e => e.Value.Extensions.Contains(extension))
                        .Select(e => e.Key)
                        .ToList();
                }
            }
            else
            {
                string hash = FileHashHelper.CalculateMD5(filePath);

                // Tenta encontrar por hash
                if (ConfigManager.Config.Files.TryGetValue(hash, out var fileEntry))
                {
                    return fileEntry.Emulators.ToList();
                }

                // Tenta encontrar por caminho
                var match = ConfigManager.Config.Files
                    .FirstOrDefault(pair => pair.Value.Path.Equals(filePath, StringComparison.OrdinalIgnoreCase));

                return match.Value?.Emulators?.ToList() ?? new List<string>();
            }
        }

        public static List<string> ListUnassociatedEmulatorsForFile(string filePath)
        {
            var config = ConfigManager.GetConfig();

            // Calcula o hash MD5 do arquivo
            string hash = FileHashHelper.CalculateMD5(filePath);
            HashSet<string> associatedEmulators = new();

            // Verifica se o hash existe em files
            if (config.Files.TryGetValue(hash, out var fileEntry))
            {
                associatedEmulators = fileEntry.Emulators != null
                    ? new HashSet<string>(fileEntry.Emulators)
                    : new HashSet<string>();
            }
            else
            {
                // Verifica se há alguma entrada cujo path coincida com o caminho completo
                var match = config.Files.Values
                    .FirstOrDefault(f => string.Equals(f.Path, filePath, StringComparison.OrdinalIgnoreCase));

                if (match?.Emulators != null)
                {
                    associatedEmulators = new HashSet<string>(match.Emulators);
                }
            }

            // Retorna os nomes dos emuladores que NÃO estão associados
            return config.Emulators.Keys
                .Where(name => !associatedEmulators.Contains(name))
                .ToList();
        }

        public static bool FileExistsInConfig(string filePath)
        {
            // Verifica por caminho
            bool foundByPath = ConfigManager.Config.Files.Values
                .Any(entry => string.Equals(entry.Path, filePath, StringComparison.OrdinalIgnoreCase));

            if (foundByPath)
                return true;

            // Calcula o hash MD5 e verifica por hash
            string hash = FileHashHelper.CalculateMD5(filePath);
            return ConfigManager.Config.Files.ContainsKey(hash);
        }

        public static string? GetEmulatorPathByName(string emulatorName)
        {
            if (ConfigManager.Config.Emulators.TryGetValue(emulatorName, out var emulatorEntry))
            {
                return emulatorEntry.Path;
            }

            return null;
        }
    }
}
