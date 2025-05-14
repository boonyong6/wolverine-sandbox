using WolverineSandbox.Domain.Entities;

namespace WolverineSandbox.Domain.Repositories;

public class UserRepository
{
    private readonly Dictionary<Guid, User> _users = [];

    public void Store(User user)
    {
        _users[user.Id] = user;
    }

    public User Get(Guid id)
    {
        if (_users.TryGetValue(id, out User? user))
        {
            return user;
        }

        throw new ArgumentOutOfRangeException(nameof(id), "User does not exist.");
    }
}
