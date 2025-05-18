using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmulatorExtensionHelper
{
    internal class LanguageManager
    {
        private Dictionary<string, string> _currentLang = new();
        private Dictionary<string, string> _fallbackLang = new();

        public LanguageManager(string languageFolderPath, string configPath)
        {
            string langCode = "pt-br";

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
            else
            {
                langCode = CultureInfo.CurrentCulture.Name.ToLower(); // exemplo: "pt-br"
            }

            string langFile = Path.Combine(languageFolderPath, $"{langCode}.json");
            string fallbackFile = Path.Combine(languageFolderPath, $"en-us.json");

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
    }
}
