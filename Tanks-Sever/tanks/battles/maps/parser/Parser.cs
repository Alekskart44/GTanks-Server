using System.Xml.Serialization;

namespace Tanks_Sever.tanks.battles.maps.parser
{
    public class Parser
    {
        private readonly XmlSerializer _serializer;

        public Parser()
        {
            _serializer = new XmlSerializer(typeof(map.Map));
        }

        public map.Map ParseMap(FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return (map.Map)_serializer.Deserialize(stream);
            }
        }
    }
}
