using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Week5.Utilities
{
    public sealed class Utils
    {
        private static Utils _instance;
        private static readonly object _lock = new object();

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Utils();
                    }
                }
                return _instance;
            }
        }

        // Converts the entire data set to JSON.
        public string ExportToJson<T>(T data)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }

        // Converts the data set to JSON based on selected columns.
        // Each object is converted into a dictionary containing only the selected properties.
        public string ExportToJsonSelected<T>(IEnumerable<T> data, IEnumerable<string> selectedColumns)
        {
            var exportList = new List<Dictionary<string, object?>>();

            // Retrieve properties (case-insensitive matching with selected columns)
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => selectedColumns.Any(sc => string.Equals(sc, p.Name, StringComparison.OrdinalIgnoreCase)))
                                 .ToList();

            foreach (var item in data)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var prop in props)
                {
                    dict[prop.Name] = prop.GetValue(item);
                }
                exportList.Add(dict);
            }

            return JsonSerializer.Serialize(exportList, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
