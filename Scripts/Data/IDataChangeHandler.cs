public delegate void DataChangeEventHandler();
public interface IDataChangeHandler
{
    event DataChangeEventHandler OnDataChange;
}
