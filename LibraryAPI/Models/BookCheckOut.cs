using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Models
{
	public class BookCheckOut
	{
		public long ID { get; set; }


		public string MemberId { get; set; } = "";
		public string? EmployeeId { get; set; } = "";
		public string? RecieverEmployeeId { get; set; }

		public DateTime ChekoutDate { get; set; }

		public DateTime ReturnDate { get; set; }

		public DateTime ActualDateOfReturn { get; set; } 

        public bool IsBookHarmed { get; set; }

		

		[ForeignKey(nameof(EmployeeId))]
		public Employee? Employee { get; set; }


		[ForeignKey(nameof(MemberId))]
		[JsonIgnore]
        public Member? Member { get; set; }

       
        public int? BookCopyID { get; set; }

		

		[JsonIgnore]
		[ForeignKey(nameof(BookCopyID))]
        public BookCopy? BookCopy { get; set; }


	}
}

