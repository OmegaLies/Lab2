using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;

namespace Lab2
{
    class Program
    {
        static Complex Func(double x, double y) => new Complex((x + y) * (x + y) + 1, (x - y) * (x - y) - 1);
        static void testData()
        {
            string format = "F2";

            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "DataArray.dat");
            V1DataArray Array1 = new("Array one", DateTime.Now, 5, 1, 0.7, 0.4, Func);
            Array1.SaveBinary(filename);
            V1DataArray Array2 = new();
            Array2.LoadBinary(filename);
            Console.WriteLine(Array1.ToLongString(format));
            Console.WriteLine(Array2.ToLongString(format));

            filename = System.IO.Path.Combine(Environment.CurrentDirectory, "DataList.txt");
            V1DataList List1 = Array1;
            List1.SaveAsText(filename);
            V1DataList List2 = new();
            List2.LoadAsText(filename);
            Console.WriteLine(List1.ToLongString(format));
            Console.WriteLine(List2.ToLongString(format));
        }

        static void testCollection()
        {
            string format = "F2";
            V1DataArray Array1 = new("Array one", DateTime.Now, 5, 1, 0.7, 0.4, Func);
            V1DataArray Array2 = new("Array two", DateTime.Now);
            V1DataList List1 = new("List one", DateTime.Now);
            List1.AddDefaults(6, Func);
            List1.Add(new DataItem(0.0, 0.0, new Complex(1, 2)));
            List1.Add(new DataItem(0.7, 0.4, new Complex(1, 2)));
            V1DataList List2 = new("List two", DateTime.Now);
            V1MainCollection Collection = new();
            Collection.Add(Array1);
            Collection.Add(Array2);
            Collection.Add(List1);
            Collection.Add(List2);
            Console.WriteLine(Collection.ToLongString(format));

            double aver = Collection.AverageMeasure;
            DataItem? Max = Collection.MaxFromAverage;
            IEnumerable<double> uniq = Collection.uniqueX;
            Console.WriteLine("Average measure " + aver);
            if (Max != null)
            {
                Console.WriteLine("Element with max difference from average measure:" + "\n" +((DataItem)Max).ToLongString(format));
            }
            else
            {
                Console.WriteLine("Element with max difference from average measure doesn't exist");
            }
            Console.WriteLine("X of dots, that are used at least twice in  different elements of collection: ");
            if (uniq != null) {
                foreach (var i in uniq)
                {
                    Console.Write(i.ToString(format) + " ");
                }
            }
            else
            {
                Console.WriteLine("Dots, that are used at least twice in the different elements of collection, doesn't exist");
            }
            
        }

        static void Main()
        {
            Console.WriteLine("Part one");
            testData();
            Console.WriteLine("Part two");
            testCollection();
        }
    }
}
