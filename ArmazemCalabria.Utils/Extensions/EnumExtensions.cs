using ArmazemCalabria.Utils.Attributes;
using ArmazemCalabria.Utils.DTO;
using System.ComponentModel;

namespace ArmazemCalabria.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum enumerationValue)
        {
            var type = enumerationValue.GetType();
            var memberInfo = type.GetMembers().FirstOrDefault(w => w.Name == Enum.GetName(type, enumerationValue));
            var attribute = memberInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;

            return attribute?.Description ??
                              enumerationValue.ToString();
        }

        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
              ? (T)attributes[0]
              : null;
        }

        public static string StringValue(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var stringValueAttributes = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            if (stringValueAttributes == null)
                return string.Empty;
            return stringValueAttributes.Length > 0 ? stringValueAttributes[0].Value : value.ToString();
        }

        public static T GetByStringValue<T>(this string stringValue) where T : Enum
        {
            foreach (T @enum in Enum.GetValues(typeof(T)))
            {
                var fieldInfo = @enum.GetType().GetField(@enum.ToString());

                if (fieldInfo != null)
                {
                    var stringValueAttributes = (StringValueAttribute[])fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false);

                    if (stringValueAttributes.Length > 0 && stringValueAttributes[0].Value == stringValue)
                        return @enum;
                }
            }

            throw new ArgumentException($"StringValue '{stringValue}' não encontrado.");
        }

        public static T GetByDescription<T>(this string description) where T : Enum
        {
            foreach (T @enum in Enum.GetValues(typeof(T)))
            {
                var fieldInfo = @enum.GetType().GetField(@enum.ToString());

                if (fieldInfo != null)
                {
                    var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (descriptionAttributes.Length > 0 && descriptionAttributes[0].Description == description)
                        return @enum;
                }
            }

            return default;
        }

        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static int AsInt(this Enum value)
        {
            return (int)Enum.Parse(value.GetType(), value.ToString());
        }

        public static string StrValue(this Enum value)
        {
            return value.AsInt().ToString();
        }

        public static bool IsValid(this Enum value)
        {
            return Enum.IsDefined(value.GetType(), value);
        }
        public static List<ChaveDescricaoDTO<int>> ConverterParaLista<T>(params T[] enums) where T : Enum
        {
            var enumList = new List<ChaveDescricaoDTO<int>>();
            foreach (var parametroEnum in enums)
            {
                enumList.Add(new()
                {
                    Chave = parametroEnum.AsInt(),
                    Descricao = parametroEnum.GetDescription()
                });
            }

            return enumList;
        }
        public static List<ChaveDescricaoDTO<int>> ConverterParaLista<T>(List<T> enums) where T : Enum
        {
            var enumList = new List<ChaveDescricaoDTO<int>>();
            foreach (var parametroEnum in enums)
            {
                enumList.Add(new()
                {
                    Chave = parametroEnum.AsInt(),
                    Descricao = parametroEnum.GetDescription()
                });
            }

            return enumList;
        }

        public static List<ChaveDescricaoDTO<int>> ConverterParaLista<T>()
        {
            var enumList = new List<ChaveDescricaoDTO<int>>();
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                enumList.Add(new ChaveDescricaoDTO<int>
                {
                    Chave = (int)Enum.Parse(typeof(T), name),
                    Descricao = ((Enum)Enum.Parse(typeof(T), name)).GetDescription()
                });
            }

            return enumList;
        }

        public static List<ChaveDescricaoDTO<int>> ConverterParaListaValor<T>()
        {
            var enumList = new List<ChaveDescricaoDTO<int>>();
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                enumList.Add(new ChaveDescricaoDTO<int>
                {
                    Chave = (int)Enum.Parse(typeof(T), name),
                    Descricao = ((Enum)Enum.Parse(typeof(T), name)).ToString()
                });
            }

            return enumList;
        }

        public static List<int> ConverterParaListaNumerica<T>()
        {
            var enumList = new List<int>();
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                enumList.Add((int)Enum.Parse(typeof(T), name));
            }

            return enumList;
        }

        public static List<int> ConverterParaListaNumerica<T>(params T[] enums) where T : Enum
        {
            return enums.Select(enumerador => enumerador.AsInt()).ToList();
        }

        public static int? ObterValorEnumPorDescricao<T>(string descricao) where T : Enum
        {
            if (string.IsNullOrEmpty(descricao))
                return default;

            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute &&
                    attribute.Description == descricao)
                {
                    return (int)field.GetValue(null);
                }
                else if (field.Name == descricao)
                {
                    return (int)field.GetValue(null);
                }
            }

            return default;
        }

        public static List<ChaveDescricaoDTO<int>> ConverterParaListaStringValue<T>()
        {
            var enumList = new List<ChaveDescricaoDTO<int>>();
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                enumList.Add(new ChaveDescricaoDTO<int>
                {
                    Chave = (int)Enum.Parse(typeof(T), name),
                    Descricao = ((Enum)Enum.Parse(typeof(T), name)).StringValue()
                });
            }

            return enumList;
        }

        public static List<T> ConverterParaListaEnum<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static List<string> ConverterParaListaDescription<T>()
        {
            var enumList = new List<string>();
            foreach (var name in Enum.GetNames(typeof(T)))
                enumList.Add(((Enum)Enum.Parse(typeof(T), name)).GetDescription());

            return enumList;
        }

        public static List<string> ConverterParaListaDescription<T>(params T[] enums) where T : Enum
        {
            var enumList = new List<string>();
            foreach (var item in enums)
                enumList.Add(item.GetDescription());

            return enumList;
        }

        public static string ConverterParaEnumDescription<T>(this int valorEnumInt) where T : Enum
        {
            if (Enum.TryParse(typeof(T), valorEnumInt.ToString(), out var valorEnumObj))
                return ((Enum)valorEnumObj).GetDescription();

            return string.Empty;
        }
    }
}
