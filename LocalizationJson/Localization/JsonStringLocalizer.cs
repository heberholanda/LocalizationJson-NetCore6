using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Text;

namespace Localization.Languages
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly string _basePath = "Localization/Languages";
        private readonly string filePath;

        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
            filePath = $"{_basePath}/{Thread.CurrentThread.CurrentCulture.Name}.json";
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                    : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var languageValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));
            var localizedValues = new List<LocalizedString>();
            foreach (var language in languageValues)
                localizedValues.Add(new LocalizedString(language.Key, language.Value));

            return localizedValues;
        }

        private string? GetString(string key)
        {
            if (File.Exists(Path.GetFullPath(filePath)))
            {
                var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cacheValue = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return cacheValue;
                }

                var result = GetValueFromJSON(key, Path.GetFullPath(filePath));

                if (!string.IsNullOrEmpty(result))
                {
                    _cache.SetString(cacheKey, result);

                }
                return result;
            }
            return default;
        }

        private string? GetValueFromJSON(string propertyName, string filePath)
        {
            if ((propertyName == null) || (filePath == null))
                return default;

            var languageValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));
            var localizedValues = new List<LocalizedString>();
            foreach (var language in languageValues)
            {
                if (language.Key == propertyName)
                {
                    return language.Value;
                }
            }

            return default;
        }
    }
}
