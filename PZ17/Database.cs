using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MySqlConnector;
using PZ17.Models;

namespace PZ17;

public class Database : IDisposable, IAsyncDisposable {
    public const string ConnectionString =
        // "Server=10.10.1.24;Uid=user_01;Database=pro1_2;Port=3306;Pwd=user01pro;";
    "Server=10.10.1.24;Database=pro1_2;Uid=user_01;Pwd=user01pro;";

    private MySqlConnection _connection;

    public Database() {
        _connection = new MySqlConnection(ConnectionString);
        _connection.Open();
    }

    /// <summary>
    /// Крутой Generic метод для аннотированных атрибутом Column классов
    /// </summary>
    /// <typeparam name="T">Тип получаемых объектов</typeparam>
    /// <returns>Все объекты таблицы преобразованные в тип T</returns>
    public IEnumerable<T> Get<T>() where T : new() {
        var columns = GetColumns<T>().ToList();
        var tableInfo = GetTableName<T>();
        if (tableInfo is null) throw new Exception($"Тип {nameof(T)} не имеет атрибута Table");

        if (_connection.State != ConnectionState.Open) _connection.Open();
        using var cmd = new MySqlCommand($"select * from `{tableInfo.Name}`", _connection);
        var reader = cmd.ExecuteReader();

        while (reader.Read()) {
            var obj = new T();
            foreach (var column in columns) {
                if (column.ColumnAttribute.Name is null) {
                    throw new Exception($"Атрибут Column свойства {column.Property.Name} типа {nameof(T)} " +
                                        "не имеет заданного имени");
                }
                column.Property.SetValue(obj, reader.GetValue(column.ColumnAttribute.Name));
            }

            yield return obj;
        }
    }

    [Obsolete("Генерик лучше")]
    public IEnumerable<Client> GetUsers() {
        if (_connection.State != ConnectionState.Open) _connection.Open();
        using var cmd = new MySqlCommand("select * from clients", _connection);
        var reader = cmd.ExecuteReader();
        var columns = GetColumns<Client>();
        while (reader.Read()) {
            yield return new Client() {
                ClientId = reader.GetInt32("client_id"),
                LastName = reader.GetString("last_name"),
                FirstName = reader.GetString("first_name"),
            };
        }
    }

    public T? GetById<T>(int id) where T : new() {
        var columns = GetColumns<T>().ToList();
        var tableInfo = GetTableName<T>();
        if (tableInfo is null) throw new Exception($"Тип {nameof(T)} не имеет атрибута Table");

        var keys = columns
            .Where(it => it.Property.GetCustomAttribute<KeyAttribute>() is not null)
            .Select(it => it.Property)
            .ToList();
        if (!keys.Any()) {
            throw new Exception($"Тип {nameof(T)} не содержит свойства с атрибутом Key");
        }

        var primaryKeyAttribute = keys.First().GetCustomAttribute<ColumnAttribute>();
        if (primaryKeyAttribute is null) {
            throw new Exception($"Свойство {keys.First().Name} типа {nameof(T)} не имеет атрибута Column");
        }

        var primaryKey = primaryKeyAttribute.Name;

        using var cmd = new MySqlCommand($"select * from `{tableInfo.Name}` where `{primaryKey}` = {id}", _connection);
        var reader = cmd.ExecuteReader();
        while (reader.Read()) {
            var obj = new T();
            foreach (var column in columns) {
                if (column.ColumnAttribute.Name is null) {
                    throw new Exception($"Атрибут Column свойства {column.Property.Name} типа {nameof(T)} " +
                                        "не имеет заданного имени");
                }
                column.Property.SetValue(obj, reader.GetValue(column.ColumnAttribute.Name));
            }
            return obj;
        }
        cmd.Cancel();
        return default;
    }
    
    static Func<CustomAttributeData, bool> typeChecker = p => p.AttributeType == typeof(ColumnAttribute);

    private static IEnumerable<ColumnInfo> GetColumns<T>() {
        return typeof(T)
            .GetProperties()
            .Where(it => it.CustomAttributes.Any(typeChecker))
            .Select(
                it => new ColumnInfo(it, it.GetCustomAttribute<ColumnAttribute>()!)
            );
    }

    private static TableInfo? GetTableName<T>() {
        IEnumerable<TableInfo?> info = typeof(T)
            .GetCustomAttributes<TableAttribute>()
            .Select(it => new TableInfo(typeof(T), it.Name));
        return info.Any() ? info.First() : null;
    }

    public record ColumnInfo(PropertyInfo Property, ColumnAttribute ColumnAttribute);

    public record TableInfo(Type Type, String Name);

    public void Dispose() {
        _connection.Dispose();
    }

    public async ValueTask DisposeAsync() {
        await _connection.DisposeAsync();
    }

    #region Async

    public async Task<T?> GetByIdAsync<T>(int id) where T : new() {
        var columns = GetColumns<T>().ToList();
        var tableInfo = GetTableName<T>();
        if (tableInfo is null) throw new Exception($"Тип {nameof(T)} не имеет атрибута Table");

        var keys = columns
            .Where(it => it.Property.GetCustomAttribute<KeyAttribute>() is not null)
            .Select(it => it.Property)
            .ToList();
        if (!keys.Any()) {
            throw new Exception($"Тип {nameof(T)} не содержит свойства с атрибутом Key");
        }

        var primaryKeyAttribute = keys.First().GetCustomAttribute<ColumnAttribute>();
        if (primaryKeyAttribute is null) {
            throw new Exception($"Свойство {keys.First().Name} типа {nameof(T)} не имеет атрибута Column");
        }

        var primaryKey = primaryKeyAttribute.Name;

        using var cmd = new MySqlCommand($"select * from `{tableInfo.Name}` where `{primaryKey}` = {id}", _connection);
        var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            var obj = new T();
            foreach (var column in columns) {
                if (column.ColumnAttribute.Name is null) {
                    throw new Exception($"Атрибут Column свойства {column.Property.Name} типа {nameof(T)} " +
                                        "не имеет заданного имени");
                }

                column.Property.SetValue(obj, reader.GetValue(column.ColumnAttribute.Name));
            }

            return obj;
        }
        return default;
    }
    
    public async IAsyncEnumerable<T> GetAsync<T>() where T : new() {
        var columns = GetColumns<T>().ToList();
        var tableInfo = GetTableName<T>();
        if (tableInfo is null) throw new Exception($"Тип {nameof(T)} не имеет атрибута Table");

        if (_connection.State != ConnectionState.Open) _connection.Open();
        await using var cmd = new MySqlCommand($"select * from `{tableInfo.Name}`", _connection);
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            var obj = new T();
            foreach (var column in columns) {
                if (column.ColumnAttribute.Name is null) {
                    throw new Exception($"Атрибут Column свойства {column.Property.Name} типа {nameof(T)} " +
                                        "не имеет заданного имени");
                }
                column.Property.SetValue(obj, reader.GetValue(column.ColumnAttribute.Name));
            }

            yield return obj;
        }
    }

    #endregion
}