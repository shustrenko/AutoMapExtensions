using AutoMapper;
using System.Reflection;

namespace Application.Profiles
{
    internal static class AutoMapExtensions
    {
        public static void CreateNestedMap<TSource, TDestination>(this Profile profile)
        {
            FillMapperConfig(profile, typeof(TSource), typeof(TDestination));
        }

        private static void FillMapperConfig(Profile profile, Type T1, Type T2)
        {
            if (T1 == T2)
            {
                return;
            }

            profile.CreateMap(T1, T2);

            foreach (PropertyInfo propertyInfo in T1.GetProperties()
                .Where(x => !x.PropertyType.IsEnum))
            {
                PropertyInfo correspondingProperty =
                    T2.GetProperties()
                        .FirstOrDefault(p =>
                            p.Name == propertyInfo.Name);

                if (correspondingProperty != null)
                {
                    if (propertyInfo.PropertyType.IsGenericType &&
                        correspondingProperty.PropertyType.IsGenericType)
                    {
                        FillMapperConfig(
                            profile,
                            propertyInfo.PropertyType.GetGenericArguments()[0],
                            correspondingProperty.PropertyType.GetGenericArguments()[0]);
                    }
                    else if (propertyInfo.PropertyType.IsArray)
                    {
                        FillMapperConfig(
                            profile,
                            propertyInfo.PropertyType.GetElementType(),
                            correspondingProperty.PropertyType.GetElementType());
                    }
                    else if (propertyInfo.PropertyType.IsClass &&
                        correspondingProperty.PropertyType.IsClass)
                    {
                        FillMapperConfig(
                            profile,
                            propertyInfo.PropertyType,
                            correspondingProperty.PropertyType);
                    }
                }
            }
        }
    }
}
