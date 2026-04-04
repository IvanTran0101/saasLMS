using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.NotificationService.Controllers;

public class HomeController : AbpController
{
    public ActionResult Index()
    {
        return Redirect("/swagger");
    }
}
