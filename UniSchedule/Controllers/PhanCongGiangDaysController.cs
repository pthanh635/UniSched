using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniSchedule.Models;

namespace UniSchedule.Controllers
{
    public class PhanCongGiangDaysController : Controller
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
        // GET: PhanCongGiangDays
        public ActionResult Index()
        {
            var phanCongGiangDays = db.PhanCongGiangDays.Include(p => p.GiangVien).Include(p => p.LopHocPhan);
            return View(phanCongGiangDays.ToList());
        }

        // GET: PhanCongGiangDays/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhanCongGiangDay phanCongGiangDay = db.PhanCongGiangDays.Find(id);
            if (phanCongGiangDay == null)
            {
                return HttpNotFound();
            }
            return View(phanCongGiangDay);
        }

        // GET: PhanCongGiangDays/Create
        // GET: PhanCongGiangDays/Create
        public ActionResult Create()
        {
            var hocKiList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Học kỳ 1" },
                new SelectListItem { Value = "2", Text = "Học kỳ 2" },
                new SelectListItem { Value = "3", Text = "Học kỳ hè" }
            };
            ViewBag.HocKiList = hocKiList;


            int currentYear = DateTime.Now.Year;
            var namHocList = new List<SelectListItem>
            {
                new SelectListItem { Value = $"{currentYear - 1}-{currentYear}", Text = $"{currentYear - 1}-{currentYear}" }, // Năm trước-Năm nay
                new SelectListItem { Value = $"{currentYear}-{currentYear + 1}", Text = $"{currentYear}-{currentYear + 1}" }  // Năm nay-Năm sau
            };
            ViewBag.NamHocList = namHocList;

            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV");
            ViewBag.MaLHP = new SelectList(db.LopHocPhans, "MaLHP", "TenMH");
            return View();
        }

        // POST: PhanCongGiangDays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: PhanCongGiangDays/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaPhanCong,MaGV,MaLHP,GhiChu,HocKi,NamHoc")] PhanCongGiangDay phanCongGiangDay)
        {
            int currentYear = DateTime.Now.Year;
            var hocKiList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Học kỳ 1" },
                    new SelectListItem { Value = "2", Text = "Học kỳ 2" },
                    new SelectListItem { Value = "3", Text = "Học kỳ hè" }
                };
            var namHocList = new List<SelectListItem>
                {
                    new SelectListItem { Value = $"{currentYear - 1}-{currentYear}", Text = $"{currentYear - 1}-{currentYear}" },
                    new SelectListItem { Value = $"{currentYear}-{currentYear + 1}", Text = $"{currentYear}-{currentYear + 1}" }
                };

            if ((phanCongGiangDay.MaGV == "" || phanCongGiangDay.MaGV == null) || (phanCongGiangDay.MaLHP == "" || phanCongGiangDay.MaLHP == null) || string.IsNullOrEmpty(phanCongGiangDay.NamHoc))
            {
                if (phanCongGiangDay.MaGV == "" || phanCongGiangDay.MaGV == null)
                {
                    ModelState.AddModelError("MaGV", "Chọn một Giảng Viên");

                }
                if (phanCongGiangDay.MaLHP == "" || phanCongGiangDay.MaLHP == null)
                {
                    ModelState.AddModelError("MaLHP", "Chọn một lớp học phần");
                }
                if (!phanCongGiangDay.HocKi.HasValue || !new[] { 1, 2, 3 }.Contains(phanCongGiangDay.HocKi.Value))
                {
                    ModelState.AddModelError("HocKi", "Vui lòng chọn học kỳ hợp lệ (1, 2 hoặc 3).");
                }

                var validNamHocs = new[] { $"{currentYear - 1}-{currentYear}", $"{currentYear}-{currentYear + 1}" };
                if (string.IsNullOrEmpty(phanCongGiangDay.NamHoc) || !validNamHocs.Contains(phanCongGiangDay.NamHoc))
                {
                    ModelState.AddModelError("NamHoc", "Vui lòng chọn năm học hợp lệ.");
                }

                ViewBag.HocKiList = hocKiList;
                ViewBag.NamHocList = namHocList;
                ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", phanCongGiangDay.MaGV);
                ViewBag.MaLHP = new SelectList(db.LopHocPhans, "MaLHP", "TenMH", phanCongGiangDay.MaLHP);
                return View(phanCongGiangDay);
            }

            bool isHavePC = db.PhanCongGiangDays.Any(p => p.MaLHP == phanCongGiangDay.MaLHP && p.HocKi == phanCongGiangDay.HocKi && p.NamHoc == phanCongGiangDay.NamHoc);
           
            if (isHavePC)
            {
                ModelState.AddModelError("", $"Lớp học phần này đã được phân công ở kì {phanCongGiangDay.HocKi} năm học {phanCongGiangDay.NamHoc}");

                ViewBag.HocKiList = hocKiList;
                ViewBag.NamHocList = namHocList;
                ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", phanCongGiangDay.MaGV);
                ViewBag.MaLHP = new SelectList(db.LopHocPhans, "MaLHP", "TenMH", phanCongGiangDay.MaLHP);
                return View(phanCongGiangDay);
            }

            if (ModelState.IsValid)
            {
                db.PhanCongGiangDays.Add(phanCongGiangDay);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HocKiList = hocKiList;
            ViewBag.NamHocList = namHocList;
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", phanCongGiangDay.MaGV);
            ViewBag.MaLHP = new SelectList(db.LopHocPhans, "MaLHP", "TenMH", phanCongGiangDay.MaLHP);
            return View(phanCongGiangDay);
        }

        // GET: PhanCongGiangDays/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhanCongGiangDay phanCongGiangDay = db.PhanCongGiangDays.Find(id);
            if (phanCongGiangDay == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", phanCongGiangDay.MaGV);
            ViewBag.MaLHP = new SelectList(db.LopHocPhans, "MaLHP", "TenMH", phanCongGiangDay.MaLHP);
            return View(phanCongGiangDay);
        }

        // POST: PhanCongGiangDays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaPhanCong,MaGV,MaLHP,GhiChu,HocKi,NamHoc")] PhanCongGiangDay phanCongGiangDay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phanCongGiangDay).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", phanCongGiangDay.MaGV);
            ViewBag.MaLHP = new SelectList(db.LopHocPhans, "MaLHP", "TenMH", phanCongGiangDay.MaLHP);
            return View(phanCongGiangDay);
        }

        // GET: PhanCongGiangDays/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhanCongGiangDay phanCongGiangDay = db.PhanCongGiangDays.Find(id);
            if (phanCongGiangDay == null)
            {
                return HttpNotFound();
            }
            return View(phanCongGiangDay);
        }

        // POST: PhanCongGiangDays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            bool isHaveLichDay = db.LichDays.Any(p => p.MaPhanCong == id);
            if (isHaveLichDay)
            {
                ModelState.AddModelError("", "Không thể xóa phân công này vì đã có lịch dạy liên quan.");
                return RedirectToAction("Index");
            }

            PhanCongGiangDay phanCongGiangDay = db.PhanCongGiangDays.Find(id);
            db.PhanCongGiangDays.Remove(phanCongGiangDay);
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
