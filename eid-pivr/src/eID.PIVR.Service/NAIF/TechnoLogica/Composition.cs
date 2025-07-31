using System.ComponentModel.Composition.Hosting;

namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Contains an instance of the composition container
    /// </summary>
    public static class Composition
    {
        /// <summary>
        /// Instance of the composition container
        /// </summary>
        public static CompositionContainer CompositionContainer { get; set; }
    }
}
