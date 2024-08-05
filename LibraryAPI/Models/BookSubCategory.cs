using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace LibraryAPI.Models
{
	public class BookSubCategory
	{
		public int BooksID { get; set; }
		public short SubCategoryId { get; set; }

		[JsonIgnore]
		[ForeignKey(nameof(BooksID))]
		public Book? Book { get; set; }

		[ForeignKey(nameof(SubCategoryId))]
		public SubCategory? SubCategory { get; set; }
	}
}

