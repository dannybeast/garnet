using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer, icbcode_cms_admin, icbcode_cms_content_manager")]
    public class FileController : BaseController
    {
        [HttpPost()]
        public ActionResult Remove(long id)
        {
            ModelUtility.RemoveFile(id);

            return new EmptyResult();
        }

        [HttpPost()]
        public ActionResult Load(string guid, string field)
        {
            List<dynamic> files = new List<dynamic>();

            using (FileRepository database = new FileRepository())
            {
                foreach (var file in database.Get(guid, field))
                {
                    files.Add(new { file_id = file.file_id, file_extension = file.file_extension, file_desc = file.file_desc ?? string.Empty });
                }
            }

            return Json(files);
        }

        [HttpPost()]
        public ActionResult Upload(string guid, string field)
        {
            long file_id = 0; string file_extension = string.Empty; string file_desc = string.Empty; int file_size = 0; int file_count = 0;

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName] as HttpPostedFileBase;

                if (file.ContentLength == 0 || file.FileName == "blob")
                    continue;

                file_count++;

                // save in db

                file_desc = file.FileName; file_size = file.ContentLength; file_extension = Path.GetExtension(file.FileName);

                using (FileRepository database = new FileRepository())
                {
                    file_id = database.CreateGlobalID();

                    database.Save(file_id, file_extension, guid, file.FileName, file.ContentLength, field);
                }

                // save in file system

                try
                {
                    file.SaveAs(Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", file_id, file_extension)));
                }
                catch
                {
                    using (FileRepository database = new FileRepository())
                    {
                        database.Remove(file_id);
                    }
                }
            }

            return file_count == 0 ? null : (guid == null ? Json(new { filelink = string.Format("/content/cms/files/{0}{1}", file_id, file_extension) }) : Json(new { file_id = file_id, file_extension = file_extension, file_desc = file_desc, file_size = file_size }));
        }

        [HttpPost()]
        public ActionResult UpdateOrder(long[] ids, long[] orders)
        {
            using (FileRepository database = new FileRepository())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    database.UpdateOrder(ids[i], orders[i]);
                }
            }

            return new EmptyResult();
        }
    }
}