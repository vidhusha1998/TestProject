using RestSharp;
using Xunit;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace APITests
{
    public class RestfulApiTests
    {
        private readonly string baseUrl = "https://api.restful-api.dev/";

        // A helper method that centralizes the request execution and validation logic
        public async Task<RestResponse> ExecuteRequestAndCheckConnection(RestRequest request, HttpStatusCode[] expectedStatusCodes)
        {
            var client = new RestClient(baseUrl);
            var response = await client.ExecuteAsync(request);

            // Check if the request completed successfully
            Assert.True(response.ResponseStatus == ResponseStatus.Completed,
                        $"Request failed. ErrorMessage: {response.ErrorMessage}, Exception: {response.ErrorException}");

            // Check if the status code is within the expected range
            Assert.Contains(response.StatusCode, expectedStatusCodes);

            // Ensure the response has content (except for 204 NoContent)
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Assert.NotNull(response.Content);
            }

            return response;
        }

        public async Task<string> Create_Object()
        {
            var request = new RestRequest("objects", Method.Post);
            var newObject = new
            {
                name = "Test Object",
                data = new { info = "Sample Data" }
            };

            request.AddJsonBody(newObject);
            var response = await ExecuteRequestAndCheckConnection(request, new[] { HttpStatusCode.Created, HttpStatusCode.OK });

            // Parse the response to get the object ID
            JObject responseObject = JObject.Parse(response.Content);
            return responseObject["id"].ToString();
        }

        [Fact]
        public async Task Get_All_Objects_Should_Return_OK_Status()
        {
            var request = new RestRequest("objects", Method.Get);
            await ExecuteRequestAndCheckConnection(request, new[] { HttpStatusCode.OK });
        }

        [Fact]
        public async Task Add_Object_Should_Return_Created_Or_OK_Status()
        {
            string objectId = await Create_Object();
            Assert.NotNull(objectId); // Ensure the object was created and has a valid ID
        }

        [Fact]
        public async Task Get_Object_By_Id_Should_Return_Correct_Object()
        {
            // Create an object first
            string objectId = await Create_Object();

            // Fetch the newly created object using the ID
            var request = new RestRequest($"objects/{objectId}", Method.Get);
            var response = await ExecuteRequestAndCheckConnection(request, new[] { HttpStatusCode.OK });

            JObject getObject = JObject.Parse(response.Content);
            Assert.Equal(objectId, getObject["id"].ToString());
        }

        [Fact]
        public async Task Update_Object_Should_Return_OK_Status()
        {
            // Create an object first
            string objectId = await Create_Object();

            // Update the object
            var request = new RestRequest($"objects/{objectId}", Method.Put);
            var updatedObject = new
            {
                name = "Updated Object",
                data = new { info = "Updated Data" }
            };

            request.AddJsonBody(updatedObject);
            await ExecuteRequestAndCheckConnection(request, new[] { HttpStatusCode.OK });

            // Fetch the updated object to verify the changes
            var getRequest = new RestRequest($"objects/{objectId}", Method.Get);
            var getResponse = await ExecuteRequestAndCheckConnection(getRequest, new[] { HttpStatusCode.OK });

            JObject getObject = JObject.Parse(getResponse.Content);
            Assert.Equal("Updated Object", getObject["name"].ToString());
        }

        [Fact]
        public async Task Delete_Object_Should_Return_NoContent_Or_OK_Status()
        {
            // First, create an object
            string objectId = await Create_Object();

            // Delete the object
            var request = new RestRequest($"objects/{objectId}", Method.Delete);
            await ExecuteRequestAndCheckConnection(request, new[] { HttpStatusCode.NoContent, HttpStatusCode.OK });

            // Verify that the object no longer exists
            var getRequest = new RestRequest($"objects/{objectId}", Method.Get);
            var getResponse = await ExecuteRequestAndCheckConnection(getRequest, new[] { HttpStatusCode.NotFound });
        }
    }
}

