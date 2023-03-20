using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using System;
using System.IO;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class FileController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FileController));

        public byte[] base64toByteArray(string x)
        {
            string xxx = x.Remove(0, 28);

            byte[] newArray = System.Convert.FromBase64String(xxx);
            return newArray;
        }

        public ActionResult FileDownload(string ReqNo)
        {
            byte[] b = GetPdfFromDB(ReqNo);
            var pdfStream = new MemoryStream();
            pdfStream.Write(b, 0, b.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf");
        }

        public string FileUpload(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data, string base64Str, ORSession mySession)
        {
            try
            {
                byte[] bytes = null;
                if (!string.IsNullOrEmpty(base64Str))                
                    bytes = base64toByteArray(base64Str);
                
                conn = db.getConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO OnlineRequest.diagnosticFiles (reqNumber, diagnostic, syscreated, syscreator) " +
                " VALUES (@reqNumber, @diagnostic, @syscreated, @syscreator) ";
                cmd.Parameters.AddWithValue("@reqNumber", Data.RequestNo);
                cmd.Parameters.AddWithValue("@diagnostic", bytes);
                cmd.Parameters.AddWithValue("@syscreated", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                cmd.Parameters.AddWithValue("@syscreator", mySession.s_usr_id);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                log.Info("Successful file upload || request no: " + Data.RequestNo);
                return "success";
            }
            catch (Exception c)
            {
                tran.Rollback();
                log.Error("Error in uploading diagnostic file. --- " + c.Message, c);
                return "error";
            }
        }

        private byte[] GetPdfFromDB(string ReqNo)
        {
            byte[] bytes = { };
            var db = new ORtoMySql();
            using (var con = db.getConnection())
            {
                var cmd = con.CreateCommand();

                cmd.CommandText = "SELECT diagnostic FROM diagnosticFiles WHERE reqNumber = @reqNo";
                cmd.Parameters.AddWithValue("@reqNo", ReqNo);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.HasRows == true)
                    {
                        r.Read();
                        bytes = (byte[])r["diagnostic"];
                    }
                }
                con.Close();
            }
            log.Info("Successful getting pdf from DB || request no: " + ReqNo);
            return bytes;
        }
    }
}