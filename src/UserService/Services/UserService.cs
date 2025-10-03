using System.Collections.Concurrent;
using System.Threading;
using AutoMapper;
using Loft.Common.DTOs;
using UserService.Entities;
using Microsoft.AspNetCore.Identity;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly ConcurrentDictionary<long, User> _users = new();
    private readonly ConcurrentDictionary<string, long> _emailIndex = new(StringComparer.OrdinalIgnoreCase);
    private long _lastId;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly IMapper _mapper;

    public UserService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public Task<UserDTO?> GetUserById(long userId)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            return Task.FromResult<UserDTO?>(_mapper.Map<UserDTO>(user));
        }
        return Task.FromResult<UserDTO?>(null);
    }

    public Task<UserDTO?> GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult<UserDTO?>(null);

        if (_emailIndex.TryGetValue(email, out var id) && _users.TryGetValue(id, out var user))
        {
            return Task.FromResult<UserDTO?>(_mapper.Map<UserDTO>(user));
        }

        return Task.FromResult<UserDTO?>(null);
    }

    public Task<UserDTO> CreateUser(UserDTO userDto, string password)
    {
        if (userDto == null) throw new ArgumentNullException(nameof(userDto));
        if (string.IsNullOrWhiteSpace(userDto.Email)) throw new ArgumentException("Email is required", nameof(userDto));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required", nameof(password));

        var email = userDto.Email.Trim();

        // Try reserve email to avoid race
        if (!_emailIndex.TryAdd(email, -1))
            throw new InvalidOperationException("Email already taken");

        try
        {
            var id = Interlocked.Increment(ref _lastId);
            var user = new User
            {
                Id = id,
                Email = email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                AvatarUrl = userDto.AvatarUrl,
                Phone = userDto.Phone,
                // For self-registered users role is always CUSTOMER; selling permission is controlled by CanSell
                Role = Loft.Common.Enums.Role.CUSTOMER,
                CanSell = userDto.CanSell
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            if (!_users.TryAdd(id, user))
                throw new Exception("Failed to add user");

            // update email index with real id
            _emailIndex[email] = id;

            return Task.FromResult(_mapper.Map<UserDTO>(user));
        }
        catch
        {
            // cleanup reserved email
            _emailIndex.TryRemove(email, out _);
            throw;
        }
    }

    public Task<UserDTO?> UpdateUser(long userId, UserDTO user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        if (!_users.TryGetValue(userId, out var existing))
            return Task.FromResult<UserDTO?>(null);

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.AvatarUrl = user.AvatarUrl;
        existing.Phone = user.Phone;
        // Role and Email updates are omitted here; implement carefully if needed

        return Task.FromResult<UserDTO?>(_mapper.Map<UserDTO>(existing));
    }

    public Task DeleteUser(long userId)
    {
        if (_users.TryRemove(userId, out var removed))
        {
            if (!string.IsNullOrWhiteSpace(removed.Email))
                _emailIndex.TryRemove(removed.Email, out _);
        }
        return Task.CompletedTask;
    }

    public Task<bool> IsEmailTaken(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return Task.FromResult(false);
        return Task.FromResult(_emailIndex.ContainsKey(email.Trim()));
    }

    public Task<UserDTO?> AuthenticateUser(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Task.FromResult<UserDTO?>(null);

        if (!_emailIndex.TryGetValue(email, out var id))
            return Task.FromResult<UserDTO?>(null);

        if (!_users.TryGetValue(id, out var user))
            return Task.FromResult<UserDTO?>(null);

        if (string.IsNullOrEmpty(user.PasswordHash))
            return Task.FromResult<UserDTO?>(null);

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Success || verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            return Task.FromResult<UserDTO?>(_mapper.Map<UserDTO>(user));
        }

        return Task.FromResult<UserDTO?>(null);
    }
}