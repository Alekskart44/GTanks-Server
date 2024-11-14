using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.main.procotol.commands
{
    public class Command
    {
        public CommandType Type { get; private set; }
        public string[] Args { get; private set; }

        public Command(CommandType type, string[] args)
        {
            Type = type;
            Args = args;
        }

        public override string ToString()
        {
            return $"{Type} {string.Join(", ", Args)}";
        }
    }
}
