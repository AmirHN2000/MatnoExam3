using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Competition3.Data;
using Competition3.Enum;
using Competition3.Helper;
using Competition3.Services;
using Competition3.ViewModels;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Drawing;

namespace Competition3.Controllers
{
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<User> _userManager;

        public PaymentController(AppDbContext dbContext, IPaymentService paymentService, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        /// <summary>
        /// redirect to getAway
        /// </summary>
        /// <param name="amount"></param>
        /// amount pay
        /// <param name="description"></param>
        /// description for pay
        /// <returns></returns>
        [HttpGet]
        [Route("pay")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Pay([FromQuery] int amount, [FromQuery] string description)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId.ToString()))
            {
                return Problem();
            }

            var payment = new ZarinpalSandbox.Payment(amount);
            var response = await payment.PaymentRequest(description, "https://localhost:5001/verify");
            if (response.Status == 100)
            {
                var p = new Payment
                {
                    Amount = amount,
                    UserId = userId,
                    Status = EnPaymentStatus.Pending,
                    Description = description,
                    SystemCode = response.Authority,
                    PaymentDate = DateTime.Now
                };
                await _paymentService.AddPaymentAsync(p);
                var result = await _dbContext.SaveChangesAsync();
                if (result == 0) return Problem();

                return Ok(new ResultObject
                    {Success = true, Message = "لینک را در مرورگر وارد کنید", Extera = response.Link});
            }

            return Json(new ResultObject {Success = false, Message = "خطا در برقراری ارتباط با درگاه پرداخت"});
        }

        [HttpGet]
        [Route("verify")]
        public async Task<IActionResult> Verify()
        {
            if (string.IsNullOrEmpty(Request.Query["Status"].ToString()) ||
                string.IsNullOrEmpty(Request.Query["Authority"].ToString()))
            {
                return Content("خطا در درگاه پرداخت");
            }

            var authority = Request.Query["Authority"].ToString();
            var payment = await _paymentService.GetPaymentAsync(authority);
            if (Request.Query["Status"].ToString().ToLower() == "ok")
            {
                var paymentD = new ZarinpalSandbox.Payment(payment.Amount);
                var verifyD = await paymentD.Verification(authority);
                if (verifyD.Status == 100)
                {
                    payment.Status = EnPaymentStatus.Success;
                    payment.RefCode = verifyD.RefId.ToString();

                    await _dbContext.SaveChangesAsync();
                    return Content("کاربر گرامی \n" + "پرداخت وجه " + payment.Amount.ToString("N0") + " تومان با موفقیت انجام شد.\n" +
                                   "کد رهگیری : " + verifyD.RefId);
                }
            }

            payment.Status = EnPaymentStatus.Failed;
            await _dbContext.SaveChangesAsync();

            return Content("پرداخت ناموفق");
        }

        [HttpGet]
        [Route("get-user-pays")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserPays()
        {
            var userId = User.GetUserId();
            var pays = await _paymentService.GetAll().Where(x => x.UserId == userId).ToListAsync();
            var list = pays.Select(x => new PaymentVm()
            {
                Amount = x.Amount.ToString("N0") + " تومان ",
                Description = x.Description,
                Id = x.Id,
                Status = x.Status,
                NStatus = x.Status.GetDisplayName(),
                NPaymentDate = x.PaymentDate.ToShortPersianDateTimeString(),
                UserId = userId,
                PaymentDate = x.PaymentDate,
                RefCode = x.RefCode
            }).ToList();

            return Ok(new ResultObject {Success = true, Extera = list});
        }

        [HttpGet]
        [Route("get-user-pay/{payId=int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserPay([FromRoute]int payId)
        {
            var pay = await _paymentService.FindByIdAsync(payId);
            if (pay == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
            if (user == null)
                return NotFound();
            
            var info = Environment.NewLine + user.FullName + " : کاربر " +
                       Environment.NewLine + " تومان " + pay.Amount.ToString("N0") + " : مبلغ تراکنش " +
                       Environment.NewLine + pay.Status.GetDisplayName() + " : وضعیت تراکنش " +
                       Environment.NewLine + pay.RefCode + " : کد تراکنش " +
                       Environment.NewLine + " زرینپال : درگاه پرداخت " +
                       Environment.NewLine + pay.PaymentDate.ToShortPersianDateTimeString() + " : تاریخ پرداخت " +
                       Environment.NewLine + pay.Description + " : توضیحات ";

            using var document  = new WordDocument();
            var section = document.AddSection();
            var paragraph = section.AddParagraph();
            paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;

            var title = paragraph.AppendText("اطلاعات پرداخت");
            title.CharacterFormat.Bold = true;
            title.CharacterFormat.FontSize = 16;
            title.CharacterFormat.Shadow = true;
            title.CharacterFormat.TextColor = Color.Goldenrod;

            paragraph.AppendBreak(BreakType.LineBreak);

            var text = paragraph.AppendText(info);
            text.CharacterFormat.FontSize = 11;
            
            var stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            stream.Position = 0;

            return new FileStreamResult(stream, "application/pdf");
        }
        
        [HttpGet]
        [Route("get-all-pays")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPays()
        {
            var pays = await GetAllPaymentVms();

            return Ok( new ResultObject{Success = true, Extera = pays});
        }

        /// <summary>
        /// get report pays from start_at to end_at on word file
        /// </summary>
        /// <param name="start_at"></param>
        /// persian date with format 1400/12/1
        /// <param name="end_at"></param>
        /// /// persian date with format 1400/12/3
        /// <returns></returns>
        [HttpGet]
        [Route("get-report-pays")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReportPays([FromQuery] string start_at, [FromQuery] string end_at)
        {
            if (string.IsNullOrEmpty(start_at) || string.IsNullOrEmpty(end_at))
                return BadRequest();
            
            var pays = await GetAllPaymentVms(start_at.ToEnglishDateTime(), end_at.ToEnglishDateTime());
            
            var document = new WordDocument();
            var section = document.AddSection();
            var table = section.AddTable();
            table.ResetCells(pays.Count+1, 7);
            table.TableFormat.BackColor = Color.LightGray;
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;
            table.TableFormat.Borders.Color = Color.Black;
            table.FirstRow.RowFormat.BackColor = Color.DarkRed;

            FillCell(table[0, 6], "شماره", true);
            FillCell(table[0, 5], "نام کاربر", true);
            FillCell(table[0, 4], "مبلغ", true);
            FillCell(table[0, 3], "وضعیت تراکنش", true);
            FillCell(table[0, 2], "شماره تراکنش", true);
            FillCell(table[0, 1], "تاریخ پرداخت", true);
            FillCell(table[0, 0], "توضیحات", true);

            for (var i = 0; i < pays.Count; i++)
            {
                var row = i + 1;
                FillCell(table[row, 6], row.ToString());
                FillCell(table[row, 5], pays[i].FullName);
                FillCell(table[row, 4], pays[i].Amount);
                FillCell(table[row, 3], pays[i].NStatus);
                FillCell(table[row, 2], pays[i].RefCode??"");
                FillCell(table[row, 1], pays[i].NPaymentDate);
                FillCell(table[row, 0], pays[i].Description);
                
                table[row, 3].CellFormat.BackColor = pays[i].Status switch
               {
                   EnPaymentStatus.Failed => Color.Red,
                   EnPaymentStatus.Pending => Color.Yellow,
                   _ => Color.Green
               };
            }
            
            var stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            stream.Position = 0;

            return new FileStreamResult(stream, "application/msword");
        }

        [NonAction] 
        private void FillCell(ITextBody cell, string text, bool isBold = false)
        {
            var paragraph = cell.AddParagraph();
            paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            var textCell = paragraph.AppendText(text);
            if (isBold)
            {
                textCell.CharacterFormat.Bold = true;
            }
        }

        [NonAction]
        private async Task<List<PaymentVm>> GetAllPaymentVms(DateTime? startDate=null, DateTime? endDate=null)
        {
            var query = _paymentService.GetAll();
            if (startDate != null && endDate !=null)
            {
                query = query.Where(x => x.PaymentDate.Date >= ((DateTime)startDate).Date 
                                         && x.PaymentDate.Date <= ((DateTime)endDate));
            }
                
            return await query.Select(x => new PaymentVm()
            {
                Id = x.Id,
                Amount = x.Amount.ToString("N0") + " تومان ",
                Description = x.Description,
                Status = x.Status,
                NStatus = x.Status.GetDisplayName(),
                PaymentDate = x.PaymentDate,
                NPaymentDate = x.PaymentDate.ToShortPersianDateTimeString(true),
                RefCode = x.RefCode,
                UserId = x.UserId,
                FullName = x.User.FullName
            }).ToListAsync();
        }
    }
}