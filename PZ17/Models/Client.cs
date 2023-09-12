using System.ComponentModel.DataAnnotations.Schema;

namespace PZ17.Models;

public class Client {
    [Column("client_id")]
    public int ClientId { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; }
    [Column("last_name")]
    public string LastName { get; set; }
}