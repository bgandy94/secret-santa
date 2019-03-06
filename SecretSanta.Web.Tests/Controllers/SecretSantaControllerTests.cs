using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SecretSanta.Web.Controllers;
using SecretSanta.Web.Models;
namespace SecretSanta.Web.Tests.Controllers
{
    [TestFixture]
    public class SecretSantaControllerTests
    {
        private SecretSantaController secretSantaController;

        #region Setup
        [SetUp]
        public void Setup()
        {
            secretSantaController = new SecretSantaController();
        }
        #endregion

        #region Tests
        [Test]
        public void GenerateGiftPairs_DuplicateFamilyNamesFound_ErrorReturned()
        {
            // Arange
            List<Family> invalidFamilyPayload = new List<Family>
                {
                new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            },
                            new FamilyMember
                            {
                                Name = "member2"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            },
                            new FamilyMember
                            {
                                Name = "member2"
                            }
                        }
                    }
                };
            var expectedResult = new SecretSantaResult
            {
                Message = "Duplicate family names found"
            };

            // Act
            var result = GetResultValue<BadRequestObjectResult>(secretSantaController.GenerateGiftPairs(invalidFamilyPayload));

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.ToString(), Is.EqualTo(expectedResult.ToString()));
        }


        [Test]
        public void GenerateGiftPairs_FamilyTooLargeProvided_ErrorReturned()
        {
            // Arange
            List<Family> invalidFamilyPayload = new List<Family>
                {
                new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            },
                            new FamilyMember
                            {
                                Name = "member2"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family2",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member4"
                            },
                            new FamilyMember
                            {
                                Name = "member5"
                            },
                            new FamilyMember
                            {
                                Name = "member6"
                            }
                        }
                    }
                };
            var expectedResult = new SecretSantaResult
            {
                Message = "A single family's members cannot be more than half the total members"
            };

            // Act
            var result = GetResultValue<BadRequestObjectResult>(secretSantaController.GenerateGiftPairs(invalidFamilyPayload));

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.ToString(), Is.EqualTo(expectedResult.ToString()));
        }
        [Test]
        public void GenerateGiftPairs_DuplicateMembersFound_ErrorReturned()
        {
            // Arange
            List<Family> invalidFamilyPayload = new List<Family>
                {
                new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            },
                            new FamilyMember
                            {
                                Name = "member2"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family2",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            },
                            new FamilyMember
                            {
                                Name = "member2"
                            }
                        }
                    }
                };
            var expectedResult = new SecretSantaResult
            {
                Message = "Duplicate member names found"
            };

            // Act
            var result = GetResultValue<BadRequestObjectResult>(secretSantaController.GenerateGiftPairs(invalidFamilyPayload));

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.ToString(), Is.EqualTo(expectedResult.ToString()));
        }

        [Test]
        public void GenerateGiftPairs_3FamiliesDifferingSizes_GiftPairsReturned()
        {
            // Arrange
            var validFamilyPayload = new List<Family>
                {
                new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            },
                            new FamilyMember
                            {
                                Name = "member2"
                            },
                            new FamilyMember
                            {
                                Name = "member9"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family2",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member3"
                            },
                            new FamilyMember
                            {
                                Name = "member4"
                            },
                            new FamilyMember
                            {
                                Name = "member5"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family3",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member6"
                            },
                            new FamilyMember
                            {
                                Name = "member7"
                            },
                            new FamilyMember
                            {
                                Name = "member8"
                            },
                            new FamilyMember
                            {
                                Name = "member10"
                            }
                        }
                    }
                };
            // Act
            var result = GetResultValue<OkObjectResult>(secretSantaController.GenerateGiftPairs(validFamilyPayload));

            // Assert
            Assert.That(result, Is.Not.Null);

            var secretSantaResult = result.Value as SecretSantaResult;
            var pairs = secretSantaResult.Pairs;

            Assert.That(pairs.Count, Is.EqualTo(validFamilyPayload.SelectMany(x => x.Members).Count()));
            Assert.That(!pairs.Select(x => x.Giver.Name).GroupBy(x => x).Any(x => x.Count() > 1));
            Assert.That(!pairs.Select(x => x.Recipient.Name).GroupBy(x => x).Any(x => x.Count() > 1));
        }

        [Test]
        public void GenerateGiftPairs_2FamiliesSameSize_GiftPairsReturned()
        {
            // Arrange
            var validFamilyPayload = new List<Family>
                {
                new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            },
                            new FamilyMember
                            {
                                Name = "member2"
                            },
                            new FamilyMember
                            {
                                Name = "member9"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family2",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member3"
                            },
                            new FamilyMember
                            {
                                Name = "member4"
                            },
                            new FamilyMember
                            {
                                Name = "member5"
                            }
                        }
                    },
                };
            // Act
            var result = GetResultValue<OkObjectResult>(secretSantaController.GenerateGiftPairs(validFamilyPayload));

            // Assert
            Assert.That(result, Is.Not.Null);

            var secretSantaResult = result.Value as SecretSantaResult;
            var pairs = secretSantaResult.Pairs;

            Assert.That(pairs.Count, Is.EqualTo(validFamilyPayload.SelectMany(x => x.Members).Count()));
            Assert.That(!pairs.Select(x => x.Giver.Name).GroupBy(x => x).Any(x => x.Count() > 1));
            Assert.That(!pairs.Select(x => x.Recipient.Name).GroupBy(x => x).Any(x => x.Count() > 1));
        }

        [Test]
        public void GenerateGiftPairs_FamilyWithOneMemberExists_ErrorReturned()
        {
            // Arange
            List<Family> invalidFamilyPayload = new List<Family>
                {
                new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family2",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            }
                        }
                    }
                };
            var expectedResult = new SecretSantaResult
            {
                Message = "Family with less than 2 members found"
            };

            // Act
            var result = GetResultValue<BadRequestObjectResult>(secretSantaController.GenerateGiftPairs(invalidFamilyPayload));

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.ToString(), Is.EqualTo(expectedResult.ToString()));
        }

        [Test]
        public void GenerateGiftPairs_OnlyOneFamilyEntered_ErrorReturned()
        {
            // Arange
            List<Family> invalidFamilyPayload = new List<Family>
                {
                new Family
                    {
                        Name = "family1",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            }
                        }
                    },
                    new Family
                    {
                        Name = "family2",
                        Members = new List<FamilyMember>
                        {
                            new FamilyMember
                            {
                                Name = "member1"
                            }
                        }
                    }
                };
            var expectedResult = new SecretSantaResult
            {
                Message = "Family with less than 2 members found"
            };

            // Act
            var result = GetResultValue<BadRequestObjectResult>(secretSantaController.GenerateGiftPairs(invalidFamilyPayload));

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.ToString(), Is.EqualTo(expectedResult.ToString()));
        }

        #endregion

        #region Private Methods

        private TActionObjectType GetResultValue<TActionObjectType>(IActionResult objectResult) where TActionObjectType : ActionResult
        {
            return objectResult as TActionObjectType;
        }

        #endregion

    }
}
