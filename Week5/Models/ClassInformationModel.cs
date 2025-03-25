using System.ComponentModel.DataAnnotations;

namespace Week5.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required]
        public string ClassName { get; set; } = string.Empty;  //  default empty

        [Range(1, 1000, ErrorMessage = "Öğrenci sayısı 1 ile 1000 arasında olmalıdır.")]
        public int StudentCount { get; set; }

        public string Description { get; set; } = string.Empty;  // 
    }
}
