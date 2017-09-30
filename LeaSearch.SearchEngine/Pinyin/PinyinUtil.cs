using System;

namespace LeaSearch.SearchEngine.Pinyin
{
    public class PinyinUtil
    {
        public static bool ContainsChinese(String s)
        {
            if ((s == null) || (string.Empty == s.Trim()))
                return false;

            foreach (char t in s)
            {
                if (IsChinese(t))
                    return true;
            }
            return false;
        }

        public static bool IsChinese(char a)
        {
            int v = a;
            return (v >= 19968) && (v <= 171941);
        }


        public static string GetPinyinString(string chinese)
        {
            string chinesePinyin = null;
            chinesePinyin = NPinyin.Pinyin.GetPinyin(chinese);
            return chinesePinyin.Replace(" ", string.Empty).ToLower();
        }

        public static string GetInitPinyinString(string chinese)
        {
            string chinesePinyin = null;
            chinesePinyin = NPinyin.Pinyin.GetInitials(chinese);
            return chinesePinyin.Replace(" ", string.Empty).ToLower();
        }
    }
}
