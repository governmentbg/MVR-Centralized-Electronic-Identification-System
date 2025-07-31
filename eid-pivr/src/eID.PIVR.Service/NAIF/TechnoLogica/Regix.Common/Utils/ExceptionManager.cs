using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoLogica.RegiX.Common.Utils
{
    public static class ExceptionManager
    {
        //public static Exception DecodeException(Exception e)
        //{
        //    Exception result = new Exception();

        //    if (e is UpdateException)
        //    {
        //        DataBaseException dbException = null;

        //        if (e.InnerException != null &&
        //            e.InnerException is SqlException)
        //        {
        //            switch ((e.InnerException as SqlException).Number)
        //            {
        //                case 515:// Not null exception
        //                    dbException = new DataBaseException(Old.StringResources.ExceptionCodes.DB_515, "515");
        //                    break;
        //                case 547:// Foreign key exception
        //                    dbException = GetDBExceptionForFkConstraint(e.InnerException.Message);
        //                    break;
        //                case 2627:// Unique and Primary key exception
        //                    dbException = GetDBExceptionForUniqueOrPkConstraint(e.InnerException.Message);
        //                    break;
        //                default:
        //                    dbException = new DataBaseException(Old.StringResources.ExceptionCodes.DB_0, "0");
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            dbException = new DataBaseException("Generic DB error", "0");
        //        }

        //        dbException.AdditionalInfo = ExceptionManager.GetExceptionFullStackTrace(e);
        //        result = new FaultException<DataBaseException>(dbException);
        //    }
        //    else
        //    {
        //        SqlException sqlException = e as SqlException;
        //        if (sqlException != null && sqlException.Number == 50000)
        //        {
        //            //var rm = new System.Resources.ResourceManager(typeof(TechnoLogica.RegiX.Common.StringResources.ExceptionCodes));
        //            //string errorMessage = rm.GetString(sqlException.Message);
        //            //if (string.IsNullOrEmpty(errorMessage))
        //            //{
        //            //    result = new Exception("Unknown");
        //            //}
        //            //else
        //            //{
        //            //    BusinessLogicException businessException =
        //            //        new BusinessLogicException(errorMessage, "DB_50000");
        //            //    result = new FaultException<BusinessLogicException>(businessException);
        //            //}
        //        }
        //        else
        //        {
        //            result = new Exception("Unknown");//CreteGenericBusinessLogicException(e);
        //            //LogException(e);
        //        }
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// Parse the exception message to extract the name of the constraint that caused the exception
        ///// and then construct the correct DataBaseException object.
        ///// </summary>
        ///// <param name="exceptionMessage"></param>
        ///// <returns></returns>
        //private static DataBaseException GetDBExceptionForUniqueOrPkConstraint(string exceptionMessage)
        //{
        //    DataBaseException dbException = null;

        //    string doubleQuoteSign = "\"";
        //    string singleQuoteSign = "'";
        //    string reString = "(?<=(UNIQUE|PRIMARY) KEY constraint \\" + doubleQuoteSign + "|\\" + singleQuoteSign + ").+?(?=\\" + doubleQuoteSign + "|\\" + singleQuoteSign + ")";
        //    Regex re = new Regex(reString);
        //    Match exMessageMatch = re.Match(exceptionMessage);
        //    if (exMessageMatch.Success)
        //    {
        //        //string resourceString = StringResources.DbConstraintErrors.ResourceManager.GetString(exMessageMatch.Groups[0].Value);
        //        //if (!String.IsNullOrEmpty(resourceString))
        //        //{
        //        //    dbException = new DataBaseException(resourceString);
        //        //}
        //        //else
        //        //{
        //        //fallback scenario
        //        dbException = new DataBaseException(Old.StringResources.ExceptionCodes.DB_2627, "2627");
        //        //}
        //    }
        //    else
        //    {
        //        //fallback scenario
        //        dbException = new DataBaseException(Old.StringResources.ExceptionCodes.DB_2627, "2627");
        //    }

        //    return dbException;
        //}

        ///// <summary>
        ///// Parse the exception message to extract the name of the constraint that caused the exception
        ///// and then construct the correct DataBaseException object. Two cases for FOREIGN KEY violation:
        ///// 1. Non-existant parent
        ///// 2. Attempt to delete parent before child
        ///// </summary>
        ///// <param name="exceptionMessage"></param>
        ///// <returns></returns>
        //private static DataBaseException GetDBExceptionForFkConstraint(string exceptionMessage)
        //{
        //    DataBaseException dbException = null;

        //    string doubleQuoteSign = "\"";
        //    string singleQuoteSign = "'";
        //    string regexFkString = "(?<=FOREIGN KEY constraint (\\" + doubleQuoteSign + "|\\" + singleQuoteSign + ")).+?(?=\\" + doubleQuoteSign + "|\\" + singleQuoteSign + ")";
        //    string regexRefString = "(?<=REFERENCE constraint (\\" + doubleQuoteSign + "|\\" + singleQuoteSign + ")).+?(?=\\" + doubleQuoteSign + "|\\" + singleQuoteSign + ")";

        //    Regex fkRegex = new Regex(regexFkString);
        //    Regex refRegex = new Regex(regexRefString);

        //    Match fkMessageMatch = fkRegex.Match(exceptionMessage);
        //    Match refMessageMatch = refRegex.Match(exceptionMessage);

        //    if (fkMessageMatch.Success)
        //    {
        //        //string resourceString = StringResources.DbConstraintErrors.ResourceManager.GetString(fkMessageMatch.Groups[0].Value);
        //        //if (!String.IsNullOrEmpty(resourceString))
        //        //{
        //        //    dbException = new DataBaseException(resourceString);
        //        //}
        //        //else
        //        //{
        //        //fallback scenario
        //        dbException = new DataBaseException(Old.StringResources.ExceptionCodes.DB_547, "547");
        //        //}
        //    }
        //    else if (refMessageMatch.Success)
        //    {
        //        //string resourceString =
        //        //    StringResources.DbConstraintErrors.ResourceManager.GetString(refMessageMatch.Groups[0].Value + "Ref");
        //        //if (!String.IsNullOrEmpty(resourceString))
        //        //{
        //        //    dbException = new DataBaseException(resourceString);
        //        //}
        //        //else
        //        //{
        //        //fallback scenario
        //        dbException = new DataBaseException(Old.StringResources.ExceptionCodes.DB_547, "547");
        //        //}
        //    }
        //    else
        //    {
        //        //fallback scenario
        //        dbException = new DataBaseException(Old.StringResources.ExceptionCodes.DB_547, "547");
        //    }

        //    return dbException;
        //}

        public static string GetExceptionFullStackTrace(Exception e)
        {
            string stackTrace = string.Format("==========================={0}{1}{0}{2}{0}{3}{0}{4}{0}",
                Environment.NewLine,
                e.GetType().ToString(),
                e.Source,
                e.Message,
                e.StackTrace);
            if (e.InnerException != null)
            {
                stackTrace = stackTrace + GetExceptionFullStackTrace(e.InnerException);
            }
            return stackTrace;
        }

        /// <summary>
        /// Извлича рекурсивно Съобщението на грешката и StackTrace
        /// </summary>
        /// <param name="ex">Грешка</param>
        /// <param name="adapterResultMessage">Съобщението на най-вътрешната грешка, подходящо за връщане в Response-a</param>
        /// <param name="message">Всички съобщения(наконкатенирани), подходящи за логване</param>
        /// <param name="stackTrace">Всички StackTrace-и(наконкатенирани)</param>
        public static void ExtractMessageAndStackTrace(Exception ex, out string adapterResultMessage, out string message)
        {
            adapterResultMessage = string.Empty;
            message = string.Empty;
            if (ex != null)
            {
                message = ex.Message;
                adapterResultMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    string innerMessage;
                    ExtractMessageAndStackTrace(ex.InnerException, out adapterResultMessage, out innerMessage);
                    message = message + string.Format("{0}==========================={0}{1}",
                            Environment.NewLine,
                            innerMessage);
                }
            }
        }

        //public static string GetEntryPointExceptionArgumentExceptionNameByCode(string code)
        //{
        //    switch (code)
        //    {
        //        case "Operation":
        //            return Old.StringResources.ExceptionCodes.InvalidArgumentOperation;
        //        case "Argument":
        //            return Old.StringResources.ExceptionCodes.InvalidArgumentRequestArgument;
        //        case "ServiceCallID":
        //            return Old.StringResources.ExceptionCodes.InvalidServiceCallID;
        //        default:
        //            return code;
        //    }

        //}
    }
}
