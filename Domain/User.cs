using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Domain;


    [Table("User")] 
    public class User
    {
        [Key] 
        public int UserId { get; set; }

        [Column("name")] 
        public string Name { get; set; }

        [Column("e_mail")] 
        public string Email { get; set; }

        [Column("password")] 
        public string Password { get; set; }

        [Column("phone")] 
        public string Phone { get; set; }
        

        [Column("type_user")] 
        public string TypeUser { get; set; } 
        
        [ForeignKey("Address")]   
        public int AddressId { get; set; }
        public Address Address { get; set; }
        


    }
