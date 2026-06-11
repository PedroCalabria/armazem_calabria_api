namespace ArmazemCalabria.Utils.DTO
{
    public class ChaveDescricaoDTO<T>
    {
        public ChaveDescricaoDTO()
        {
        }

        public ChaveDescricaoDTO(T chave, string descricao)
        {
            Chave = chave;
            Descricao = descricao;
        }

        public T Chave { get; set; }
        public string Descricao { get; set; }
    }
}
