using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
	public class Vote
	{
        public int BookId { get; set; }
		[ForeignKey(nameof(BookId))]
		public Book? Book { get; set; }

		[Range(1,5)]
		public float VoteValue { get; set; }
		[JsonIgnore]
		public string? MemberId { get; set; }

		[ForeignKey(nameof(MemberId))]
		public Member? Member { get; set; }

		
	}
}

