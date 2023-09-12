using System;
using System.Collections.Generic;
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
    /// <param name="table_name">TEMPORARY: Имя таблицы из которой будет производится чтение</param>
    /// <typeparam name="T">Тип получаемых объектов</typeparam>
    /// <returns>Все объекты таблицы преобразованные в тип T</returns>
    public IEnumerable<T> Get<T>(string table_name) where T : new() {
        if (_connection.State != ConnectionState.Open) _connection.Open();
        using var cmd = new MySqlCommand(table_name, _connection);
        cmd.CommandType = CommandType.TableDirect;
        var reader = cmd.ExecuteReader();
        var columns = GetColumns<T>();
        while (reader.Read()) {
            var obj = new T();
            foreach (var column in columns) { 
                column.Property.SetValue(obj, reader.GetValue(column.ColumnAttribute.Name));
            }
            yield return obj;
        }
    }

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

    
    static Func<CustomAttributeData, bool> typeChecker = p => p.AttributeType == typeof(ColumnAttribute);
    private static IEnumerable<ColumnInfo> GetColumns<T>() {
        return typeof(T)
            .GetProperties()
            .Where((it, i) 
                => it.CustomAttributes.Any(typeChecker)
            ).Select(it => new ColumnInfo {
                Property = it,
                ColumnAttribute = (ColumnAttribute)it.GetCustomAttributes().First()
            });
    }

    class ColumnInfo {
        public PropertyInfo Property { get; set; }
        public ColumnAttribute ColumnAttribute { get; set; }
    }

    public void Dispose() {
        _connection.Dispose();
    }

    public async ValueTask DisposeAsync() {
        await _connection.DisposeAsync();
    }
}