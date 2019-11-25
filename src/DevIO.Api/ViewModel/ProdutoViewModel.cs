using System;
using System.ComponentModel.DataAnnotations;

namespace DevIO.Api.ViewModel
{
    public class ProdutoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigatório")]
        public Guid FornecedorId { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigatório")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigatório")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Descricao { get; set; }

        /// <summary>
        /// Representa os dados da Imagem a ser Feito Upload.
        /// Serealizada em BASE64.
        /// </summary>
        public string ImagemUpload { get; set; }

        /// <summary>
        /// Represanta a Imagem que vem do Banco de Dados.
        /// Nome da Imagem
        /// </summary>
        public string Imagem { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigatório")]
        public decimal Valor { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; }

        [ScaffoldColumn(false)]
        public string NomeFornecedor { get; set; }
    }
}