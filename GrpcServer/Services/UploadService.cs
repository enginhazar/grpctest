using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Google.Protobuf;

namespace GrpcServer
{
    public class UploadService : Upload.UploadBase
    {
        private readonly ILogger<UploadService> _logger;

        public UploadService(ILogger<UploadService> logger)
        {
            _logger = logger;
        }

        public override async Task<uploadFileResponse> UploadFile(IAsyncStreamReader<uploadFileRequest> requestStream,
            ServerCallContext context)
        {



            string filePath = $"D:\\a1.jpg";
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
              
                while (await requestStream.MoveNext())
                {
                    var uploadFileRequest = requestStream.Current;
                   // byte[] bb = uploadFileRequest.ChunkData.ToByteArray();
                    fs.Write(uploadFileRequest.ChunkData.ToByteArray());
                }
            }
      
            return new uploadFileResponse() { FileId = "22", FileSize = 22 };
        }

        public override async Task UploadFileStream(IAsyncStreamReader<uploadFileRequest> requestStream,
            IServerStreamWriter<uploadFileResponse> responseStream, ServerCallContext context)
        {
            string filePath = $"D:\\a2.png";
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {

                while (await requestStream.MoveNext())
                {
                    var uploadFileRequest = requestStream.Current;
                    
                    fs.Write(uploadFileRequest.ChunkData.ToByteArray());
                    _logger.LogInformation($"Upload Size={(uint)uploadFileRequest.ChunkData.Length}");

                    uploadFileResponse uploadFileResponseret = new uploadFileResponse();
                    uploadFileResponseret.FileSize = (uint)uploadFileRequest.ChunkData.Length;
                    await responseStream.WriteAsync(uploadFileResponseret);

                }
            }

           
        }

        public override Task<uploadFileResponse> SaveFile(uploadFileRequest request, ServerCallContext context)
        {


            Stream imageStream = new MemoryStream(request.ChunkData.ToByteArray());

            string filePath = $"D:\\aa.jpg";
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(request.ChunkData.ToByteArray());
            }
            return Task.FromResult(new uploadFileResponse() { FileId = filePath });
           
        }
    }
}
