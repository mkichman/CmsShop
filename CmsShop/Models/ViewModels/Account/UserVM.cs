using CmsShop.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace CmsShop.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM()
        {
                
        }

        public UserVM(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            Username = row.Username;
            Password = row.Password;
            Address = row.Address;
            Post = row.Post;
            PostOffice = row.PostOffice;
        }
                          
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Post { get; set; }
        [Required]
        public string PostOffice { get; set; }



    }
}