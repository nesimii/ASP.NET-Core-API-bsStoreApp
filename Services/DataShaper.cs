using Services.Contracts;
using System.Dynamic;
using System.Reflection;

namespace Services;

public class DataShaper<T> : IDataShaper<T> where T : class
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ExpandoObject> ShapeListData(IEnumerable<T> entities, string? fieldsString)
    {
        IEnumerable<PropertyInfo> requiredFields = GetRequiredProperties(fieldsString);
        return FetchDataForList(entities, requiredFields);
    }

    public ExpandoObject ShapeData(T entity, string? fieldsString)
    {
        IEnumerable<PropertyInfo> requiredProperties = GetRequiredProperties(fieldsString);
        return FetchDataForEntity(entity, requiredProperties);
    }

    private IEnumerable<PropertyInfo> GetRequiredProperties(string? fieldsString)
    {
        if (string.IsNullOrWhiteSpace(fieldsString)) return Properties.ToList();

        List<PropertyInfo> requiredFields = new List<PropertyInfo>();

        string[] fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (string field in fields)
        {
            PropertyInfo? property = Properties.FirstOrDefault(p => p.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (property is not null) requiredFields.Add(property);
        }
        return requiredFields;
    }

    private ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
    {
        ExpandoObject shapedObject = new ExpandoObject();

        foreach (PropertyInfo property in requiredProperties)
        {
            object? objectPropertyValue = property.GetValue(entity);
            shapedObject.TryAdd(property.Name, objectPropertyValue);
        }
        return shapedObject;
    }

    private IEnumerable<ExpandoObject> FetchDataForList(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
    {
        List<ExpandoObject> shapedData = new List<ExpandoObject>();
        foreach (T entity in entities)
        {
            ExpandoObject shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }
        return shapedData;
    }
}
