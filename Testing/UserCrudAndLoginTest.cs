using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Models.DTOs.User;
using Xunit;
using System.Net.Http.Headers;
using System.Net;
using Xunit.Abstractions;

namespace Testing
{
    public class UserCrudAndLoginTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public UserCrudAndLoginTest(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task CreateNewUserAndLogin()
        {

            var newUser = new
            {
                firstName = "User",
                lastName = "Testing",
                alias = "UserTesting",
                password = "@TestPassword123",
                email = "UserTesting@gmail.com",
                countryCode = "AR",
                avatarUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_640.png"
            };

            var response = await _client.PostAsJsonAsync("User/Register", newUser);
            response.EnsureSuccessStatusCode();
            var createdUser = await response.Content.ReadFromJsonAsync<UserResponseDto>();


            Assert.NotNull(createdUser);
            Assert.Equal(newUser.email, createdUser.Email);
            Assert.Equal(newUser.alias, createdUser.Alias);


            var loginUser = new
            {
                alias = "UserTesting",
                password = "@TestPassword123",
            };

            var loginResponse = await _client.PostAsJsonAsync("Auth/login", loginUser);
            loginResponse.EnsureSuccessStatusCode();

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();


            Assert.NotNull(loginResult);
            Assert.NotNull(loginResult.Token);
            Assert.Equal(loginUser.alias, loginResult.User.Alias);

            //El token para el resto de los 
            var token = loginResult.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //El ID del primer usuario creado
            var idPrincipal = loginResult.User.Id;



            //Actualizar usuario
            var updateRequest = new
            {
                firstName = "UserUpdate",
                lastName = "TestingUpdate",
                alias = "UserTestingUpdate",
                email = "UserTestingUpdate@gmail.com",
                countryCode = "US",
                avatarUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_640.png"
            };

            var updateResponse = await _client.PutAsJsonAsync($"User/{idPrincipal}", updateRequest);
            updateResponse.EnsureSuccessStatusCode();
            var updatedUser = await updateResponse.Content.ReadFromJsonAsync<UserResponseDto>();

            Assert.NotNull(updatedUser);
            Assert.Equal(updateRequest.firstName, updatedUser.FirstName);

            //Desactivar usuario

            var desactivateResponse = await _client.DeleteAsync($"User/{idPrincipal}");
            desactivateResponse.EnsureSuccessStatusCode();

            //Logueo erroneo

            var invalidLoginUser = new
            {
                alias = "UserTestingUpdate",
                password = "@TestPassword123",
            };

            var invalidLoginResponse = await _client.PostAsJsonAsync("Auth/login", invalidLoginUser);
            Assert.Equal(HttpStatusCode.Unauthorized, invalidLoginResponse.StatusCode);

            //Crear nuevo usuario con mismo mail

            var secondNewUser = new
            {
                firstName = "User",
                lastName = "Testing",
                alias = "UserTesting2",
                password = "@TestPassword123",
                email = "UserTesting@gmail.com",
                countryCode = "AR",
                avatarUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_640.png"
            };

            var secondNewUserResponse = await _client.PostAsJsonAsync("User/Register", secondNewUser);
            secondNewUserResponse.EnsureSuccessStatusCode();
            var createdSecondUser = await secondNewUserResponse.Content.ReadFromJsonAsync<UserResponseDto>();

            Assert.NotNull(createdSecondUser);
            Assert.Equal(secondNewUser.email, createdSecondUser.Email);
            Assert.Equal(secondNewUser.alias, createdSecondUser.Alias);

            //Logueo del nuevo usuario

            var loginNewUser = new
            {
                alias = "UserTesting2",
                password = "@TestPassword123",
            };

            var loginNewResponse = await _client.PostAsJsonAsync("Auth/login", loginNewUser);
            loginNewResponse.EnsureSuccessStatusCode();

            var loginNewResult = await loginNewResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginNewResult);
            Assert.NotNull(loginNewResult.Token);
            Assert.Equal(loginNewUser.alias, loginNewResult.User.Alias);

            //El token para el resto de los 
            var tokenNewUser = loginNewResult.Token;

            //El ID del primer usuario creado
            var idNewUser = loginNewResult.User.Id;

            //Eliminar los 2 usuarios creados
            var DeleteResponse = await _client.DeleteAsync($"User/DeletePermanent/{idPrincipal}");
            DeleteResponse.EnsureSuccessStatusCode();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenNewUser);

            var DeleteNewUserResponse = await _client.DeleteAsync($"User/DeletePermanent/{idNewUser}");
            DeleteNewUserResponse.EnsureSuccessStatusCode();

        }
    }
}
