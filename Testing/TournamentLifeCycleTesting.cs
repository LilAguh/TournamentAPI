

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Win32;
using Models.DTOs.CardDecks;
using Models.DTOs.Decks;
using Models.DTOs.Matches;
using Models.DTOs.Tournament;
using Models.DTOs.User;
using Models.Entities;
using Models.Enums;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Testing
{
    public class TournamentLifeCycleTesting : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public TournamentLifeCycleTesting(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task TournamentLifeCycle()
        {
            // Login de un admin
            var loginAdmin = new
            {
                alias = "AdminTesting",
                password = "AdminTesting123"
            };

            var loginResponse = await _client.PostAsJsonAsync("Auth/login", loginAdmin);
            loginResponse.EnsureSuccessStatusCode();

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(loginResult);
            Assert.NotNull(loginResult.Token);
            Assert.Equal(loginAdmin.alias, loginResult.User.Alias);

            var adminToken = loginResult.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            // Creación de un torneo
            var newTournament = new
            {
                name = "Torneo Testing",
                startDate = DateTime.UtcNow.AddDays(1).Date.Add(new TimeSpan(10, 0, 0)), // Fecha de inicio: día siguiente a las 10:00 AM
                endDate = DateTime.UtcNow.AddDays(2).Date.Add(new TimeSpan(18, 0, 0)),   // Fecha de finalización: dos días después a las 6:00 PM
                countryCode = "AR"
            };

            var createTournamentResponse = await _client.PostAsJsonAsync("/api/tournaments", newTournament);
            createTournamentResponse.EnsureSuccessStatusCode();
            var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponseDto>();

            Assert.NotNull(createdTournament);

            var tournamentId = createdTournament.Id;

            // Agregar una serie al torneo (serie con id 6)
            int seriesId = 6;
            var addSeriesResponse = await _client.PostAsync($"/api/tournaments/{tournamentId}/series/{seriesId}", null);
            addSeriesResponse.EnsureSuccessStatusCode();

            // Creación de jugadores, agregar mazos e inscribirse al torneo

            var totalPlayers = createdTournament.MaxPlayers;

            var players = new List<(string alias, int userId, int deckId)>();

            for (int i = 1; i <= totalPlayers; i++)
            {
                var newUser = new
                {
                    firstName = $"Player{i}{tournamentId}",
                    lastName = "Testing",
                    alias = $"PlayerTesting{i}{tournamentId}",
                    password = "@TestPassword123",
                    email = $"PlayerTesting{i}{tournamentId}@gmail.com",
                    countryCode = "AR",
                    avatarUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_640.png"
                };

                var createUserResponse = await _client.PostAsJsonAsync("User/Register", newUser);
                createUserResponse.EnsureSuccessStatusCode();
                var createdUser = await createUserResponse.Content.ReadFromJsonAsync<UserResponseDto>();

                Assert.NotNull(createdUser);
                Assert.Equal(newUser.email, createdUser.Email);
                Assert.Equal(newUser.alias, createdUser.Alias);

                // Logueo del usuario recién creado
                var loginNewUser = new
                {
                    alias = newUser.alias,
                    password = newUser.password,
                };

                var loginNewUserResponse = await _client.PostAsJsonAsync("Auth/login", loginNewUser);
                loginNewUserResponse.EnsureSuccessStatusCode();

                var loginNewUserResult = await loginNewUserResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

                Assert.NotNull(loginNewUserResult);
                Assert.NotNull(loginNewUserResult.Token);
                Assert.Equal(loginNewUser.alias, loginNewUserResult.User.Alias);

                // El token para crear deck, agregar cartas y registrarse al torneo 
                var tokenNewUser = loginNewUserResult.Token;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenNewUser);
                var idNewUserPrincipal = loginNewUserResult.User.Id;

                // Crear el deck para el jugador
                var newDeck = new
                {
                    name = $"Deck Test {i}"
                };

                var createDeckResponse = await _client.PostAsJsonAsync("/api/decks", newDeck);
                createDeckResponse.EnsureSuccessStatusCode();
                var createdDeckResult = await createDeckResponse.Content.ReadFromJsonAsync<DeckResponseDto>();

                Assert.NotNull(createdDeckResult);
                Assert.NotNull(createdDeckResult.Id);
                Assert.Equal(newDeck.name, createdDeckResult.Name);

                var idDeckUser = createdDeckResult.Id;

                // Agregar cartas al Deck
                for (int c = 68; c <= 82; c++)
                {
                    var addCardDeck = new
                    {
                        cardId = c
                    };

                    var addCardDeckResponse = await _client.PostAsJsonAsync($"/api/decks/{idDeckUser}/cards", addCardDeck);
                    addCardDeckResponse.EnsureSuccessStatusCode();
                    var addCardDeckResult = await addCardDeckResponse.Content.ReadAsStringAsync();

                    Assert.NotNull(addCardDeckResult);
                }

                // Agregar jugador al torneo
                var addUserTournament = new
                {
                    tournamentID = tournamentId,
                    deckId = idDeckUser
                };

                var addUserTournamentResponse = await _client.PostAsJsonAsync($"/api/tournaments/{tournamentId}/register", addUserTournament);
                addUserTournamentResponse.EnsureSuccessStatusCode();
                var addUserTournamentResult = await addUserTournamentResponse.Content.ReadAsStringAsync();
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

            // Ejecución de otros endpoints o validaciones adicionales según sea necesario
        }


        [Fact]
        public async Task GenerateWinnersForLastConfirmedRound()
        {
            // ID del torneo a utilizar
            int tournamentId = 34;

            // Login como admin para tener permisos
            var loginAdmin = new
            {
                alias = "AdminTesting",
                password = "AdminTesting123"
            };

            var loginResponse = await _client.PostAsJsonAsync("Auth/login", loginAdmin);
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResult);
            var adminToken = loginResult.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            // Obtener todos los partidos del torneo
            var matchesResponse = await _client.GetAsync($"/api/tournaments/{tournamentId}/matches");
            matchesResponse.EnsureSuccessStatusCode();
            var matches = await matchesResponse.Content.ReadFromJsonAsync<List<MatchResponseDto>>();
            Assert.NotNull(matches);

            // Filtrar los partidos que estén en estado Confirmed (MatchEnum.Confirmed)
            var confirmedMatches = matches.Where(m => m.Status == MatchEnum.Confirmed).ToList();
            Assert.NotEmpty(confirmedMatches);

            // Seleccionar el último partido confirmado para obtener el número de round
            var lastConfirmedMatch = confirmedMatches.Last();
            int roundNumber = lastConfirmedMatch.Round;

            // Filtrar todos los partidos que pertenezcan al mismo round
            var roundMatches = matches.Where(m => m.Round == roundNumber).ToList();
            Assert.NotEmpty(roundMatches);

            // Asignar de forma aleatoria el ganador para cada partido del round
            var random = new Random();
            foreach (var match in roundMatches)
            {
                int winnerId = random.Next(2) == 0 ? match.Player1ID : match.Player2ID;

                // Payload para asignar el resultado del partido
                var resultPayload = new
                {
                    matchId = match.Id,
                    winnerId = winnerId
                };

                // Llamada al endpoint para asignar el resultado del partido
                var resultResponse = await _client.PostAsJsonAsync("/api/matches/result", resultPayload);
                resultResponse.EnsureSuccessStatusCode();

                var resultContent = await resultResponse.Content.ReadAsStringAsync();
                _output.WriteLine($"Match {match.Id} (Round {match.Round}): Ganador asignado -> {winnerId}");
            }
        }
    }
}