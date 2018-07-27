using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace IdentityServer4.MicroService.Host.Filters
{
    /// <summary>
    /// used for swagger ui
    /// when web api parameter is [FromForm]IFormFile value,can show input file html element in ui page
    /// </summary>
    internal class FormFileOperationFilter : IOperationFilter
    {
        private struct ContainerParameterData
        {
            public readonly ParameterDescriptor Parameter;
            public readonly PropertyInfo Property;

            public string Name => $"{Parameter.Name}.{Property.Name}";

            public ContainerParameterData(ParameterDescriptor parameter, PropertyInfo property)
            {
                Parameter = parameter;
                Property = property;
            }
        }

        private class ParameterByNameComparison : IComparer<IParameter>
        {
            public int Compare(IParameter x, IParameter y) => string.Compare(x.Name, y.Name);
        }

        private static readonly IComparer<IParameter> comparer = new ParameterByNameComparison();

        private static readonly ImmutableArray<string> iFormFilePropertyNames =
            typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(p => p.Name).ToImmutableArray();

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var parameters = operation.Parameters;
            if (parameters == null)
                return;

            var @params = context.ApiDescription.ActionDescriptor.Parameters;
            if (parameters.Count == @params.Count)
                return;

            var formFileParams =
                (from parameter in @params
                 where parameter.ParameterType.IsAssignableFrom(typeof(IFormFile))
                 select parameter).ToArray();

            var iFormFileType = typeof(IFormFile).GetTypeInfo();
            var containerParams =
                @params.Select(p => new KeyValuePair<ParameterDescriptor, PropertyInfo[]>(
                    p, p.ParameterType.GetProperties()))
                .Where(pp => pp.Value.Any(p => iFormFileType.IsAssignableFrom(p.PropertyType)))
                .SelectMany(p => p.Value.Select(pp => new ContainerParameterData(p.Key, pp)))
                .ToImmutableArray();

            if (!(formFileParams.Any() || containerParams.Any()))
                return;

            var consumes = operation.Consumes;
            consumes.Clear();
            consumes.Add("application/form-data");

            if (!containerParams.Any())
            {
                var nonIFormFileProperties =
                    parameters.Where(p =>
                        !(iFormFilePropertyNames.Contains(p.Name)
                       && string.Compare(p.In, "formData", StringComparison.OrdinalIgnoreCase) == 0))
                       .ToImmutableArray();

                parameters.Clear();
                foreach (var parameter in nonIFormFileProperties) parameters.Add(parameter);

                foreach (var parameter in formFileParams)
                {
                    parameters.Add(new NonBodyParameter
                    {
                        Name = parameter.Name,
                        //Required = , // TODO: find a way to determine
                        Type = "file"
                    });
                }
            }
            else
            {
                var paramsToRemove = new List<IParameter>();
                foreach (var parameter in containerParams)
                {
                    var parameterFilter = parameter.Property.Name + ".";
                    paramsToRemove.AddRange(from p in parameters
                                            where p.Name.StartsWith(parameterFilter)
                                            select p);
                }
                paramsToRemove.ForEach(x => parameters.Remove(x));

                foreach (var parameter in containerParams)
                {
                    if (iFormFileType.IsAssignableFrom(parameter.Property.PropertyType))
                        parameters.Add(new NonBodyParameter
                        {
                            Name = parameter.Name,
                            Required = IsRequired(parameter.Property),
                            Type = "file"
                        });
                    else
                    {
                        var indexesOfTopLevelParam = @params
                            .Zip(Enumerable.Range(0, @params.Count), (p, i) => new KeyValuePair<ParameterDescriptor, int>(p, i))
                            .Where(pi => pi.Key.Name == parameter.Property.Name)
                            .Select(pi => pi.Value);
                        int skipIndex;
                        if (indexesOfTopLevelParam.Any())
                            skipIndex = indexesOfTopLevelParam.First();
                        else
                            skipIndex = 0;

                        var filteredParameters = parameters
                            .Where(p => p.Name == parameter.Property.Name
                                     || p.Name.EndsWith("." + parameter.Property.Name))
                            .ToList();
                        filteredParameters.RemoveAt(skipIndex);
                        filteredParameters.First(p => p.Name == parameter.Property.Name)
                                          .Name = parameter.Name;
                    }
                }

            }
            foreach (IParameter param in parameters)
            {
                param.In = "formData";
            }
            (parameters as List<IParameter>)?.Sort(comparer);
        }

        private static bool IsRequired(PropertyInfo propertyInfo)
            => propertyInfo.CustomAttributes
                           .OfType<RequiredAttribute>()
                           .Any();
    }
}
