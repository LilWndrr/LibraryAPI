using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models;

public class PunishmentInvoice
{
    public long ID { get; set; }
    
    
    [Required]
    public short PunishmentReasonId { get; set; }
    [Required]
    public string MemberId { get; set; } = "";

    public bool IsClosed { get; set; }

    public long? BookCheckoutId { get; set; }

    [ForeignKey(nameof(BookCheckoutId))]
    public BookCheckOut? BookCheckOut { get; set; }


    [JsonIgnore]
    [ForeignKey(nameof(MemberId))]
    public Member? Member { get; set; }
    [ForeignKey(nameof(PunishmentReasonId))]
    public PunishmentReason? PunishmentReason { get; set; }

    
}