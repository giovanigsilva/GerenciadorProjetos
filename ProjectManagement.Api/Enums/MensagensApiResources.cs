public static class MensagensApiResources
{
    public static string GetMensagem(this MensagemApi mensagemEnum)
    {
        return mensagemEnum switch
        {
            MensagemApi.DadosInvalidos => "Dados de entrada inválidos.",
            MensagemApi.LoginSucesso => "Login realizado com sucesso.",
            MensagemApi.LoginFalha => "Email ou senha incorretos.",
            MensagemApi.ErroInterno => "Erro interno ao processar a requisição.",
            MensagemApi.AcessoNaoAutorizado => "Acesso não autorizado.",
            MensagemApi.ComentarioViaSpSucesso => "Comentário adicionado com sucesso via stored procedure.",
            MensagemApi.ComentarioDiretoSucesso => "Comentário adicionado diretamente no banco.",
            _ => "Erro desconhecido."
        };
    }
}
