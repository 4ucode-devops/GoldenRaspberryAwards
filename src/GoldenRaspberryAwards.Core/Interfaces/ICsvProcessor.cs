using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenRaspberryAwards.Core.Interfaces
{
   public interface ICsvProcessor<T> where T : class
    {
        Task<List<T>> ProcessCsvAsync(string filePath);
    }
}
