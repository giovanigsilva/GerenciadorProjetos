public static class MensagensApiResources
{
    public static string GetMensagem(this MensagemApi mensagemEnum)
    {
        return mensagemEnum switch
        {
            MensagemApi.DadosInvalidos            => "Dados de entrada inválidos.",
            MensagemApi.ErroInterno               => "Erro interno ao processar a requisição.",
            MensagemApi.AcessoNaoAutorizado       => "Acesso não autorizado.",
            MensagemApi.LoginSucesso              => "Login realizado com sucesso.",
            MensagemApi.LoginFalha                => "Email ou senha incorretos.",
            MensagemApi.ComentarioViaSpSucesso    => "Comentário adicionado com sucesso via stored procedure.",
            MensagemApi.ComentarioDiretoSucesso   => "Comentário adicionado diretamente no banco.",
            MensagemApi.CriacaoSucesso            => "Registro criado com sucesso.",
            MensagemApi.AtualizacaoSucesso        => "Registro atualizado com sucesso.",
            MensagemApi.RemocaoSucesso            => "Registro removido com sucesso.",
            MensagemApi.ListaSucesso              => "Lista obtida com sucesso.",
            MensagemApi.RegistroNaoEncontrado     => "Registro não encontrado.",
            _                                     => "Erro desconhecido."
        };
    }
}
