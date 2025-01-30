using System.IO;
using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.ChatUser;

public class CreateMessageAction : IAction<CreateMessageRequest>
{
    private readonly string _uploadDirectory;

    private CreateMessageAction(CreateMessageRequest request)
    {
        var env = (IWebHostEnvironment)AppContext.GetData("WebHostEnvironment");
        if (env == null)
        {
            throw new InvalidOperationException("IWebHostEnvironment is not available.");
        }

        _uploadDirectory = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "uploads");
        Request = request;
    }

    public static IAction<CreateMessageRequest> Make(CreateMessageRequest request)
    {
        return new CreateMessageAction(request);
    }

    public IActionResult Execute()
    {
        using var connection = NpgsqlConnector.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Ensure the upload directory exists
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }

            // Validate Parent Message ID
            if (Request.ParentId != 0)
            {
                var validateParentCommand = new NpgsqlCommand(
                    @"SELECT 1 FROM messages WHERE id = @parentId", 
                    connection
                );
                validateParentCommand.Parameters.AddWithValue("parentId", Request.ParentId);

                var parentExists = validateParentCommand.ExecuteScalar() != null;

                if (!parentExists)
                {
                    return new BadRequestObjectResult(new { Error = "ParentId does not exist." });
                }
            }

            // Insert Message
            var createMessageCommand = new NpgsqlCommand(
                @"
                INSERT INTO messages (body, parentid, chatuserid, createdat)
                VALUES (
                    @body, 
                    @parentId, 
                    (SELECT id FROM chatusers WHERE userid = @userId AND chatid = @chatId), 
                    NOW()
                ) RETURNING id",
                connection
            );

            createMessageCommand.Parameters.AddWithValue("body", Request.Body);
            createMessageCommand.Parameters.AddWithValue("parentId", Request.ParentId == 0 ? DBNull.Value : Request.ParentId);
            createMessageCommand.Parameters.AddWithValue("chatId", Request.ChatId);
            createMessageCommand.Parameters.AddWithValue("userId", Request.GetCurrentUserId());

            var messageId = (long)createMessageCommand.ExecuteScalar();

            // Save Files and Link to Message
            foreach (var file in Request.Files)
            {
                // Generate unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

                // Save the file to disk
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Insert File Metadata into Database
                var createFileCommand = new NpgsqlCommand(
                    @"
                    INSERT INTO files (filename, filepath, filesize, contenttype, createdat)
                    VALUES (@fileName, @filePath, @fileSize, @contentType, NOW())
                    RETURNING id",
                    connection
                );

                createFileCommand.Parameters.AddWithValue("fileName", file.FileName);
                createFileCommand.Parameters.AddWithValue("filePath", filePath);
                createFileCommand.Parameters.AddWithValue("fileSize", file.Length);
                createFileCommand.Parameters.AddWithValue("contentType", file.ContentType ?? (object)DBNull.Value);

                var fileId = (long)createFileCommand.ExecuteScalar();

                // Link File to Message
                var createMessageFileCommand = new NpgsqlCommand(
                    @"
                    INSERT INTO messagefiles (messageid, fileid)
                    VALUES (@messageId, @fileId)",
                    connection
                );

                createMessageFileCommand.Parameters.AddWithValue("messageId", messageId);
                createMessageFileCommand.Parameters.AddWithValue("fileId", fileId);

                createMessageFileCommand.ExecuteNonQuery();
            }

            transaction.Commit();

            return new OkObjectResult(new { MessageId = messageId });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new ObjectResult(new { Error = ex.Message }) { StatusCode = 500 };
        }
    }

    public CreateMessageRequest Request { get; }
}
