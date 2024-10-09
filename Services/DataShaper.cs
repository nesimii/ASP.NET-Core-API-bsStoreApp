using Entities.Models;
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

    public IEnumerable<ShapedEntity> ShapeListData(IEnumerable<T> entities, string? fieldsString)
    {
        IEnumerable<PropertyInfo> requiredFields = GetRequiredProperties(fieldsString);
        return FetchDataForList(entities, requiredFields);
    }

    public ShapedEntity ShapeData(T entity, string? fieldsString)
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

    private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
    {
        ShapedEntity shapedObject = new ShapedEntity();

        foreach (PropertyInfo property in requiredProperties)
        {
            object? objectPropertyValue = property.GetValue(entity);
            shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
        }
        var objectProperty = entity.GetType().GetProperty("Id");
        shapedObject.Id = (int)objectProperty.GetValue(entity);
        return shapedObject;
    }

    private IEnumerable<ShapedEntity> FetchDataForList(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
    {
        List<ShapedEntity> shapedData = new List<ShapedEntity>();
        foreach (T entity in entities)
        {
            ShapedEntity shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }
        return shapedData;
    }
}
