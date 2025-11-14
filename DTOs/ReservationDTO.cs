namespace DTOs
{
    public class ReservationDTO
    {

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomId { get; set; }
        public RoomDTO Room { get; set; } = null!;
        public int ReservationStatusId { get; set; }
        public string Status { get; set; } = string.Empty;// 1-Pending, 2-Confirmed, 3-Cancelled
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OutlookEventId { get; set; }
        public DateTime CancelledAt { get; set; }
        public TimeSpan Duration => EndDate - StartDate;
        public bool Extended { get; set; }
        public DateTime? ExtendedUntil { get; set; }
    }
}
