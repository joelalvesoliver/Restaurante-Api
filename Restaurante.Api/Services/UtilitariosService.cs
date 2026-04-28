using Restaurante.Api.DTOs;

namespace Restaurante.Api.Services
{
    /* 
    * Serviço responsável por gerenciar metodos Úteis ao longo do sistema].
    */
    public class UtilitariosService
    {
        public static string GetMimeType(string nomeArquivo)
        {
            var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
            return extensao switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

    }
}
