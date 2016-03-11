using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Models
{
    /// <summary>
    /// User record.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User's database ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User's email.
        /// </summary>
        [Required]
        public string EMail { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// User's surname
        /// </summary>
        [Required]
        public string Surname { get; set; }
    }
}