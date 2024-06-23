namespace API.Models
{
    public class Usuarios
    {
        public int id { get; set; }
        public string primeiro_nome { get; set; }
        public string ultimo_nome { get; set; }
        public string email { get; set; }
        public string pais { get; set; }
        public string telefone { get; set; }
        public string data_nascimento { get; set; }
        public string foto_url { get; set; }

        public Usuarios()
        {
            primeiro_nome = string.Empty;
            ultimo_nome = string.Empty;
            email = string.Empty;
            pais = string.Empty;
            telefone = string.Empty;
            data_nascimento = string.Empty;
            foto_url = string.Empty;
        }
    }
}
