using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
	public class BookCopy
	{
		public int Id { get; set; }

		public bool IsBorrowed { get; set; }
		
		public bool IsHarmed { get; set; }
		[Required]
		public int BookID { get; set; }

		[JsonIgnore]
        public bool isDeleted { get; set; }
		public BookCheckOut? BookCheckOut { get; set; }

		[ForeignKey(nameof(BookID))]
		[JsonIgnore]
		public Book? Book { get; set; }
	}
}

