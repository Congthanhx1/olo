namespace DigitalStore.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    string Role { get; }
}
