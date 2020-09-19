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
    public class ImageController : BaseController
    {
        [HttpPost()]
        public ActionResult Remove(long id)
        {
            ModelUtility.RemoveImage(id);

            return new EmptyResult();
        }

        [HttpPost()]
        public ActionResult Load(string guid, string field)
        {
            List<dynamic> images = new List<dynamic>();

            using (ImageRepository database = new ImageRepository())
            {
                foreach (var image in database.Get(guid, field))
                {
                    images.Add(new { image_id = image.image_id, image_extension = image.image_extension, image_desc = image.image_desc ?? string.Empty });
                }
            }

            return Json(images);
        }

        [HttpPost()]
        public ActionResult Upload(string guid, string field)
        {
            long image_id = 0; string image_extension = string.Empty; int file_count = 0;

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName] as HttpPostedFileBase;

                if (file.ContentLength == 0 || file.FileName == "blob")
                    continue;

                file_count++;

                // save in db

                image_extension = Path.GetExtension(file.FileName);

                using (ImageRepository database = new ImageRepository())
                {
                    image_id = database.CreateGlobalID();

                    database.Save(image_id, image_extension, guid, field);
                }

                // save in file system

                try
                {
                    file.SaveAs(Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", image_id, image_extension)));
                }
                catch
                {
                    using (ImageRepository database = new ImageRepository())
                    {
                        database.Remove(image_id);
                    }
                }
            }

            return file_count == 0 ? null : (guid == null ? Json(new { filelink = string.Format("/content/cms/files/{0}{1}", image_id, image_extension) }) : Json(new { image_id = image_id, image_extension = image_extension }));
        }

        [HttpPost()]
        public ActionResult UpdateOrder(long[] ids, long[] orders)
        {
            using (ImageRepository database = new ImageRepository())
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