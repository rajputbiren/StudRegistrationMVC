using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudRegistrationMVC.Models
{
    public class StudentViewModel
    {
        public int StudentId { get; set; }
        [Required(ErrorMessage ="Student name is required.")]
        [StringLength(50,ErrorMessage = "Max length is 50 charecters.")]
        public string StudentName { get; set; }
        [Required(ErrorMessage ="Email is required.")]
        [EmailAddress(ErrorMessage ="Pls enter valid email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(50,ErrorMessage = "Phone no must be 10 digit.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        public int? CountryId { get; set; }
        [Required(ErrorMessage = "State is required.")]
        public int? StateId { get; set; }
        [Required(ErrorMessage = "City is required.")]
        public int? CityId { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address no must be 10 digit.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "DOB is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DOB { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }
        public bool IsActive { get; set; }
    }
}