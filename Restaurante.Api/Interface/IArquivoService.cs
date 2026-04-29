/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;


using Restaurante.Api.DTOs;



// Interface do Requisito 4 - Upload de Arquivo

namespace Restaurante.Api.Interface
{
    public interface IArquivoService
    {

        //Task<string> UploadArquivoAsync(IFormFile arquivo);
        Task<(bool sucesso, ArquivoResponseDTO resultado)> UploadArquivoAsync(IFormFile arquivo);
    }
}

*/

using Microsoft.AspNetCore.Http;
using Restaurante.Api.DTOs;

namespace Restaurante.Api.Interface
{
    public interface IArquivoService
    {
        // O "contrato" precisa listar tudo o que o Controller vai usar
        Task<(bool sucesso, ArquivoResponseDTO resultado)> UploadArquivoAsync(IFormFile arquivo);
        List<ArquivoResponseDTO> ListarArquivos();
        FileStream? ObterArquivoParaDownload(string nomeArquivo);
        bool RemoverArquivo(string nomeArquivo);
        bool ArquivoExiste(string nomeArquivo);
    }
}




