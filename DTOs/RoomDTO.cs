namespace DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public int RoomStatusId { get; set; }
        public string Status { get; set; } = string.Empty; // 1-Available, 2-Reserved, 3-OutOfService
        public int LocationId { get; set; }
        public LocationDTO Location { get; set; } = null!;
    }
}
