using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace FuelManagerGraphQL.Models
{
    public class Mutation
    {
        [Authorize]
        public async Task<Veiculo> AddVeiculo([Service] AppDbContext _context, Veiculo model)
        {
            _context.Veiculos.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<Veiculo> UpdateVeiculo([Service] AppDbContext _context, Veiculo model)
        {
            _context.Veiculos.Update(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<Veiculo> DeleteVeiculo([Service] AppDbContext _context, int id)
        {
            var model = await _context.Veiculos.FindAsync(id);
            _context.Veiculos.Remove(model);
            await _context.SaveChangesAsync();

            return model;
        }

        [Authorize]
        public async Task<Consumo> AddConsumo([Service] AppDbContext _context, Consumo model)
        {
            _context.Consumos.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<Consumo> UpdateConsumo([Service] AppDbContext _context, Consumo model)
        {
            _context.Consumos.Update(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<Consumo> DeleteConsumo([Service] AppDbContext _context, int id)
        {
            var model = await _context.Consumos.FindAsync(id);
            _context.Consumos.Remove(model);
            await _context.SaveChangesAsync();

            return model;
        }


        public async Task<Usuario> AddUsuario([Service] AppDbContext _context, Usuario model)
        {
            model.Password = BC.HashPassword(model.Password);
            _context.Usuarios.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<Usuario> UpdateUsuario([Service] AppDbContext _context, Usuario model)
        {
            _context.Usuarios.Update(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<Usuario> DeleteUsuario([Service] AppDbContext _context, int id)
        {
            var model = await _context.Usuarios.FindAsync(id);
            _context.Usuarios.Remove(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<UserDTO> Authenticate([Service] AppDbContext _context, UserDTO model)
        {
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(x => x.Email == model.Email);

            if (usuario == null || (!BC.Verify(model.Password, usuario.Password)))
                model.Jwt =  "Email e/ou senha Inválidos!";

            model.Jwt = GenerateJwtToken(usuario);

            return model;
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("SECRETO@COMPARTILHADO");
            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                   SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
