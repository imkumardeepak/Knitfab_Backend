using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.ProductionConfirmation
{
    public class WeightDataRequestDto
    {
        [Required]
        public string IpAddress { get; set; }

        public int Port { get; set; } = 23;
    }

    public class WeightDataResponseDto
    {
        public string GrossWeight { get; set; }
        public string TareWeight { get; set; }
        public string NetWeight { get; set; }
    }
}