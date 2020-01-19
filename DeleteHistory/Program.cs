using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteHistory
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isContinue = true;
            do
            {
                Console.Write("[INPUT] Keyword from url that you want to delete: ");
                var url = Console.ReadLine();
                var urls = new Chrome().GetHistory(url);
                Console.WriteLine("=========================================================");
                Console.Write("[INPUT] Do you want to continue? y/n: ");
                string answer = Console.ReadLine();
                if (answer.Equals("n") || answer.Equals("N"))
                {
                    isContinue = false;
                }
            } while (isContinue);
            Console.Write("[INFO] Thank you for using my little app. From NhaAn with love <3");
            Console.Write("[INFO] Press anykey to exit.");
            Console.Read();
        }
    }
}
