using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using TechnoLogica.RegiX.Adapters.Common.DataContracts;
using TechnoLogica.RegiX.Adapters.Common.Interfaces;
using TechnoLogica.RegiX.Common.DataContracts.Parameter;

namespace TechnoLogica.RegiX.Adapters.Common
{
    public abstract class BaseParameterStore : IParameterStore
    {
        [Import(typeof(CompositionContainer))]
        protected CompositionContainer CompositionContainer { get; set; }

        public virtual Parameters GetParameters(BaseAdapterService adapterService)
        {
            var exportedParameters = GetExportedParameter(adapterService);
            var result =
                new Parameters()
                {
                    ParameterList =
                        new List<ParameterInfo>(
                            exportedParameters
                            .Select(
                                pi =>
                                    new ParameterInfo()
                                    {
                                        Description = pi.Description,
                                        Key = pi.Key,
                                        CurrentValue = pi.CurrentValue
                                    }
                            )
                        )
                };
            return result;
        }

        public virtual T GetParameterValue<T>(ParameterInfo<T> parameterInfo)
        {
            string key = $"{parameterInfo.OwnerAssembly.GetName().Name}:{parameterInfo.Key}";
            string value = GetParameterValue(key);
            if (value != null)
            {
                return StringToValue<T>(value);
            }
            else if (parameterInfo.Defaultvalue != null)
            {
                return parameterInfo.Defaultvalue;
            }
            else
            {
                throw new ApplicationException($"Parameter with key: '{key}' doesn't have value");
            }
        }

        protected abstract string GetParameterValue(string key);

        protected abstract void SetParameterValue(string key, string value);

        public virtual void SetParameter(BaseAdapterService adapterService, string key, string value)
        {
            ParameterInfo foundParameter = GetExportedParameter(adapterService).Where(ai => ai.Key == key).FirstOrDefault();
            if (foundParameter != null)
            {
                foundParameter.SetValue(value);
            }
            else
            {
                throw new ApplicationException($"There is no parameter with key {key}"); //"This is not a valid parameter.");
            }
        }

        public virtual void SetParameters(BaseAdapterService adapterService, List<ParameterInfo> parameters)
        {
            foreach (var paramInfo in parameters)
            {
                this.SetParameter(adapterService, paramInfo.Key, paramInfo.CurrentValue);
            }
        }

        public virtual void SetParameterValue<T>(ParameterInfo<T> parameterInfo, T value)
        {
            string key = $"{parameterInfo.OwnerAssembly.GetName().Name}:{parameterInfo.Key}";
            string stringValue = value.ToString();
            this.SetParameterValue(key, stringValue);
        }

        protected virtual List<ParameterInfo> GetExportedParameter(BaseAdapterService adapterService)
        {
            List<ParameterInfo> parameterInfos = new List<ParameterInfo>();
            Type currentType = adapterService.GetType();
            while (currentType != typeof(BaseAdapterService))
            {
                AddExportedParameters(parameterInfos, currentType);
                currentType = currentType.BaseType;
            }
            AddExportedParameters(parameterInfos, currentType);
            return parameterInfos;
        }

        protected virtual void AddExportedParameters(List<ParameterInfo> parameterInfos, Type currentType)
        {
            parameterInfos.AddRange(CompositionContainer.GetExportedValues<ParameterInfo>(currentType.FullName));
        }

        /// <summary>
        /// Конвертира String-овото представяне на параметър към <typeparamref name="T"/>
        /// </summary>
        /// <param name="stringValue">String-ово представяне на параметър</param>
        /// <returns>Стойността на параметъра като <typeparamref name="T"/></returns>
        protected virtual T StringToValue<T>(string stringValue)
        {
            return (T)(stringValue as object);
        }
    }
}
