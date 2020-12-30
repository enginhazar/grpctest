using Grpc.Net.Client;
using System;
using GrpcServer;
using System.Threading.Tasks;
using System.IO;
using Google.Protobuf;
using System.Text;
using System.Threading;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var channel = GrpcChannel.ForAddress("https://localhost:5001");
            //var client = new Greeter.GreeterClient(channel);

            //var reply = await client.SayHelloAsync(new HelloRequest() { Name = "engin" });


            //Console.WriteLine(reply.Message);
            //Console.ReadLine();

            //var channnel = GrpcChannel.ForAddress("https://localhost:5001");
            //var customerClient = new Customer.CustomerClient(channnel);

            //var customerModel=await customerClient.GetCustomerInfoAsync(new CustomerLookupModelRequest() { UserID = 1 });

            //Console.WriteLine($" Customer {customerModel.FirstName} {customerModel.LastName}");

            //using (var call = customerClient.GetNewCustomers(new NewCustomerRequest()))
            //{
            //    while (await call.        .MoveNext())
            //    {
            //        var currentCustomer = call.ResponseStream.Current;
            //        Console.WriteLine($" Customer Stream {currentCustomer.FirstName} ");
            //    }
            //}

           
            var channnel = GrpcChannel.ForAddress("https://localhost:5001");

            var uploadClient = new Upload.UploadClient(channnel);




            //  string fileName = @"C:\Users\e.hazar\Pictures\mobilv3.png";
            string fileName = @"D:\data\aa.png";

            #region Save
            // byte[] bytes = File.ReadAllBytes(fileName);


            // ByteString bs = ByteString.CopyFrom(bytes);
            // uploadFileRequest uploadData = new uploadFileRequest()
            // {
            //     ChunkData = bs,
            //     Info = new ImageInfo() { FileName = fileName, ImageType = ".jpg" }
            // };
            // uploadData.ChunkData = bs;

            //await uploadClient.SaveFileAsync(uploadData);
            #endregion

            #region UploadFile
            //using var uploadFile = uploadClient.UploadFile();
            //using (FileStream fs = File.OpenRead(fileName))
            //{

            //    byte[] b = new byte[1024];
            //    UTF8Encoding temp = new UTF8Encoding(true);


            //    while (fs.Read(b, 0, b.Length) > 0)
            //    {



            //        uploadFileRequest uploadFileReq = new uploadFileRequest()
            //        {
            //            Info = new ImageInfo() { FileName = fileName, ImageType = ".jpg" }
            //        };

            //        uploadFileReq.ChunkData = ByteString.CopyFrom(b);

            //         await uploadFile.RequestStream.WriteAsync(uploadFileReq);
            //        Console.WriteLine(temp.GetString(b));
            //    }
            //}
            #endregion

            #region uploadFileStream
            using var uploadFileStream = uploadClient.UploadFileStream();

            using (FileStream fs = File.OpenRead(fileName))
            {

                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                int uploadedBytes = 0;
                while (fs.Read(b, 0, b.Length) > 0)
                {

                    Console.SetCursorPosition(10, 10);

                    var responseTask = Task.Run(async () =>
                    {
                        while (await uploadFileStream.ResponseStream.MoveNext(CancellationToken.None))
                        {
                            uploadedBytes += (int)uploadFileStream.ResponseStream.Current.FileSize;
                           // Console.Clear();
                            Console.WriteLine($"{fileName} - Upload File bytes {fs.Length}/{uploadedBytes}");
                        }
                    });
                   


                    uploadFileRequest uploadFileReq = new uploadFileRequest()
                    {
                        Info = new ImageInfo() { FileName = fileName, ImageType = ".png" }
                    };

                    uploadFileReq.ChunkData = ByteString.CopyFrom(b);

                    await uploadFileStream.RequestStream.WriteAsync(uploadFileReq);
                  
                }
            }
#endregion

            Console.ReadLine();

        }
    }
}
