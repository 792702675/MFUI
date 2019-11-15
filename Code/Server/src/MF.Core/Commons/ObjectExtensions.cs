using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Extensions;
using Abp.ObjectMapping;

namespace MF
{
    public static class ObjectExtensions
    {

        public static TDestination MapTo<TDestination>(this object source)
        {
            var mapper = IocManager.Instance.Resolve<IObjectMapper>();
            return mapper.Map<TDestination>(source);
        }
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            var mapper = IocManager.Instance.Resolve<IObjectMapper>();
            return mapper.Map(source, destination);
        }
    }
}
