using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
namespace LibraryAPI.Models
{
	public class AuthorBook
	{
		
		public long AuthorsId { get; set; }

		
		public int  BooksID { get; set; }

        
        [ForeignKey(nameof(AuthorsId))]
		public Author? Author { get; set; }

		[JsonIgnore]
        [ForeignKey(nameof(BooksID))]
        public Book? Book { get; set; }

    }
}

