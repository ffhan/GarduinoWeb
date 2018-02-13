namespace Garduino.Data.Interfaces
{
    public interface ITimeable<T, U>
    {
        T GetLatest(U fromWhere);
    }
}
