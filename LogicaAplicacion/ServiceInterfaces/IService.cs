namespace LogicaAplicacion.ServiceInterfaces
{
    public interface IService<T>
    {
       // T Create(T obj);
        T FindById(int id);
        IEnumerable<T> List();
        T Update(T obj);
        void Delete(int id);
    }
}
