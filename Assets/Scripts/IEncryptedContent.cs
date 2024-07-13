public interface IEncryptedContent
{
    public bool IsLocked { get; }
    public string Password { get; }
}