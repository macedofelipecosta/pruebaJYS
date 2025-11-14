using AutoMapper;
using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using LogicaNegocio.Dominio;
using LogicaNegocio.InterfacesRepositorios;

namespace LogicaAplicacion.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repo;

        private readonly IMapper _mapper;
        public LocationService(ILocationRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public LocationDTO Create(LocationDTO obj)
        {
            var l = _mapper.Map<Location>(obj);
            l.Validate();
            _repo.Add(l);
            return obj;
        }

        public LocationDTO FindById(int id)
        {
            var result = _repo.GetById(id);
            return _mapper.Map<LocationDTO>(result);
        }

        public IEnumerable<LocationDTO> List()
        {
            IEnumerable<Location> locations = _repo.GetAll();
            return _mapper.Map<IEnumerable<LocationDTO>>(locations);
        }

        public LocationDTO Update(LocationDTO obj)
        {
            var l = _mapper.Map<Location>(obj);
            l.Validate();
            _repo.Update(l);
            return _mapper.Map<LocationDTO>(l);
        }

        public void Delete(int id)
        {
            var room = _repo.GetById(id);
            _repo.Remove(room);
        }
    }
}
