#r "System.IO"

using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

public static async Task Run(string inputMsg, TraceWriter log)
{
    // convert the message to dynamic object
    dynamic inputJson = JsonConvert.DeserializeObject(inputMsg);

    // extract the image url
    var imageUrl = inputJson.job_definition.input.image_url.ToString();

    // detect the faces in the image
    var faces = await DetectFaces(imageUrl, log);

    // store the results in DocumentDB
    await ProcessOutput(faces, log);
}

public static async Task<dynamic> DetectFaces(string imageUrl, TraceWriter log)
{
    // NOTE: pulling variables ffrom appsettings.json works when debugging locally. Set in portal for live deployment in Azure.
    FaceServiceClient faceClient = new FaceServiceClient(ConfigurationManager.AppSettings["FACE_API_KEY"]);

    var attributes = new List<FaceAttributeType> { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.FacialHair, FaceAttributeType.Smile, FaceAttributeType.HeadPose, FaceAttributeType.Glasses };
    Face[] faces = null;

    try
    {
        faces = faceClient.DetectAsync(imageUrl, true, true, attributes).Result;
    }
    catch(FaceAPIException e)
    {
        log.Error(e.Message);
        log.Error(e.StackTrace);
    }
    return new { faces = faces };
}

public static async Task ProcessOutput(dynamic inputJson, TraceWriter log)
{
    string endpoint = ConfigurationManager.AppSettings["DOCDB_ENDPOINT_STRING"];
    string key = ConfigurationManager.AppSettings["DOCDB_AUTH_KEY"];
    string link = "dbs/imaginem/colls/codetheater";

    try
    {
        using (var docClient = new DocumentClient(new Uri(endpoint), key))
        {
            var doc = await docClient.CreateDocumentAsync(link, inputJson);
            log.Info($"Document created: { doc.Resource }");
        }
    }
    catch (Exception e)
    {
        log.Error(e.Message);
        log.Error(e.StackTrace);
    }
}