using System.Globalization;
using System.Text;

namespace ArmazemCalabria.Utils.Helpers
{
    public static class TextoHelper
    {
        /// <summary>
        /// Normaliza um texto para comparação: remove acentos, espaços nas pontas e converte para minúsculas.
        /// Usado para casar descrições de domínio da planilha com as tabelas do banco de forma tolerante.
        /// </summary>
        public static string Normalizar(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            var decomposto = valor.Trim().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(decomposto.Length);

            foreach (var caractere in decomposto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
                    builder.Append(caractere);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }
    }
}
