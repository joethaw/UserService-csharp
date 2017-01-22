using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UserService.Models;

namespace UserService.Controllers
{
    // use interface so that how the data is stored is decoupled from how the consumer uses it
    // design by contract
    public interface IUsersRepository
    {
        IEnumerable<User> GetUsers();
        User GetUser(string id);
        string CreateUser(UserPost user);
        bool UpdateUser(string id, UserPut userInfo);
    }

    // This is a simple implementation of IUserRepository.
    // In real project, we can implement using a database.
    public class UsersRepository: IUsersRepository
    {
        // user ConcurrentDictionary to make it thread safe because in real world multiple requests can come at the same time
        private static readonly ConcurrentDictionary<string, User> _userDict = new ConcurrentDictionary<string, User>();

        public IEnumerable<User> GetUsers()
        {
            return _userDict.Values;
        }

        public User GetUser(string id)
        {
            if (!_userDict.ContainsKey(id))
            {
                return null;
            }

            return _userDict[id];
        }

        public string CreateUser(UserPost user)
        {
            var newUser = new User();

            // generate user id
            string id = Guid.NewGuid().ToString();

            newUser.Id = id;
            newUser.Name = user.Name;
            newUser.Points = user.Points;

            if (!_userDict.TryAdd(id, newUser))
            {
                throw new Exception("Could not add user.");
            }

            return id;
        }

        public bool UpdateUser(string id, UserPut user)
        {
            if (!_userDict.ContainsKey(id))
            {
                return false;
            }

            _userDict[id].Points = user.Points;

            return true;
        }
    }
}