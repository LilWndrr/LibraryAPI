using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models;

public class PunishmentReason
{

    public short Id { get; set; }
    [Column(TypeName="varchar(100)") ]
    [Required]
    public string Name { get; set; } = "";
    [StringLength(1000)]
    public string? Description { get; set; }
    [Required]
    public int Price { get; set; }
}