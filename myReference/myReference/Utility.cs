using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Linq.Expressions;
using System.Reflection;
using Cometd.Bayeux.Client;
using Cometd.Client;
using Cometd.Client.Transport;

namespace myReference
{
    public sealed class Utility
    {
        /// <summary>
        /// Get value from Application Configuration
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AppSetting(string key) 
        {
            
            try
            {
                string value = ConfigurationManager.AppSettings["key"];
                if (value != null)
                    return value;
            }
            catch {}
            return "";
        }

        /// <summary>
        /// Get Number From String Value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>int</returns>
        public static int GetNumberFromString(string str)
        {
            int number = 0; 
            
            try
            {
                int.TryParse(Regex.Replace(str,"[^\\d]",""),out number);

            }
            catch {}
            return number;
        }

        /// <summary>
        /// Get Number from String Value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string GetNumberStrFromString(string str)
        {
            try
            {
                return Regex.Replace(str, "[^\\d]", "");
            }
            catch{}
            return "";
        }

        /// <summary>
        /// Convert Null to Empty String.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string NullToEmpty(string str)
        {
            try
            {
                if (str == null)
                    return "";
            }
            catch{}
            return str;
        }
        /// <summary>
        /// Write the Message Log File
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="LogFilePath"></param>
        public static void writeToLogFile(string logMessage, string LogFilePath) 
        {
            try
            {
                if (!string.IsNullOrEmpty(LogFilePath))
                {
                    FileInfo fi = new FileInfo(LogFilePath);
                    if (!Directory.Exists(fi.DirectoryName)) return;

                    string strLogMesssage = string.Empty;
                    StreamWriter swLog;

                    strLogMesssage = string.Format("{0}: {1}", DateTime.Now, logMessage);

                    if (!File.Exists(LogFilePath))
                        swLog = new StreamWriter(LogFilePath);
                    else
                        swLog = File.AppendText(LogFilePath);

                    swLog.WriteLine(strLogMesssage);
                    swLog.WriteLine();
                    swLog.Flush();
                    swLog.Dispose();
                    swLog.Close();
                }

            }
            catch{}
        
        }

        /// <summary>
        /// Convert Minute to Milleseconds
        /// </summary>
        /// <param name="Minute"></param>
        /// <returns></returns>
        public static int MinuteToMilleseconds(int Minute)
        {

            int Milleseconds;

            Milleseconds = (Minute * 60) * 1000;

            return Milleseconds;
        }

        /// <summary>
        /// Generate Access Code
        /// </summary>
        /// <returns></returns>
        public static string GenerateAccessCode()
        {
            string AccessCode = string.Empty;
            bool flag = true;
            int Code;
            Random rand = new Random();
            try
            {
                while (flag)
                {

                    Code = rand.Next(0, 9);
                    if (Code >= 0)
                    {
                        AccessCode += Code.ToString();
                        if (AccessCode.Length == 5)
                        {
                            flag = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return AccessCode;
        }

    }
    public sealed class JSON 
    {
        private static Dictionary<string, string> dictionaryJSON = new Dictionary<string, string>();

        /// <summary>
        /// Return the value of the dictionaryJSON.
        /// </summary>
        public static Dictionary<string,string> Create
        {
            get { return dictionaryJSON; }
        
        }

        /// <summary>
        /// Convert the Key:Value to JSON Format.
        /// </summary>
        /// <param name="key_value"></param>
        /// <returns></returns>
        public static string SetJsonData(params object[] key_value)
        {
            List<string> list_kvpair = new List<string>();
            StringBuilder sb = new StringBuilder();

            string[] arr_key_value = null;
            string[] arr_key_value_pair = null;
            string JSONData = string.Empty;
            string key=string.Empty;
            string value = string.Empty;

            char openBracket = '{';
            char closeBracket = '}';
            string quotationMarks = "\"";
            char colon = ':';
            char comma = ',';

            int count = 0;
            bool needsComma = false;

            sb.Append(openBracket);
            sb.Append(closeBracket);
            try
            {
                list_kvpair = key_value.Select(k => k.ToString()).ToList();
                arr_key_value_pair = list_kvpair[0].Split(',');
                count = arr_key_value_pair.Count();
                if (count > 0)
                {
                    sb = new StringBuilder();
                    if (count > 1)
                        needsComma = true;
                    sb.Append(openBracket);
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                            needsComma = false;
                        arr_key_value = arr_key_value_pair[i].Split(':');
                        key = arr_key_value[0];
                        value = arr_key_value[1];

                        //set to JSON Format
                        sb.Append(quotationMarks);
                        sb.Append(key);
                        sb.Append(quotationMarks);
                        sb.Append(colon);
                        sb.Append(quotationMarks);
                        sb.Append(value);
                       //sb.Append(HttpUtility.UrlDecode(value));
                        sb.Append(quotationMarks);
                        if (needsComma)
                            sb.Append(comma);
                    }
                    sb.Append(closeBracket);
                }
                JSONData = sb.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JSONData.Replace("\\", "");
        }

        /// <summary>
        /// Parse a JSON Formatted String into a JObject
        /// </summary>
        /// <param name="JSONString"></param>
        /// <returns></returns>
        public static JObject ParseJSONData(string JSONString)
        {
            JObject jobject = null;
            string jsonString = "{\"{0}\":\"{1}\"}";
            string jsonError = "";
            try
            {
                jobject = JObject.Parse(JSONString);
            }
            catch (JsonReaderException jrex)
            {
                jsonError = String.Format(jsonString, "JsonReaderException", jrex.Message);
                jobject = JObject.Parse(jsonError);
            }
            catch (Exception ex)
            {
                jobject = JObject.Parse(String.Format(jsonString, "Exception", ex.Message));
                jobject = JObject.Parse(jsonError);
            }
            return jobject;
        }

        /// <summary>
        /// Convert Dictionary to Json Format.
        /// </summary>
        /// <param name="dictJson"></param>
        /// <returns></returns>
        public static string DictionaryToJson(Dictionary<string, string> dictJson)
        {
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(dictJson);     
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return json;
        }

        /// <summary>
        /// Get and Sets value to a Dictionary of String.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprList"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Make<T>(Expression<T>[] exprList)
        {
            Dictionary<string, string> dictJSON = new Dictionary<string, string>();

            try
            {
                foreach (var expr in exprList)
                {
                    var body = ((MemberExpression)expr.Body);

                    string key = body.Member.Name;

                    if (!dictJSON.ContainsKey(key))
                    {
                        dictJSON.Add(key, (string)((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value));

                    }
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return dictJSON;
        }

        /// <summary>
        /// Get and Sets value to a List Dictionary of string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprList"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Make<T>(List<Expression<T>> exprList)
        {

            Dictionary<string, string> dictJSON = new Dictionary<string, string>();

            try
            {
                foreach (var expr in exprList)
                {
                    var body = ((MemberExpression)expr.Body);

                    string key = body.Member.Name;

                    // Check key if not exist on Dictionary
                    if (!dictJSON.ContainsKey(key))
                    {
                        dictJSON.Add(key, (string)((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value));
                    }
                    // If key already exist
                    // Do not insert

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return dictJSON;
        }

        /// <summary>
        /// Get and Sets value to a Dictionary of object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprList"></param>
        /// <returns></returns>
        public static Dictionary<object, object> MakeObject<T>(Expression<T>[] exprList)
        {
            Dictionary<object, object> dictJSON = new Dictionary<object, object>();

            try
            {
                foreach (var expr in exprList)
                {
                    var body = ((MemberExpression)expr.Body);

                    string key = body.Member.Name;

                    // Check key if not exist on Dictionary
                    if (!dictJSON.ContainsKey(key))
                    {
                        dictJSON.Add(key, ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value));
                    }
                    // If key already exist

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dictJSON;
        }

        /// <summary>
        /// Get and Sets value to a List Dictionary of object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprList"></param>
        /// <returns></returns>
        public static Dictionary<object, object> MakeObject<T>(List<Expression<T>> exprList)
        {
            Dictionary<object, object> dictJSON = new Dictionary<object, object>();

            try
            {
                foreach (var expr in exprList)
                {
                    var body = ((MemberExpression)expr.Body);

                    string key = body.Member.Name;

                    // Check key if not exist on Dictionary
                    if (!dictJSON.ContainsKey(key))
                    {
                        dictJSON.Add(key, ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value));
                    }
                    // If key already exist

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dictJSON;
        }

                         
    }
    public sealed class ServiceRequest
    {
        /// <summary>
        /// Make a Request in WebService using POST Method. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string Post(string url, string method, string postData)
        {
            bool retry = true;
            int retryMAX = 3;
            int i = 0;
            string jsonResponse = "";

            try
            {
                while (retry && i < retryMAX)
                {
                    Uri address = new Uri(url + method);
                    HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                    request.Method = "POST";
                    request.ContentType = "application/json";

                    byte[] byteData = UTF8Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = byteData.Length;
                    request.Timeout = 10000;

                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                        postStream.Close();
                    }

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        jsonResponse = reader.ReadToEnd();
                        response.Close();
                        retry = false;

                    }
                }

            }
            catch (WebException)
            {
                i++;
                retry = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return jsonResponse;
        }

        /// <summary>
        /// Make a Request in WebService using GET Method.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string Get(string url, string method, string postData) 
        {
            bool retry = true;
            int retryMax = 3;
            int i = 0;
            string jsonResponse = string.Empty;

            try
            {
                while (retry && i < retryMax)
                {
                    Uri address = new Uri(url + method);
                    HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                    request.Timeout = 20000;

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII);
                        jsonResponse = reader.ReadToEnd();
                        reader.Close();
                        response.Close();
                        retry = false;
                    }
                }
            }
            catch (WebException ex)
            {
                if (i < retryMax)
                {
                    i++;
                    retry = true;
                }
                else
                    throw ex;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResponse;
        }

        /// <summary>
        /// Make a request to a webservice and Deserialize the result into a Dictionary of string.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DeserializedGet(string url, string method, string Params)
        {
            Dictionary<string, string> result = null;
            try
            {
                string request = Get(url, method, Params);

                if (!string.IsNullOrEmpty(request))
                    result = JsonConvert.DeserializeObject <Dictionary<string, string>>(request); 

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Make a request to a webservice and Parse the Json Format to object.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static JObject JObjectGet(string url, string method, string Params)
        {
            JObject result = null;
            try
            {
                result = JObject.Parse(Get(url,method,Params));
                
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return result;
        }

    }
    public sealed class Faye
    {
        /// <summary>
        /// Push a Faye to the corresponding channel.
        /// </summary>
        /// <param name="fayeServer"></param>
        /// <param name="channel"></param>
        /// <param name="JsonData"></param>
        public static void PushToFaye(string fayeServer, string channel, string JsonData)
        {
            try
            {
                if (!string.IsNullOrEmpty(JsonData))
                {
                    Dictionary<string, string> dictFaye = new Dictionary<string, string>();
                    dictFaye.Add("channel", "/" + channel);
                    dictFaye.Add("data", JsonData);

                    string json = JSON.DictionaryToJson(dictFaye);

                    ServiceRequest.Post(fayeServer, "", json);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// Listen to the Faye Channel.
        /// </summary>
        /// <param name="fayeUri"></param>
        /// <param name="fayeChannel"></param>
        /// <returns></returns>
        public static IClientSessionChannel Subscribe(string fayeUri, string fayeChannel) 
        {
            IClientSessionChannel channel = null;
            try
            {
                BayeuxClient client = new BayeuxClient(fayeUri, new List<ClientTransport>() { new LongPollingTransport(null) });
                client.handshake();
                client.waitFor(1000, new List<BayeuxClient.State>() { BayeuxClient.State.CONNECTED });

                channel = client.getChannel(fayeChannel);

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return channel;
        }  
    
    }
}
