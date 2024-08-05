﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Models
{
    public class AppUser:IdentityUser
    {
    
        public long IdNumber { get; set; }
        public string Name { get; set; } = "";
        public string? MiddleName { get; set; }
        public string? FamilyName { get; set; }
 
        
        public string Address { get; set; } = "";
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public byte Status { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public string? Password { get; set; }
        [NotMapped]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
        


    }
    
    public class Member 
    {
        [Key]
        public string Id { get; set; } = "";
        

        [ForeignKey(nameof(Id))]
        public AppUser? AppUser{ get; set; }

        public byte EducationalDegree { get; set; }
        [JsonIgnore]
        public List<Vote>? Votes { get; set; }
        public List<BookCheckOut>? BookCheckOuts { get; set; }

        public List<PunishmentInvoice>? PunishmentHistory { get; set; }
        

    }    

    public class Employee 
    {
        [Key]
        public string Id { get; set; } = "";
        
        [ForeignKey(nameof(Id))]
        public AppUser? AppUser { get; set; }
        

        public string Title { get; set; } = "";
        public float Salary { get; set; }
        public string Department { get; set; } = "";
        public string? Shift { get; set; }
        public List<BookCheckOut>? BookCheckOuts { get; set; }
        
    }
}