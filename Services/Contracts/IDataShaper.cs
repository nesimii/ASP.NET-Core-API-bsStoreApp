using Entities.Models;
using System.Dynamic;

namespace Services.Contracts;

public interface IDataShaper<T>
{
    IEnumerable<ShapedEntity> ShapeListData(IEnumerable<T> entities, string? fieldsString);
    ShapedEntity ShapeData(T entity, string? fieldsString);
}
