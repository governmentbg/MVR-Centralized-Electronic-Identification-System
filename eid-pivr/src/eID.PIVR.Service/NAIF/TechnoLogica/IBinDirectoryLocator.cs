namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Defines service for locating the bin folder of the running application
    /// </summary>
    public interface IBinDirectoryLocator
    {
        /// <summary>
        /// Retrieves the bin directory location
        /// </summary>
        /// <returns></returns>
        string GetBinDirectory();
    }
}
