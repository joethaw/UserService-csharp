using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using UserService.Models;

namespace UserService.Controllers
{
    // The service class is where we do the business logic
    // as well as accessing other external services
    public class UsersService
    {
        private readonly IUsersRepository _repo;

        // make the constants public so that they can be referred to in unit tests
        public const string USER_NOT_FOUND = "User not found";
        public const string BAD_USER = "Bad user data";

        public UsersService(IUsersRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<User> GetUsers()
        {
            return _repo.GetUsers();
        }

        public User GetUser(string id)
        {
            User user = _repo.GetUser(id);
            if (user == null)
            {
                throw new NotFoundException(USER_NOT_FOUND);
            }

            return user;
        }

        public string CreateUser(UserPost user)
        {
            if (user == null)
            {
                throw new BadRequestException(BAD_USER);
            }

            return _repo.CreateUser(user);
        }

        public void UpdateUser(string id, UserPut user)
        {
            if (user == null)
            {
                throw new BadRequestException(BAD_USER);
            }

            if (_repo.UpdateUser(id, user) == false)
            {
                throw new NotFoundException(USER_NOT_FOUND);
            }
        }
    }
}