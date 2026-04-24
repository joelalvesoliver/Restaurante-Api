// Importa as ferramentas para trabalhar com XML no .NET.
using System.Xml.Serialization;

namespace Restaurante.Api.DTOs
{
    // DTO específico para receber dados no formato XML.
    // Normalmente as APIs usam JSON (padrão moderno), mas algumas integrações antigas
    // ou sistemas corporativos ainda enviam dados em XML.
    // Este DTO demonstra como o .NET pode receber e interpretar XML automaticamente.
    //
    // Exemplo de XML que este DTO espera:
    // <Usuario>
    //   <Id>1</Id>
    //   <Nome>João Silva</Nome>
    //   <Email>joao@restaurante.com</Email>
    // </Usuario>

    // [XmlRoot] define o nome da TAG raiz (principal) no XML.
    // Aqui, a tag raiz será <Usuario>.
    [XmlRoot("Usuario")]
    public class UsuarioXmlDto
    {
        // [XmlElement] mapeia uma propriedade C# para uma TAG no XML.
        // Esta propriedade corresponde à tag <Id> dentro do XML.
        [XmlElement("Id")]
        public int Id { get; set; }

        // Corresponde à tag <Nome> no XML.
        [XmlElement("Nome")]
        public string Nome { get; set; }

        // Corresponde à tag <Email> no XML.
        [XmlElement("Email")]
        public string Email { get; set; }
    }
}
