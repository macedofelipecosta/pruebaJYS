namespace LogicaNegocio.InterfacesRepositorios
{
    public interface IRepository<T>
    {
        public void Add(T obj);
        public void Update(T obj);
        public void Remove(T obj);
        public T GetById(int id);
        public IEnumerable<T> GetAll();

    }
}
