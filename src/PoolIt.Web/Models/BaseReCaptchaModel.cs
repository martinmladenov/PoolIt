namespace PoolIt.Web.Models
{
    using Attributes;
    using Microsoft.AspNetCore.Mvc;

    public abstract class BaseReCaptchaModel
    {
        [ValidateReCaptcha]
        [BindProperty(Name = "g-recaptcha-response")]
        public string ReCaptchaResponse { get; set; }
    }
}