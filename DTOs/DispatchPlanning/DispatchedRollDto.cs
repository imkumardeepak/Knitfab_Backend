namespace AvyyanBackend.DTOs.DispatchPlanning
{
    public class DispatchedRollDto
    {
        public int Id { get; set; }
        public int DispatchPlanningId { get; set; }
        public string LotNo { get; set; } = string.Empty;
        public string FGRollNo { get; set; } = string.Empty;
        public bool IsLoaded { get; set; }
        public DateTime? LoadedAt { get; set; }
        public string LoadedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}