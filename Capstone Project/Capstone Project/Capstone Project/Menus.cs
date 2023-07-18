using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//This Menu system is the one used in summative 14. Simon said in lecture on capstone project we were welcome to take and use it.
namespace Capstone_Project_441101_2223
{
    public static class ConsoleHelpers
    {
        // TODO Can I remove the "int" from this to allow "floats" in a nice way
        public static int GetIntegerInRange(int pMin, int pMax, string pMessage)
        {
            if (pMin > pMax)
            {
                throw new Exception($"Minimum value {pMin} cannot be greater than maximum value {pMax}");
            }

            int result;

            do
            {
                Console.WriteLine(pMessage);
                Console.WriteLine($"Please enter a number between {pMin} and {pMax} inclusive.");

                string userInput = Console.ReadLine();

                try
                {
                    result = int.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"{userInput} is not a number");
                    continue;
                }

                if (result >= pMin && result <= pMax)
                {
                    return result;
                }
                Console.WriteLine($"{result} is not between {pMin} and {pMax} inclusive.");
            } while (true);
        }
        public static int GetInteger(int pMin, string pMessage)
        {
            int result;

            do
            {
                Console.WriteLine(pMessage);
                Console.WriteLine($"Please enter an amount higher than {pMin}.");

                string userInput = Console.ReadLine();

                try
                {
                    result = int.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"{userInput} is not a number");
                    continue;
                }

                if (result >= pMin)
                {
                    return result;
                }
                Console.WriteLine($"{result} is not higher than {pMin}");
            } while (true);
        }

        public static decimal GetDecimal(int pMin, string pMessage)
        {
            decimal result;

            do
            {
                Console.WriteLine(pMessage);
                Console.WriteLine($"Please enter an amount higher than {pMin}.");

                string userInput = Console.ReadLine();

                try
                {
                    result = decimal.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"{userInput} is not a number");
                    continue;
                }

                if (result >= pMin)
                {
                    return result;
                }
                Console.WriteLine($"{result} is not higher than {pMin}");
            } while (true);
        }
    }

    abstract class ConsoleMenu : MenuItem
    {
        protected List<MenuItem> _menuItems = new List<MenuItem>();

        public bool IsActive { get; set; }

        public abstract void CreateMenu();

        public override void Select()
        {
            IsActive = true;
            do
            {
                CreateMenu();
                string output = $"{MenuText()}{Environment.NewLine}";
                int selection = ConsoleHelpers.GetIntegerInRange(1, _menuItems.Count, this.ToString()) - 1;
                _menuItems[selection].Select();
            } while (IsActive);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(MenuText());
            for (int i = 0; i < _menuItems.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {_menuItems[i].MenuText()}");
            }
            return sb.ToString();
        }
    }

    abstract class MenuItem // TODO Make Interface?
    {
        public abstract string MenuText();
        public abstract void Select();
    }

    class ExitMenuItem : MenuItem
    {
        private ConsoleMenu _menu;

        public ExitMenuItem(ConsoleMenu parentItem)
        {
            _menu = parentItem;
        }

        public override string MenuText()
        {
            return "Exit";
        }

        public override void Select()
        {
            _menu.IsActive = false;
        }
    }
}
