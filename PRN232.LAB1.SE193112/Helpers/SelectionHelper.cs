using System.Collections;
using System.Dynamic;
using System.Reflection;

namespace PRN232.LAB1.SE193112.Helpers
{
    public static class SelectionHelper
    {
        public static IEnumerable<object> SelectFields<T>(IEnumerable<T> source, string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
                return (IEnumerable<object>)source;

            var fieldList = fields.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(f => f.Trim())
                                  .ToList();

            var result = new List<ExpandoObject>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var item in source)
            {
                var expando = new ExpandoObject();
                var expandoDict = (IDictionary<string, object?>)expando;

                foreach (var field in fieldList)
                {
                    var property = properties.FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        expandoDict[property.Name] = property.GetValue(item);
                    }
                }
                result.Add(expando);
            }

            return result;
        }
    }
}
