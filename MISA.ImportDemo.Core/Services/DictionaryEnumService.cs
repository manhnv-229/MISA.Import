using MISA.ImportDemo.Core.Entities.Directory;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Enums;
using MISA.ImportDemo.Core.Helpers;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Services
{
    public class DictionaryEnumService: IDictionaryEnumService
    {
        List<EnumDictionary> dics = new List<EnumDictionary>();
        public IEnumerable<EnumDictionary> GetGenders()
        {
            return GetListEnumDictionary(typeof(Gender).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetTrainningLevels()
        {
            return GetListEnumDictionary(typeof(TrainningLevel).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetWorkStatus()
        {
            return GetListEnumDictionary(typeof(WorkStatus).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetMaritalStatus()
        {
            return GetListEnumDictionary(typeof(MaritalStatus).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetDegreeClassifications()
        {
            return GetListEnumDictionary(typeof(DegreeClassification).GetEnumValues());
        }

        private IEnumerable<EnumDictionary> GetListEnumDictionary(Array enumProp)
        {
            var _enumUtility = new EnumUtility();
            foreach (var item in enumProp)
            {
                var itemString = CommonFunction.GetResourceStringByEnum(item as Enum);
                dics.Add(new EnumDictionary() { Text = itemString, Value = (int)item });
            }
            return dics;
        }

        public IEnumerable<EnumDictionary> GetExerciseTypes()
        {
            return GetListEnumDictionary(typeof(ExerciseType).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetQuestionType()
        {
            return GetListEnumDictionary(typeof(QuestionType).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetVideoTypes()
        {
            return GetListEnumDictionary(typeof(VideoType).GetEnumValues());
        }

        public IEnumerable<EnumDictionary> GetThreadTypes()
        {
            return GetListEnumDictionary(typeof(ThreadType).GetEnumValues());
        }
    }
}
