using AmbevWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace AmbevWeb.Utils
{
    public abstract class XUiController : Controller
    {
        private const string _Message = "mensagem";

        protected void RegistraSucesso(string pMensagem)
        {
            TempData[_Message] = MensagemModel.Serializar(pMensagem);
        }

        protected void RegistraFalha(string pMensagem)
        {
            TempData[_Message] = MensagemModel.Serializar(pMensagem, TipoMensagem.Erro);
        }
    }
}