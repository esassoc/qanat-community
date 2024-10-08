using Qanat.Common;

namespace Qanat.GDALAPI.Services
{
    public class GDALRunnerService : IRun
    {
        private readonly ILogger<GDALRunnerService> _logger;

        public GDALRunnerService(ILogger<GDALRunnerService> logger)
        {
            _logger = logger;
        }

        public ProcessUtilityResult Ogr2Ogr(List<string> arguments)
        {
            const string exeFileName = "ogr2ogr";
            return Run(arguments, exeFileName);
        }

        public ProcessUtilityResult OgrInfo(List<string> arguments)
        {
            const string exeFileName = "ogrinfo";
            return Run(arguments, exeFileName);
        }

        public ProcessUtilityResult GDALWarp(List<string> arguments)
        {
            const string exeFileName = "gdalwarp";
            return Run(arguments, exeFileName);
        }

        public ProcessUtilityResult GDALInfo(List<string> arguments)
        {
            const string exeFileName = "gdalinfo";
            return Run(arguments, exeFileName);
        }

        public ProcessUtilityResult GDALSrsInfo(List<string> arguments)
        {
            const string exeFileName = "gdalsrsinfo";
            return Run(arguments, exeFileName);
        }

        private ProcessUtilityResult Run(List<string> arguments, string exeFileName)
        {
            var processUtilityResult = ProcessUtility.ShellAndWaitImpl(null, exeFileName, arguments, true, 250000000, _logger);
            if (processUtilityResult.ReturnCode != 0)
            {
                var argumentsAsString =
                    string.Join(" ", arguments.Select(ProcessUtility.EncodeArgumentForCommandLine).ToList());
                var fullProcessAndArguments =
                    $"{ProcessUtility.EncodeArgumentForCommandLine(exeFileName)} {argumentsAsString}";
                var errorMessage =
                    $"Process \"{exeFileName}\" returned with exit code {processUtilityResult.ReturnCode}, expected exit code 0.\r\n\r\nStdErr and StdOut:\r\n{processUtilityResult.StdOutAndStdErr}\r\n\r\nProcess Command Line:\r\n{fullProcessAndArguments}";
                throw new ApplicationException(errorMessage);
            }

            return processUtilityResult;
        }

        private void LogOutput(string output, bool isError)
        {
            if (string.IsNullOrWhiteSpace(output)) return;

            if (isError)
            {
                _logger.LogError(output);
            }
            else
            {
                _logger.LogInformation(output);
            }
        }
    }
}
