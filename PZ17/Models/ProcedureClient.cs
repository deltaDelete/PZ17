using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PZ17.Models; 

public class ProcedureClient {
    [Column("id")]
    public int Id { get; set; }
    [Column("procedure_id")]
    public int ProcedureId { get; set; }
    [Column("client_id")]
    public int ClientId { get; set; }
    [Column("price")] 
    public decimal Price { get; set; }
    [Column("date")] 
    public DateTime Date { get; set; }
}