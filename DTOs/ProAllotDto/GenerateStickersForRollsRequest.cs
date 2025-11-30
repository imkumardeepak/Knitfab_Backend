using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.ProAllotDto
{
    // Request DTO for generating stickers for specific roll numbers
    public class GenerateStickersForRollsRequest
    {
        [Required(ErrorMessage = "Roll numbers must be provided")]
        public List<int> RollNumbers { get; set; }
    }
}