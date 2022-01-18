using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Models
{
    [Table("SituacaoCashBack")]
    public class SituacaoCashBackModel
    {
        [Key]
        public int IdSituacaoCashBack { get; set; }

        [Required, MaxLength(32)]
        public string Nome { get; set; }

        public static void Seed(ModelBuilder pModelBuilder)
        {
            pModelBuilder.Entity<SituacaoCashBackModel>().HasData(
                new SituacaoCashBackModel { IdSituacaoCashBack = 1, Nome = "Disponível" }, 
                new SituacaoCashBackModel { IdSituacaoCashBack = 2, Nome = "Resgatado" }
            );
        }
    }
}