using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace LibraryAPI.Models
{
    public class Book
	{
        public int ID { get; set; }

        [StringLength(13)]
        [Column(TypeName="varchar(13)") ]
        public string? ISBN { get; set; }

        [Required]
        [StringLength(2000)]
        public string Title { get; set; } = "";

        [JsonIgnore]
        public bool isDeleted { get; set; }

        [Range(1, short.MaxValue)]
        public short PageCount { get; set; }

        [Range(-4000, 2100)]
        public short PublishingYear { get; set; }

        [StringLength(5000)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int PrintCount { get; set; }

        public bool Banned { get; set; }

        public int VotesCount { get; set; }
        [JsonIgnore]
        public float SumOfVotes { get; set; }

        public float Rating { get; set; }

        [Required]
        public int PublisherId { get; set; }

        
        [Range(0,short.MaxValue)]
        
        public short CopyCount { get; set; }

        //[NotMapped]
        //public string? Image { get; set; }


        [Required]
        [StringLength(6, MinimumLength = 3)]
        [Column(TypeName = "varchar(6)")]
        public string LocationShelf { get; set; } = "";

        [NotMapped]
        public List<long>? AuthorIds { get; set; }

        [NotMapped]
        public List<string>? LanguagesCodes { get; set; }

        [NotMapped]
        public List<short>? SubCategoriesIDs { get; set; }


        public List<AuthorBook>? AuthorBooks{ get; set; }
        public List<BookSubCategory>? BookSubCategories { get; set; }
        public List<BookLanguage>? BookLanguages { get; set; }
        public List<BookCopy>? BookCopies { get; set; }
        public List<Vote>? Votes { get; set; }



        [ForeignKey(nameof(PublisherId))]
        public Publisher? Publisher { get; set; }


        [JsonIgnore]
        [ForeignKey(nameof(LocationShelf))]
        public Location? Location { get; set; }

    }
}

