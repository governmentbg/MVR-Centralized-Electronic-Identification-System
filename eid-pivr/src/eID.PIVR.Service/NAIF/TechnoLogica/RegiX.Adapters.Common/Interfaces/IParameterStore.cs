using System;
using System.Collections.Generic;
using System.Text;
using TechnoLogica.RegiX.Adapters.Common.DataContracts;
using TechnoLogica.RegiX.Common.DataContracts.Parameter;

namespace TechnoLogica.RegiX.Adapters.Common.Interfaces
{
    /// <summary>
    /// Defines functionality for storing/reading parameters
    /// </summary>
    public interface IParameterStore
    {
        /// <summary>
        /// Set parameter values
        /// </summary>
        /// <param name="adapterService">AdapterService instance</param>
        /// <param name="parameters">List of parameters to set</param>
        void SetParameters(BaseAdapterService adapterService, List<ParameterInfo> parameters);     

        /// <summary>
        /// Set parameter value
        /// </summary>
        /// <param name="adapterService">AdapterService instance</param>
        /// <param name="key">Key of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        void SetParameter(BaseAdapterService adapterService, string key, string value);    

        /// <summary>
        /// Retrieve parmater values
        /// </summary>
        /// <param name="adapterService">AdapterService instance</param>
        /// <returns>Retireved parameters</returns>
        Parameters GetParameters(BaseAdapterService adapterService);

        /// <summary>
        /// Retrieve parameter value
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="parameterInfo">Paramter info instance</param>
        /// <returns>Retrieved parameter's value</returns>
        T GetParameterValue<T>(ParameterInfo<T> parameterInfo);

        /// <summary>
        /// Set parameter value
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="parameterInfo">Parameter info instance</param>
        /// <param name="value">Value of the parameter</param>
        void SetParameterValue<T>(ParameterInfo<T> parameterInfo, T value);
    }
}
