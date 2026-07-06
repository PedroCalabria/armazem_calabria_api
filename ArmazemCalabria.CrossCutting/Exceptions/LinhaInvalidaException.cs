namespace ArmazemCalabria.CrossCutting.Exceptions
{
    /// <summary>
    /// Sinaliza que uma linha da planilha é inválida. É capturada pelo processador,
    /// que registra o erro e segue para a próxima linha (processamento parcial).
    /// </summary>
    public class LinhaInvalidaException(string mensagem, string? coluna = null) : Exception(mensagem)
    {
        public string? Coluna { get; } = coluna;
    }
}
