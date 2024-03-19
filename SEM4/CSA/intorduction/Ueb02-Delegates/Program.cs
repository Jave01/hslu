using System;

namespace Ueb02_Delegates
{
    public delegate void MyDelegate(string msg);

    public class Program
    {
        public Program()
        {
            MyDelegate del;

            // Beispiel mit Unicast-Delegate
            del = new MyDelegate(CallbackMethod1);
            del("test example 1");

            del = new MyDelegate(CallbackMethod1);
            del = new MyDelegate(CallbackMethod2);
            del("test example 2");

            // Beispiel mit Multicast-Delegate
            del += new MyDelegate(CallbackMethod1);
            del("test multicast example 1");

            del += new MyDelegate(CallbackMethod2);
            del("test multicast example 2");

            del += new MyDelegate(CallbackMethod2);
            del("test multicast example 3");

            del -= new MyDelegate(CallbackMethod2);
            del -= new MyDelegate(CallbackMethod2);
            del("test multicast example 4");

            del = null; // Alle registrierten Handler löschen
            del += new MyDelegate(CallbackMethod1);
            del += new MyDelegate(CallbackMethod2);
            del += new MyDelegate(CallbackMethod3);
            del("test multicast example 5");

            // Alle registrierten Handler ermitteln
            Delegate[] delegates = del.GetInvocationList();
            foreach (MyDelegate d in delegates)
            {
                d("Hallo");
            }
        }


        private void CallbackMethod1(string msg)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CallbackMethod1: " + msg);
            Console.ForegroundColor = oldColor;
        }


        private void CallbackMethod2(string msg)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("CallbackMethod2: " + msg);
            Console.ForegroundColor = oldColor;
        }

        private void CallbackMethod3(string msg)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("CallbackMethod3: " + msg);
            Console.ForegroundColor = oldColor;
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            Console.Read();
        }
    }
}
