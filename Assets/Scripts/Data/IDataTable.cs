/// <summary>
/// 数据表接口，用于约束泛型
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
public interface IDataTable<T>
{
    void AddItem(T item);
}
