public interface IDataContainer<T> {
    bool HasData { get; }
    T Load();
    void Save(T data);
    void Delete();
}
