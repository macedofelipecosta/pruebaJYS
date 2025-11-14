using AutoMapper;
using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using LogicaNegocio.Dominio.Rooms;
using LogicaNegocio.InterfacesRepositorios;

namespace LogicaAplicacion.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repo;

        private readonly IMapper _mapper;
        public RoomService(IRoomRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public RoomDTO Create(RoomDTO objDto)
        {
            var r = _mapper.Map<Room>(objDto);
            _repo.Add(r);
            return _mapper.Map<RoomDTO>(r);
        }

        public RoomDTO FindById(int id)
        {
            var result = _repo.GetById(id);
            return _mapper.Map<RoomDTO>(result);
        }

        public IEnumerable<RoomDTO> List()
        {
            IEnumerable<Room> rooms = _repo.GetAll();
            return _mapper.Map<IEnumerable<RoomDTO>>(rooms);
        }

        public RoomDTO Update(RoomDTO obj)
        {
            var r = _mapper.Map<Room>(obj);
            _repo.Update(r);
            return _mapper.Map<RoomDTO>(r);
        }

        public void Delete(int id)
        {
            var room = _repo.GetById(id);
            _repo.Remove(room);
        }
    }
}
