using System.Text.Json.Serialization;

namespace AvyyanBackend.Models.ProductionConfirmation
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InspectionStatus
    {
        Accepted = 0,
        Rejected = 1,
        Hold = 2
    }
}
