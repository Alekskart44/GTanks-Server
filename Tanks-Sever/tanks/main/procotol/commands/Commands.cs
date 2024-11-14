using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.main.procotol.commands
{
    public static class Commands
    {
        public static Command Decrypt(string request)
        {
            // Разделяет строку по символу ';'
            var parts = request.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new ArgumentException("Invalid request format.");
            }

            // Первый элемент - это тип команды
            if (!Enum.TryParse(parts[0], true, out CommandType commandType))
            {
                throw new ArgumentException("Unknown command type.");
            }

            // Остальные элементы - это аргументы команды
            var args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, parts.Length - 1);

            return new Command(commandType, args);
        }
    }
}
