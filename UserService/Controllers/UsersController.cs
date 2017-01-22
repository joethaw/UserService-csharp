using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Swashbuckle.Swagger.Annotations;
using UserService.Models;

namespace UserService.Controllers
{
    // keep the controller as simple as possible
    // do all heavy lifting in service
    public class UsersController : ApiController
    {
        private UsersService _service;
        private const string INTERNAL_ERROR_MESSAGE = "Internal Server Error. Please contact support.";

        public UsersController()
        {
            IUsersRepository repo = new UsersRepository();
            _service = new UsersService(repo);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <remarks>
        /// Returns all users
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Successfully get all users</response>
        // GET api/users
        [ResponseType(typeof(IEnumerable<User>))]
        public IEnumerable<User> Get()
        {
            try
            {
                return _service.GetUsers();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ReturnError(HttpStatusCode.InternalServerError, INTERNAL_ERROR_MESSAGE);
                return null;
            }
        }

        /// <summary>
        /// Get a user by Id
        /// </summary>
        /// <remarks>
        /// Returns the user with that Id
        /// </remarks>
        /// <param name="id">Id of the user</param>
        /// <returns></returns>
        /// <response code="200">Successfully get the user with that Id</response>
        /// <response code="404">User not found</response>
        // GET api/users/{id
        [ResponseType(typeof(User))]
        public User Get(string id)
        {
            try
            {
                return _service.GetUser(id);
            }
            catch (NotFoundException nfex)
            {
                ReturnError(HttpStatusCode.NotFound, nfex.Message);
            }
            catch (Exception ex)
            {
                LogError(ex);
                ReturnError(HttpStatusCode.InternalServerError, INTERNAL_ERROR_MESSAGE);
            }

            return null;
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <remarks>
        /// Returns Id and url of the created user
        /// </remarks>
        /// <param name="user">User to be created</param>
        /// <returns></returns>
        /// <response code="201">Successfully created the user</response>
        /// <response code="400">Bad request</response>
        // POST api/users
        [ResponseType(typeof(PostResponse))]
        [ValidateModel]
        public CreatedNegotiatedContentResult<PostResponse> Post([FromBody]UserPost user)
        {
            try
            {
                string id = _service.CreateUser(user);
                var postResponse = CreatePostResponse(id);

                // This will return 201 - Created http code, which is what a successful POST should return per REST standard
                // it will also add Location header, which again is a best practice for POST response
                return Created(new Uri(postResponse.Uri), postResponse);
            }
            catch (BadRequestException brex)
            {
                ReturnError(HttpStatusCode.BadRequest, brex.Message);
            }
            catch (Exception ex)
            {
                LogError(ex);
                ReturnError(HttpStatusCode.InternalServerError, INTERNAL_ERROR_MESSAGE);
            }

            return null;
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <remarks>
        /// Update a user
        /// </remarks>
        /// <param name="id">Id of the user</param>
        /// <param name="user">User</param>
        /// <response code="204">Successfully updated the user</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">User not found</response>
        // PUT api/users/{id}
        [ValidateModel]
        public void Put(string id, [FromBody]UserPut user)
        {
            try
            {
                _service.UpdateUser(id, user);
            }
            catch (NotFoundException nfex)
            {
                ReturnError(HttpStatusCode.NotFound, nfex.Message);
            }
            catch (BadRequestException brex)
            {
                ReturnError(HttpStatusCode.BadRequest, brex.Message);
            }
            catch (Exception ex)
            {
                LogError(ex);
                ReturnError(HttpStatusCode.InternalServerError, INTERNAL_ERROR_MESSAGE);
            }

        }

        // DELETE api/users/{id}
        //public void Delete(string id)
        //{
        //}

        #region private methods
        private PostResponse CreatePostResponse(string id)
        {
            return new PostResponse()
            {
                Id = id,
                Uri = Request.RequestUri.AbsoluteUri + "/" + id
            };
        }

        private void ReturnError(HttpStatusCode httpCode, string errorMessageForUser)
        {
            if (Request != null)
                throw new HttpResponseException(Request.CreateResponse(httpCode, new ErrorResponse { Message = errorMessageForUser }));

            // Only valid in test scenario.
            throw new HttpResponseException(new HttpResponseMessage(httpCode) { ReasonPhrase = errorMessageForUser });
        }

        private void LogError(Exception ex)
        {
            // do logging here
            Trace.WriteLine(ex);
        }
        #endregion
    }
}
