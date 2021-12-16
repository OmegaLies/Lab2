using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Lab2
{
    struct DataItem
    {
        public double X { get; set; }
        public double Y { get; set; }
        public System.Numerics.Complex C { get; set; }

        public DataItem(double a, double b, System.Numerics.Complex i)
        {
            X = a;
            Y = b;
            C = i;
        }

        public string ToLongString(string format)
        {
            return $"X: {X.ToString(format)}, Y:{Y.ToString(format)}, Value: {C.ToString(format)}, Abs: {C.Magnitude.ToString(format)}";
        }

        public override string ToString()
        {
            return $"X: {X}, Y:{Y}, Value: {C}, Abs:{C.Magnitude}";
        }
    }

    public delegate System.Numerics.Complex FdblComplex(double x, double y);

    abstract class V1Data: IEnumerable<DataItem>
    {
        public string Str { get; protected set; }
        public DateTime Time { get; protected set; }
        
        public V1Data()
        {
            Str = default(string);
            Time = default(DateTime);
        }

        public V1Data(string s, DateTime t)
        {
            Str = s;
            Time = t;
        }
        public abstract int Count { get; }
        public abstract double AverageValue { get; }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return base.ToString();
        }

        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    class DataListEnumerator : IEnumerator<DataItem>
    {
        public List<DataItem> Enum = new();
        public int curr;
        public DataItem currItem;

        public DataListEnumerator(List<DataItem> A)
        {
            Enum = A;
            curr = -1;
            currItem = default(DataItem);
        }

        public bool MoveNext()
        {
            if (++curr >= Enum.Count)
            {
                return false;
            }
            else
            {
                currItem = Enum[curr];
                return true;
            }
        }

        public void Reset() => curr = -1;

        void IDisposable.Dispose() { }

        public DataItem Current { get{ return currItem; } }

        object IEnumerator.Current { get { return Current; } }
    }

    class V1DataList : V1Data, IEnumerable<DataItem>
    {
        public List<DataItem> List { get; private set; }

        public V1DataList()
        {
            Str = default(string);
            Time = default(DateTime);
            List = new();
        }

        public V1DataList(string s, DateTime t) : base(s, t)
        {
            List = new();
        }

        public bool Add(DataItem newItem)
        {
            foreach (DataItem i in List)
            {
                if (newItem.X == i.X && newItem.Y == i.Y)
                {
                    return false;
                }
            }
            List.Add(newItem);
            return true;
        }

        public int AddDefaults(int nItems, FdblComplex F)
        {
            Random rnd = new();
            int count = 0;
            for (int i = 0; i < nItems; ++i)
            {
                double x = rnd.Next(-500, 500) / 119;
                double y = rnd.Next(-500, 500) / 119;
                DataItem tmp = new(x, y, F(x, y));
                if (Add(tmp))
                {
                    ++count;
                }
            }
            return count;
        }

        public override int Count => List.Count;

        public override double AverageValue
        {
            get
            {
                double sum = 0;
                foreach (DataItem i in List) { sum += i.C.Magnitude; }
                return sum / Count;
            }

        }

        public override string ToString()
        {
            return $"Type: V1DataList, Name: {base.Str},  Date: {base.Time}, Count {Count}";
        }

        public override string ToLongString(string format)
        {
            string str = $"Type: V1DataList, Name: {base.Str},  Date: {base.Time}, Count: {Count}\n";
            foreach (DataItem i in List)
            {
                str += i.ToLongString(format) + "\n";
            }
            return str;
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return new DataListEnumerator(new List<DataItem>());
        }

        public bool SaveAsText(string filename)
        {
            bool res = true;
            try
            {
                using (StreamWriter Stream = new(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    Stream.WriteLine(Str);
                    Stream.WriteLine(Time.ToString());
                    int count = this.Count;
                    Stream.WriteLine(count);
                    for (int i = 0; i < count; i++)
                    {
                        Stream.WriteLine(List[i].X);
                        Stream.WriteLine(List[i].Y);
                        Stream.WriteLine(List[i].C.Real);
                        Stream.WriteLine(List[i].C.Imaginary);
                    }
                }
            }
            catch
            {
                res = false;
            }
            return res;
        }
        public bool LoadAsText(string filename)
        {
            bool res = true;
            try
            {
                using (StreamReader Stream = new(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    string line = Stream.ReadLine();
                    Str = line;
                    line = Stream.ReadLine();
                    Time = DateTime.Parse(line);
                    line = Stream.ReadLine();
                    int count = int.Parse(line);
                    List = new(count);
                    for (int i = 0; i < count; i++)
                    {
                        
                        line = Stream.ReadLine();
                        string line1 = Stream.ReadLine();
                        string line2 = Stream.ReadLine();
                        string line3 = Stream.ReadLine();
                        List.Add(new(double.Parse(line), double.Parse(line1), new(double.Parse(line2), double.Parse(line3))));
                    }
                }
            }
            catch (Exception ex)
            {
                res = false;
                throw ex;
            }
            return res;
        }
    }

    class DataArrayEnumerator : IEnumerator<DataItem>
    {
        public List<DataItem> Enum = new();
        public int curr;
        public DataItem currItem;
        public DataArrayEnumerator(int X, int Y, double Dx, double Dy, FdblComplex F)
        {
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    DataItem dot = new DataItem(i, j, F(i * Dx, j * Dy));
                    Enum.Add(dot);
                }
            }
            curr = -1;
            currItem = default(DataItem);
        }
        public bool MoveNext()
        {
            if (++curr >= Enum.Count)
            {
                return false;
            }
            else
            {
                currItem = Enum[curr];
                return true;
            }
        }

        public void Reset() => curr = -1;

        void IDisposable.Dispose() { }

        public DataItem Current { get { return currItem; } }

        object IEnumerator.Current { get { return Current; } }
    }

    class V1DataArray : V1Data, IEnumerable<DataItem>
    {
        public int Ox { get; private set; }
        public int Oy { get; private set; }
        public double Dx { get; private set; }
        public double Dy { get; private set; }
        public FdblComplex F1;
        public System.Numerics.Complex[,] List { get; private set; }

        public V1DataArray()
        {
            Str = default(string);
            Time = default(DateTime);
            List = new System.Numerics.Complex[0, 0];
        }

        public V1DataArray(string s, DateTime t) : base(s, t)
        {
            List = new System.Numerics.Complex[2, 2];
        }

        public V1DataArray(string s, DateTime t, int ox, int oy, double dx, double dy, FdblComplex F) : base(s, t)
        {
            Ox = ox;
            Oy = oy;
            Dx = dx;
            Dy = dy;
            F1 = F;
            List = new System.Numerics.Complex[Ox, Oy];
            for (int i = 0; i < Ox; ++i)
            {
                for (int j = 0; j < Oy; ++j)
                {
                    List[i, j] = F1(Dx * i, Dy * j);
                }
            }
        }

        public override int Count => Ox * Oy;
        public override double AverageValue
        {
            get
            {
                double sum = 0;
                foreach (System.Numerics.Complex i in List) { sum += i.Magnitude; }
                return sum / Count;
            }
        }
        public override string ToString()
        {
            return $"Type: V1DataArray, Name: {base.Str},  Date: {base.Time}, Count: {Count}, Ox: {Ox}, Oy: {Oy}, Dx: {Dx}, Dy: {Dy}";
        }
        public override string ToLongString(string format)
        {
            string str = $"Type: V1DataArray, Name: {base.Str},  Date: {base.Time}, Count: {Count}, Ox: {Ox}, Oy: {Oy}, Dx: {Dx.ToString(format)}, Dy: {Dy.ToString(format)}\n";
            for (int i = 0; i < Ox; ++i)
            {
                for (int j = 0; j < Oy; ++j)
                {
                    str += $"X: {(Dx * i).ToString(format)}, Y: {(Dy * j).ToString(format)}, Value: {List[i, j].ToString(format)}, Abs: {List[i, j].Magnitude.ToString(format)}\n";
                }
            }
            return str;
        }
        public static implicit operator V1DataList(V1DataArray arr)
        {
            V1DataList res = new(arr.Str, arr.Time);
            for (int i = 0; i < arr.Ox; ++i)
            {
                for (int j = 0; j < arr.Oy; ++j)
                {
                    res.Add(new DataItem(arr.Dx * i, arr.Dy * j, arr.List[i, j]));
                }
            }
            return res;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            return new DataArrayEnumerator(Ox, Oy, Dx, Dy, F1);
        }

        public bool SaveBinary(string filename)
        {
            bool res = true;
            try
            {
                using (BinaryWriter writer = new(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    writer.Write(Str);
                    writer.Write(Time.ToString());
                    writer.Write(Ox);
                    writer.Write(Oy);
                    writer.Write(Dx);
                    writer.Write(Dy);
                    foreach (var i in List)
                    {
                        writer.Write(i.Real);
                        writer.Write(i.Imaginary);
                    }
                }
            }
            catch
            {
                res = false;
            }
            return res;
        }
        public bool LoadBinary(string filename)
        {
            bool res = true;
            try
            {

                using (BinaryReader reader = new(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    Str = reader.ReadString();
                    Time = DateTime.Parse(reader.ReadString());
                    Ox = reader.ReadInt32();
                    Oy = reader.ReadInt32();
                    Dx = reader.ReadDouble();
                    Dy = reader.ReadDouble();
                    List = new System.Numerics.Complex[Ox, Oy];
                    for (int i = 0; i < Ox; i++)
                    {
                        for (int j = 0; j < Ox; j++)
                        {
                            double R = reader.ReadDouble();
                            double Im = reader.ReadDouble();
                            List[i, j] = new System.Numerics.Complex(R, Im);
                        }
                    }
                }
            }
            catch
            {
                res = false;
            }
            return res;
        }
    }

    class V1MainCollection
    {
        private List<V1Data> List = new();
        public int Count => List.Count;
        public V1Data this[int index]
        {
            get => List[index];
        }

        public bool Contains(string ID)
        {
            if (List == null)
            {
                return false;
            }
            foreach (V1Data i in List)
            {
                if (i.Str == ID)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Add(V1Data v1Data)
        {
            if (!Contains(v1Data.Str))
            {
                List.Add(v1Data);
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            string str = "";
            foreach (V1Data i in List)
            {
                str += i.ToString() + " ";
            }
            return str;
        }
        public string ToLongString(string format)
        {
            string str = "";
            foreach (V1Data i in List)
            {
                str += i.ToLongString(format) + " ";
            }
            return str;
        }

        public double AverageMeasure
        {
            get
            {
                var comb = from v1 in List where v1.Count != 0 select v1.AverageValue;
                double Aver = double.NaN;
                if (comb.Any())
                {
                    Aver = comb.Average();
                }
                return Aver;
            }
        }
        public DataItem? MaxFromAverage
        {
            get
            {
                var comb = (from v1 in List.OfType<V1DataArray>()
                            where v1.Count != 0
                            from Item in v1
                            select new { c = Item, len = Math.Abs(Item.C.Magnitude - this.AverageMeasure) });
                var comb1 = from v1 in List.OfType<V1DataList>()
                            where v1.Count != 0
                            from Item in v1.List
                            select new { c = Item, len = Math.Abs(Item.C.Magnitude - this.AverageMeasure) };
                comb = comb.Union(comb1);
                if (comb.Any())
                {
                    double tmp = (from i in comb select i.len).Max();
                    comb = comb.Where(x => (x.len == tmp));
                    if (comb.Any())
                    {
                        return comb.First().c;
                    }
                }
                return null;
            }
        }

        public IEnumerable<double> uniqueX
        {
            get
            {
                var comb = (from v1 in List.OfType<V1DataList>()
                            where v1.Count != 0
                            select (V1DataList)v1).Union(
                            from v1 in List.OfType<V1DataArray>()
                            where v1.Count != 0
                            select (V1DataList)v1).Distinct();
                var res = (from v1 in comb
                           from v2 in comb
                           where v1.Str != v2.Str
                           from Item1 in v1.List
                           from Item2 in v2.List
                           where Item1.X == Item2.X && Item1.Y == Item2.Y
                           select Item1.X).Distinct();
                /*var res = (from v1 in comb
                           from v2 in comb
                           where v1.Str != v2.Str
                           from Item1 in v1.List
                           from Item2 in v2.List
                           where Item1.X == Item2.X
                           select Item1.X).Distinct();*/
                if (res.Any())
                {
                    return res;
                }
                return null;
            }
        }
    }
}
