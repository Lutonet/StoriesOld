using Microsoft.Extensions.Logging;
using Stories.Model;
using Stories.Pages.Administration.FirstRun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Data
{
    public static class DbInit
    {
        public static async Task<bool> Initialize(ApplicationDbContext dbContext, ILogger logger)
        {
            try
            {
                IndexModel.Report = IndexModel.Report += "Entered Database Creation mode.#information##";
                await dbContext.Database.EnsureCreatedAsync();
                IndexModel.Report = IndexModel.Report += "Database check is OK.#success##";
            }
            catch (Exception ex)
            {
                IndexModel.Report = IndexModel.Report += "Database is not created.#error##";
                logger.LogError("Database is not created", ex);
                return false;
            }
            if (!dbContext.Countries.Any())
            {
                var countries = new Country[]
                 {
                new Country { CountryName="Afghanistan", PhonePrefix=93, Timezone= 270},
                new Country { CountryName="Aland Islands", PhonePrefix=35818, Timezone= 120},
                new Country { CountryName="Albania", PhonePrefix=355, Timezone=60},
                new Country { CountryName="Algeria", PhonePrefix=213, Timezone= 60},
                new Country { CountryName="American Samoa", PhonePrefix=1684, Timezone= -660},
                new Country { CountryName="Andorra", PhonePrefix=376, Timezone= 60},
                new Country { CountryName="Angola", PhonePrefix=244, Timezone= 60},
                new Country { CountryName="Anguilla", PhonePrefix=1264, Timezone= -240},
                new Country { CountryName="Antigua and Barbuda", PhonePrefix=1268, Timezone= -240},
                new Country { CountryName="Argentina", PhonePrefix=54, Timezone= -180},
                new Country { CountryName="Armenia", PhonePrefix=374, Timezone= 240},
                new Country { CountryName="Aruba", PhonePrefix=297, Timezone= -240},
                new Country { CountryName="Ascension", PhonePrefix=247, Timezone= 0},
                new Country { CountryName="Australia", PhonePrefix = 61, Timezone = 480},
                new Country { CountryName="Azerbaijan", PhonePrefix=994, Timezone= 240},
                new Country { CountryName="Austria", PhonePrefix=43, Timezone= 60},
                new Country { CountryName="Bahamas", PhonePrefix=1242, Timezone= -300},
                new Country { CountryName="Bahrain", PhonePrefix=973, Timezone= 180},
                new Country { CountryName="Bangladesh", PhonePrefix=880, Timezone= 360},
                new Country { CountryName="Barbados", PhonePrefix=1246, Timezone= -240},
                new Country { CountryName="Barbuda", PhonePrefix=1268, Timezone= -240},
                new Country { CountryName="Belarus", PhonePrefix=375, Timezone= 180},
                new Country { CountryName="Belgium", PhonePrefix=32, Timezone= 180},
                new Country { CountryName="Belize", PhonePrefix=501, Timezone= -360},
                new Country { CountryName="Benin", PhonePrefix=229, Timezone= 60},
                new Country { CountryName="Bermuda", PhonePrefix=1441, Timezone= -240},
                new Country { CountryName="Bhutan", PhonePrefix=975, Timezone= 360},
                new Country { CountryName="Bolivia", PhonePrefix=591, Timezone= -240},
                new Country { CountryName="Bonaire", PhonePrefix=5997, Timezone= -240},
                new Country { CountryName="Bosnia and Herzegovina", PhonePrefix=387, Timezone= 60},
                new Country { CountryName="Botswana", PhonePrefix=267, Timezone= 120},
                new Country { CountryName="Brazil", PhonePrefix=55, Timezone= -300},
                new Country { CountryName="British Indian Ocean Territory", PhonePrefix=246, Timezone= 360},
                new Country { CountryName="British Virgin Islands", PhonePrefix=1284, Timezone= -240},
                new Country { CountryName="Brunei Darussalam", PhonePrefix=673, Timezone= 480},
                new Country { CountryName="Bulgaria", PhonePrefix=359, Timezone= 120},
                new Country { CountryName="Burkina Faso", PhonePrefix=226, Timezone= 0},
                new Country { CountryName="Burundi", PhonePrefix=257, Timezone= 120},
                new Country { CountryName="Cape Verde", PhonePrefix=238, Timezone= -60},
                new Country { CountryName="Cambodia", PhonePrefix=855, Timezone= 420},
                new Country { CountryName="Cameroon", PhonePrefix=237, Timezone= 60},
                new Country { CountryName="Canada", PhonePrefix=1, Timezone= -480},
                new Country { CountryName="Caribbean Netherlands", PhonePrefix=599, Timezone= -240},
                new Country { CountryName="Cayman Islands", PhonePrefix=1345, Timezone= -300},
                new Country { CountryName="Central African Republic", PhonePrefix=236, Timezone= 60},
                new Country { CountryName="Chad", PhonePrefix=235, Timezone= 60},
                new Country { CountryName="Chatham Island", PhonePrefix=64, Timezone= 765},
                new Country { CountryName="Chile", PhonePrefix=56, Timezone= -360},
                new Country { CountryName="China", PhonePrefix=86, Timezone= 480},
                new Country { CountryName="Christmas Island", PhonePrefix=6189164, Timezone= 420},
                new Country { CountryName="Cocos (Keeling) Islands", PhonePrefix=6189162, Timezone= 390},
                new Country { CountryName="Colombia", PhonePrefix=57, Timezone= -300},
                new Country { CountryName="Comoros", PhonePrefix=269, Timezone= 180},
                new Country { CountryName="Congo", PhonePrefix=242, Timezone= 60},
                new Country { CountryName="Democratic Republic of the Congo", PhonePrefix=243, Timezone= 60},
                new Country { CountryName="Cook Islands", PhonePrefix=682, Timezone= -600},
                new Country { CountryName="Costa Rica", PhonePrefix=506, Timezone= -360},
                new Country { CountryName="Ivory Coast", PhonePrefix=225, Timezone= 0},
                new Country { CountryName="Croatia", PhonePrefix=385, Timezone= 60},
                new Country { CountryName="Cuba", PhonePrefix=53, Timezone= -300},
                new Country { CountryName="Curacao", PhonePrefix=5999, Timezone= -240},
                new Country { CountryName="Cyprus", PhonePrefix=357, Timezone= 120},
                new Country { CountryName="Czech Republic", PhonePrefix=420, Timezone= 60},
                new Country { CountryName="Diego Garcia", PhonePrefix=246, Timezone= 360},
                new Country { CountryName="Denmark", PhonePrefix=45, Timezone= 60},
                new Country { CountryName="Djibouti", PhonePrefix=253, Timezone= 60},
                new Country { CountryName="Dominica", PhonePrefix=18, Timezone= -240},
                new Country { CountryName="Dominican Republic", PhonePrefix=18, Timezone= -240},
                new Country { CountryName="Easter Island", PhonePrefix=56, Timezone= -360},
                new Country { CountryName="Ecuador", PhonePrefix=593, Timezone= -360},
                new Country { CountryName="Egypt", PhonePrefix=20, Timezone= 120},
                new Country { CountryName="El Salvador", PhonePrefix=503, Timezone= -360},
                new Country { CountryName="Equatorial Guinea", PhonePrefix=240, Timezone= 60},
                new Country { CountryName="Eritrea", PhonePrefix=291, Timezone= 180},
                new Country { CountryName="Estonia", PhonePrefix=372, Timezone= 120},
                new Country { CountryName="eSwatini", PhonePrefix=268, Timezone= 120},
                new Country { CountryName="Ethiopia", PhonePrefix=251, Timezone= 180},
                new Country { CountryName="Falkland Island", PhonePrefix=298, Timezone= -180},
                new Country { CountryName="Faroe Islands", PhonePrefix=298, Timezone= 0},
                new Country { CountryName="Fiji", PhonePrefix=679, Timezone= 720},
                new Country { CountryName="Finland", PhonePrefix=358, Timezone= 120},
                new Country { CountryName="France", PhonePrefix=33, Timezone= 60},
                new Country { CountryName="French Antilles", PhonePrefix=596, Timezone= -180},
                new Country { CountryName="French Guiana", PhonePrefix=594, Timezone= -180},
                new Country { CountryName="French Polynesia", PhonePrefix=689, Timezone= -600},
                new Country { CountryName="Gabon", PhonePrefix=241, Timezone= 60},
                new Country { CountryName="Gambia", PhonePrefix=220, Timezone= 0},
                new Country { CountryName="Georgia", PhonePrefix=995, Timezone= 240},
                new Country { CountryName="Germany", PhonePrefix=49, Timezone= 60},
                new Country { CountryName="Ghana", PhonePrefix=233, Timezone= 0},
                new Country { CountryName="Gibraltar", PhonePrefix=350, Timezone= 60},
                new Country { CountryName="Greece", PhonePrefix=30, Timezone= 120},
                new Country { CountryName="Greenland", PhonePrefix=299, Timezone= -240},
                new Country { CountryName="Grenada", PhonePrefix=1473, Timezone= 240},
                new Country { CountryName="Guadeloupe", PhonePrefix=590, Timezone= 240},
                new Country { CountryName="Guernsey", PhonePrefix=44, Timezone= 0},
                new Country { CountryName="Guinea", PhonePrefix=224, Timezone= 0},
                new Country { CountryName="Guinea-Bissau", PhonePrefix=245, Timezone= 0},
                new Country { CountryName="Guyana", PhonePrefix=592, Timezone= -240},
                new Country { CountryName="Haiti", PhonePrefix=509, Timezone= -300},
                new Country { CountryName="Honduras", PhonePrefix=504, Timezone= 360},
                new Country { CountryName="Hong Kong", PhonePrefix=852, Timezone= 480},
                new Country { CountryName="Hungary", PhonePrefix=36, Timezone= 60},
                new Country { CountryName="Iceland", PhonePrefix=354, Timezone= 0},
                new Country { CountryName="India", PhonePrefix=91, Timezone= 330},
                new Country { CountryName="Indonesia", PhonePrefix=62, Timezone= 480},
                new Country { CountryName="Iran", PhonePrefix=98, Timezone= 210},
                new Country { CountryName="Iraq", PhonePrefix=964, Timezone= 180},
                new Country { CountryName="Ireland", PhonePrefix=353, Timezone= 0},
                new Country { CountryName="Isle of Man", PhonePrefix=44, Timezone= 0},
                new Country { CountryName="Israel", PhonePrefix=972, Timezone= 120},
                new Country { CountryName="Italy", PhonePrefix=39, Timezone= 60},
                new Country { CountryName="Jamaica", PhonePrefix=1876, Timezone= -300},
                new Country { CountryName="Jan Mayen", PhonePrefix=4779, Timezone= 60},
                new Country { CountryName="Japan", PhonePrefix=91, Timezone= 540},
                new Country { CountryName="Jersey", PhonePrefix=441534, Timezone= 0},
                new Country { CountryName="Jordan", PhonePrefix=962, Timezone= 120},
                new Country { CountryName="Kazakhstan", PhonePrefix=7, Timezone= 300},
                new Country { CountryName="Kenya", PhonePrefix=254, Timezone= 180},
                new Country { CountryName="Kiribati", PhonePrefix=686, Timezone= 660},
                new Country { CountryName="Korea, North", PhonePrefix=850, Timezone= 540},
                new Country { CountryName="Korea, South", PhonePrefix=82, Timezone= 540},
                new Country { CountryName="Kosovo", PhonePrefix=383, Timezone= 60},
                new Country { CountryName="Kuwait", PhonePrefix=965, Timezone= 60},
                new Country { CountryName="Kyrgyzstan", PhonePrefix=996, Timezone= 300},
                new Country { CountryName="Laos", PhonePrefix=856, Timezone= 420},
                new Country { CountryName="Latvia", PhonePrefix=371, Timezone= 120},
                new Country { CountryName="Lebanon", PhonePrefix=961, Timezone= 120},
                new Country { CountryName="Lesotho", PhonePrefix=266, Timezone= 120},
                new Country { CountryName="Liberia", PhonePrefix=231, Timezone= 0},
                new Country { CountryName="Libya", PhonePrefix=218, Timezone= 120},
                new Country { CountryName="Liechtenstein", PhonePrefix=423, Timezone= 60},
                new Country { CountryName="Lithuana", PhonePrefix=370, Timezone= 120},
                new Country { CountryName="Luxembourg", PhonePrefix=352, Timezone= 60},
                new Country { CountryName="Macau", PhonePrefix=853, Timezone= 480},
                new Country { CountryName="Madagascar", PhonePrefix=261, Timezone= 180},
                new Country { CountryName="Malawi", PhonePrefix=265, Timezone= 120},
                new Country { CountryName="Malaysia", PhonePrefix=60, Timezone= 480},
                new Country { CountryName="Maldives", PhonePrefix=960, Timezone= 300},
                new Country { CountryName="Mali", PhonePrefix=223, Timezone= 0},
                new Country { CountryName="Malta", PhonePrefix=356, Timezone= 60},
                new Country { CountryName="Marshall Islands", PhonePrefix=692, Timezone= 720},
                new Country { CountryName="Martinique", PhonePrefix=596, Timezone= -240},
                new Country { CountryName="Mauritania", PhonePrefix=222, Timezone= 0},
                new Country { CountryName="Mauritius", PhonePrefix=230, Timezone= 240},
                new Country { CountryName="Mayotte", PhonePrefix=262, Timezone= 180},
                new Country { CountryName="Mexico", PhonePrefix=52, Timezone= -480},
                new Country { CountryName="Micronesia", PhonePrefix=691, Timezone= 600},
                new Country { CountryName="Midway Island USA", PhonePrefix=45, Timezone= 60},
                new Country { CountryName="Moldowa", PhonePrefix=373, Timezone= 120},
                new Country { CountryName="Monaco", PhonePrefix=373, Timezone= 120},
                new Country { CountryName="Mongolia", PhonePrefix=976, Timezone= 420},
                new Country { CountryName="Montenegro", PhonePrefix=382, Timezone= 60},
                new Country { CountryName="Montserrat", PhonePrefix=1664, Timezone= -240},
                new Country { CountryName="Morocco", PhonePrefix=212, Timezone= 60},
                new Country { CountryName="Mozambique", PhonePrefix=258, Timezone= 120},
                new Country { CountryName="Myanmar", PhonePrefix=95, Timezone= 390},
                new Country { CountryName="Nagorno-Karabakh", PhonePrefix=374, Timezone= 240},
                new Country { CountryName="Namibia", PhonePrefix=264, Timezone= 120},
                new Country { CountryName="Nauru", PhonePrefix=674, Timezone= 720},
                new Country { CountryName="Nepal", PhonePrefix=977, Timezone= 345},
                new Country { CountryName="Netherlands", PhonePrefix=31, Timezone= 60},
                new Country { CountryName="Nevis", PhonePrefix=1869, Timezone= -240},
                new Country { CountryName="New Caledonia", PhonePrefix=687, Timezone= 660},
                new Country { CountryName="New Zealand", PhonePrefix=64, Timezone= 720},
                new Country { CountryName="Nicaragua", PhonePrefix=505, Timezone= -360},
                new Country { CountryName="Niger", PhonePrefix=227, Timezone= 60},
                new Country { CountryName="Nigeria", PhonePrefix=234, Timezone= 60},
                new Country { CountryName="Niue", PhonePrefix=683, Timezone= -660},
                new Country { CountryName="Norfolk Island", PhonePrefix=6723, Timezone= 660},
                new Country { CountryName="North Macedonia", PhonePrefix=389, Timezone= 60},
                new Country { CountryName="Northern Cyprus", PhonePrefix=90392, Timezone= 120},
                new Country { CountryName="Northern Ireland", PhonePrefix=4428, Timezone= 0},
                new Country { CountryName="Northern Mariana Islands", PhonePrefix=1670, Timezone= 600},
                new Country { CountryName="Norway", PhonePrefix=47, Timezone= 60},
                new Country { CountryName="Oman", PhonePrefix=968, Timezone= 240},
                new Country { CountryName="Pakistan", PhonePrefix=92, Timezone= 300},
                new Country { CountryName="Palau", PhonePrefix=680, Timezone= 540},
                new Country { CountryName="Palestine", PhonePrefix=970, Timezone= 120},
                new Country { CountryName="Papua New Guinea", PhonePrefix=675, Timezone= 600},
                new Country { CountryName="Paraguay", PhonePrefix=595, Timezone= -240},
                new Country { CountryName="Peru", PhonePrefix=51, Timezone= 300},
                new Country { CountryName="Philippines", PhonePrefix=63, Timezone= 480},
                new Country { CountryName="Pitcairn Islands", PhonePrefix=64, Timezone= -480},
                new Country { CountryName="Poland", PhonePrefix=48, Timezone= 60},
                new Country { CountryName="Portugal", PhonePrefix=351, Timezone= 0},
                new Country { CountryName="Puerto Rico", PhonePrefix=1, Timezone= -240},
                new Country { CountryName="Qatar", PhonePrefix=974, Timezone= 180},
                new Country { CountryName="Reunion", PhonePrefix=262, Timezone= 140},
                new Country { CountryName="Romania", PhonePrefix=40, Timezone= 120},
                new Country { CountryName="Russia", PhonePrefix=7, Timezone= 120},
                new Country { CountryName="Rwanda", PhonePrefix=250, Timezone= 120},
                new Country { CountryName="Saba", PhonePrefix=5994, Timezone= -240},
                new Country { CountryName="Saint Barthelemy", PhonePrefix=590, Timezone= -240},
                new Country { CountryName="Saint Helena", PhonePrefix=290, Timezone= 0},
                new Country { CountryName="Saint Kitts and Nevis", PhonePrefix=1869, Timezone= -240},
                new Country { CountryName="Saint Lucia", PhonePrefix=1758, Timezone= -240},
                new Country { CountryName="Saint Martin (France)", PhonePrefix=590, Timezone= -240},
                new Country { CountryName="Saint Pierre and Miquelon", PhonePrefix=508, Timezone= 180},
                new Country { CountryName="Saint Vincent and the Grenadines", PhonePrefix=1784, Timezone= -240},
                new Country { CountryName="Samoa", PhonePrefix=685, Timezone= 660},
                new Country { CountryName="San Marino", PhonePrefix=378, Timezone= 60},
                new Country { CountryName="Sao Tome and Principle", PhonePrefix=239, Timezone= 0},
                new Country { CountryName="Saudi Arabia", PhonePrefix=966, Timezone= 180},
                new Country { CountryName="Senegal", PhonePrefix=221, Timezone= 0},
                new Country { CountryName="Serbia", PhonePrefix=381, Timezone= 60},
                new Country { CountryName="Seychelles", PhonePrefix=248, Timezone= 240},
                new Country { CountryName="Sierra Leone", PhonePrefix=232, Timezone= 0},
                new Country { CountryName="Singapore", PhonePrefix=65, Timezone= 480},
                new Country { CountryName="Sint Eustatius", PhonePrefix=5993, Timezone= -240},
                new Country { CountryName="Sint Maarten (Netherlands)", PhonePrefix=1721, Timezone= -240},
                new Country { CountryName="Slovakia", PhonePrefix=421, Timezone= 60},
                new Country { CountryName="Slovenia", PhonePrefix=386, Timezone= 60},
                new Country { CountryName="Solomon Islands", PhonePrefix=677, Timezone= 660},
                new Country { CountryName="Somalia", PhonePrefix=252, Timezone= 180},
                new Country { CountryName="South Africa", PhonePrefix=27, Timezone= 120},
                new Country { CountryName="South Georgia and the Sounth Sandwich Islands", PhonePrefix=500, Timezone= -120},
                new Country { CountryName="South Ossetia", PhonePrefix=99534, Timezone= 180},
                new Country { CountryName="South Sudan", PhonePrefix=211, Timezone= 180},
                new Country { CountryName="Spain", PhonePrefix=34, Timezone= 60},
                new Country { CountryName="Sri Lanka", PhonePrefix=94, Timezone= 330},
                new Country { CountryName="Sudan", PhonePrefix=249, Timezone= 120},
                new Country { CountryName="Suriname", PhonePrefix=597, Timezone= -180},
                new Country { CountryName="Svalbard", PhonePrefix=4779, Timezone= 60},
                new Country { CountryName="Sweden", PhonePrefix=46, Timezone= 60},
                new Country { CountryName="Switzerland", PhonePrefix=41, Timezone= 60},
                new Country { CountryName="Syria", PhonePrefix=963, Timezone= 120},
                new Country { CountryName="Taiwan", PhonePrefix=886, Timezone= 480},
                new Country { CountryName="Tajikistan", PhonePrefix=992, Timezone= 300},
                new Country { CountryName="Tanzania", PhonePrefix=255, Timezone= 180},
                new Country { CountryName="Thailand", PhonePrefix=66, Timezone= 420},
                new Country { CountryName="East Timor", PhonePrefix=670, Timezone= 540},
                new Country { CountryName="Togo", PhonePrefix=228, Timezone= 0},
                new Country { CountryName="Tokelau", PhonePrefix=690, Timezone= 660},
                new Country { CountryName="Tonga", PhonePrefix=676, Timezone= 660},
                new Country { CountryName="Transnistria", PhonePrefix=373, Timezone= 120},
                new Country { CountryName="Trinidad and Tobago", PhonePrefix=1868, Timezone= -240},
                new Country { CountryName="Tristan da Cunha", PhonePrefix=2908, Timezone= 0},
                new Country { CountryName="Tunisia", PhonePrefix=216, Timezone= 60},
                new Country { CountryName="Turkey", PhonePrefix=90, Timezone= 180},
                new Country { CountryName="Turkmenistan", PhonePrefix=993, Timezone= 300},
                new Country { CountryName="Turks and Caicos Islands", PhonePrefix=1649, Timezone= -300},
                new Country { CountryName="Tuvalu", PhonePrefix=688, Timezone= 720},
                new Country { CountryName="Uganda", PhonePrefix=256, Timezone= 180},
                new Country { CountryName="Ukraine", PhonePrefix=380, Timezone= 120},
                new Country { CountryName="United Arab Emirates", PhonePrefix=971, Timezone= 240},
                new Country { CountryName="United Kingdom", PhonePrefix=44, Timezone= 0},
                new Country { CountryName="United States", PhonePrefix=1, Timezone= -600},
                new Country { CountryName="Uruguay", PhonePrefix=598, Timezone= -180},
                new Country { CountryName="US Virgin Island", PhonePrefix=1340, Timezone= -240},
                new Country { CountryName="Uzbekistan", PhonePrefix=998, Timezone= 300},
                new Country { CountryName="Vanatau", PhonePrefix=678, Timezone= 660},
                new Country { CountryName="Vatican City State", PhonePrefix=3906698, Timezone= 60},
                new Country { CountryName="Venezuela", PhonePrefix=58, Timezone= -240},
                new Country { CountryName="Vietnam", PhonePrefix=84, Timezone= 420},
                new Country { CountryName="Wake Island, USA", PhonePrefix=1808, Timezone= 720},
                new Country { CountryName="Wallis and Futuna", PhonePrefix=681, Timezone= 720},
                new Country { CountryName="Yemen", PhonePrefix=967, Timezone= 180},
                new Country { CountryName="Zambia", PhonePrefix=260, Timezone= 120},
                new Country { CountryName="Zanzibar", PhonePrefix=25524, Timezone= 180},
                new Country { CountryName="Zimbabwe", PhonePrefix=263, Timezone= 120}
                };
                try
                {
                    await dbContext.Countries.AddRangeAsync(countries);
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation("Countries list added to the database");
                    IndexModel.Report += "List of countries added to the database#success##";
                }
                catch (Exception ex)
                {
                    logger.LogError("Can't write Countries to the database table", ex);
                    IndexModel.Report += "List of countries couldn't be created.#error##";
                    return false;
                }
            }
            else
                IndexModel.Report += "Database already contains some Countries.#information##";

            if (!dbContext.AgeRestrictions.Any())
            {
                var ageRestrictions = new AgeRestriction[]
                {
                    new AgeRestriction { AgeFrom=3},  //1
                    new AgeRestriction { AgeFrom=6},  //2
                    new AgeRestriction { AgeFrom=12}, //3
                    new AgeRestriction { AgeFrom=16}, //4
                    new AgeRestriction { AgeFrom=18}  //5
                };
                try
                {
                    await dbContext.AgeRestrictions.AddRangeAsync(ageRestrictions);
                    await dbContext.SaveChangesAsync();
                    IndexModel.Report += "Database record for Age restrictions created.#success##";
                    logger.LogInformation("List of Age Restrictions added to the database");
                }
                catch (Exception ex)
                {
                    logger.LogError("Can't write the Age Restrictions into the database table", ex);
                    IndexModel.Report += "Can't write the Age Restrictions into the database.#error##";
                    return false;
                }
            }
            else
                IndexModel.Report += "Database already contains some Age restrictions.#information##";

            if (!dbContext.CategoryGroups.Any())
            {
                var categoryGroup = new CategoryGroup[]
                {
                    new CategoryGroup { GroupName="Seriously Meant Prose" },    //1
                    new CategoryGroup { GroupName="Seriously Meant Poetry" },   //2
                    new CategoryGroup { GroupName="Reflexive Compositions" },   //3
                    new CategoryGroup { GroupName="Funny Categories" },         //4
                    new CategoryGroup { GroupName="Culture"},                   //5
                    new CategoryGroup { GroupName="Other Categories"}           //6
                };
                try
                {
                    await dbContext.CategoryGroups.AddRangeAsync(categoryGroup);
                    await dbContext.SaveChangesAsync();
                    IndexModel.Report += "Category groups record created successfully.#success##";
                    logger.LogInformation("List of Category groups added to the database");
                }
                catch (Exception ex)
                {
                    logger.LogError("Can't write Category Groups into the database table", ex);
                    IndexModel.Report += "Can't write Category Groups into the database.#error##";
                    return false;
                }
            }
            else
                IndexModel.Report += "Database already contains Categories groups records.#information##";

            if (!dbContext.Categories.Any())
            {
                IList<CategoryGroup> CategoryGroups = dbContext.CategoryGroups.ToList();
                int seriousProse = (from c in CategoryGroups
                                    where c.GroupName == "Seriously Meant Prose"
                                    select c.Id).FirstOrDefault();
                int seriousPoetry = (from c in CategoryGroups
                                     where c.GroupName == "Seriously Meant Poetry"
                                     select c.Id).FirstOrDefault();
                int reflexiveCompositions = (from c in CategoryGroups
                                             where c.GroupName == "Reflexive Compositions"
                                             select c.Id).FirstOrDefault();
                int funnyCategories = (from c in CategoryGroups
                                       where c.GroupName == "Funny Categories"
                                       select c.Id).FirstOrDefault();
                int culture = (from c in CategoryGroups
                               where c.GroupName == "Culture"
                               select c.Id).FirstOrDefault();
                int other = (from c in CategoryGroups
                             where c.GroupName == "Other Categories"
                             select c.Id).FirstOrDefault();
                Console.WriteLine("Seriously meant prose:" + seriousProse);
                Console.WriteLine("Seriously meant poetry:" + seriousPoetry);
                Console.WriteLine("Reflexive composition:" + reflexiveCompositions);
                Console.WriteLine("Funny categories:" + funnyCategories);
                Console.WriteLine("Culture:" + culture);
                Console.WriteLine("Other Categories:" + other);

                var categories = new Category[]
                {
                    new Category { CategoryName="Drama, Screenplays" , CategoryGroupId=seriousProse },
                    new Category { CategoryName="Miniature Prose", CategoryGroupId=seriousProse },
                    new Category { CategoryName="Poetry in prose", CategoryGroupId=seriousProse },
                    new Category { CategoryName="Fairy tales and fables ", CategoryGroupId=seriousProse },
                    new Category { CategoryName="Short stories", CategoryGroupId=seriousProse },
                    new Category { CategoryName="Prose for the sequel", CategoryGroupId=seriousProse },

                    new Category { CategoryName="Haiku", CategoryGroupId = seriousPoetry },
                    new Category { CategoryName="Miniatures", CategoryGroupId = seriousPoetry },
                    new Category { CategoryName="Senrjú", CategoryGroupId = seriousPoetry },
                    new Category { CategoryName="Mixed verses", CategoryGroupId = seriousPoetry },
                    new Category { CategoryName="Bound verses", CategoryGroupId = seriousPoetry },
                    new Category { CategoryName="Free verses", CategoryGroupId = seriousPoetry },

                    new Category { CategoryName="Quotes", CategoryGroupId = reflexiveCompositions },
                    new Category { CategoryName="Feuilletons", CategoryGroupId = reflexiveCompositions },
                    new Category { CategoryName="Art critics", CategoryGroupId = reflexiveCompositions },
                    new Category { CategoryName="Literary theory", CategoryGroupId = reflexiveCompositions },
                    new Category { CategoryName="Medallions of literary personalities", CategoryGroupId = reflexiveCompositions},
                    new Category { CategoryName="Translations", CategoryGroupId = reflexiveCompositions},
                    new Category { CategoryName="Reviews, Product reviews", CategoryGroupId = reflexiveCompositions},
                    new Category { CategoryName="Thoughs", CategoryGroupId = reflexiveCompositions},

                    new Category { CategoryName="Aphorism", CategoryGroupId = funnyCategories},
                    new Category { CategoryName="Silly writting", CategoryGroupId = funnyCategories},
                    new Category { CategoryName="Puzzles", CategoryGroupId = funnyCategories},
                    new Category { CategoryName="Just for fun", CategoryGroupId = funnyCategories},
                    new Category { CategoryName="Paipu", CategoryGroupId = funnyCategories},
                    new Category { CategoryName="For kids", CategoryGroupId = funnyCategories},
                    new Category { CategoryName="Lyrics", CategoryGroupId = funnyCategories},

                    new Category { CategoryName="Debate", CategoryGroupId = culture},
                    new Category { CategoryName="Theatres, Movies, Presentations", CategoryGroupId = culture},
                    new Category { CategoryName="Books and Magazines", CategoryGroupId = culture},
                    new Category { CategoryName="Literar Debate", CategoryGroupId = culture},
                    new Category { CategoryName="Reports", CategoryGroupId = culture},
                    new Category { CategoryName="Meetings and actions", CategoryGroupId = culture},
                    new Category { CategoryName="Events", CategoryGroupId = culture},

                    new Category { CategoryName="Ailment", CategoryGroupId = other},
                    new Category { CategoryName="Out of mind", CategoryGroupId = other},
                    new Category { CategoryName="Other, Uncategorized", CategoryGroupId = other},
                };
                try
                {
                    await dbContext.Categories.AddRangeAsync(categories);
                    await dbContext.SaveChangesAsync();
                    IndexModel.Report += "List of Categories added to the database.#success##";
                    logger.LogInformation("List of Categories added to the database");
                }
                catch (Exception ex)
                {
                    logger.LogError("Can't write Categories into the database table", ex);
                    IndexModel.Report += "Can't crete List of Categories in the database.#error##";
                    return false;
                }
            }
            else
                IndexModel.Report += "Database already contains some Categories.#information##";

            return true;
        }
    }
}