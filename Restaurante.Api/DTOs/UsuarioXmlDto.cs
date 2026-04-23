using System.Xml.Serialization;

namespace Restaurante.Api.DTOs
{
    [XmlRoot("Usuario")]
    public class UsuarioXmlDto
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Nome")]
        public string Nome { get; set; }

        [XmlElement("Email")]
        public string Email { get; set; }
    }
}
