using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BaseSpace;

namespace InternelSDK
{
    public class FileTool : Singleton<FileTool>
    {


        public async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true);

            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

        public async Task<string> ProcessReadAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath) != false)
                {
                    string text = await ReadTextAsync(filePath);
                    return text;
                }
                else
                {
                    Debug.LogError($"file not found: {filePath}");
                    return "";
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }


        public bool CheckLoaclFileState(string filePath, string url, asyncState asyncState)
        {
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                ResponseState responseState = new ResponseState();
                responseState.request = request;
                HttpWebResponse response = (HttpWebResponse)request.BeginGetResponse((r) => {
                    ResponseState responseState = r.AsyncState as ResponseState;

                    responseState.response = (HttpWebResponse)responseState.request.EndGetResponse(r);
                    if (responseState.response.ContentLength == fileInfo.Length)
                    {
                        Debug.Log("ContentLength: " + responseState.response.ContentLength + "    fileInfo.Length: " + fileInfo.Length);
                        asyncState.state = true;
                    }
                    asyncState.isDone = true;
                }, responseState);
            }
            return false;
        }

        public bool CheckLoaclFileStateSync(string filePath, string url)
        {
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                ResponseState responseState = new ResponseState();
                responseState.request = request;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                responseState.response = response;
                Debug.Log("ContentLength: " + responseState.response.ContentLength + "    fileInfo.Length: " + fileInfo.Length);
                if (responseState.response.ContentLength == fileInfo.Length)
                {
                 
                    return true;
                }
            }
            return false;
        }

        async Task<string> ReadTextAsync(string filePath)
        {
            using var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true);

            var sb = new StringBuilder();

            byte[] buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }
    }
}

public class asyncState
{
    public bool isDone;
    public bool state;
}
class ResponseState
{
    public HttpWebRequest request;
    public HttpWebResponse response;
}