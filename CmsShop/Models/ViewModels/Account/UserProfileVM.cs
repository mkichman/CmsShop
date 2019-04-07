using CmsShop.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace CmsShop.Models.ViewModels.Account
{
    public class UserProfileVM
    {
        public UserProfileVM()
        {

        }
       public UserProfileVM(UserDTO row)
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
        
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }

        public string Address { get; set; }

        public string Post { get; set; }

        public string PostOffice { get; set; }


    }
}