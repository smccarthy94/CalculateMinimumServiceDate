# CalculateMinimumServiceDate
## Find a minimum booking date, while taking into account a given notice period and public holidays (singular and consecutive)/weekends

To run unit tests, navigate to solution root and execute `dotnet test`.
Tests verify the following cases: 
1. MondayBeforeSameDayCutOffReturnsWednesday
1. MondayAfterSameDayCutOffReturnsThursday
1. FridayBeforeSameDayCutOffReturnsTheFollowingTuesday
1. FridayBeforeAnzacDayAfterSameDayCutOffReturnsTheFollowingThursday
1. WednesdayBeforeEasterBeforeSameDayCutOffReturnsTheFollowingTuesday
1. WednesdayBeforeEasterAfterSameDayCutOffReturnsTheFollowingWednesday
1. ThursdayBeforeAnzacDayBeforeSameDayCutOffReturnsTheFollowingTuesday
1. ThursdayBeforeAnzacDayAfterSameDayCutOffReturnsTheFollowingWednesday
1. FridayAfterSameDayCutOffReturnsTheFollowingWednesday
1. FridayBeforeAnzacDayBeforeSameDayCutOffReturnsTheFollowingWednesday

API to retrieve a minimum date from "Now":
`GET:http://localhost:60151/booking/minimum-service-date/123`
