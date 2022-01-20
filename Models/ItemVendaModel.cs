using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbevWeb.Models
{
    [Table("ItemVenda")]
    public class ItemVendaModel
    {
        [Key]
        public int IdItemVenda { get; set; }
        public int IdVenda { get; set; }
        public int IdCerveja { get; set; }
        public int IdCashBack { get; set; }
        public int IdSituacaoCashBack { get; set; }
        public int Quantidade { get; set; }
        public double ValorUnitario { get; set; }
        public double FracaoCachBack { get; set; }

        [ForeignKey("IdVenda")]
        public VendaModel Venda { get; set; }

        [ForeignKey("IdCerveja")]
        public CervejaModel Cerveja { get; set; }

        [NotMapped]
        public double ValorItem { get => this.Quantidade * this.ValorUnitario; }

        [NotMapped]
        public double ValorTotalCachBack { get => ValorItem * FracaoCachBack; }
    }
}