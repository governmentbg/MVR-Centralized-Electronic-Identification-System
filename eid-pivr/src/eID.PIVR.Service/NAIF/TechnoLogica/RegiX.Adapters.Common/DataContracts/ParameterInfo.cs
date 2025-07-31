using System;
using System.ComponentModel;
using TechnoLogica.RegiX.Adapters.Common.Interfaces;
using TechnoLogica.RegiX.Common;

namespace TechnoLogica.RegiX.Adapters.Common.DataContracts
{
    /// <summary>
    /// Parameter of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Type of the parameter</typeparam>
    [Serializable]
    public class ParameterInfo<T> : RegiX.Common.DataContracts.Parameter.ParameterInfo
    {
        /// <summary>
        /// Parameter store
        /// </summary>
        protected IParameterStore ParameterStore
        {
            get
            {
                return Composition.CompositionContainer.GetExportedValue<IParameterStore>();
            }
        }

        /// <summary>
        /// Default value field
        /// </summary>
        private T _defaultValue;

        /// <summary>
        /// Default value
        /// </summary>
        public T Defaultvalue
        {
            get
            {
                return _defaultValue;
            }
        }

        /// <summary>
        /// Value field
        /// </summary>
        private T _value;

        /// <summary>
        /// Constructor
        /// </summary>
        public ParameterInfo()
        {
        }

        /// <summary>
        /// Constructor with default value
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        public ParameterInfo(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Set parameter's value
        /// </summary>
        /// <param name="value">Value to set</param>
        public override void SetValue(string value)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            object propValue = typeConverter.ConvertFromString(value);
            Value = (T)propValue;
        }

        /// <summary>
        /// Current value of the parameter
        /// </summary>
        public override string CurrentValue
        {
            get
            {
                return Value.ToString();
            }
            set
            {
                base.CurrentValue = value;
            }
        }

        /// <summary>
        /// Typed value of the parameter
        /// </summary>
        public T Value
        {
            get
            {
                if (_value == null)
                {
                    _value = ParameterStore.GetParameterValue(this);
                }
                return _value;
            }
            private set
            {
                _value = value;
                ParameterStore.SetParameterValue(this, value);
            }
        }
    }    
}
