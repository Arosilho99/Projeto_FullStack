using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Npgsql;

class Program
{
    static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=postgres";
    private static int limitador;

    static async Task Main(string[] args)
    {
        Console.Write("Digite o número de usuários que deseja gerar e salvar no banco de dados: ");
        if (!int.TryParse(Console.ReadLine(), out limitador) || limitador <= 0)
        {
            Console.WriteLine("Valor inválido para o limitador.");
            return;
        }

        int usuariosSalvos = 0; 

        using (HttpClient client = new HttpClient())
        {
            try
            {
                for (int i = 0; usuariosSalvos < limitador; i++)
                {
                    HttpResponseMessage response = await client.GetAsync("https://randomuser.me/api/");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JObject data = JObject.Parse(json);

                        string primeiroNome = (string)data["results"][0]["name"]["first"];
                        string ultimoNome = (string)data["results"][0]["name"]["last"];


                        if (ContemCaracteresNaoLatinos(primeiroNome) || ContemCaracteresNaoLatinos(ultimoNome))
                        {
                            Console.WriteLine($"Ignorando usuário {i + 1} devido a caracteres não latinos nos nomes.");
                            continue;  
                        }

                        string email = (string)data["results"][0]["email"];
                        string pais = (string)data["results"][0]["location"]["country"];
                        string telefone = (string)data["results"][0]["phone"];
                        string data_nascimento = ((DateTime)data["results"][0]["dob"]["date"]).ToString("yyyy-MM-dd");
                        string fotoUrl = (string)data["results"][0]["picture"]["large"];

                        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();


                            string selectQuery = "SELECT COUNT(*) FROM Usuarios WHERE primeiro_nome = @primeiroNome AND ultimo_nome = @ultimoNome";
                            using (NpgsqlCommand selectCommand = new NpgsqlCommand(selectQuery, connection))
                            {
                                selectCommand.Parameters.AddWithValue("@primeiroNome", primeiroNome);
                                selectCommand.Parameters.AddWithValue("@ultimoNome", ultimoNome);

                                int count = Convert.ToInt32(selectCommand.ExecuteScalar());

                                if (count > 0)
                                {

                                    string updateQuery = "UPDATE Usuarios SET email = @email, pais = @pais, telefone = @telefone, data_nascimento = @data_nascimento";
                                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@primeiroNome", primeiroNome);
                                        updateCommand.Parameters.AddWithValue("@ultimoNome", ultimoNome);
                                        updateCommand.Parameters.AddWithValue("@email", email);
                                        updateCommand.Parameters.AddWithValue("@pais", pais);
                                        updateCommand.Parameters.AddWithValue("@telefone", telefone);
                                        updateCommand.Parameters.AddWithValue("@data_nascimento", data_nascimento);

                                        updateCommand.ExecuteNonQuery();

                                        Console.WriteLine($"Usuário {primeiroNome} {ultimoNome} atualizado no banco de dados.");
                                    }
                                }
                                else
                                {

                                    string insertQuery = "INSERT INTO Usuarios (primeiro_nome, ultimo_nome, email, pais, telefone, data_nascimento, foto_url) VALUES (@primeiroNome, @ultimoNome, @email, @pais, @telefone, @data_nascimento, @fotoUrl)";
                                    using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, connection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@primeiroNome", primeiroNome);
                                        insertCommand.Parameters.AddWithValue("@ultimoNome", ultimoNome);
                                        insertCommand.Parameters.AddWithValue("@email", email);
                                        insertCommand.Parameters.AddWithValue("@pais", pais);
                                        insertCommand.Parameters.AddWithValue("@telefone", telefone);
                                        insertCommand.Parameters.AddWithValue("@data_nascimento", data_nascimento);
                                        insertCommand.Parameters.AddWithValue("@fotoUrl", fotoUrl);

                                        insertCommand.ExecuteNonQuery();

                                        Console.WriteLine($"Usuário {primeiroNome} {ultimoNome} inserido no banco de dados.");
                                        usuariosSalvos++;  
                                    }
                                }
                            }
                        }
                        Console.WriteLine($"Usuário {i + 1} salvo no banco de dados.");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao fazer solicitação: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        Console.WriteLine($"Total de {usuariosSalvos} usuários salvos no banco de dados.");
    }

    static bool ContemCaracteresNaoLatinos(string texto)
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
        return false;  
    }
}
