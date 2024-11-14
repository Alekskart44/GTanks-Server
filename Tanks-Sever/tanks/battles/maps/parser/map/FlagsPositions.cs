using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tanks_Sever.tanks.battles.maps.parser.map
{
    [XmlRoot("ctf-flags")]
    public class FlagsPositions
    {
        [XmlElement("flag-red")]
        public Vector3d _redFlag;

        [XmlElement("flag-blue")]
        public Vector3d _blueFlag;

        public Vector3d GetRedFlag()
        {
            return _redFlag;
        }

        public void SetRedFlag(Vector3d redFlag)
        {
            _redFlag = redFlag;
        }

        public Vector3d GetBlueFlag()
        {
            return _blueFlag;
        }

        public void SetBlueFlag(Vector3d blueFlag)
        {
            _blueFlag = blueFlag;
        }

        public override string ToString()
        {
            return $"red flag: {_redFlag} blue: {_blueFlag}";
        }
    }
}