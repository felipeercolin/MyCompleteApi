namespace DevIO.Api.Extentions
{
    public class AppSettings
    {
        /// <summary>
        /// Chave do Token
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// Quantidade de Horas que o Token irá Expirar
        /// </summary>
        public int ExiperacaoHoras { get; set; }
        /// <summary>
        /// Quem está Emitindo, no caso, a aplicação...
        /// </summary>
        public string Emissor { get; set; }
        /// <summary>
        /// Quais Urls o Token é Valido.
        /// </summary>
        public string ValidoEm { get; set; }
    }
}
