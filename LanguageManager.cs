using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmulatorExtensionHelper
{
    internal class LanguageManager
    {
        private Dictionary<string, string> _currentLang = new();
        private Dictionary<string, string> _fallbackLang = new();

        private const string LanguageFolderPath = "language";
        private const string LanguageFileName = "en-us.json";
        private const string LanguageFileUrl = $"https://raw.githubusercontent.com/ulissesemuman/EmulatorExtensionHelper/master/{LanguageFolderPath}/{LanguageFileName}";

        private static LanguageManager lang = new LanguageManager();

        public LanguageManager()
        {
            string langCode = System.Globalization.CultureInfo.CurrentUICulture.Name.ToLower(); ; // exemplo: "pt-br"
            string configPath = ConfigManager.ConfigPath;

            if (File.Exists(configPath))
            {
                try
                {
                    var config = ConfigManager.GetConfig();

                    if (config != null)
                        langCode = config.Language.ToLower();
                }
                catch { }
            }

            string langFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LanguageFolderPath, $"{langCode}.json");
            string fallbackFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LanguageFolderPath, LanguageFileName);

            if (File.Exists(fallbackFile))
                _fallbackLang = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(fallbackFile))!;

            if (File.Exists(langFile))
                _currentLang = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(langFile))!;
            else
                _currentLang = _fallbackLang;
        }

        public string T(string key)
        {
            if (_currentLang.TryGetValue(key, out var value)) return value;
            if (_fallbackLang.TryGetValue(key, out var fallback)) return fallback;
            return key; // retorna a chave se não encontrado
        }

        public static void EnsureLanguageFolderExists()
        {
            if (!Directory.Exists(LanguageFolderPath))
            {
                Directory.CreateDirectory(LanguageFolderPath);
            }
        }

        public static async Task EnsureDefaultLanguageFileAsync()
        {
            string filePath = Path.Combine(LanguageFolderPath, LanguageFileName);

            if (!File.Exists(filePath))
            {
                var result = MessageBox.Show(
                    lang.T("LanguageManager.MissingLanguageFileMessage"),
                    lang.T("LanguageManager.MissingLanguageFileTitle"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);


                Directory.CreateDirectory(LanguageFolderPath);

                try
                {
                    using HttpClient client = new HttpClient();
                    string content = await client.GetStringAsync(LanguageFileUrl);
                    await File.WriteAllTextAsync(filePath, content);
                }
                catch (Exception ex)
                {
                    // Aqui você pode logar ou alertar o usuário
                    Console.WriteLine($"Failed to download default language file: {ex.Message}");
                }
            }
        }
    }
}
