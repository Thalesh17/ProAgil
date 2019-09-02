using System;
using System.ComponentModel.DataAnnotations;

namespace ProAgil.API.Dtos
{
    public class LoteDTO
    {
        public int Id  {get; set;}
        [Required]
        public string Nome { get; set; }
        [Required]
        public decimal Preco { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        
        [Range(5, 120000)]
        public int Quantidade { get; set; }
    }
}