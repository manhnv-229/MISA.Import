using MISA.ImportDemo.Core.Entities.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces
{
    public interface IDictionaryEnumService
    {
        IEnumerable<EnumDictionary> GetGenders();
        IEnumerable<EnumDictionary> GetTrainningLevels();
        IEnumerable<EnumDictionary> GetWorkStatus();
        IEnumerable<EnumDictionary> GetMaritalStatus();
        IEnumerable<EnumDictionary> GetDegreeClassifications();
        IEnumerable<EnumDictionary> GetExerciseTypes();
        IEnumerable<EnumDictionary> GetQuestionType();
        IEnumerable<EnumDictionary> GetVideoTypes();
        IEnumerable<EnumDictionary> GetThreadTypes();
    }
}
