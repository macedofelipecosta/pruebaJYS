using AutoMapper;
using LogicaNegocio.Dominio;
using LogicaNegocio.Dominio.Reservations;
using LogicaNegocio.Dominio.Rooms;

namespace DTOs.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //Salas
            CreateMap<Room, RoomDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));

            CreateMap<RoomDTO, Room>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore());

            //Sedes
            CreateMap<Location, LocationDTO>().ReverseMap();

            //Reservas
            CreateMap<Reservation, ReservationDTO>()
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
                 .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Room.Id))
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                 .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                 .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<ReservationDTO, Reservation>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id != 0));
        }
    }
}

