public interface IProgressManager : IManager
{
    void Save();

    string SkinId { get; set; }
}
