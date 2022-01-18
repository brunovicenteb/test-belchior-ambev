using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Models
{
    [Table("Cliente")]
    public class ClienteModel : UsuarioModel
    {
        [Required, Column(TypeName = "char(14)")]
        public string CPF { get; set; }

        public DateTime DataNascimento { get; set; }

        [NotMapped]
        public int Idade
        {
            get => (int)Math.Floor((DateTime.Now - DataNascimento).TotalDays / 365.2425);
        }

        public ICollection<VendaModel> Pedidos { get; set; }

        public static void Seed(ModelBuilder pModelBuilder)
        {
            pModelBuilder.Entity<ClienteModel>().HasData(
                new ClienteModel { IdUsuario = 1, Nome = "Natalie Portman", Email = "natalie.portman@ambev.com.br", Senha = "123456", CPF = "791.647.240-69", DataNascimento = new DateTime(1981, 6, 9) },
                new ClienteModel { IdUsuario = 2, Nome = "Penelope Cruz", Email = "penelope.cruz@ambev.com.br", Senha = "123456", CPF = "409.579.270-10", DataNascimento = new DateTime(1981, 6, 9) },
                new ClienteModel { IdUsuario = 3, Nome = "Sharon Stone", Email = "sharon.stone@ambev.com.br", Senha = "123456", CPF = "616.150.370-04", DataNascimento = new DateTime(1958, 3, 10) },
                new ClienteModel { IdUsuario = 4, Nome = "Jennifer Lawrence", Email = "jennifer.lawrence@ambev.com.br", Senha = "123456", CPF = "", DataNascimento = new DateTime(1990, 8, 15) },
                new ClienteModel { IdUsuario = 5, Nome = "Jennifer Connelly", Email = "jennifer.connelly@ambev.com.br", Senha = "123456", CPF = "", DataNascimento = new DateTime(1970, 12, 12) },
                new ClienteModel { IdUsuario = 6, Nome = "Liv Tyler", Email = "liv.tyler@ambev.com.br", Senha = "123456", CPF = "", DataNascimento = new DateTime(1977, 6, 1) },
                new ClienteModel { IdUsuario = 8, Nome = "Jennifer Lopez", Email = "jennifer.lopez@ambev.com.br", Senha = "123456", CPF = "", DataNascimento = new DateTime(1969, 6, 24) },
                new ClienteModel { IdUsuario = 9,Nome = "Gal Gadot", Email = "gal.gadot@ambev.com.br", Senha = "123456", CPF = "", DataNascimento = new DateTime(1985, 4, 30) },
                new ClienteModel { IdUsuario = 10, Nome = "Odette Annable", Email = "odette.annable.com.br", Senha = "123456", CPF = "", DataNascimento = new DateTime(1985, 5, 10) }
            );
        }
    }
}