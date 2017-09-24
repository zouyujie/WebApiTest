using CrossDomainAccessWebAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CrossDomainAccessWebAPI.Controllers
{
    public class UserInfoController : ApiController
    {
        /// <summary>
        /// 获取用户信息集合的方法 http://localhost:29874/api/userinfo
        /// </summary>
        /// <returns>返回用户信息集合</returns>
        public IHttpActionResult GetList()
        {
            //对象集合模拟数据
            List<UserInfo> list = new List<UserInfo>()
            {
                  new UserInfo(){Id = 1, UserName = "张三",RoleName="班组组长" }
            };
            return Ok(list);
        }
        ///// <summary>
        /////  http://localhost:29874/api/Post
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<Dictionary<string, string>> Post(int id = 0)
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    string root = HttpContext.Current.Server.MapPath("~/App_Data");//指定要将文件存入的服务器物理位置  
        //    var provider = new MultipartFormDataStreamProvider(root);
        //    try
        //    {
        //        // Read the form data.  
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        // This illustrates how to get the file names.  
        //        foreach (MultipartFileData file in provider.FileData)
        //        {//接收文件  
        //            Trace.WriteLine(file.Headers.ContentDisposition.FileName);//获取上传文件实际的文件名  
        //            Trace.WriteLine("Server file path: " + file.LocalFileName);//获取上传文件在服务上默认的文件名  
        //        }//TODO:这样做直接就将文件存到了指定目录下，暂时不知道如何实现只接收文件数据流但并不保存至服务器的目录下，由开发自行指定如何存储，比如通过服务存到图片服务器  
        //        foreach (var key in provider.FormData.AllKeys)
        //        {//接收FormData  
        //            dic.Add(key, provider.FormData[key]);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return dic;
        //}

        [HttpPost]
        public Task<Hashtable> ImgUpload()
        {
            // 检查是否是 multipart/form-data
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            //文件保存目录路径
            string SaveTempPath = "~/SayPlaces/";
            String dirTempPath = HttpContext.Current.Server.MapPath(SaveTempPath);
            // 设置上传目录
            var provider = new MultipartFormDataStreamProvider(dirTempPath);
            //var queryp = Request.GetQueryNameValuePairs();//获得查询字符串的键值集合
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<Hashtable>(o =>
                {
                    Hashtable hash = new Hashtable();
                    hash["error"] = 1;
                    hash["errmsg"] = "上传出错";
                    var fileCounts = provider.FileData.Count;
                    for (int i = 0; i < fileCounts; i++)
                    {
                        var file = provider.FileData[i];//provider.FormData
                        string orfilename = file.Headers.ContentDisposition.FileName.TrimStart('"').TrimEnd('"');
                        FileInfo fileinfo = new FileInfo(file.LocalFileName);
                        //最大文件大小
                        int maxSize = 10000000;
                        if (fileinfo.Length <= 0)
                        {
                            hash["error"] = 1;
                            hash["errmsg"] = "请选择上传文件。";
                        }
                        else if (fileinfo.Length > maxSize)
                        {
                            hash["error"] = 1;
                            hash["errmsg"] = "上传文件大小超过限制。";
                        }
                        else
                        {
                            string fileExt = orfilename.Substring(orfilename.LastIndexOf('.'));
                            //定义允许上传的文件扩展名
                            String fileTypes = "gif,jpg,jpeg,png,bmp";
                            if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(fileTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
                            {
                                hash["error"] = 1;
                                hash["errmsg"] = "上传文件扩展名是不允许的扩展名。";
                            }
                            else
                            {
                                String ymd = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                fileinfo.CopyTo(Path.Combine(dirTempPath, newFileName + fileExt), true);
                                fileinfo.Delete();
                                hash["error"] = 0;
                                hash["errmsg"] = "上传成功";
                            }
                        }
                    }
                    return hash;
                });
            return task;
        }
    }
}
