using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace LibraryAPI.Models

{
	public class BookLanguage
	{
		public int  BooksID { get; set; }

		
        [Column(TypeName = "char(3)")]
        public string LanguagesCode { get; set; } = "";

		[JsonIgnore]
		[ForeignKey(nameof(BooksID))]
		public Book? Book { get; set; }

        [ForeignKey(nameof(LanguagesCode))]
        public Language? Language { get; set; }

	}
}

