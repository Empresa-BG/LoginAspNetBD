using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using MySql.Data.MySqlClient;
using ProjetoLogin.Models;
using System.Data;
using System.Runtime.CompilerServices;


namespace ProjetoLogin.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string _connectionString;

        private readonly PasswordHasher<UsuarioModel> _passwordHasher;

        public UsuarioRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Connection") ?? string.Empty;
            _passwordHasher = new PasswordHasher<UsuarioModel>();
        }
        public async Task<bool>Cadastrar(UsuarioModel usuario, string senha)
        {
            try
            {
                usuario.SenhaHash = _passwordHasher.HashPassword(usuario, senha);
                using var conexao = new MySqlConnection(_connectionString);
                await conexao.OpenAsync();
                var query = "INSERT INTO usuarios (Nome,Email,SenhaHash) VALUES(@nome,@email,@senhahash)";
                using var cmd = new MySqlCommand(query, conexao);

                cmd.Parameters.AddWithValue("@nome", usuario.Nome);
                cmd.Parameters.AddWithValue("@email", usuario.Email);
                cmd.Parameters.AddWithValue("@senhahash", usuario.SenhaHash);

                var linhasAdicionadas = await cmd.ExecuteNonQueryAsync();
                return linhasAdicionadas > 0;

            }
            catch ( Exception ex)
            {
                throw new Exception($"Erro{ex.Message}");
            }
        }
        public async Task<UsuarioModel> BuscarEmail(string email)
        {
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            var query = "SELECT Id, Nome, Email, SenhaHash FROM usuarios WHERE Email = @email";
            using var cmd = new MySqlCommand(query, conexao);

            cmd.Parameters.AddWithValue("@email" ,email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UsuarioModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Nome = reader.GetString("Nome"),
                    Email = reader.GetString("Email"),
                    SenhaHash = reader.GetString("SenhaHash")
                };
            }
            return null;
    }
  }  
    
}
