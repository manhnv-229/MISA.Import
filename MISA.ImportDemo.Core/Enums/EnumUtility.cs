using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MISA.ImportDemo.Core.Enums
{
    public class EnumUtility
    {
        public string GetResourceNameByValue(string value)
        {
            System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("MISA.ImportDemo.Core.Properties.Resources", this.GetType().Assembly);
            var entry =
                resourceManager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                  .OfType<DictionaryEntry>()
                  .FirstOrDefault(e => e.Value.ToString() == value);
            if (entry.Key == null)
                return null;
            return entry.Key.ToString();
        }

        /// <summary>
        /// Lấy tên Resource theo giá trị (VD lấy ra Enum_Gender_Male theo giá trị là "Nam")
        /// </summary>
        /// <param name="value">Giá trị truyền vào</param>
        /// <param name="enumNameStringContains">Contain của Resource (VD:Enum_Gender)</param>
        /// <returns>Tên đầy đủ của Resource (Enum_Gender_Male)</returns>
        /// CreatedBy: NVMANH (10/06/2020)
        public string GetResourceNameByValue(string value, string enumNameStringContains)
        {
            var enumValueRemoveDiacritic = RemoveDiacritics(value);
            System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("MISA.ImportDemo.Core.Properties.Resources", this.GetType().Assembly);
            var entry =
                resourceManager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                  .OfType<DictionaryEntry>()
                  .FirstOrDefault(e => RemoveDiacritics(e.Value.ToString()) == enumValueRemoveDiacritic && e.Key.ToString().Contains(enumNameStringContains));
            if (entry.Key == null)
                return null;
            return entry.Key.ToString();
        }

        /// <summary>
        /// Chuyển chuỗi bỏ hết khoảng cách và dấu - mục đích compare chuỗi gần đúng
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string RemoveDiacritics(string text)
        {
            var newText = string.Concat(
                text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) !=
                                              UnicodeCategory.NonSpacingMark)
              ).Normalize(NormalizationForm.FormC);
            return newText.Replace(" ", string.Empty);
        }
    }
}
