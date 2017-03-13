using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ML.Lab
{
    [TestClass]
    public class UnitTest1
    {
        // https://th.wikipedia.org/wiki/%E0%B8%81%E0%B8%B2%E0%B8%A3%E0%B9%81%E0%B8%9A%E0%B9%88%E0%B8%87%E0%B8%81%E0%B8%A5%E0%B8%B8%E0%B9%88%E0%B8%A1%E0%B8%82%E0%B9%89%E0%B8%AD%E0%B8%A1%E0%B8%B9%E0%B8%A5%E0%B9%81%E0%B8%9A%E0%B8%9A%E0%B9%80%E0%B8%84%E0%B8%A1%E0%B8%B5%E0%B8%99

        List<Subject> dataSet = new List<Subject>();
        
        [TestInitialize]
        public void Setup()
        {
            dataSet = new List<Subject>
            {
                new Subject(1){ A = 1, B = 1 },
                new Subject(2){ A = 1.5, B = 2 },
                new Subject(3){ A = 3, B = 4 },
                new Subject(4){ A = 5, B = 7 },
                new Subject(5){ A = 3.5, B = 5 },
                new Subject(6){ A = 4.5, B = 5 },
                new Subject(7){ A = 3.5, B = 4.5 },
            };


        }

        [TestMethod]
        public void KMean()
        {
            // K = 2 random
            //var p1 = dataSet.Where(x => x.Id == 1).Single();
            //Ditances(p1, dataSet).ToList();
            //Console.WriteLine("=============================");
            //var p4 = dataSet.Where(x => x.Id == 4).Single();
            //Ditances(p4, dataSet).ToList();

            var dicOfAverage = new Dictionary<Subject, double>();
            foreach (var center in dataSet)
            {
                var average = Distance(center, dataSet).Average( x => x.Item1 );
                dicOfAverage.Add(center, average);
                Console.WriteLine("Cen: {0} = {1}", center.Id, average);
            }

            // select p7 center
            var minDisP = dicOfAverage
                .Where( y => y.Value == dicOfAverage.Min(x => x.Value)).First();
            
            //dataSet.Remove(p7);
            var group1 = new List<Subject>();
            foreach (var dis in Distance(minDisP.Key, dataSet))
            {
                Console.WriteLine("p: {0}, d: {1}", dis.Item2.Id, dis.Item1);
                if (dis.Item1 <= minDisP.Value)
                {
                    group1.Add(dis.Item2);
                }
            }
            group1.Add(minDisP.Key);

            foreach (var item in group1)
            {
                Console.WriteLine("p: {0}", item.Id);
            }

            //-----------------------------------------------------------------
            var remainData = dataSet.Where(x => !group1.Contains(x)).ToList();

            if (remainData.Count() > 0)
            {
                Console.WriteLine("-----------------------------------------------------------------");
                dicOfAverage = new Dictionary<Subject, double>();
                foreach (var center in remainData)
                {
                    var average = Distance(center, remainData).Average(x => x.Item1);
                    dicOfAverage.Add(center, average);
                    Console.WriteLine("Cen: {0} = {1}", center.Id, average);
                }

                minDisP = dicOfAverage.Where(y => y.Value == dicOfAverage.Min(x => x.Value)).First();

                var group2 = new List<Subject>();
                foreach (var dis in Distance(minDisP.Key, remainData))
                {
                    Console.WriteLine("p: {0}, d: {1}", dis.Item2.Id, dis.Item1);
                    if (dis.Item1 <= minDisP.Value)
                    {
                        group2.Add(dis.Item2);
                    }
                }
                group2.Add(minDisP.Key);

                foreach (var item in group2)
                {
                    Console.WriteLine("p: {0}", item.Id);
                }

                remainData = remainData.Where(x => !group2.Contains(x)).ToList();
            }
        }

        [TestMethod]
        public void test_FindCenter()
        {
            var centerList = FindCenter(dataSet);
            foreach (var center in centerList)
            {
                Console.WriteLine("{0}", center.Id);
            }
        }


        public IEnumerable<Subject> FindCenter(IEnumerable<Subject> dataSet)
        {
            if (dataSet.Count() == 1)
            {
                yield return dataSet.First();
            }
            else
            {
                var dicOfAverage = dataSet.Select(center => new 
                {
                    Ditance = Distance(center, dataSet).Average(x => x.Item1),
                    Center = center
                }).OrderBy(x => x.Ditance);

                var minDisP = dicOfAverage.First();

                var grp = Distance(minDisP.Center, dataSet)
                            .Where(x => x.Item1 <= minDisP.Ditance)
                            .Select( y => y.Item2 );

                yield return minDisP.Center;

                var remainData = dataSet.Where(x => !grp.Contains(x) && x != minDisP.Center);
                if (remainData.Count() > 0)
                {
                    foreach (var center in FindCenter(remainData))
                    {
                        yield return center;
                    }
                }
            }
        }

        public IEnumerable<Tuple<double, Subject>> Distance(Subject center, IEnumerable<Subject> dataSet)
        {
            foreach (var p in dataSet.Where(x => x != center))
            {
                yield return new Tuple<double, Subject>(center.DistanceTo(p), p);
            }
        }
    }

    // http://mnemstudio.org/clustering-k-means-example-1.htm
    public class Subject : PositionBase, IDistance<Subject>
    {
        public Subject(int id)
        {
            Id = id;
        }
        public int Id { get; protected set; }
        public double A { get; set; }
        public double B { get; set; }

        public double DistanceTo(Subject to)
        {
            // Euclidean distance measure
            return Math.Sqrt(Math.Pow(A - to.A, 2) + Math.Pow(B - to.B, 2));
        }
    }

    public abstract class PositionBase
    {

    }

    public interface IDistance<E> where E : PositionBase
    {
        double DistanceTo(E whereBase);
    } 
}
