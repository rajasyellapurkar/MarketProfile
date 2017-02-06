using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please Enter the Stock / Index Symbol");
                string s = Console.ReadLine();
                if (s.ToLower() != "exit")
                {
                    try
                    {
                        string quote = Extract("https://www.google.com/finance/getprices?q=" + s.ToUpper() + "&x=NSE&i=1800&p=1d&f=h,l");
                        string[] quoteArray = quote.Split('\n');

                        List<double> TPOHigh = new List<double>();
                        List<double> TPOLow = new List<double>();


                        for (int i = 7; i < quoteArray.Length - 1; i++)
                        {
                            string[] parseHL = quoteArray[i].Split(',');
                            TPOHigh.Add(Double.Parse(parseHL[0]));
                            TPOLow.Add(Double.Parse(parseHL[1]));
                        }


                        double high = TPOHigh.Max();
                        double low = TPOLow.Min();
                        double dayRange = high - low;
                        double dayAverage = (high + low) / 2;
                        double tickSize = dayAverage * 0.001;
                        int arraySize = (int)Math.Ceiling(dayRange / tickSize);
                        char c = 'A';
                        arraySize++;
                        double[] b = new Double[arraySize];
                        b[0] = high;
                        for (int i = 1; i < b.Length; i++)
                        {
                            b[i] = b[i - 1] - tickSize;
                        }


                        char[,] a = new Char[arraySize, 13];

                        for (int i = 0; i < TPOHigh.Count; i++)
                        {


                            int startIndex = findIndex(b, TPOHigh[i]);
                            int stopIndex = findIndex(b, TPOLow[i]);
                            for (int row = startIndex; row <= stopIndex; row++)
                            {
                                a[row, i] = c;
                            }
                            c++;
                        }
                        /*Print 1*/
                        for (int row = 0; row < arraySize; row++)
                        {
                            Console.Write(b[row].ToString("#.00") + "  ");
                            for (int col = 0; col < 13; col++)
                            {
                                Console.Write(a[row, col]);
                                Console.Write("   ");
                            }
                            Console.WriteLine();
                        }


                        Console.WriteLine();


                        /*Print 2*/
                        for (int row = 0; row < arraySize; row++)
                        {
                            Console.Write(b[row].ToString("#.00") + "  ");
                            for (int col = 0; col < 13; col++)
                            {
                                if (a[row, col] == '\0') continue;
                                Console.Write(a[row, col]);
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                        Console.WriteLine("Enter \"exit\" to exit");
                        Console.WriteLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("You might have entered wrong Symbol or please check your internet connection: "+ e.Message);
                    }
                }
                else
                { return; }
            }
        }


        public static int findIndex(double[] b, double value)
        {
            for (int i = 0; i < b.Length - 1; i++)
            {
                if (value <= b[i] && value >= b[i + 1])
                {
                    double diff1 = b[i] - value;
                    double diff2 = value - b[i + 1];
                    if (diff1 <= diff2) return i;
                    else return i + 1;
                }

            }
            return -1;
        }


        public static string Extract(string googleHttpRequestString)
        {
            //if need to pass proxy using local configuration  
            System.Net.WebClient webClient = new WebClient();
            webClient.Proxy = HttpWebRequest.GetSystemWebProxy();
            webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;


            Stream strm = webClient.OpenRead(googleHttpRequestString);
            StreamReader sr = new StreamReader(strm);
            string result = sr.ReadToEnd();
            strm.Close();
            return result;
        }
    }
}






