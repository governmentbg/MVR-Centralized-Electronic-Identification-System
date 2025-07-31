using eID.PJS.LocalLogsSearch.Service;
using eID.PJS.LocalLogsSearch.Service.Requests;
using Microsoft.AspNetCore.Mvc;

namespace eID.PJS.LocalLogsSearch.API.Controllers
{
    /// <summary>
    /// Audit log search controller
    /// </summary>
    /// <seealso cref="eID.PJS.LocalLogsSearch.API.BaseV1Controller" />
    public class SearchController : BaseV1Controller
    {
        private AuditLogsFileService _reader;
        private ILogger<SearchController> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="reader"></param>
        public SearchController(ILogger<SearchController> logger, AuditLogsFileService reader) : base(logger)
        {
            if (reader == null) throw new ArgumentException(nameof(reader));
            if (logger == null) throw new ArgumentException(nameof(logger));

            _reader = reader;
            _logger = logger;
        }

        /// <summary>
        /// Get the audit log records from the files based on the filter.
        /// You can filter the files by their metadata such as date of creation and System ID
        /// You can filter also the records inside the file based on a Json query
        /// </summary>
        /// <returns>Instance of the SearchLogsResult</returns>
        [HttpPost("logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> SearchLogsAsync([FromBody] SearchLogsRequest request)
        {

            try
            {
                if (request == null)
                    throw new ArgumentException("Request body is empty or invalid.", nameof(request));

                var errors = request.Validate();
                if (!errors.IsValid)
                {
                    return BadRequest(errors);
                }

                var result = _reader.FilterLogs(request);

                if (result.Errors.Any() || !result.ValidationErrors.IsValid)
                    return StatusCode((int)System.Net.HttpStatusCode.MultiStatus, result);
                else
                    return StatusCode((int)System.Net.HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing logs search");
                return BadRequest(ex);
            }

        }

        /// <summary>
        /// Estimate the effort in matter of CPU, RAM and number of records found in audit log files that would be returned based on the
        /// Json query.
        /// You can filter the files by their metadata such as date of creation and System ID
        /// You can filter also the records inside the file based on a Json query
        /// </summary>
        /// <returns>Instance of the QueryEstimationResult</returns>
        [HttpPost("estimate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> EstimateSearchQueryAsync([FromBody] SearchLogsRequest request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentException("Request body is empty or invalid.", nameof(request));

                var errors = request.Validate();
                if (!errors.IsValid)
                {
                    return BadRequest(errors);
                }

                var result = _reader.EstimateQueryScope(request);

                if (result.Errors.Any() || !result.ValidationErrors.IsValid)
                    return StatusCode((int)System.Net.HttpStatusCode.MultiStatus, result);
                else
                    return StatusCode((int)System.Net.HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing logs search estimation");
                return BadRequest(ex);
            }

        }

        /// <summary>
        /// Gets the list of files in a speciffic folders based on the date range filter.
        /// </summary>
        /// <returns></returns>
        [HttpPost("files")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> SearchFilesAsync([FromBody] FilesListRequest request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentException("Request body is empty or invalid.", nameof(request));

                var errors = request.Validate();
                if (!errors.IsValid)
                {
                    return BadRequest(errors);
                }

                var result = _reader.FilterFiles(request);

                if (result.Errors.Any() || !result.ValidationErrors.IsValid)
                    return StatusCode((int)System.Net.HttpStatusCode.MultiStatus, result);
                else
                    return StatusCode((int)System.Net.HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing files search");
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Gets the content of an audit log file.
        /// </summary>
        /// <returns>Instance of the FileContentResult</returns>
        [HttpPost("file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> GetFileAsync([FromBody] FileContentRequest request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentException("Request body is empty or invalid.", nameof(request));

                var errors = request.Validate();
                if (!errors.IsValid)
                {
                    return BadRequest(errors);
                }

                var result = _reader.GetFileContent(request.FilePath);

                if (result.Errors.Any() || !result.ValidationErrors.IsValid)
                    return StatusCode((int)System.Net.HttpStatusCode.MultiStatus, result);
                else
                    return StatusCode((int)System.Net.HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file");
                return BadRequest(ex);
            }
        }
    }
}
