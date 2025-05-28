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
    public class GiangViensController : Controller
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
        // GET: GiangViens
        public ActionResult Index()
        {


            var giangViens = db.GiangViens
                .Include(g => g.Khoa)
                .Include(g => g.PhanCongGiangDays.Select(p => p.LopHocPhan));
            return View(giangViens.ToList());
        }

        // GET: GiangViens/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiangVien giangVien = db.GiangViens.Find(id);
            if (giangVien == null)
            {
                return HttpNotFound();
            }
            return View(giangVien);
        }

        // GET: GiangViens/Create
        public ActionResult Create()
        {
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            return View();
        }

        // POST: GiangViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaGV,TenGV,MaKhoa,Email,SoDienThoai")] GiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelError("", "Thiếu thông tin.");
                if ((giangVien.MaGV == null || giangVien.MaGV == "") || (giangVien.TenGV == null || giangVien.TenGV == "") || (giangVien.Email == null || giangVien.Email == "") || (giangVien.SoDienThoai == null || giangVien.SoDienThoai == "") || (giangVien.MaKhoa == null || giangVien.MaKhoa == ""))
                {
                    if (giangVien.MaGV == null || giangVien.MaGV == "")
                    {
                        ModelState.AddModelError("MaGV", "Mã giảng viên không được để trống.");
                    }
                    if (giangVien.TenGV == null || giangVien.TenGV == "")
                    {
                        ModelState.AddModelError("TenGV", "Tên giảng viên không được để trống.");
                    }
                    if (giangVien.MaKhoa == null || giangVien.MaKhoa == "")
                    {
                        ModelState.AddModelError("MaKhoa", "Chọn một khoa cho giảng viên");
                    }
                    if (giangVien.Email == null || giangVien.Email == "")
                    {
                        ModelState.AddModelError("Email", "Email không được để trống.");
                    }
                    if (giangVien.SoDienThoai == null || giangVien.SoDienThoai == "")
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại không được để trống.");
                    }
                    ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
                    return View(giangVien);
                }


                bool isExist = db.GiangViens.Any(g => g.MaGV == giangVien.MaGV);
                if (isExist)
                {
                    ModelState.AddModelError("", "Mã giảng viên đã tồn tại.");
                    ModelState.AddModelError("MaGV", "Nhập một mã giảng viên khác.");
                    ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
                    return View(giangVien);
                }

                db.GiangViens.Add(giangVien);
                TempData["SuccessMessage"] = "Thêm thành công!!";
                TaiKhoan taiKhoan = new TaiKhoan
                {
                    TenDangNhap = giangVien.SoDienThoai,
                    MatKhau = "123abc",
                    MaGV = giangVien.MaGV,
                    MaVaiTro = 1
                };

                db.TaiKhoans.Add(taiKhoan);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
            return View(giangVien);
        }



        // GET: GiangViens/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiangVien giangVien = db.GiangViens.Find(id);
            if (giangVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
            return View(giangVien);
        }

        // POST: GiangViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaGV,TenGV,MaKhoa,Email,SoDienThoai")] GiangVien giangVien)
        {
            if ((giangVien.TenGV == null || giangVien.TenGV == "") || (giangVien.Email == null || giangVien.Email == "") || (giangVien.SoDienThoai == null || giangVien.SoDienThoai == ""))
            {
                if (giangVien.TenGV == null || giangVien.TenGV == "")
                {
                    ModelState.AddModelError("TenGV", "Tên Giảng Viên không được để trống.");
                }
                if (giangVien.Email == null || giangVien.Email == "")
                {
                    ModelState.AddModelError("Email", "Email không được để trống.");
                }
                if (giangVien.SoDienThoai == null || giangVien.SoDienThoai == "")
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại không được để trống");
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(giangVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
            return View(giangVien);
        }

        // GET: GiangViens/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiangVien giangVien = db.GiangViens.Find(id);
            if (giangVien == null)
            {
                return HttpNotFound();
            }
            return View(giangVien);
        }

        // POST: GiangViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GiangVien giangVien = db.GiangViens.Find(id);

            bool isTeacherHaveClass = db.PhanCongGiangDays.Any(p => p.MaGV == id);
            if (isTeacherHaveClass)
            {
                ModelState.AddModelError("", "Không thể xóa giảng viên này vì đã có phân công giảng dạy.");
                return View(giangVien);
            }

            bool isIdNotValid = db.TaiKhoans.Any(t => t.MaGV == id);
            if (isIdNotValid)
            {
                ModelState.AddModelError("", "Không thể xóa giảng viên này vì đã có tài khoản.");
                return View(giangVien);
            }
            db.GiangViens.Remove(giangVien);
            db.SaveChanges();
            TempData.Add("SuccessMessage", "Xóa thành công!!");
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
