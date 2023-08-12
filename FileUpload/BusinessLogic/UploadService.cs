
using FileUpload.CustomException;
using FileUpload.Enum;
using FileUpload.Interface.BusinessLogic;
using FileUpload.Interface.Services;
using FileUpload.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace FileUpload.BusinessLogic
{
    public class UploadService : IUploadService
    {
        private readonly ILoggerService logger;

        public UploadService(ILoggerService logger)
        {
            this.logger = logger;
        }
        public async Task<ValidationResultModel> Upload(IFormFile file)
        {
            
            try
            {              
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    byte[] fileData = stream.ToArray();

                    FileType fileType = DetermineFileType(fileData);

                    var validationResult = ValidateAndProcessFile(fileType, fileData);

                    LogAuditEvent(fileType, validationResult);

                    return validationResult;
                }
            }
            catch(InvalidFileTypeException ex)
            {
                var validationResult = new ValidationResultModel
                {
                    Status = ValidationStatus.Failed,
                    ErrorMessage = ex.Message
                };
                LogAuditEvent(ex.FileType, validationResult);
                return validationResult;
            }
            catch(Exception ex)
            {
                var validationResult = new ValidationResultModel
                {
                    Status = ValidationStatus.Failed,
                    ErrorMessage = ex.Message
                };
                logger.LogException(ex, $"Status Code is {validationResult.Status} and Error Message is {validationResult.ErrorMessage}");
                return validationResult;
            }
           
        }
        private FileType DetermineFileType(byte[] fileData)
        {
            string fileContent = Encoding.UTF8.GetString(fileData);

            if (fileContent.Contains("Vacation Balance"))
            {
                return FileType.VacationBalance;
            }
            else if(fileContent.Contains("Leave Application"))
            {
                return FileType.LeaveApplication;
            }
            else
            {
                throw new InvalidFileTypeException(FileType.WrongFile, "Uploaded wrong File!");               
            }

        }

        private ValidationResultModel ValidateAndProcessFile(FileType fileType, byte[] fileData)
        {
            try
            {
                string fileContent = Encoding.UTF8.GetString(fileData);
                string[] lines = fileContent.Split('\n');

                if (fileType == FileType.VacationBalance)
                {
                    var validationErrors = new List<string>();
                    var existingVacationBalances = new HashSet<string>();

                    for (int i = 2; i < lines.Length-1; i++)
                    {
                        string line = lines[i];
                        string[] fields = line.Split(',');

                        if (fields.Length >= 3)
                        {
                            string employeeNumber = fields[0].Trim();
                            string monthYear = fields[1].Trim();
                            int balance;
                            string primaryKey = $"{employeeNumber}-{monthYear}";
                            if (existingVacationBalances.Contains(primaryKey))
                            {
                                validationErrors.Add($"Duplicate primary key ,{primaryKey}, on line {i + 1}");
                                continue;
                            }

                            existingVacationBalances.Add(primaryKey);

                            if (!int.TryParse(fields[2], out balance))
                            {
                                validationErrors.Add($"Invalid balance for primary key,{primaryKey}, on line {i + 1}");
                                continue;
                            }

                            if (employeeNumber.Length != 5 || !IsValidMonthYear(monthYear))
                            {
                                validationErrors.Add($"Invalid data for primary key,{primaryKey}, on line {i + 1}");
                                continue;
                            }

                        }
                        else
                        {
                            validationErrors.Add($"Insufficient data on line {i + 1}");
                        }
                    }

                    if (validationErrors.Any())
                    {
                        return new ValidationResultModel
                        {
                            StatusDesc = "Validation Failure",
                            Status = ValidationStatus.Failed,
                            ErrorMessage = string.Join(Environment.NewLine, validationErrors),
                        };
                    }
                }
                else if (fileType == FileType.LeaveApplication)
                {
                    var validationErrors = new List<string>();
                    var existingLeaveApplications = new HashSet<string>();
                    for (int i = 2; i < lines.Length-1; i++)
                    {
                        string line = lines[i];
                        string[] fields = line.Split(',');

                        if (fields.Length >= 5)
                        {
                            string employeeNumber = fields[0].Trim();
                            DateTime startDate, endDate;
                            string reason = fields[3].Trim();
                            string comments = fields[4].Trim();

                            if (!DateTime.TryParse(fields[1], out startDate) || !DateTime.TryParse(fields[2], out endDate))
                            {
                                validationErrors.Add($"Invalid date format on line {i + 1}");
                                continue;
                            }
                            string primaryKey = $"{employeeNumber}-{startDate}-{endDate}";
                            if (existingLeaveApplications.Contains(primaryKey))
                            {
                                validationErrors.Add($"Duplicate primary key,{primaryKey}, on line {i + 1}");
                                continue;
                            }
                            existingLeaveApplications.Add(primaryKey);
                            if (employeeNumber.Length != 5 || startDate > endDate || string.IsNullOrWhiteSpace(reason))
                            {
                                validationErrors.Add($"Invalid data for primary key,{primaryKey}, on line {i + 1}");
                                continue;
                            }

                        }
                        else
                        {
                            validationErrors.Add($"Insufficient data on line {i + 1}");
                        }
                    }

                    if (validationErrors.Any())
                    {

                        return new ValidationResultModel
                        {
                            StatusDesc = "Validation Failure",
                            Status = ValidationStatus.Failed,
                            ErrorMessage = string.Join(Environment.NewLine, validationErrors)
                        };
                    }
                }

                return new ValidationResultModel
                {
                    StatusDesc = "Validation Successful",
                    Status = ValidationStatus.Success,
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new ValidationResultModel
                {
                    StatusDesc = "Something went wrong",
                    Status = ValidationStatus.Failed,
                    ErrorMessage = ex.Message
                };
            }
        }

        private bool IsValidMonthYear(string monthYear)
        {
            DateTime parsedDate;
            return DateTime.TryParseExact(monthYear, "MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }


        private void LogAuditEvent(FileType fileType, ValidationResultModel validationResult)
        {
            try
            {
                var auditLog = new AuditLogModel
                {
                    Timestamp = DateTime.UtcNow,
                    FileUploadType = fileType,
                    Status = validationResult.Status,
                    ErrorMessage = validationResult.ErrorMessage
                };
                logger.LogTrace($"Audit event: FileUploadType={auditLog.FileUploadType}, Status={auditLog.Status}, ErrorMessage={auditLog.ErrorMessage ?? "No Error"}");

            }
            catch (Exception ex)
            {
                logger.LogTrace(ex.Message);
            }
           
        }

    }
}
