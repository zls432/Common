
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using BaseSpace;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting.Antlr3.Runtime;



namespace InternelSDK
{
    public class HttpHelper : Singleton<HttpHelper>
    {
        public int timeOut = 30000;
        public Action<string> httpLog;
        public Action<string> httpLogWarning;
        public Action<string> httpLogError;
        public async Task ResumeDownload(string url, string localPath, bool isOverWrite = true, Action callback = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            TurnToHttpsProtoal(url, request);
            request.Method = "GET";
            FileInfo localFile = new FileInfo(localPath);
            long startPosition = 0;
            httpLog?.Invoke("Start");
            if (localFile.Exists)
            {
                // ��������ļ��Ѿ����ڣ����ȡ�Ѿ����ص����ݳ���
                httpLog?.Invoke("Exists");
                if (!isOverWrite)
                {
                    startPosition = localFile.Length;
                    httpLog?.Invoke("startPosition:" + (int)startPosition);
                    request.AddRange((int)startPosition);
                }
                // ����http����ͷ�е�Range���ԣ��Ա������֪����Ҫ������Щ����
            }
            // �������󣬻�ȡ��������Ӧ
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream responseStream = response.GetResponseStream();
            // ��������ļ������ڣ����½�һ���ļ�
            if (!localFile.Exists)
            {
                localFile.Create().Close();
            }
            byte[] buffer = new byte[2048];
            int len;
            len = await responseStream.ReadAsync(buffer, 0, buffer.Length);
            //Debug.Log("length:" + responseStream.Length);
            // ʹ���ļ������������������ݶ�д
            int i = 0;
            using (FileStream localFileStream = localFile.Open(FileMode.OpenOrCreate))
            {
                localFileStream.Seek(startPosition, SeekOrigin.Begin); // ���ļ�ָ��ָ��Ӧ�ÿ�ʼ���ص�λ��
                httpLog?.Invoke("Start");
                while (len != 0)
                {
                    httpLog?.Invoke("write");
                    // д�뱾���ļ�
                    i++;
                    await localFileStream.WriteAsync(buffer, 0, len);
                    localFileStream.Flush();
                    len = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                }
            }
            callback?.Invoke();
        }



        public async Task<bool> ResumeDownload(string url, MemoryStream ms, Action callback = null)
        {
            return await ResumeDownload(url, ms, callback: callback);
        }
        public async Task<bool> ResumeDownload(string url, MemoryStream ms, String token, Action callback = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                TurnToHttpsProtoal(url, request);
                request.Method = "GET";

                long startPosition = 0;
                httpLog?.Invoke("Start");


                // �������󣬻�ȡ��������Ӧ
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                Stream responseStream = response.GetResponseStream();

                byte[] buffer = new byte[2048];
                int len;
                len = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                //Debug.Log("length:" + responseStream.Length);
                // ʹ���ļ������������������ݶ�д
                int i = 0;

                ms.Seek(startPosition, SeekOrigin.Begin); // ���ļ�ָ��ָ��Ӧ�ÿ�ʼ���ص�λ��
                while (len != 0)
                {
                    // д�뱾���ļ�
                    i++;
                    await ms.WriteAsync(buffer, 0, len);
                    ms.Flush();
                    len = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                }
                callback?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                httpLogError?.Invoke(e.ToString());
                return false;
            }
        }

        public async Task<String> GetString(string url)
        {
            return await GetString(url, "");
        }

        public async Task<string> GetString(string url, string token)
        {

            if (string.IsNullOrEmpty(url))
            {
                httpLogWarning?.Invoke("url is null or empty");
                return null;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            TurnToHttpsProtoal(url, request, token);
            foreach (var key in request.Headers.AllKeys)
            {
                Debug.Log($"  {key}: {request.Headers[key]}");
            }

            // �������󣬻�ȡ��������Ӧ
            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                Stream responseStream = response.GetResponseStream();
                Encoding encoding = GetResposeEncoding(response);
                string result;
                using (StreamReader reader = new StreamReader(responseStream, encoding))
                {
                    result = reader.ReadToEnd();
                }

                return result;

            }
            catch (Exception e)
            {
                Debug.LogError("Error Url:" + url + e.ToString());
                httpLogError?.Invoke(e.ToString());
                return null;
            }
        }

        public async Task<Sprite> LoadImage(string url, Vector2Int size)
        {
            if (string.IsNullOrEmpty(url))
            {
                httpLogWarning?.Invoke("url is null or empty");
                return null;
            }
            MemoryStream ms = new MemoryStream();
            await HttpHelper.Instance.ResumeDownload(url,
               ms);
            Debug.Log(ms.Length);
            Texture2D tx = new Texture2D(size.x, size.y);
            tx.LoadImage(ms.ToArray());
            Debug.Log("width:" + tx.width + "   height:" + tx.height);

            Sprite sp = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), Vector2.zero);
            return sp;

        }

        public async Task<bool> CheckFile(string hash, string localPath)
        {
            string code = await FileTool.Instance.ProcessReadAsync(localPath + ".mate");
            if (code.Trim().ToLower() == hash.Trim().ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<HttpWebRequest> GenerateRequest(string url, byte[] jsonData)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            TurnToHttpsProtoal(url, webRequest);
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = jsonData.Length;
            try
            {
                using (Stream reqStream = await webRequest.GetRequestStreamAsync())
                {
                    reqStream.Write(jsonData, 0, jsonData.Length);
                    reqStream.Close();
                }
            }
            catch (Exception e)
            {
                httpLogError?.Invoke(e.ToString());
            }
            return webRequest;
        }

        public async Task<string> PostJsonAsync(string url, byte[] jsonData)
        {
            HttpWebRequest webRequest = await GenerateRequest(url, jsonData);
            return await PostAsync(webRequest);
        }

        public async Task<Stream> PostJsonAsyncS(string url, byte[] jsonData)
        {
            HttpWebRequest webRequest = await GenerateRequest(url, jsonData);
            return await PostAsyncS(webRequest);
        }

        public async Task<string> PostJsonAsyncs(string url, byte[] jsonData)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            TurnToHttpsProtoal(url, webRequest);
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = jsonData.Length;
            try
            {
                using (Stream reqStream = await webRequest.GetRequestStreamAsync())
                {
                    reqStream.Write(jsonData, 0, jsonData.Length);
                    reqStream.Close();
                }
            }
            catch (Exception e)
            {
                httpLogError?.Invoke(e.ToString());
            }
            return await PostAsync(webRequest);
        }


        async Task<HttpWebResponse> GetResponse(HttpWebRequest request)
        {
            string result = "";
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                httpLogError?.Invoke(e.ToString());
                return null;
            }
            return response;



        }
        public async Task<Stream> PostAsyncS(HttpWebRequest request)
        {
            HttpWebResponse response = await GetResponse(request);
            if (response == null)
                return null;
            try
            {
                Stream stream = response.GetResponseStream();

                if (response.StatusCode.ToString().Substring(0, 1) != "2")
                {
                    httpLogWarning?.Invoke("StautCode:" + response.StatusCode);
                }
                return stream;
            }
            catch (Exception ex)
            {
                httpLogError?.Invoke(ex.ToString());
            }
            return null;

        }
        public async Task<string> PostAsync(HttpWebRequest request)
        {
            HttpWebResponse response = await GetResponse(request);
            if (response == null)
                return "";

            Encoding encoding = GetResposeEncoding(response);
            string result = string.Empty;
            try
            {
                Stream stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    result = reader.ReadToEnd();
                }

                if (response.StatusCode.ToString().Substring(0, 1) != "2")
                {
                    httpLogWarning?.Invoke("StautCode:" + response.StatusCode + "   Context:" + result);
                }
            }
            catch (Exception ex)
            {
                httpLogError?.Invoke(ex.ToString());
            }


            return result;
        }


        public Encoding GetResposeEncoding(HttpWebResponse response)
        {
            Encoding encoding = Encoding.UTF8;
            if (string.IsNullOrEmpty(response.ContentEncoding) && string.IsNullOrEmpty(response.CharacterSet))
            {
                string[] contents = response.ContentType.Split(ConstCode.semicolonSeparator, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in contents)
                {
                    if (item.Contains("charset"))
                    {
                        encoding = Encoding.GetEncoding(item.Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(response.ContentEncoding))
            {
                encoding = Encoding.GetEncoding(response.ContentEncoding);
            }
            else
            {
                encoding = Encoding.GetEncoding(response.CharacterSet);
            }
            return encoding;
        }
        private const string PlayStoreUrl = "https://play.google.com/store/apps/details?id={0}";//�ȸ��̵�
        private const string AppStoreUrl = "https://itunes.apple.com/app/apple-store/id{0}";//ƻ��Ӧ���̵�  

        void TurnToHttpsProtoal(string url, HttpWebRequest request, String token = "")
        {
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;
                request.ProtocolVersion = HttpVersion.Version11;
                // ����������Э�����͡�
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                request.KeepAlive = false;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.AllowAutoRedirect = true;
                //request.Accept = "*/*";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            }

            request.Headers.Add("Authorization", token);

            request.ContentType = "application/json";

        }
    }


    //string BuildQuery()
    //{

    //}

    public class ContentType
    {
        public const string Text_Plain = "text/plain";
        public const string Application_Json = "application/json";
        public const string Application_Octet_Stream = "application/octet-stream";
        public const string Www_Form_Urlencoded = "application/x-www-form-urlencoded";
        public const string Www_Form_GB2312 = "application/x-www-form-urlencoded;charset=gb2312";
        public const string Www_Form_Utf8 = "application/x-www-form-urlencoded;charset=utf-8";
        public const string Multipart_Form_Data = "multipart/form-data";
    }
}