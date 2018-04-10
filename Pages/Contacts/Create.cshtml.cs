using System;
using ContactManager.Authorization;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ContactManager.Pages.Contacts
{
    #region snippetCtor
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }
        #endregion

        public IActionResult OnGet()
        {
            Contact = new Contact
            {
                Name = "Rick",
                Address = "123 N 456 S",
                City = "GF",
                State = "MT",
                Zip = "59405",
                Email = "rick@rick.com"
            };
            return Page();
        }

        [BindProperty]
        public Contact Contact { get; set; }

        #region snippet_Create
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("[Contact.CreateModel][OnPostAsync]: BEGIN");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Contact.OwnerID = UserManager.GetUserId(User);
            Console.WriteLine("[Contact.CreateModel][OnPostAsync]: Contact.OwnerID: " + Contact.OwnerID);

            // requires using ContactManager.Authorization;
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Contact,
                                                        ContactOperations.Create);

            Console.WriteLine("[Contact.CreateModel][OnPostAsync]: isAuthorized: " + isAuthorized);
                                                        
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Context.Contact.Add(Contact);
            await Context.SaveChangesAsync();

            Console.WriteLine("[Contact.CreateModel][OnPostAsync]: END (Return to ./Index");

            return RedirectToPage("./Index");
        }
        #endregion
    }
}