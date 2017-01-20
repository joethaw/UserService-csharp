using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
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

        // GET api/users
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

        // GET api/users/{id}
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

        // POST api/users
        [ValidateModel]
        public CreatedNegotiatedContentResult<PostResponse> Post([FromBody]User user)
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
