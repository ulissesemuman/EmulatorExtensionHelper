using System;
using System.Collections.Generic;
using System.IO;
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
        public string Language { get; set; } = LanguageManager.DefaultLanguage;

        [JsonPropertyName("emulators")]
        public Dictionary<string, EmulatorConfig> Emulators { get; set; } = new();

        [JsonPropertyName("files")]
        public Dictionary<string, FileEntry> Files { get; set; } = new();
    }

    public class EmulatorConfig
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("extensions")]
        public HashSet<string> Extensions { get; set; } = new();
    }

    public class EmulatorInfo
    {
        public string Name { get; set; }
        public EmulatorConfig Config { get; set; }
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
        private static LanguageManager lang;

        public static readonly string ConfigPath ;

        private static ConfigModel _config = new ConfigModel();

        public static ConfigModel Config { get => _config; set => _config = value; }

        static ConfigManager()
        {
            ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

            LoadConfig();

            lang = new LanguageManager();
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

        public static void AddOrUpdateEmulator(string friendlyName, string path, string extension, string iconFileName)
        {
            if (!Config.Emulators.ContainsKey(friendlyName))
            {
                Config.Emulators[friendlyName] = new EmulatorConfig
                {
                    Extensions = new HashSet<string>()
                };
            }

            var emulator = Config.Emulators[friendlyName];

            emulator.Path = path;
            emulator.Icon = iconFileName;

            emulator.Extensions.Add(extension);
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

        public static void AssociateFileWithEmulator(string filePath, string emulatorName, string emulatorPath, string iconFileName)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            string hash = FileHashHelper.CalculateMD5(filePath);
            //emulatorPath = GetEmulatorPathByName(emulatorName);

            ConfigManager.AddOrUpdateEmulator(emulatorName, emulatorPath, extension, iconFileName);

            string existingHash = FindFileHash(hash, filePath);
            if (existingHash == null)
                ConfigManager.AddOrUpdateFileEntry(hash, filePath, emulatorName);
            else
                ConfigManager.AddOrUpdateFileEntry(existingHash, filePath, emulatorName);

            ConfigManager.SaveConfig();

            FileAssociationHelper.AssociateExtensionToLauncher(Path.GetExtension(filePath));
        }

        public static void AssociateExtensionWithEmulator(string filePath, string emulatorName, string emulatorPath, string iconFileName)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            //emulatorPath = GetEmulatorPathByName(emulatorName);

            // Atualiza ou adiciona o emulador com a nova extensão
            ConfigManager.AddOrUpdateEmulator(emulatorName, emulatorPath, extension, iconFileName);

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
                ConfigManager.Config.Files.Remove(matchingKey);
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
                    return Config.Emulators
                        .Where(pair => !pair.Value.Extensions
                            .Any(ext => string.Equals(ext, Path.GetExtension(filePath.ToLowerInvariant()), StringComparison.OrdinalIgnoreCase)))
                        .Select(pair => pair.Key)
                        .ToList();
                }
                else
                {
                    var extension = Path.GetExtension(filePath.ToLowerInvariant());

                    return Config.Emulators
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
            // Calcula o hash MD5 do arquivo
            string hash = FileHashHelper.CalculateMD5(filePath);
            HashSet<string> associatedEmulators = new();

            // Verifica se o hash existe em files
            if (Config.Files.TryGetValue(hash, out var fileEntry))
            {
                associatedEmulators = fileEntry.Emulators != null
                    ? new HashSet<string>(fileEntry.Emulators)
                    : new HashSet<string>();
            }
            else
            {
                // Verifica se há alguma entrada cujo path coincida com o caminho completo
                var match = Config.Files.Values
                    .FirstOrDefault(f => string.Equals(f.Path, filePath, StringComparison.OrdinalIgnoreCase));

                if (match?.Emulators != null)
                {
                    associatedEmulators = new HashSet<string>(match.Emulators);
                }
            }

            // Retorna os nomes dos emuladores que NÃO estão associados
            return Config.Emulators.Keys
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

            // Calcula o hash MD5 e verifica por hash"
            string hash = FileHashHelper.CalculateMD5(filePath);
            return ConfigManager.Config.Files.ContainsKey(hash);
        }

        public static string? GetEmulatorPathByName(string emulatorName)
        {
            if (Config.Emulators.TryGetValue(emulatorName, out var emulatorEntry))
            {
                return emulatorEntry.Path;
            }

            return null;
        }

        public static void EnsureConfigFileExists()
        {
            if (File.Exists(ConfigPath))
                return;

            var config = new
            {
                language = "en-us"
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(ConfigPath, json);
        }

        public static void UpdateLanguage(string newLanguageCode)
        {
            Config.Language = newLanguageCode;

            SaveConfig();
        }

        public static List<EmulatorConfig> ListConfigEmulators(string caminhoConfig)
        {
            if (!File.Exists(caminhoConfig))
                throw new FileNotFoundException("Arquivo config.json não encontrado.", caminhoConfig);

            string json = File.ReadAllText(caminhoConfig);

            // Define um dicionário da seção "emulators"
            using JsonDocument document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("emulators", out JsonElement emulatorsElement))
                throw new InvalidDataException("Seção 'emulators' não encontrada no config.json.");

            var emuladores = new List<EmulatorConfig>();

            foreach (JsonProperty emulador in emulatorsElement.EnumerateObject())
            {
                EmulatorConfig config = JsonSerializer.Deserialize<EmulatorConfig>(emulador.Value.GetRawText());
                if (config != null)
                    emuladores.Add(config);
            }

            return emuladores;
        }

        public static List<EmulatorInfo> GetEmulatorsByExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            var resultado = new List<EmulatorInfo>();

            foreach (var kvp in Config.Emulators)
            {
                if (kvp.Value.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    resultado.Add(new EmulatorInfo
                    {
                        Name = kvp.Key,
                        Config = new EmulatorConfig { Path = kvp.Value.Path, Icon = kvp.Value.Icon, Extensions = kvp.Value.Extensions }
                    });
                }
            }

            return resultado;
        }

        public static List<EmulatorInfo> GetEmulatorsByRom(string filePath)
        {
            var resultado = new List<EmulatorInfo>();
            var associados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string hash = FileHashHelper.CalculateMD5(filePath);

            if (Config.Files.TryGetValue(hash, out var fileEntry))
            {
                foreach (var nomeEmulador in fileEntry.Emulators)
                    associados.Add(nomeEmulador);
            }
            else
            {
                string nomeArquivo = Path.GetFileName(filePath);
                foreach (var kvp in Config.Files)
                {
                    if (Path.GetFileName(kvp.Value.Path).Equals(nomeArquivo, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var nomeEmulador in kvp.Value.Emulators)
                            associados.Add(nomeEmulador);
                    }
                }
            }

            foreach (var nome in associados)
            {
                if (Config.Emulators.TryGetValue(nome, out var emulador))
                {
                    resultado.Add(new EmulatorInfo
                    {
                        Name = nome,
                        Config = new EmulatorConfig { Path = emulador.Path, Icon = emulador.Icon, Extensions = emulador.Extensions } 
                    });
                }
            }

            return resultado;
        }

        // Retorna os emuladores que NÃO estão associados à extensão informada
        public static List<EmulatorInfo> GetEmulatorsWithoutExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            var resultado = new List<EmulatorInfo>();

            foreach (var kvp in Config.Emulators)
            {
                if (!kvp.Value.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    resultado.Add(new EmulatorInfo
                    {
                        Name = kvp.Key,
                        Config = new EmulatorConfig { Path = kvp.Value.Path, Icon = kvp.Value.Icon, Extensions = kvp.Value.Extensions }
                    });
                }
            }

            return resultado;
        }

        // Retorna os emuladores que NÃO estão associados à ROM informada (busca por hash, e se não houver, por nome de arquivo)
        public static List<EmulatorInfo> GetEmulatorsWithoutRom(string filePath)
        {
            var associados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string hash = FileHashHelper.CalculateMD5(filePath);

            if (Config.Files.TryGetValue(hash, out var fileEntry))
            {
                foreach (var nomeEmulador in fileEntry.Emulators)
                    associados.Add(nomeEmulador);
            }
            else
            {
                string nomeArquivo = Path.GetFileName(filePath);
                foreach (var kvp in Config.Files)
                {
                    if (Path.GetFileName(kvp.Value.Path).Equals(nomeArquivo, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var nomeEmulador in kvp.Value.Emulators)
                            associados.Add(nomeEmulador);
                    }
                }
            }

            var resultado = new List<EmulatorInfo>();

            foreach (var kvp in Config.Emulators)
            {
                if (!associados.Contains(kvp.Key))
                {
                    resultado.Add(new EmulatorInfo
                    {
                        Name = kvp.Key,
                        Config = new EmulatorConfig { Path = kvp.Value.Path, Icon = kvp.Value.Icon, Extensions = kvp.Value.Extensions }
                    });
                }
            }

            return resultado;
        }
    }
}
