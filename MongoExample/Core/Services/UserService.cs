using MongoDB.Bson;
using MongoExample.Core.Models;
using MongoExample.Core.Repositories;

namespace MongoExample.Core.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    
    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public void AddUser(UserModel user)
    {
        _userRepository.InsertOne(user);
    }
    
    public void UpdateUser(UserModel user)
    {
        _userRepository.UpdateOne(user);
    }
    
    public void UpdateUserName(string userId, string newName)
    {
        if (!ObjectId.TryParse(userId, out ObjectId userObjectId))
        {
            throw new ArgumentException("Invalid user ID format");
        }

        _userRepository.UpdateUserName(userObjectId, newName);
    }
}