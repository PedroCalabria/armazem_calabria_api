namespace ArmazemCalabria.CrossCutting.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public BusinessException(string mensagem): base(mensagem) { }
        public BusinessException(string mensagem, Exception innerException): base(mensagem, innerException) { }
    }
}
