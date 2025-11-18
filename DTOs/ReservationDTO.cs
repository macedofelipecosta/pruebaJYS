namespace DTOs
{
    public class ReservationDTO
    {

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomId { get; set; }
        public RoomDTO Room { get; set; } = null!; //ToDo: quizas no poner este objeto solo el id para desacoplar
        public int ReservationStatusId { get; set; } //ToDo: esto al final es un objeto o un enum?
        public string Status { get; set; } = string.Empty;// 1-Pending, 2-Confirmed, 3-Cancelled
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> UsersToNotifyId { get; set; } = new List<string>();
        public int OutlookEventId { get; set; } //ToDo: Esto no se para que es?!?!?!?!?!?!?
        public DateTime CancelledAt { get; set; }
        public TimeSpan Duration => EndDate - StartDate;
        public bool Extended { get; set; }
        public DateTime? ExtendedUntil { get; set; }
    }
}
