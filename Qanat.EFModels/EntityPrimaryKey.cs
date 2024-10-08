using System;
using System.Globalization;

namespace Qanat.EFModels;

/// <summary>
/// Provides a way to have the MVC controller action context bind and load the object from the database
/// </summary>
public class EntityPrimaryKey<T> where T : class, IHavePrimaryKey
{
    private int _primaryKeyValue;
    public EntityPrimaryKey()
    {

    }
    public EntityPrimaryKey(int primaryKeyValue)
    {
        _primaryKeyValue = primaryKeyValue;
        // This isn't solved but I'm not sure we need it
        // _entityObject = new Lazy<T>(() => (T)SitkaHttpRequestStorage.LtInfoEntityTypeLoader.LoadType(typeof(T), primaryKeyValue));
    }
    public EntityPrimaryKey(T theObject)
    {
        _primaryKeyValue = theObject.PrimaryKey;
        _entityObject = new Lazy<T>(() => theObject);
    }
    private Lazy<T> _entityObject;
    public T EntityObject
    {
        get => _entityObject?.Value;
        set => _entityObject = new Lazy<T>(value);
    }
    public int PrimaryKeyValue
    {
        get => _entityObject?.IsValueCreated ?? false ? _entityObject.Value.PrimaryKey : _primaryKeyValue;
        set => _primaryKeyValue = value;
    }
    public override string ToString()
    {
        return PrimaryKeyValue.ToString(CultureInfo.InvariantCulture);
    }
}