using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomUserController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RandomUserController> _logger;
        private readonly string _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=postgres";

        public RandomUserController(IHttpClientFactory httpClientFactory, ILogger<RandomUserController> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("randomuser")]
        public async Task<IActionResult> GetRandomUser()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://randomuser.me/api/");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var randomUserApiResponse = JsonSerializer.Deserialize<RandomUserApiResponse>(jsonString, options);

                    if (randomUserApiResponse?.Results?.Length > 0)
                    {
                        var firstUser = randomUserApiResponse.Results[0];

                        var firstName = firstUser.Name.First;
                        var lastName = firstUser.Name.Last;

                        // Verifica se há caracteres não latinos nos nomes
                        if (ContemCaracteresNaoLatinos(firstName) || ContemCaracteresNaoLatinos(lastName))
                        {
                            _logger.LogInformation($"Ignorando usuário devido a caracteres não latinos nos nomes: {firstName} {lastName}");
                            return BadRequest("Usuário ignorado devido a caracteres não latinos nos nomes.");
                        }

                        var country = firstUser.Location.Country;
                        var email = firstUser.Email;
                        var phone = firstUser.Phone;
                        var dateOfBirth = firstUser.DateOfBirth.ToString("yyyy-MM-dd");
                        var largePicture = firstUser.Picture.Large;

                        var responseData = new
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Country = country,
                            Email = email,
                            Phone = phone,
                            DateOfBirth = dateOfBirth,
                            LargePicture = largePicture
                        };

                        // Insere os dados no banco de dados PostgreSQL
                        await InsertRandomUserIntoDatabase(firstName, lastName, email, country, phone, dateOfBirth, largePicture);

                        return Ok(responseData);
                    }
                    else
                    {
                        return NotFound("Nenhum usuário encontrado na resposta.");
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Erro ao obter usuário aleatório");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Erro ao fazer requisição HTTP: {ex.Message}");
                return StatusCode(500, $"Erro ao fazer requisição HTTP: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Erro ao desserializar resposta JSON: {ex.Message}");
                return StatusCode(500, $"Erro ao desserializar resposta JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro inesperado: {ex.Message}");
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        private async Task InsertRandomUserIntoDatabase(string firstName, string lastName, string email, string country, string phone, string dateOfBirth, string largePicture)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"
                        INSERT INTO Usuarios (primeiro_nome, ultimo_nome, email, pais, telefone, data_nascimento, foto_url)
                        VALUES (@FirstName, @LastName, @Email, @Country, @Phone, @DateOfBirth, @LargePicture)
                    ";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Country", country);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        cmd.Parameters.AddWithValue("@LargePicture", largePicture);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao inserir usuário no banco de dados: {ex.Message}");
                throw; // Re-throw para sinalizar erro ao chamador
            }
        }

        private bool ContemCaracteresNaoLatinos(string texto)
        {
            foreach (char c in texto)
            {
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                      c == 'á' || c == 'é' || c == 'í' || c == 'ó' || c == 'ú' ||
                      c == 'à' || c == 'è' || c == 'ì' || c == 'ò' || c == 'ù' ||
                      c == 'â' || c == 'ê' || c == 'î' || c == 'ô' || c == 'û' ||
                      c == 'ä' || c == 'ë' || c == 'ï' || c == 'ö' || c == 'ü' ||
                      c == 'ã' || c == 'õ' ||
                      c == 'ç' || c == 'ñ' ||
                      c == 'Á' || c == 'É' || c == 'Í' || c == 'Ó' || c == 'Ú' ||
                      c == 'À' || c == 'È' || c == 'Ì' || c == 'Ò' || c == 'Ù' ||
                      c == 'Â' || c == 'Ê' || c == 'Î' || c == 'Ô' || c == 'Û' ||
                      c == 'Ä' || c == 'Ë' || c == 'Ï' || c == 'Ö' || c == 'Ü' ||
                      c == 'Ã' || c == 'Õ' ||
                      c == 'Ç' || c == 'Ñ' || c == ' '))
                {
                    return true;
                }
            }
            return false; // Não há caracteres não latinos
        }
    }

    public class RandomUserApiResponse
    {
        public RandomUser[] Results { get; set; }
    }

    public class RandomUser
    {
        public Name Name { get; set; }
        public Location Location { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Picture Picture { get; set; }
    }

    public class Name
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Title { get; set; }
    }

    public class Location
    {
        public string Country { get; set; }
    }

    public class Picture
    {
        public string Large { get; set; }
    }
}
