using System.ComponentModel.DataAnnotations.Schema;

namespace PZ17.Models; 

public class Procedure {
    [Column("procedure_id")]
    public int ProcedureId { get; set; }
    [Column("procedure_name")]
    public string ProcedureName { get; set; }
    [Column("base_price")]
    public decimal BasePrice { get; set; }
}