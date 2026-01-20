using System.Collections.Generic;

namespace TravelersKalimba
{
     public static class LocalizationManager
     {
        private static Dictionary<TextTranslation.Language, Dictionary<string, string>> _translateSheet =
            new Dictionary<TextTranslation.Language, Dictionary<string, string>>
            {
                {
                    TextTranslation.Language.ENGLISH, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Take Out Kalimba" },
                        { ModStaticState.KalimbaUnequipStringId, "Put Away The Kalimba" },
                        { ModStaticState.KalimbaPlayStringId, "Play The Kalimba (Hold)" },
                    }
                },

                {
                    TextTranslation.Language.RUSSIAN, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Взять калимбу" },
                        { ModStaticState.KalimbaUnequipStringId, "Убрать калимбу" },
                        { ModStaticState.KalimbaPlayStringId, "Играть на калимбе (Зажать)" },
                    }
                },

                {
                    TextTranslation.Language.SPANISH_LA, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Sacar la kalimba" },
                        { ModStaticState.KalimbaUnequipStringId, "Guardar la kalimba" },
                        { ModStaticState.KalimbaPlayStringId, "Tocar la kalimba (Mantener presionado)" },
                    }
                },

                {
                    TextTranslation.Language.GERMAN, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Kalimba herausholen" },
                        { ModStaticState.KalimbaUnequipStringId, "Kalimba weglegen" },
                        { ModStaticState.KalimbaPlayStringId, "Kalimba spielen (Halten)" },
                    }
                },

                {
                    TextTranslation.Language.FRENCH, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Sortir la kalimba" },
                        { ModStaticState.KalimbaUnequipStringId, "Ranger la kalimba" },
                        { ModStaticState.KalimbaPlayStringId, "Jouer de la kalimba (Maintenir)" },
                    }
                },

                {
                    TextTranslation.Language.ITALIAN, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Tirare fuori la kalimba" },
                        { ModStaticState.KalimbaUnequipStringId, "Riporre la kalimba" },
                        { ModStaticState.KalimbaPlayStringId, "Suonare la kalimba (Tieni premuto)" },
                    }
                },

                {
                    TextTranslation.Language.POLISH, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Wyciągnij kalimbę" },
                        { ModStaticState.KalimbaUnequipStringId, "Schowaj kalimbę" },
                        { ModStaticState.KalimbaPlayStringId, "Graj na kalimbie (Przytrzymaj)" },
                    }
                },

                {
                    TextTranslation.Language.PORTUGUESE_BR, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Pegar a kalimba" },
                        { ModStaticState.KalimbaUnequipStringId, "Guardar a kalimba" },
                        { ModStaticState.KalimbaPlayStringId, "Tocar a kalimba (Segurar)" },
                    }
                },

                {
                    TextTranslation.Language.JAPANESE, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "カリンバを取り出す" },
                        { ModStaticState.KalimbaUnequipStringId, "カリンバをしまう" },
                        { ModStaticState.KalimbaPlayStringId, "カリンバを演奏する（長押し）" },
                    }
                },

                {
                    TextTranslation.Language.CHINESE_SIMPLE, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "取出卡林巴琴" },
                        { ModStaticState.KalimbaUnequipStringId, "收起卡林巴琴" },
                        { ModStaticState.KalimbaPlayStringId, "演奏卡林巴琴（按住不放）" },
                    }
                },

                {
                    TextTranslation.Language.KOREAN, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "칼림바 꺼내기" },
                        { ModStaticState.KalimbaUnequipStringId, "칼림바 집어넣기" },
                        { ModStaticState.KalimbaPlayStringId, "칼림바 연주 (길게 누르기)" },
                    }
                },

                {
                    TextTranslation.Language.TURKISH, new Dictionary<string, string>
                    {
                        { ModStaticState.KalimbaEquipStringId, "Kalimbayı çıkar" },
                        { ModStaticState.KalimbaUnequipStringId, "Kalimbayı sakla" },
                        { ModStaticState.KalimbaPlayStringId, "Kalimba çal (Basılı tut)" },
                    }
                },
            };

        private static TextTranslation.Language _defaultLocale = TextTranslation.Language.ENGLISH;
        private static TextTranslation.Language _currentLocale = _defaultLocale;
        
        public static string GetTranslatedString(string id)
        {
            TextTranslation.Language locale = GetCurrentLocale();

            if (_translateSheet[locale].ContainsKey(id))
                return _translateSheet[locale][id]; 
            else if (_translateSheet[_defaultLocale].ContainsKey(id))
                return _translateSheet[_defaultLocale][id];
            else
                return id;
        } 

        public static bool HaveString(string id)
        {
            TextTranslation.Language locale = GetCurrentLocale();

            return _translateSheet[locale].ContainsKey(id);
        }

        public static bool HaveLocale(TextTranslation.Language locale)
        {
            return _translateSheet.ContainsKey(locale);
        }

        public static TextTranslation.Language GetCurrentLocale()
        {
            TextTranslation.Language locale;

            if(TextTranslation.Get() != null)
                _currentLocale = TextTranslation.Get().GetLanguage();

            if (_translateSheet.ContainsKey(_currentLocale))
                locale = _currentLocale;
            else
                locale = _defaultLocale;

            return locale;
        }
    }
}
