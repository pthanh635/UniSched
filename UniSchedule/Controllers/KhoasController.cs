using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniSchedule.Models;

namespace UniSchedule.Controllers
{
    public class KhoasController : Controller
    {
        private XepLichGiangVienEntities db = new XepLichGiangVienEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["MaVaiTro"] == null || (int)Session["MaVaiTro"] != 0)
            {
                // Không phải GiaoVu thì chuyển về trang login
                filterContext.Result = RedirectToAction("Login", "Home");
            }
            base.OnActionExecuting(filterContext);
        }
        // GET: Khoas
        public ActionResult Index()
        {
            return View(db.Khoas.ToList());
        }

        // GET: Khoas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khoa khoa = db.Khoas.Find(id);
            if (khoa == null)
            {
                return HttpNotFound();
            }
            return View(khoa);
        }

        // GET: Khoas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Khoas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaKhoa,TenKhoa")] Khoa khoa)
        {
            bool isExist = db.Khoas.Any(k => k.MaKhoa == khoa.MaKhoa);
            if (isExist)
            {
                ModelState.AddModelError("MaKhoa", "Mã khoa đã tồn tại.");
                ModelState.AddModelError("", "Mã khoa đã tồn tại.");
                return View(khoa);
            }

            if ((khoa.MaKhoa == null || khoa.MaKhoa == "") || (khoa.TenKhoa == null || khoa.TenKhoa == ""))
            {
                ModelState.AddModelError("", "Thiếu thông tin");
                if (khoa.MaKhoa == null || khoa.MaKhoa == "")
                {
                    ModelState.AddModelError("MaKhoa", "Mã khoa không được để trống");
                }
                if (khoa.TenKhoa == null || khoa.TenKhoa == "")
                {
                    ModelState.AddModelError("TenKhoa", "Điền tên khoa.");
                }
                return View(khoa);
            }

            if (ModelState.IsValid)
            {

                db.Khoas.Add(khoa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(khoa);
        }

        // GET: Khoas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khoa khoa = db.Khoas.Find(id);
            if (khoa == null)
            {
                return HttpNotFound();
            }
            return View(khoa);
        }

        // POST: Khoas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKhoa,TenKhoa")] Khoa khoa)
        {
            bool isExist = db.Khoas.Any(k => k.MaKhoa == khoa.MaKhoa && k.MaKhoa != khoa.MaKhoa);
            if (isExist)
            {
                ModelState.AddModelError("MaKhoa", "Mã khoa đã tồn tại.");
            }
            if (ModelState.IsValid)
            {
                db.Entry(khoa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khoa);
        }

        // GET: Khoas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khoa khoa = db.Khoas.Find(id);
            if (khoa == null)
            {
                return HttpNotFound();
            }
            return View(khoa);
        }

        // POST: Khoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool isExist = db.GiangViens.Any(gv => gv.MaKhoa == id);
            if (isExist)
            {
                ModelState.AddModelError("", "Không thể xóa khoa này vì có giảng viên thuộc khoa này.");
                return RedirectToAction("Index");
            }

            Khoa khoa = db.Khoas.Find(id);
            db.Khoas.Remove(khoa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
