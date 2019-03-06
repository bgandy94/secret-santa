using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using SecretSanta.Web.Models;

namespace SecretSanta.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretSantaController : Controller
    {
        #region Routes
        [HttpPost("pair")]
        public IActionResult GenerateGiftPairs([FromBody]List<Family> families)
        {

            var returnPairs = new List<GiftPair>();
            var errorFound = ValidateIncomingPayload(families);

            if (errorFound != null)
            {
                return errorFound;
            }

            families.OrderByDescending(x => x.Members.Count).ToList().ForEach(family =>
            {
                var giversChosen = new List<FamilyMember>();

                family.Members.ForEach(member =>
                {
                    var potentialRecipients = families
                    .SelectMany(x => x.Members)
                    .Except(returnPairs.Select(x => x.Recipient))
                    .Except(family.Members);

                    var random = new Random();
                    if (potentialRecipients.Count() > 1
                        && returnPairs.Count() > 0
                        && (families.Count == 2 && families.Select(x => x.Members.Count).Distinct().Skip(1).Any()))
                    {
                        var lastPickedFamily = families.Where(x => x.Members.Contains(returnPairs.Last().Recipient)).FirstOrDefault();
                        potentialRecipients = potentialRecipients.Except(lastPickedFamily.Members);
                    }

                    int index = random.Next(potentialRecipients.Count());
                    var recipient = potentialRecipients.ToList()[index];
                    returnPairs.Add(new GiftPair
                    {
                        Giver = member,
                        Recipient = recipient
                    });
                });

            });


            return Ok(new SecretSantaResult
            {
                Message = "success",
                Pairs = returnPairs
            });
        }
        #endregion

        #region Private Methods
        private bool FamilyTooLargeExists(List<Family> families) =>
            families.Any(x => x.Members.Count > (families.SelectMany(y => y.Members).Count() / 2));
        private bool FamilyWithOneMemberExists(List<Family> families) =>
            families.Select(x => x.Members.Count).Any(x => x < 2);
        private bool HasDuplicateFamilyNames(List<Family> families) =>
             families.Select(family => family.Name).GroupBy(x => x).Any(y => y.Count() > 1);
        private bool HasDuplicateMemberNames(List<Family> families) =>
            families.SelectMany(family => family.Members).GroupBy(member => member.Name).Any(names => names.Count() > 1);
        private bool InsufficientFamiliesProvided(List<Family> families) =>
            families.Count < 2;
        private BadRequestObjectResult ValidateIncomingPayload(List<Family> families)
        {
            if (InsufficientFamiliesProvided(families))
            {
                return BadRequest(new SecretSantaResult
                {
                    Message = "Please enter 2 or more families"
                });
            }
            if (FamilyWithOneMemberExists(families))
            {
                return BadRequest(new SecretSantaResult
                {
                    Message = "Family with less than 2 members found"
                });

            }
            if (HasDuplicateFamilyNames(families))
            {
                return BadRequest(new SecretSantaResult
                {
                    Message = "Duplicate family names found"
                });

            }
            if (HasDuplicateMemberNames(families))
            {
                return BadRequest(new SecretSantaResult
                {
                    Message = "Duplicate member names found"
                });
            }
            if (FamilyTooLargeExists(families))
            {
                return BadRequest(new SecretSantaResult
                {
                    Message = "A single family's members cannot be more than half the total members"
                });
            }
            return null;
        }
        #endregion
    }
}