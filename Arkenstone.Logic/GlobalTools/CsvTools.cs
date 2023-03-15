using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Logic.GlobalTools
{
    public static class CsvTools
    {
        public static List<T> ReadCsv<T>(string path, ClassMap classMap = null)
        {
            List<T> csvrecords;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                if (classMap != null)
                {
                    csv.Context.RegisterClassMap(classMap);
                }
                csvrecords = csv.GetRecords<T>().ToList();
            }
            return csvrecords;
        }
    }
}
