using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LogicaDatos.Migrations
{
    /// <inheritdoc />
    public partial class DominioActualizado12112025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Rooms",
                newName: "RoomStatusId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Reservations",
                newName: "ReservationStatusId");

            migrationBuilder.RenameColumn(
                name: "ParticipantStatus",
                table: "ReservationParticipants",
                newName: "ParticipantStatusId");

            migrationBuilder.AddColumn<int>(
                name: "RoomNumber",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "Parameters",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ParticipantStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservationStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsReservable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomStatus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "Description", "Name", "Value" },
                values: new object[,]
                {
                    { 1, "Tiempo mínimo entre reservas en minutos", "TiempoEntreReservas", 15 },
                    { 2, "Tiempo máximo permitido de extensión de una reserva en minutos", "TiempoMaximoExtension", 60 },
                    { 3, "Días máximos de anticipación para reservar", "MaximoDiasAnticipacion", 30 }
                });

            migrationBuilder.InsertData(
                table: "ParticipantStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Accepted" },
                    { 3, "Declined" }
                });

            migrationBuilder.InsertData(
                table: "ReservationStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Confirmed" },
                    { 3, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "RoomStatus",
                columns: new[] { "Id", "IsReservable", "Name" },
                values: new object[,]
                {
                    { 1, true, "Available" },
                    { 2, false, "Reserved" },
                    { 3, false, "OutOfService" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomStatusId",
                table: "Rooms",
                column: "RoomStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationStatusId",
                table: "Reservations",
                column: "ReservationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationParticipants_ParticipantStatusId",
                table: "ReservationParticipants",
                column: "ParticipantStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationParticipants_ParticipantStatus_ParticipantStatusId",
                table: "ReservationParticipants",
                column: "ParticipantStatusId",
                principalTable: "ParticipantStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ReservationStatus_ReservationStatusId",
                table: "Reservations",
                column: "ReservationStatusId",
                principalTable: "ReservationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomStatus_RoomStatusId",
                table: "Rooms",
                column: "RoomStatusId",
                principalTable: "RoomStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationParticipants_ParticipantStatus_ParticipantStatusId",
                table: "ReservationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ReservationStatus_ReservationStatusId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomStatus_RoomStatusId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "ParticipantStatus");

            migrationBuilder.DropTable(
                name: "ReservationStatus");

            migrationBuilder.DropTable(
                name: "RoomStatus");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomStatusId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReservationStatusId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_ReservationParticipants_ParticipantStatusId",
                table: "ReservationParticipants");

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "RoomStatusId",
                table: "Rooms",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ReservationStatusId",
                table: "Reservations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ParticipantStatusId",
                table: "ReservationParticipants",
                newName: "ParticipantStatus");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Parameters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
